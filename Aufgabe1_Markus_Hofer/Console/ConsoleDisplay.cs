//-----------------------------------------------------------------------
// <copyright file="ConsoleDisplay.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents a display which will be rendered on a console.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe1_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents a display which will be rendered on a console.
    /// </summary>
    public class ConsoleDisplay : IDisplay
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleDisplay"/> class.
        /// </summary>
        /// <param name="settings">Specific settings for the console window.</param>
        public ConsoleDisplay(ConsoleSettings settings)
        {
            StoneRenderer stoneRenderer = new StoneRenderer(settings);

            stoneRenderer.SetStoneSize(4, 3);

            this.MapRenderer = new GameMapRenderer(stoneRenderer);
        }

        /// <summary> Gets the map renderer, which is needed for rendering the map on the console. </summary>
        /// <value> The map renderer, which is needed for rendering the map on the console.</value>
        public GameMapRenderer MapRenderer { get; private set; }

        /// <summary>
        /// Sets the position of the game map.
        /// </summary>
        /// <param name="x">X - position of the game map.</param>
        /// <param name="y">Y - position of the game map.</param>
        public void SetMapPosition(int x, int y)
        {
            this.MapRenderer.SetPosition(x, y);
        }

        /// <summary>
        /// Shows the animated insertion of a player's stone.
        /// </summary>
        /// <param name="map">The map, in which the stone will be inserted.</param>
        /// <param name="stone">The stone, which will be inserted.</param>
        /// <param name="column">The column, which at the stone will be inserted.</param>
        public void ShowAnimatedInsertion(GameMap map, Stone stone, int column)
        {
            StoneRenderer stoneRenderer = this.MapRenderer.StoneRenderer;

            int targetRow = map.GetRowForInsertion(column);

            if (targetRow >= 0)
            {
                for (int i = 0; i < ((stoneRenderer.StoneHeight + 1) * targetRow) + 1; i++)
                {
                    this.MapRenderer.Render(map);

                    stoneRenderer.Render(
                        stone,
                        this.MapRenderer.GetPosition()[0] + ((stoneRenderer.StoneWidth + 1) * column) + 1,
                        this.MapRenderer.GetPosition()[1] + i);

                    System.Threading.Thread.Sleep(25);
                }
            }
        }

        /// <summary>
        /// Shows the game map on a display.
        /// </summary>
        /// <param name="map">The game map which will be shown.</param>
        public void ShowGameMap(GameMap map)
        {
            this.MapRenderer.Render(map);
        }
    }
}
