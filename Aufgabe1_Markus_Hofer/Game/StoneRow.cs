//-----------------------------------------------------------------------
// <copyright file="StoneRow.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents a row of lined up stones.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe1_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents a row of lined up stones.
    /// </summary>
    public class StoneRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoneRow"/> class.
        /// </summary>
        /// <param name="row">List of integer arrays with x - and y - coordinates represents the row. </param>
        /// <param name="orientation">The orientation of the stone row.</param>
        public StoneRow(List<int[]> row, RowOrientation orientation)
        {
            this.Row = row;
            this.Orientation = orientation;
        }

        /// <summary>
        /// Describes the orientation of the row.
        /// </summary>
        public enum RowOrientation
        {
            /// <summary> Row is orientated horizontally (##). </summary>
            Horizontally,

            /// <summary> Row is orientated vertically (#)
            /// -                                      (#). </summary>
            Vertically,

            /// <summary> Row is represents a rising line ( #)
            /// -                                         (# ). </summary>
            Rising,

            /// <summary> Row is represents a falling line (# )
            /// -                                          ( #). </summary>
            Falling
        }

        /// <summary> Gets the row representing list of coordinates. </summary>
        /// <value> The row representing list of coordinates.</value>
        public List<int[]> Row { get; private set; }

        /// <summary> Gets the orientation of the row. </summary>
        /// <value> The orientation of the row.</value>
        public RowOrientation Orientation { get; private set; }
        
        /// <summary>
        /// This method scans the game map to find all lined up stones in 4 possible directions.
        /// </summary>
        /// <param name="map">The map which will be scanned.</param>
        /// <param name="stone">The type of stone to be compared.</param>
        /// <returns>A list of StoneRow instances.</returns>
        public static List<StoneRow> GetCoordinatesOfLinedUpStones(GameMap map, Stone stone)
        {
            Stone[,] temp = new Stone[map.Rows, map.Columns];

            List<StoneRow> foundRows = new List<StoneRow>();

            /* Search for ## rows (horizontal) */

            // Create a temporary map because found rows will be deleted.
            Array.Copy(map.Field, 0, temp, 0, map.Field.Length);

            // Beginning at the bottom left corner the map will be scanned.
            for (int i = map.Rows - 1; i >= 0; i--)
            {
                for (int j = 0; j < map.Columns; j++)
                {
                    // A stone has been found.
                    if (temp[i, j] != null)
                    {
                        int startX = j;

                        List<int[]> foundCoords = new List<int[]>();

                        // Go left until the field doesn't contain a stone of the same team.
                        while (startX >= 0 && (temp[i, startX] != null && temp[i, startX].IsSameTeamStone(stone)))
                        {
                            foundCoords.Add(new int[] { i, startX });

                            startX--;
                        }

                        startX = j + 1;

                        // Go right until the field doesn't contain a stone of the same team.
                        while (startX < map.Columns && (temp[i, startX] != null && temp[i, startX].IsSameTeamStone(stone)))
                        {
                            foundCoords.Add(new int[] { i, startX });

                            startX++;
                        }

                        // Remove the found row.
                        for (int k = 0; k < foundCoords.Count; k++)
                        {
                            temp[foundCoords[k][0], foundCoords[k][1]] = null;
                        }

                        // If two or more stones have been found, it's a row.
                        if (foundCoords.Count > 1)
                        {
                            foundRows.Add(new StoneRow(foundCoords, StoneRow.RowOrientation.Horizontally));
                        }
                    }
                }
            }

            /*Search for # rows (vertical)
                         #                     */

            // Create a temporary map because found rows will be deleted.
            Array.Copy(map.Field, 0, temp, 0, map.Field.Length);

            // Beginning at the bottom left corner the map will be scanned.
            for (int i = map.Rows - 1; i >= 0; i--)
            {
                for (int j = 0; j < map.Columns; j++)
                {
                    // A stone has been found.
                    if (temp[i, j] != null)
                    {
                        int startY = i;

                        List<int[]> foundCoords = new List<int[]>();

                        // Go up until the field doesn't contain a stone of the same team.
                        while (startY >= 0 && (temp[startY, j] != null && temp[startY, j].IsSameTeamStone(stone)))
                        {
                            foundCoords.Add(new int[] { startY, j });

                            startY--;
                        }

                        startY = i + 1;

                        // Go down until the field doesn't contain a stone of the same team.
                        while (startY < map.Rows && (temp[startY, j] != null && temp[startY, j].IsSameTeamStone(stone)))
                        {
                            foundCoords.Add(new int[] { startY, j });

                            startY++;
                        }

                        // Remove the found row.
                        for (int k = 0; k < foundCoords.Count; k++)
                        {
                            temp[foundCoords[k][0], foundCoords[k][1]] = null;
                        }

                        // If two or more stones have been found, it's a row.
                        if (foundCoords.Count > 1)
                        {
                            foundRows.Add(new StoneRow(foundCoords, StoneRow.RowOrientation.Vertically));
                        }
                    }
                }
            }

            /*Search for  # rows (rising)
                         #                     */

            Array.Copy(map.Field, 0, temp, 0, map.Field.Length);

            for (int i = map.Rows - 1; i >= 0; i--)
            {
                for (int j = 0; j < map.Columns; j++)
                {
                    if (temp[i, j] != null)
                    {
                        int startX = j;
                        int startY = i;

                        List<int[]> foundCoords = new List<int[]>();

                        while ((startY >= 0 && startX < map.Columns) && (temp[startY, startX] != null && temp[startY, startX].IsSameTeamStone(stone)))
                        {
                            foundCoords.Add(new int[] { startY, startX });

                            startY--;
                            startX++;
                        }

                        startY = i + 1;
                        startX = j - 1;

                        while ((startY < map.Rows && startX >= 0) && (temp[startY, startX] != null && temp[startY, startX].IsSameTeamStone(stone)))
                        {
                            foundCoords.Add(new int[] { startY, j });

                            startY++;
                            startX--;
                        }

                        for (int k = 0; k < foundCoords.Count; k++)
                        {
                            temp[foundCoords[k][0], foundCoords[k][1]] = null;
                        }

                        if (foundCoords.Count > 1)
                        {
                            foundRows.Add(new StoneRow(foundCoords, RowOrientation.Rising));
                        }
                    }
                }
            }

            /*Search for #  rows (falling)
                          #                    */

            Array.Copy(map.Field, 0, temp, 0, map.Field.Length);

            for (int i = map.Rows - 1; i >= 0; i--)
            {
                for (int j = 0; j < map.Columns; j++)
                {
                    if (temp[i, j] != null)
                    {
                        int startX = j;
                        int startY = i;

                        List<int[]> foundCoords = new List<int[]>();

                        while ((startY >= 0 && startX >= 0) && (temp[startY, startX] != null && temp[startY, startX].IsSameTeamStone(stone)))
                        {
                            foundCoords.Add(new int[] { startY, startX });

                            startY--;
                            startX--;
                        }

                        startY = i + 1;
                        startX = j + 1;

                        while ((startY < map.Rows && startX < map.Columns) && (temp[startY, startX] != null && temp[startY, startX].IsSameTeamStone(stone)))
                        {
                            foundCoords.Add(new int[] { startY, j });

                            startY++;
                            startX++;
                        }

                        for (int k = 0; k < foundCoords.Count; k++)
                        {
                            temp[foundCoords[k][0], foundCoords[k][1]] = null;
                        }

                        if (foundCoords.Count > 1)
                        {
                            foundRows.Add(new StoneRow(foundCoords, RowOrientation.Falling));
                        }
                    }
                }
            }

            return foundRows;
        }

        /// <summary>
        /// Checks if the row contains a stone at the given position.
        /// </summary>
        /// <param name="y">Y - coordinate of the given position.</param>
        /// <param name="x">X - coordinate of the given position.</param>
        /// <returns>A boolean indicating whether the row contains a stone at the given position or not.</returns>
        public bool ContainsStoneAt(int y, int x)
        {
            foreach (int[] s in this.Row)
            {
                if (s[0] == y && s[1] == x)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
