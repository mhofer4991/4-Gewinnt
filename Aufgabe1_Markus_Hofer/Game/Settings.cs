//-----------------------------------------------------------------------
// <copyright file="Settings.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class contains properties which can be changed.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe1_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class contains properties which can be changed.
    /// </summary>
    public class Settings
    {
        /// <summary> Gets the constant default amount of rows. </summary>
        public const int DefaultAmountOfRows = 5;

        /// <summary> Gets the constant default amount of columns. </summary>
        public const int DefaultAmountOfColumns = 7;

        /// <summary> Gets the minimum amount of rows. </summary>
        public const int MinAmountOfRows = 4;

        /// <summary> Gets the minimum amount of columns. </summary>
        public const int MinAmountOfColumns = 6;

        /// <summary> Gets the maximum amount of rows. </summary>
        public const int MaxAmountOfRows = 8;

        /// <summary> Gets the maximum amount of columns. </summary>
        public const int MaxAmountOfColumns = 15;

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            this.SetAmountOfGameMapRows(Settings.DefaultAmountOfRows);
            this.SetAmountOfGameMapColumns(Settings.DefaultAmountOfColumns);
        }

        /// <summary> Gets the amount of rows of the game map. </summary>
        /// <value> The amount of rows of the game map.</value>
        public int AmountGameMapRows { get; private set; }

        /// <summary> Gets the amount of columns of the game map. </summary>
        /// <value> The amount of columns of the game map.</value>
        public int AmountGameMapColumns { get; private set; }

        /// <summary>
        /// Sets the amount of rows of the game map.
        /// </summary>
        /// <param name="amountGameMapRows">The new amount of game map rows.</param>
        public void SetAmountOfGameMapRows(int amountGameMapRows)
        {
            this.AmountGameMapRows = amountGameMapRows;
        }

        /// <summary>
        /// Sets the amount of columns of the game map.
        /// </summary>
        /// <param name="amountGameMapColumns">The new amount of game map columns.</param>
        public void SetAmountOfGameMapColumns(int amountGameMapColumns)
        {
            this.AmountGameMapColumns = amountGameMapColumns;
        }
    }
}
