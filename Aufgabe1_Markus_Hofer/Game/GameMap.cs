//-----------------------------------------------------------------------
// <copyright file="GameMap.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents the game map.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe1_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents the game map.
    /// </summary>
    public class GameMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameMap"/> class.
        /// </summary>
        public GameMap()
        {
            this.CreateNewGameMap(0, 0);
        }

        /// <summary> Gets the amount of rows of the game map. </summary>
        /// <value> The amount of rows of the game map.</value>
        public int Rows { get; private set; }

        /// <summary> Gets the amount of columns of the game map. </summary>
        /// <value> The amount of columns of the game map.</value>
        public int Columns { get; private set; }

        /// <summary> Gets the field of the game map as a stone array. </summary>
        /// <value> The field of the game map as a stone array.</value>
        public Stone[,] Field { get; private set; }

        /// <summary>
        /// Creates a new game map with the given size.
        /// </summary>
        /// <param name="rows">The amount of rows of the game map.</param>
        /// <param name="columns">The amount of columns of the game map.</param>
        public void CreateNewGameMap(int rows, int columns)
        {
            this.Rows = rows;
            this.Columns = columns;

            this.Field = new Stone[this.Rows, this.Columns];
        }

        /// <summary>
        /// Inserts a player's stone at the given column.
        /// </summary>
        /// <param name="stone">The stone, which will be inserted.</param>
        /// <param name="column">The column, at which the stone will be inserted.</param>
        public void Insert(Stone stone, int column)
        {
            int foundRow = this.GetRowForInsertion(column);

            if (foundRow >= 0 && foundRow < this.Columns)
            {
                this.Field[foundRow, column] = stone;
            }
        }

        /// <summary>
        /// Returns the row of the game map where the stone would land after inserted at a specific column.
        /// </summary>
        /// <param name="column">The specific column.</param>
        /// <returns>The row of the game map where the stone would land after inserted at a specific column.</returns>
        public int GetRowForInsertion(int column)
        {
            int foundRow = -1;

            if (column >= 0 && column < this.Columns)
            {
                for (int i = this.Rows - 1; i >= 0; i--)
                {
                    if (foundRow < 0 && this.Field[i, column] == null)
                    {
                        foundRow = i;
                    }
                }
            }

            return foundRow;
        }

        /// <summary>
        /// Decides whether the given team has won or not.
        /// </summary>
        /// <param name="stone">The stone, which has been used by this team.</param>
        /// <returns>A boolean indicating whether the given team has won or not.</returns>
        public bool TeamIsWinning(Stone stone)
        {
            List<StoneRow> rows = StoneRow.GetCoordinatesOfLinedUpStones(this, stone);

            foreach (StoneRow row in rows)
            {
                if (row.Row.Count == 4)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a copy of this game map.
        /// </summary>
        /// <returns>A copy of this game map.</returns>
        public GameMap GetCopy()
        {
            GameMap map = new GameMap();

            map.CreateNewGameMap(this.Rows, this.Columns);

            Array.Copy(this.Field, 0, map.Field, 0, this.Field.Length);

            return map;
        }

        /// <summary>
        /// Checks if the game map is full.
        /// </summary>
        /// <returns>A boolean indicating whether the game map is full or not.</returns>
        public bool IsFull()
        {
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Columns; j++)
                {
                    if (this.Field[i, j] == null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
