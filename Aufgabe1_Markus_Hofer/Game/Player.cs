//-----------------------------------------------------------------------
// <copyright file="Player.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>All derived classes are able to play a match of the game connect four.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe1_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// All derived classes are able to play a match of the game connect four.
    /// </summary>
    public abstract class Player
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="id">ID of the player.</param>
        public Player(int id)
        {
            this.ID = id;

            this.IsBeginning = false;
            this.IsPlaying = false;
        }

        /// <summary> Gets the ID of the player. </summary>
        /// <value> The ID of the player. </value>
        public int ID { get; private set; }

        /// <summary> Gets a value indicating whether the player is beginning or not. </summary>
        /// <value> A value indicating whether the player is beginning or not. </value>
        public bool IsBeginning { get; private set; }

        /// <summary> Gets a value indicating whether the player is playing or not. </summary>
        /// <value> A value indicating whether the player is playing or not. </value>
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// Creates a new stone, which is owned by this player.
        /// </summary>
        /// <returns>A new stone, which is owned by this player.</returns>
        public Stone GetNewStone()
        {
            return new Stone(this);
        }

        /// <summary>
        /// Informs the player that a game has been started.
        /// </summary>
        /// <param name="first">The player either makes the first move or not.</param>
        public virtual void StartGame(bool first)
        {
            this.IsPlaying = true;
            this.IsBeginning = first;
        }

        /// <summary>
        /// Informs the player that the game has been canceled.
        /// </summary>
        public virtual void StopGame()
        {
            this.IsPlaying = false;
        }

        /// <summary>
        /// Gets the column where the player wants to insert its next stone.
        /// </summary>
        /// <returns>The column where the player wants to insert its next stone.</returns>
        public abstract int GetMove();

        /// <summary>
        /// Informs the player about which move the other player just made.
        /// </summary>
        /// <param name="column">The column where the player inserted it's stone.</param>
        public virtual void NotifyAboutMove(int column)
        {
        }

        /// <summary>
        /// Decides if the given object is the same as this player.
        /// </summary>
        /// <param name="obj">The given object.</param>
        /// <returns>A boolean indicating whether the given object equals to this player.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Player)
            {
                return ((Player)obj).ID == this.ID;
            }

            return false;
        }
    }
}
