//-----------------------------------------------------------------------
// <copyright file="IDisplay.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>Classes, which implement this interface, are able to display different stages of a connect four game.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe1_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Classes, which implement this interface, are able to display different stages of a connect four game.
    /// </summary>
    public interface IDisplay
    {
        /// <summary>
        /// Shows the game map on a display.
        /// </summary>
        /// <param name="map">The game map which will be shown.</param>
        void ShowGameMap(GameMap map);
        
        /// <summary>
        /// Shows the animated insertion of a player's stone.
        /// </summary>
        /// <param name="map">The map, in which the stone will be inserted.</param>
        /// <param name="stone">The stone, which will be inserted.</param>
        /// <param name="column">The column, which at the stone will be inserted.</param>
        void ShowAnimatedInsertion(GameMap map, Stone stone, int column);
    }
}
