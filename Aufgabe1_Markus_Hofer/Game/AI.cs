//-----------------------------------------------------------------------
// <copyright file="AI.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>Represents a connect four player, which owns artificial intelligence and therefore doesn't need any user input.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe1_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a connect four player, which owns artificial intelligence and therefore doesn't need any user input.
    /// </summary>
    public class AI : Player
    {
        /// <summary> Is needed for calculating the next move. </summary>
        private GameMap map;

        /// <summary> The enemy of this AI. </summary>
        private Player enemy;

        /// <summary>
        /// Initializes a new instance of the <see cref="AI"/> class.
        /// </summary>
        /// <param name="id">ID of the player.</param>
        /// <param name="enemy">The enemy of this AI.</param>
        /// <param name="map">Is needed for calculating the next move.</param>
        public AI(int id, Player enemy, GameMap map) : base(id)
        {
            this.enemy = enemy;
            this.map = map;
        }

        /// <summary>
        /// Gets the column where the player wants to insert its next stone.
        /// </summary>
        /// <returns>The column where the player wants to insert its next stone.</returns>
        public override int GetMove()
        {
            int foundColumn = 0;
            GameMap mapCopy = this.map.GetCopy();

            // Is it possible to win with this move?
            if ((foundColumn = this.GetColumnForVictory(mapCopy, this.GetNewStone(), 0)) > -1)
            {
                return foundColumn;
            }

            // Is it possible for the enemy to win after this move?
            if ((foundColumn = this.GetColumnForVictory(mapCopy, this.enemy.GetNewStone(), 0)) > -1)
            {
                return foundColumn;
            }

            // Is it possible for the enemy to create a tricky row after this move?
            if ((foundColumn = this.GetColumnForTrickyRow(mapCopy, this.enemy.GetNewStone(), 0)) > -1)
            {
                return foundColumn;
            }

            // This list contains all columns which are either dangerous or completely filled.
            List<int> excludedColumns = new List<int>();

            for (int i = 0; i < this.map.Columns; i++)
            {
                GameMap temp = mapCopy.GetCopy();

                if (temp.GetRowForInsertion(i) == -1)
                {
                    excludedColumns.Add(i);
                }
                else
                {
                    temp.Insert(this.GetNewStone(), i);

                    if (this.GetColumnForVictory(temp, this.enemy.GetNewStone(), 0) > -1 ||
                        this.GetColumnForTrickyRow(temp, this.enemy.GetNewStone(), 0) > -1)
                    {
                        excludedColumns.Add(i);
                    }
                }
            }

            // This list contains all columns which are available to be filled.
            List<int> possibleColumns = new List<int>();

            for (int i = 0; i < this.map.Columns; i++)
            {
                if (!excludedColumns.Contains(i))
                {
                    possibleColumns.Add(i);
                }
            }

            // It seems that the defeat cannot be avoided.
            if (possibleColumns.Count == 0)
            {
                for (int i = 0; i < this.map.Columns; i++)
                {
                    if (mapCopy.GetRowForInsertion(i) > -1)
                    {
                        return i;
                    }
                }
            }

            // Is it possible to create a tricky row with this move?
            do
            {
                if ((foundColumn = this.GetColumnForTrickyRow(mapCopy, this.GetNewStone(), foundColumn + 1)) > -1)
                {
                    // But we have to consider that this move must not help the enemy to win.
                    if (!excludedColumns.Contains(foundColumn))
                    {
                        return foundColumn;
                    }
                }
            }
            while (foundColumn >= 0 && foundColumn < this.map.Columns);

            // Look for a column to build a row, which is as near as possible to the center.
            int center = this.map.Columns / 2;
            int centerDistance = this.map.Columns;
            foundColumn = -1;

            foreach (int i in possibleColumns)
            {
                GameMap temp = mapCopy.GetCopy();
                List<StoneRow> foundRows;

                int[] stonePos = new int[] { temp.GetRowForInsertion(i), i };

                temp.Insert(this.GetNewStone(), i);

                foundRows = StoneRow.GetCoordinatesOfLinedUpStones(temp, this.GetNewStone());

                foreach (StoneRow row in foundRows)
                {
                    if (row.Row.Count >= 2 &&
                        row.ContainsStoneAt(stonePos[0], stonePos[1]))
                    {
                        if (Math.Abs(center - i) < centerDistance)
                        {
                            centerDistance = Math.Abs(center - i);
                            foundColumn = i;
                        }
                    }
                }
            }

            if (foundColumn >= 0)
            {
                return foundColumn;
            }

            // Look for a column to insert, which is as near as possible to the center.
            centerDistance = this.map.Columns;

            foreach (int i in possibleColumns)
            {
                if (Math.Abs(center - i) < centerDistance)
                {
                    centerDistance = Math.Abs(center - i);
                    foundColumn = i;
                }           
            }

            if (foundColumn >= 0)
            {
                return foundColumn;
            }

            return -1;
        }

        /// <summary>
        /// Looks for a column where a player has to insert a stone to win.
        /// </summary>
        /// <param name="gameMap">The map which will be used for the check.</param>
        /// <param name="stone">The given stone of a team.</param>
        /// <param name="startColumn">The column where the search will begin.</param>
        /// <returns>The column where a player has to insert a stone to win.</returns>
        private int GetColumnForVictory(GameMap gameMap, Stone stone, int startColumn)
        {
            GameMap copy;
            List<StoneRow> foundRows;

            for (int i = startColumn; i < gameMap.Columns; i++)
            {
                // Inserts a temporary stone at the current column and looks if it creates a row for victory.
                copy = gameMap.GetCopy();

                copy.Insert(stone, i);

                foundRows = StoneRow.GetCoordinatesOfLinedUpStones(copy, stone);

                foreach (StoneRow row in foundRows)
                {
                    if (row.Row.Count >= 4)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Looks for a column where a player has to insert a stone to create a tricky row.
        /// </summary>
        /// <param name="gameMap">The map which will be used for the check.</param>
        /// <param name="stone">The given stone of a team.</param>
        /// <param name="startColumn">The column where the search will begin.</param>
        /// <returns>The column where a player has to insert a stone to create a tricky row.</returns>
        private int GetColumnForTrickyRow(GameMap gameMap, Stone stone, int startColumn)
        {
            GameMap copy;
            List<StoneRow> foundRows;

            /*
                Tricky row example: | | |#|#|#| | |

                It's impossible for the enemy to avoid it's defeat because the player can complete the row on both sides
            */

            for (int i = startColumn; i < gameMap.Columns; i++)
            {
                // Inserts a temporary stone at the current column and looks if it creates a tricky row.
                copy = gameMap.GetCopy();

                // Remember the exact position of the stone because it will be needed.
                int[] stonePos = new int[] { copy.GetRowForInsertion(i), i };

                copy.Insert(stone, i);

                foundRows = StoneRow.GetCoordinatesOfLinedUpStones(copy, stone);

                foreach (StoneRow row in foundRows)
                {
                    // Possible tricky row.
                    if (row.Row.Count == 4 - 1)
                    {
                        // A vertical tricky row is not possible.
                        if (row.Orientation != StoneRow.RowOrientation.Vertically)
                        {
                            GameMap temp = copy.GetCopy();

                            // Check if the row doesn't adjoin to the left or right side.
                            if (row.Row[0][1] > 0 && row.Row[2][1] < (gameMap.Columns - 1))
                            {
                                // Insert a stone at the begin and at the end of the found row.
                                temp.Insert(stone, row.Row[0][1] - 1);

                                temp.Insert(stone, row.Row[2][1] + 1);

                                List<StoneRow> tempRows = StoneRow.GetCoordinatesOfLinedUpStones(temp, stone);

                                foreach (StoneRow row2 in tempRows)
                                {
                                    if (row2.Row.Count >= 4 + 1 &&
                                        row.Orientation == row2.Orientation &&
                                        row.ContainsStoneAt(stonePos[0], stonePos[1]))
                                    {
                                        return i;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return -1;
        }
    }
}
