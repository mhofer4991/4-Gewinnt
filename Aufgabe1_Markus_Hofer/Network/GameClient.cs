//-----------------------------------------------------------------------
// <copyright file="GameClient.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class can be used to connect to a hosted game via p2p.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe1_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This class can be used to connect to a hosted game via p2p.
    /// </summary>
    public class GameClient
    {
        /// <summary> Looks for open games in the network. </summary>
        private UdpClient listener;

        /// <summary> Indicates that possible server interruptions were caused by the user. </summary>
        private bool forcedUserInterruption;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameClient"/> class.
        /// </summary>
        public GameClient()
        {
            this.listener = new UdpClient();

            IPEndPoint groupEP = new IPEndPoint(0, Protocol.DefaultBroadcastPort);

            this.listener.ExclusiveAddressUse = false;
            this.listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this.listener.Client.Bind(groupEP);

            this.IsSearching = false;
        }

        /// <summary>
        /// Delegate for event OnOpenGameFound.
        /// </summary>
        /// <param name="player">An instance of an online player, which has been found.</param>
        /// <param name="settings">The settings of the online game.</param>
        public delegate void OpenGameFound(OnlinePlayer player, Settings settings);

        /// <summary>
        /// Delegate for event OnNetworkErrorOccurred.
        /// </summary>
        /// <param name="error">The type of network error.</param>
        public delegate void NetworkErrorOccurred(Protocol.ErrorType error);

        /// <summary>
        /// Gets called when an open game has been found.
        /// </summary>
        public event OpenGameFound OnGameEstablished;

        /// <summary>
        /// Gets called when a network error occurs.
        /// </summary>
        public event NetworkErrorOccurred OnNetworkErrorOccurred;

        /// <summary> Gets a value indicating whether the client is searching or not. </summary>
        /// <value> A value indicating whether the player is searching or not. </value>
        public bool IsSearching { get; private set; }

        /// <summary>
        /// Connects to the game server at the given IP - address.
        /// </summary>
        /// <param name="ip">IP - address of the game server.</param>
        public void Connect(string ip)
        {
            if (!Protocol.IsValidIPAddress(ip))
            {
                return;
            }

            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ip), Protocol.DefaultServerPort);

            TcpClient gameServer = new TcpClient();

            try
            {
                gameServer.Connect(serverEndPoint);
                NetworkStream serverStream = gameServer.GetStream();

                // Connection successful
                Settings onlineSettings = new Settings();

                // Send request for joining
                byte[] request = Protocol.GetRequestForJoining();

                serverStream.Write(request, 0, request.Length);
                serverStream.Flush();

                // Wait for response
                byte[] response = new byte[4];

                if (serverStream.Read(response, 0, response.Length) == response.Length)
                {
                    if (response[0] == (byte)Protocol.MessageType.Request_accepted)
                    {
                        onlineSettings.SetAmountOfGameMapRows(response[2]);
                        onlineSettings.SetAmountOfGameMapColumns(response[3]);

                        if (this.OnGameEstablished != null)
                        {
                            OnlinePlayer player = new OnlinePlayer(Game.SecondPlayerID, gameServer);

                            player.StartGame(response[2] >= request[1]);

                            this.OnGameEstablished(player, onlineSettings);

                            return;
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
                if (this.OnNetworkErrorOccurred != null)
                {
                    this.OnNetworkErrorOccurred(Protocol.ErrorType.Client_connection_failed);
                }
            }

            this.forcedUserInterruption = false;
        }

        /// <summary>
        /// Looks for open games in the network.
        /// </summary>
        public void SearchForOpenGames()
        {
            this.IsSearching = true;

            Thread thread = new Thread(new ThreadStart(this.Listen));
            thread.Start();
        }

        /// <summary>
        /// Stops looking for open games in the network.
        /// </summary>
        public void StopSearching()
        {
            this.forcedUserInterruption = true;

            this.IsSearching = false;
        }

        /// <summary>
        /// Listens to broadcasts which indicate that a game is open.
        /// </summary>
        private void Listen()
        {
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, Protocol.DefaultBroadcastPort);

            try
            {
                byte[] broadcast = this.listener.Receive(ref groupEP);

                if (Enumerable.SequenceEqual(broadcast, Protocol.GetBroadcast()))
                {
                    if (this.IsSearching)
                    {
                        this.Connect(groupEP.Address.ToString());
                    }
                }
            }
            catch (SocketException)
            {
                if (!this.forcedUserInterruption)
                {
                    if (this.OnNetworkErrorOccurred != null)
                    {
                        this.OnNetworkErrorOccurred(Protocol.ErrorType.Client_broadcast_failed);
                    }
                }

                this.forcedUserInterruption = false;
            }

            this.IsSearching = false;
        }
    }
}
