//-----------------------------------------------------------------------
// <copyright file="StoneRenderer.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class renders a stone on a console.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe1_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class renders a stone on a console.
    /// </summary>
    public class StoneRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoneRenderer"/> class.
        /// </summary>
        /// <param name="settings">Specific settings for the console window.</param>
        public StoneRenderer(ConsoleSettings settings)
        {
            this.Settings = settings;
        }

        /// <summary> Gets the width of a stone. </summary>
        /// <value> The width of a stone.</value>
        public int StoneWidth { get; private set; }

        /// <summary> Gets the height of a stone. </summary>
        /// <value> The height of a stone.</value>
        public int StoneHeight { get; private set; }

        /// <summary> Gets the specific settings for the console window. </summary>
        /// <value> The specific settings for the console window.</value>
        public ConsoleSettings Settings { get; private set; }

        /// <summary>
        /// Sets the size of a stone, which is needed for rendering it on the console.
        /// </summary>
        /// <param name="width">Width of the stone.</param>
        /// <param name="height">Height of the stone.</param>
        public void SetStoneSize(int width, int height)
        {
            this.StoneWidth = width;
            this.StoneHeight = height;
        }

        /// <summary>
        /// Renders the stone on the console.
        /// </summary>
        /// <param name="stone">Stone, which will be rendered.</param>
        /// <param name="x">X - position, where the rendering will begin.</param>
        /// <param name="y">Y - position, where the rendering will begin.</param>
        public void Render(Stone stone, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            ConsoleColor temp = Console.ForegroundColor;

            if (this.Settings.PlayerThemes.ContainsKey(stone.Player.ID))
            {
                Console.ForegroundColor = this.Settings.PlayerThemes[stone.Player.ID];
            }

            Console.Write(" XX ");
            Console.SetCursorPosition(x, y + 1);
            Console.Write("XXXX");
            Console.SetCursorPosition(x, y + 2);
            Console.Write(" XX ");

            Console.ForegroundColor = temp;
        }
    }
}