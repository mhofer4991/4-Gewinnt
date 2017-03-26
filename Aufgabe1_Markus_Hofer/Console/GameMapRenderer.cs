//-----------------------------------------------------------------------
// <copyright file="GameMapRenderer.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class renders the map on a console.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe1_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class renders the map on a console.
    /// </summary>
    public class GameMapRenderer
    {
        /// <summary> X - position, where the rendering will begin. </summary>
        private int x;

        /// <summary> Y - position, where the rendering will begin. </summary>
        private int y;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameMapRenderer"/> class.
        /// </summary>
        /// <param name="stoneRenderer">An instance of a stone renderer.</param>
        public GameMapRenderer(StoneRenderer stoneRenderer)
        {
            this.StoneRenderer = stoneRenderer;

            this.SetPosition(0, 0);
        }

        /// <summary> Gets the stone renderer, which is needed for rendering the stones on the map. </summary>
        /// <value> The stone renderer, which is needed for rendering the stones on the map.</value>
        public StoneRenderer StoneRenderer { get; private set; }

        /// <summary>
        /// Sets the position where the rendering of the map begins.
        /// </summary>
        /// <param name="x">X - position, where the rendering will begin.</param>
        /// <param name="y">Y - position, where the rendering will begin.</param>
        public void SetPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Gets the position where the rendering of the map begins.
        /// </summary>
        /// <returns>An array containing the position where the rendering of the map begins.</returns>
        public int[] GetPosition()
        {
            return new int[] { this.x, this.y };
        }

        /// <summary>
        /// Renders the map on the console.
        /// </summary>
        /// <param name="map">Map, which will be rendered.</param>
        public void Render(GameMap map)
        {
            for (int i = 0; i < map.Rows; i++)
            {
                for (int j = 0; j < map.Columns; j++)
                {
                    this.RenderStoneEntry(
                        this.x + ((this.StoneRenderer.StoneWidth + 1) * j),
                        this.y + ((this.StoneRenderer.StoneHeight + 1) * i));

                    if (map.Field[i, j] != null)
                    {
                        this.StoneRenderer.Render(
                            map.Field[i, j],
                            this.x + ((this.StoneRenderer.StoneWidth + 1) * j) + 1,
                            this.y + ((this.StoneRenderer.StoneHeight + 1) * i));
                    }
                }
            }
        }

        /// <summary>
        /// Renders an entry for a single stone at the given position.
        /// </summary>
        /// <param name="x">X - position, where the rendering will begin.</param>
        /// <param name="y">Y - position, where the rendering will begin.</param>
        private void RenderStoneEntry(int x, int y)
        {
            string temp = string.Empty;

            for (int i = 0; i <= this.StoneRenderer.StoneHeight; i++)
            {
                Console.SetCursorPosition(x, y);

                temp = "|";

                for (int j = 0; j < this.StoneRenderer.StoneWidth; j++)
                {
                    if (i != this.StoneRenderer.StoneHeight)
                    {
                        temp += " ";
                    }
                    else
                    {
                        temp += "-";
                    }
                }

                temp += "|";

                Console.Write(temp);

                y++;
            }
        }
    }
}
