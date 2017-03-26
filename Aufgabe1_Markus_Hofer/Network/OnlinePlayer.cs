//-----------------------------------------------------------------------
// <copyright file="OnlinePlayer.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>Represents a connect four player, which plays over the network and therefore doesn't need any user input.</summary>
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
    /// Represents a connect four player, which plays over the network and therefore doesn't need any user input.
    /// </summary>
    public class OnlinePlayer : Player
    {
        /// <summary> The online player which wants to play connect four. </summary>
        private TcpClient player;

        /// <summary> The network stream of the online player. </summary>
        private NetworkStream playerStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnlinePlayer"/> class.
        /// </summary>
        /// <param name="id">ID of the player.</param>
        /// <param name="player">The connected online player.</param>
        public OnlinePlayer(int id, TcpClient player) : base(id)
        {
            this.player = player;
            this.playerStream = player.GetStream();

            this.IsOnline = true;
        }

        /// <summary> Gets a value indicating whether the player is online or not. </summary>
        /// <value> A value indicating whether the player is online or not. </value>
        public bool IsOnline { get; private set; }

        /// <summary>
        /// Gets the column where the player wants to insert its next stone.
        /// </summary>
        /// <returns>The column where the player wants to insert its next stone.</returns>
        public override int GetMove()
        {
            byte[] move = new byte[2];

            try
            {
                if (this.playerStream.Read(move, 0, move.Length) == move.Length)
                {
                    if (move[0] == (byte)Protocol.MessageType.Next_move)
                    {
                        return move[1];
                    }
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (System.IO.IOException)
            {
            }
            catch (SocketException)
            {
            }

            // The online player also goes offline if he just sends invalid messages.
            this.IsOnline = false;
            this.StopGame();

            return -1;
        }

        /// <summary>
        /// Informs the player about which move the other player just made.
        /// </summary>
        /// <param name="column">The column where the player inserted it's stone.</param>
        public override void NotifyAboutMove(int column)
        {
            base.NotifyAboutMove(column);

            byte[] move = Protocol.GetMessageForNextMove(column);

            try
            {
                this.playerStream.Write(move, 0, move.Length);
                this.playerStream.Flush();

                return;
            }
            catch (ObjectDisposedException)
            {
            }
            catch (System.IO.IOException)
            {
            }
            catch (SocketException)
            {
            }

            this.IsOnline = false;
            this.StopGame();
        }

        /// <summary>
        /// Informs the player that the game has been canceled.
        /// </summary>
        public override void StopGame()
        {
            base.StopGame();

            this.player.Close();

            this.IsOnline = false;
        }
    }
}
