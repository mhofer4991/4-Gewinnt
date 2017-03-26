//-----------------------------------------------------------------------
// <copyright file="GameServer.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class can be used to create a new connect four game online.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe1_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This class can be used to create a new connect four game online.
    /// </summary>
    public class GameServer
    {
        /// <summary> The listener waits for players. </summary>
        private TcpListener listener;

        /// <summary> Indicates that possible server interruptions were caused by the user. </summary>
        private bool forcedUserInterruption;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameServer"/> class.
        /// </summary>
        public GameServer()
        {
            this.listener = new TcpListener(IPAddress.Any, Protocol.DefaultServerPort);

            this.IsSending = false;
        }

        /// <summary>
        /// Delegate for event OnPlayerJoinedHostedGame.
        /// </summary>
        /// <param name="player">An instance of an online player, which has been found.</param>
        /// <param name="settings">The settings of the online game.</param>
        public delegate void PlayerJoinedHostedGame(OnlinePlayer player, Settings settings);

        /// <summary>
        /// Delegate for event OnNetworkErrorOccurred.
        /// </summary>
        /// <param name="error">The type of network error.</param>
        public delegate void NetworkErrorOccurred(Protocol.ErrorType error);

        /// <summary>
        /// Gets called when a player has joined the game.
        /// </summary>
        public event PlayerJoinedHostedGame OnPlayerJoinedHostedGame;

        /// <summary>
        /// Gets called when a network error occurs.
        /// </summary>
        public event NetworkErrorOccurred OnNetworkErrorOccurred;

        /// <summary> Gets a value indicating whether the server is sending broadcasts or not. </summary>
        /// <value> A value indicating whether the server is sending broadcasts or not. </value>
        public bool IsSending { get; private set; }

        /// <summary>
        /// Creates a new game online and waits for a player.
        /// </summary>
        /// <param name="settings">The settings of the online game.</param>
        public void CreateNewGame(Settings settings)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(this.Listen));
            thread.Start(settings);

            this.IsSending = true;

            Thread broadThread = new Thread(new ThreadStart(this.SendBroadcasts));
            broadThread.Start();
        }

        /// <summary>
        /// Cancels the empty, hosted game.
        /// </summary>
        public void StopWaiting()
        {
            this.forcedUserInterruption = true;

            this.IsSending = false;

            this.listener.Stop();
        }

        /// <summary>
        /// Listens on a specified port.
        /// </summary>
        /// <param name="settings">The settings of the online game.</param>
        private void Listen(object settings)
        {
            try
            {
                this.listener.Start();

                TcpClient client = this.listener.AcceptTcpClient();

                this.StopWaiting();

                NetworkStream clientStream = client.GetStream();

                // Connection successful
                byte[] request = new byte[2];

                if (clientStream.Read(request, 0, request.Length) == request.Length)
                {
                    if (request[0] == (byte)Protocol.MessageType.Request_for_joining)
                    {
                        // Send response
                        byte[] response = Protocol.GetResponseForJoining((Settings)settings);

                        clientStream.Write(response, 0, response.Length);
                        clientStream.Flush();

                        if (this.OnPlayerJoinedHostedGame != null)
                        {
                            OnlinePlayer player = new OnlinePlayer(Game.SecondPlayerID, client);
                            
                            player.StartGame(request[1] > response[2]);

                            this.OnPlayerJoinedHostedGame(player, (Settings)settings);
                        }
                    }
                }
            }
            catch (System.IO.IOException)
            {
            }
            catch (SocketException)
            {
            }

            if (!this.forcedUserInterruption)
            {
                // Network error also occurs if he just sends invalid messages.
                if (this.OnNetworkErrorOccurred != null)
                {
                    this.OnNetworkErrorOccurred(Protocol.ErrorType.Server_connection_failed);
                }
            }

            this.forcedUserInterruption = false;
        }

        /// <summary>
        /// Sends broadcasts to notify other network users that there is an open game.
        /// </summary>
        private void SendBroadcasts()
        {
            // Address 192.168.56.1 is used by VirtualBox
            // Address 192.168.230.1 is used by VMWare
            // Address 192.168.190.1 is also used by VMWare
            List<string> ignoredAddresses = new List<string>();

            ignoredAddresses.Add("192.168.56.1");
            ignoredAddresses.Add("192.168.230.1");
            ignoredAddresses.Add("192.168.190.1");

            // Find out local IP address to calculate subnet address.
            IPHostEntry ipE = Dns.GetHostEntry(Dns.GetHostName());

            IPAddress[] addresses = ipE.AddressList;
            string foundAddress = string.Empty;

            foreach (IPAddress address in addresses)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (!ignoredAddresses.Contains(address.ToString()))
                    {
                        foundAddress = address.ToString();
                    }
                }
            }

            if (!foundAddress.Equals(string.Empty))
            {
                string[] octets = foundAddress.Split('.');

                foundAddress = octets[0] + "." + octets[1] + "." + octets[2] + ".255";
            }
            else
            {
                foundAddress = IPAddress.Broadcast.ToString();
            }

            byte[] sendBuffer = Protocol.GetBroadcast();

            // IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, Protocol.DefaultBroadcastPort);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(foundAddress), Protocol.DefaultBroadcastPort);
            Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sending_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            sending_socket.EnableBroadcast = true;

            try
            {
                while (this.IsSending)
                {
                    sending_socket.SendTo(sendBuffer, ip);

                    Thread.Sleep(Protocol.DefaultBroadcastTimeout);
                }

                sending_socket.Close();
            }
            catch (SocketException)
            {
                if (!this.forcedUserInterruption)
                {
                    if (this.OnNetworkErrorOccurred != null)
                    {
                        this.OnNetworkErrorOccurred(Protocol.ErrorType.Server_broadcast_failed);
                    }
                }

                this.forcedUserInterruption = false;
            }
        }
    }
}
