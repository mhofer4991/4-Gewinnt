//-----------------------------------------------------------------------
// <copyright file="Human.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>Represents a connect four player, which will be controlled by an user input.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe1_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a connect four player, which will be controlled by an user input.
    /// </summary>
    public class Human : Player
    {
        /// <summary> Is needed for getting the next move. </summary>
        private IUserInput userInput;

        /// <summary>
        /// Initializes a new instance of the <see cref="Human"/> class.
        /// </summary>
        /// <param name="id">ID of the player.</param>
        /// <param name="userInput">Is needed for getting the next move.</param>
        public Human(int id, IUserInput userInput)
            : base(id)
        {
            this.userInput = userInput;
        }

        /// <summary>
        /// Gets the column where the player wants to insert its next stone.
        /// </summary>
        /// <returns>The column where the player wants to insert its next stone.</returns>
        public override int GetMove()
        {
            return this.userInput.GetColumnForNextStoneInsertion(this);
        }
    }
}
