//-----------------------------------------------------------------------
// <copyright file="IUserInput.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>Classes, which implement this interface, are waiting for user input and return some result.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe1_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Classes, which implement this interface, are waiting for user input and return some result.
    /// </summary>
    public interface IUserInput
    {
        /// <summary>
        /// Gets the column where the player wants to insert its next stone.
        /// </summary>
        /// <param name="player">The player, which has to select the column.</param>
        /// <returns>The column where the player wants to insert its next stone.</returns>
        int GetColumnForNextStoneInsertion(Player player);
    }
}
