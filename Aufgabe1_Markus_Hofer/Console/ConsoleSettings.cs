//-----------------------------------------------------------------------
// <copyright file="ConsoleSettings.cs" company="Markus Hofer">
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
    public class ConsoleSettings
    {
        /// <summary> Gets the constant default color for the first player. </summary>
        public const ConsoleColor FirstPlayerDefaultColor = ConsoleColor.Red;

        /// <summary> Gets the constant default color for the second player. </summary>
        public const ConsoleColor SecondPlayerDefaultColor = ConsoleColor.Green;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleSettings"/> class.
        /// </summary>
        public ConsoleSettings()
        {
            this.PlayerThemes = new Dictionary<int, ConsoleColor>();

            this.SetThemeForPlayer(Game.FirstPlayerID, ConsoleSettings.FirstPlayerDefaultColor);
            this.SetThemeForPlayer(Game.SecondPlayerID, ConsoleSettings.SecondPlayerDefaultColor);
        }

        /// <summary> Gets the color themes of all players. </summary>
        /// <value> The color themes of all players.</value>
        public Dictionary<int, ConsoleColor> PlayerThemes { get; private set; }

        /// <summary>
        /// Sets the stone theme for the given player ID.
        /// </summary>
        /// <param name="id">The given player ID.</param>
        /// <param name="color">The color of the stones of the given player.</param>
        public void SetThemeForPlayer(int id, ConsoleColor color)
        {
            if (this.PlayerThemes.ContainsKey(id))
            {
                this.PlayerThemes[id] = color;
            }
            else
            {
                this.PlayerThemes.Add(id, color);
            }
        }
    }
}
