//-----------------------------------------------------------------------
// <copyright file="Protocol.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class sets rules for playing the game connect four online.</summary>
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
    /// This class sets rules for playing the game connect four online.
    /// </summary>
    public static class Protocol
    {
        /// <summary>
        /// Gets the default port for the game host.
        /// </summary>
        public const int DefaultServerPort = 1234;

        /// <summary>
        /// Gets the default port for the broadcast.
        /// </summary>
        public const int DefaultBroadcastPort = 4321;

        /// <summary>
        /// Gets the default timeout for sending broadcasts in milliseconds.
        /// </summary>
        public const int DefaultBroadcastTimeout = 1000;

        /// <summary>
        /// This enumeration contains different types of messages.
        /// </summary>
        public enum MessageType
        {
            /// <summary>
            /// Indicates that the client wants to play a game with the server.
            /// </summary>
            Request_for_joining = 1,

            /// <summary>
            /// Indicates that the server has accepted the request.
            /// </summary>
            Request_accepted = 2,

            /// <summary>
            /// Indicates that a player sends it's next move.
            /// </summary>
            Next_move = 3
        }

        /// <summary>
        /// This enumeration contains different types of error codes.
        /// </summary>
        public enum ErrorType
        {
            /// <summary>
            /// Indicates that the client could not connect to the server.
            /// </summary>
            Client_connection_failed = 1,

            /// <summary>
            /// Indicates that the client could not listen to broadcasts.
            /// </summary>
            Client_broadcast_failed = 2,

            /// <summary>
            /// Indicates that the server could not maintain a connection to the client.
            /// </summary>
            Server_connection_failed = 3,

            /// <summary>
            /// Indicates that the server was not able to send broadcasts to the network.
            /// </summary>
            Server_broadcast_failed = 4
        }

        /// <summary>
        /// Checks if the given string is valid IP address.
        /// </summary>
        /// <param name="s">The given string.</param>
        /// <returns>A boolean indicating whether the given string is valid IP address or not.</returns>
        public static bool IsValidIPAddress(string s)
        {
            IPAddress temp;

            return IPAddress.TryParse(s, out temp);
        }

        /// <summary>
        /// Checks if the given string is valid port.
        /// </summary>
        /// <param name="s">The given string.</param>
        /// <returns>A boolean indicating whether the given string is valid port or not.</returns>
        public static bool IsValidPort(string s)
        {
            int port;

            return int.TryParse(s, out port) && port >= 0 && port <= 65535;
        }

        /// <summary>
        /// Gets the request message for joining an online game.
        /// </summary>
        /// <returns>A message represented by a byte array.</returns>
        public static byte[] GetRequestForJoining()
        {
            Random rand = new Random();

            return new byte[] { (byte)MessageType.Request_for_joining, (byte)rand.Next(0, 256) };
        }

        /// <summary>
        /// Gets the response message for joining an online game.
        /// </summary>
        /// <param name="settings">The settings of the online game.</param>
        /// <returns>A message represented by a byte array.</returns>
        public static byte[] GetResponseForJoining(Settings settings)
        {
            Random rand = new Random(settings.GetHashCode());
            byte[] resp = new byte[4];

            resp[0] = (byte)MessageType.Request_accepted;
            resp[1] = (byte)rand.Next(0, 256);
            resp[2] = (byte)settings.AmountGameMapRows;
            resp[3] = (byte)settings.AmountGameMapColumns;

            return resp;
        }

        /// <summary>
        /// Gets the broadcast message for finding an open game.
        /// </summary>
        /// <returns>A message represented by a byte array.</returns>
        public static byte[] GetBroadcast()
        {
            return new byte[] { 10, 20 };
        }

        /// <summary>
        /// Gets the message for sending the next move.
        /// </summary>
        /// <param name="column">The column where the stone will be inserted.</param>
        /// <returns>A message represented by a byte array.</returns>
        public static byte[] GetMessageForNextMove(int column)
        {
            return new byte[] { (byte)MessageType.Next_move, (byte)column };
        }
    }
}
