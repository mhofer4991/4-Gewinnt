//-----------------------------------------------------------------------
// <copyright file="Stone.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents a stone in a game map.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe1_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents a stone in a game map.
    /// </summary>
    public class Stone
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Stone"/> class.
        /// </summary>
        /// <param name="player">Player, which is the owner of this stone.</param>
        public Stone(Player player)
        {
            this.Player = player;
        }

        /// <summary> Gets the player, who is the owner of this stone. </summary>
        /// <value> The player, who is the owner of this stone.</value>
        public Player Player { get; private set; }

        /// <summary>
        /// Checks if the given stone belongs to the owner (player) of this stone.
        /// </summary>
        /// <param name="stone">The given stone which will be checked.</param>
        /// <returns>A boolean indicating whether the given stone belongs to the owner (player) of this stone.</returns>
        public bool IsSameTeamStone(Stone stone)
        {
            return this.Player.Equals(stone.Player);
        }
    }
}
