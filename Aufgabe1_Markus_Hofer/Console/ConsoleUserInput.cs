//-----------------------------------------------------------------------
// <copyright file="ConsoleUserInput.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class reads and handles input from the console.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe1_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This class reads and handles input from the console.
    /// </summary>
    public class ConsoleUserInput : IUserInput
    {
        /// <summary> Map is needed for column selection. </summary>
        private GameMap map;

        /// <summary> Map renderer is needed for knowing where to draw the cursor. </summary>
        private GameMapRenderer mapRenderer;

        /// <summary> Saves the indexes of the currently selected columns of the two players. </summary>
        private Dictionary<int, int> playerSelections;

        /// <summary> Is needed for changing game map size. </summary>
        private Settings gameSettings;

        /// <summary> Is needed for changing stone color. </summary>
        private ConsoleSettings consoleSettings;

        /// <summary> Indicates whether the user wants to exit the game. </summary>
        private bool cancelRequest;

        /// <summary> Indicates whether the user plays a game or not. </summary>
        private bool duringGame;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleUserInput"/> class.
        /// </summary>
        /// <param name="map">Map is needed for column selection.</param>
        /// <param name="mapRenderer">Map renderer is needed for knowing where to draw the cursor.</param>
        public ConsoleUserInput(GameMap map, GameMapRenderer mapRenderer)
        {
            this.map = map;
            this.mapRenderer = mapRenderer;

            this.gameSettings = new Settings();
            this.consoleSettings = new ConsoleSettings();

            this.playerSelections = new Dictionary<int, int>();
            this.playerSelections.Add(Game.FirstPlayerID, 0);
            this.playerSelections.Add(Game.SecondPlayerID, 0);

            Console.CancelKeyPress += this.Console_CancelKeyPress;

            Thread thread = new Thread(new ThreadStart(this.WaitForControlCommands));
            thread.Start();
        }

        /// <summary>
        /// Delegate for event OnGameCanceled.
        /// </summary>
        public delegate void GameCanceled();

        /// <summary>
        /// Delegate for event OnNewGameRequested.
        /// </summary>
        public delegate void NewGameRequested();

        /// <summary>
        /// Delegate for event OnNewOfflineSinglePlayerGameRequested.
        /// </summary>
        public delegate void NewOfflineSinglePlayerGameRequested();

        /// <summary>
        /// Delegate for event OnNewOfflineMultiPlayerGameRequested.
        /// </summary>
        public delegate void NewOfflineMultiPlayerGameRequested();

        /// <summary>
        /// Delegate for event OnGameServerCreationRequested.
        /// </summary>
        public delegate void GameServerCreationRequested();

        /// <summary>
        /// Delegate for event OnGameServerJoiningRequested.
        /// </summary>
        public delegate void GameServerJoiningRequested();

        /// <summary>
        /// Delegate for event OnGameServerSearchRequested.
        /// </summary>
        public delegate void GameServerSearchRequested();

        /// <summary>
        /// Delegate for event OnSettingsChanged.
        /// </summary>
        /// <param name="settings">The new settings.</param>
        public delegate void SettingsChanged(Settings settings);

        /// <summary>
        /// Delegate for event OnConsoleSettingsChanged.
        /// </summary>
        /// <param name="settings">The new console settings.</param>
        public delegate void ConsoleSettingsChanged(ConsoleSettings settings);

        /// <summary>
        /// Gets called when the user wants to exit a game.
        /// </summary>
        public event GameCanceled OnGameCanceled;

        /// <summary>
        /// Gets called when the user wants to start a new game.
        /// </summary>
        public event NewGameRequested OnNewGameRequested;

        /// <summary>
        /// Gets called when the user changes the settings.
        /// </summary>
        public event SettingsChanged OnSettingsChanged;

        /// <summary>
        /// Gets called when the user changes the console settings.
        /// </summary>
        public event ConsoleSettingsChanged OnConsoleSettingsChanged;

        /// <summary>
        /// Gets called when the user wants to start a new offline single player game.
        /// </summary>
        public event NewOfflineSinglePlayerGameRequested OnNewOfflineSinglePlayerGameRequested;

        /// <summary>
        /// Gets called when the user wants to start a new offline multi player game.
        /// </summary>
        public event NewOfflineMultiPlayerGameRequested OnNewOfflineMultiPlayerGameRequested;

        /// <summary>
        /// Gets called when the user wants to create a game server.
        /// </summary>
        public event GameServerCreationRequested OnGameServerCreationRequested;

        /// <summary>
        /// Gets called when the user wants to join a game server.
        /// </summary>
        public event GameServerJoiningRequested OnGameServerJoiningRequested;

        /// <summary>
        /// Gets called when the user wants to search for a game server.
        /// </summary>
        public event GameServerSearchRequested OnGameServerSearchRequested;

        /// <summary>
        /// Draws the main menu on the console.
        /// </summary>
        public void DrawMainMenu()
        {
            ConsoleKeyInfo cki = new ConsoleKeyInfo();
            bool validInput = false;
            
            while (!validInput)
            {
                Console.Clear();

                Console.WriteLine("\n [Ctrl + N]  Start a new game");
                Console.WriteLine("\n [Ctrl + O]  Change settings");
                Console.WriteLine("\n [Ctrl + W]  Exit program");

                cki = Console.ReadKey(true);

                if (cki.Modifiers == ConsoleModifiers.Control)
                {
                    validInput = true;

                    switch (cki.Key)
                    {
                        case ConsoleKey.N:
                            if (this.OnNewGameRequested != null)
                            {
                                this.OnNewGameRequested();
                            }

                            break;
                        case ConsoleKey.O:
                            this.DrawSettingsMenu();

                            break;
                        case ConsoleKey.W:
                            Environment.Exit(0);

                            break;
                        default:
                            validInput = false;

                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Inform if the user is currently in a game.
        /// </summary>
        /// <param name="isDuringGame">Decides if the user is currently in a game or not.</param>
        public void SetIsDuringGame(bool isDuringGame)
        {
            this.duringGame = isDuringGame;
        }

        /// <summary>
        /// Draws options for a new game.
        /// </summary>
        public void DrawNewGameOptions()
        {
            Console.Clear();

            Console.WriteLine("\n [ - ]  Start new offline game");
            Console.WriteLine("\n     [ 1 ]  with 1 human player");
            Console.WriteLine("\n     [ 2 ]  with 2 human players");
            Console.WriteLine("\n [ - ]  Online");
            Console.WriteLine("\n     [ 3 ]  Create new game and wait for an enemy");
            Console.WriteLine("\n     [ 4 ]  Join game via IP input");
            Console.WriteLine("\n     [ 5 ]  Search for a game and join it");
            Console.WriteLine("\n [Esc]  Go back to the main menu");

            ConsoleKeyInfo cki = new ConsoleKeyInfo();

            bool validInput = false;

            while (!validInput)
            {
                cki = Console.ReadKey(true);

                validInput = true;

                switch (cki.Key)
                {
                    case ConsoleKey.D1:
                        if (this.OnNewOfflineSinglePlayerGameRequested != null)
                        {
                            this.OnNewOfflineSinglePlayerGameRequested();
                        }

                        break;
                    case ConsoleKey.D2:
                        if (this.OnNewOfflineMultiPlayerGameRequested != null)
                        {
                            this.OnNewOfflineMultiPlayerGameRequested();
                        }

                        break;
                    case ConsoleKey.D3:
                        if (this.OnGameServerCreationRequested != null)
                        {
                            this.OnGameServerCreationRequested();
                        }

                        break;
                    case ConsoleKey.D4:
                        if (this.OnGameServerJoiningRequested != null)
                        {
                            this.OnGameServerJoiningRequested();
                        }

                        break;
                    case ConsoleKey.D5:
                        if (this.OnGameServerSearchRequested != null)
                        {
                            this.OnGameServerSearchRequested();
                        }

                        break;
                    case ConsoleKey.Escape:
                        this.DrawMainMenu();

                        break;
                    default:
                        validInput = false;

                        break;
                }
            }
        }
        
        /// <summary>
        /// Draws the menu which will be shown during a game.
        /// </summary>
        public void DrawTopMenuDuringGame()
        {
            Console.SetCursorPosition(0, 1);
            Console.Write("                                                                         ");

            Console.SetCursorPosition(1, 1);
            Console.WriteLine("[Ctrl + N] Start new game    [Ctrl + C] Exit game");
        }

        /// <summary>
        /// Gets the column where the player wants to insert its next stone.
        /// </summary>
        /// <param name="player">The player, which has to select the column.</param>
        /// <returns>The column where the player wants to insert its next stone.</returns>
        public int GetColumnForNextStoneInsertion(Player player)
        {
            ConsoleKeyInfo cki = new ConsoleKeyInfo();
            ConsoleColor cursorColor = ConsoleColor.Gray;

            if (this.mapRenderer.StoneRenderer.Settings.PlayerThemes.ContainsKey(player.ID))
            {
                cursorColor = this.mapRenderer.StoneRenderer.Settings.PlayerThemes[player.ID];
            }

            int beginDrawX;
            int beginDrawY;

            while (cki.Key != ConsoleKey.Enter)
            {
                if (!player.IsPlaying)
                {
                    return -1;
                }

                beginDrawX = this.mapRenderer.GetPosition()[0] + 1 + ((this.mapRenderer.StoneRenderer.StoneWidth + 1) * this.playerSelections[player.ID]) + ((this.mapRenderer.StoneRenderer.StoneWidth - 4) / 2);
                beginDrawY = this.mapRenderer.GetPosition()[1] - 4;
                                
                this.DrawCursor(beginDrawX, beginDrawY, cursorColor);

                cki = Console.ReadKey(true);

                this.DrawCursor(beginDrawX, beginDrawY, ConsoleColor.Black);
                
                switch (cki.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (this.playerSelections[player.ID] > 0)
                        {
                            this.playerSelections[player.ID]--;
                        }
                        else
                        {
                            this.playerSelections[player.ID] = this.map.Columns - 1;
                        }

                        break;
                    case ConsoleKey.RightArrow:
                        if (this.playerSelections[player.ID] < this.map.Columns - 1)
                        {
                            this.playerSelections[player.ID]++;
                        }
                        else
                        {
                            this.playerSelections[player.ID] = 0;
                        }

                        break;
                    case ConsoleKey.Enter:
                        if (this.map.GetRowForInsertion(this.playerSelections[player.ID]) < 0)
                        {
                            cki = new ConsoleKeyInfo();
                        }

                        break;
                    default:
                        this.HandleControlCommands(cki);

                        break;
                }
            }

            return this.playerSelections[player.ID];
        }

        /// <summary>
        /// Draws a selection cursor at the given position.
        /// </summary>
        /// <param name="beginDrawX">X - coordinate of the given position.</param>
        /// <param name="beginDrawY">Y - coordinate of the given position.</param>
        /// <param name="color">Color of the cursor.</param>
        private void DrawCursor(int beginDrawX, int beginDrawY, ConsoleColor color)
        {
            ConsoleColor temp = Console.ForegroundColor;

            Console.ForegroundColor = color;

            Console.SetCursorPosition(beginDrawX, beginDrawY);
            Console.WriteLine(" || ");

            Console.SetCursorPosition(beginDrawX, beginDrawY + 1);
            Console.WriteLine("_||_");

            Console.SetCursorPosition(beginDrawX, beginDrawY + 2);
            Console.WriteLine("\\  /");

            Console.SetCursorPosition(beginDrawX, beginDrawY + 3);
            Console.WriteLine(" \\/ ");

            Console.ForegroundColor = temp;
        }

        /// <summary>
        /// Gets called when the user presses Ctrl + C in a console window.
        /// </summary>
        /// <param name="sender">The object sending sender.</param>
        /// <param name="e">Arguments of this event.</param>
        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            
            if (this.duringGame)
            {
                this.cancelRequest = true;

                Console.SetCursorPosition(0, 1);
                Console.Write("                                                                         ");

                Console.SetCursorPosition(1, 1);
                Console.Write("Are you sure you want to exit this game  [Y / N]  ? ");
            }
        }

        /// <summary>
        /// Asks the user if he really wants to exit the game.
        /// </summary>
        /// <returns>A boolean indicating whether the user wants to exit or not.</returns>
        private bool PromptForExit()
        {
            Console.SetCursorPosition(0, 1);
            Console.Write("                                                                         ");

            Console.SetCursorPosition(1, 1);
            Console.Write("Are you sure you want to exit this game  [Y / N]  ? ");

            ConsoleKeyInfo cki = new ConsoleKeyInfo();

            while (true)
            {
                cki = Console.ReadKey(true);

                if (cki.Key == ConsoleKey.Y)
                {
                    return true;
                }
                else if (cki.Key == ConsoleKey.N)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Draws the settings menu on the console.
        /// </summary>
        private void DrawSettingsMenu()
        {
            ConsoleKeyInfo cki = new ConsoleKeyInfo();

            while (cki.Key != ConsoleKey.Escape)
            {
                Console.Clear();

                Console.WriteLine("\n [ S ]  Change size of the game map");
                Console.WriteLine("\n [ C ]  Change color of the stones");
                Console.WriteLine("\n\n\n [Esc]  Go back");

                cki = Console.ReadKey(true);

                switch (cki.Key)
                {
                    case ConsoleKey.S:
                        this.DrawSizeSettingsMenu();

                        break;
                    case ConsoleKey.C:
                        this.DrawColorSettingsMenu();

                        break;
                }
            }

            this.DrawMainMenu();
        }

        /// <summary>
        /// Draws the settings menu where the user can change the map size.
        /// </summary>
        private void DrawSizeSettingsMenu()
        {
            Console.Clear();

            Console.WriteLine("\n [ S ]  Change size of the game map");
            Console.WriteLine("\n\n\n\n\n\n\n [Up / Down]  Navigate");
            Console.WriteLine("\n [+ / -]      Increase / Decrease values");
            Console.WriteLine("\n [Esc]        Apply settings and go back");

            ConsoleKeyInfo cki = new ConsoleKeyInfo();
            int currentSelection = 0;

            int[] tempValues = new int[] { this.gameSettings.AmountGameMapRows, this.gameSettings.AmountGameMapColumns };

            while (cki.Key != ConsoleKey.Escape)
            {
                Console.SetCursorPosition(0, 2);

                if (currentSelection == 0)
                {
                    Console.WriteLine("\n     [ * ]  Amount of rows: {0}  [{1} - {2}]", tempValues[0], Settings.MinAmountOfRows, Settings.MaxAmountOfRows);
                    Console.WriteLine("\n     [   ]  Amount of columns: {0}  [{1} - {2}]", tempValues[1], Settings.MinAmountOfColumns, Settings.MaxAmountOfColumns);
                }
                else
                {
                    Console.WriteLine("\n     [   ]  Amount of rows: {0}  [{1} - {2}]", tempValues[0], Settings.MinAmountOfRows, Settings.MaxAmountOfRows);
                    Console.WriteLine("\n     [ * ]  Amount of columns: {0}  [{1} - {2}]", tempValues[1], Settings.MinAmountOfColumns, Settings.MaxAmountOfColumns);
                }

                cki = Console.ReadKey(true);

                switch (cki.Key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.DownArrow:
                        if (currentSelection == 0)
                        {
                            currentSelection = 1;
                        }
                        else
                        {
                            currentSelection = 0;
                        }

                        break;
                    case ConsoleKey.OemPlus:
                        if ((currentSelection == 0 && tempValues[0] < Settings.MaxAmountOfRows) ||
                            (currentSelection == 1 && tempValues[1] < Settings.MaxAmountOfColumns))
                        {
                            tempValues[currentSelection]++;
                        }

                        break;
                    case ConsoleKey.OemMinus:
                        if ((currentSelection == 0 && tempValues[0] > Settings.MinAmountOfRows) ||
                            (currentSelection == 1 && tempValues[1] > Settings.MinAmountOfColumns))
                        {
                            tempValues[currentSelection]--;
                        }

                        break;
                }
            }

            this.gameSettings.SetAmountOfGameMapRows(tempValues[0]);
            this.gameSettings.SetAmountOfGameMapColumns(tempValues[1]);

            if (this.OnSettingsChanged != null)
            {
                this.OnSettingsChanged(this.gameSettings);
            }
        }

        /// <summary>
        /// Draws the menu where the user can change the color of the stones.
        /// </summary>
        private void DrawColorSettingsMenu()
        {
            Console.Clear();

            Console.WriteLine("\n [ C ]  Change color of the stones");
            Console.WriteLine("\n\n\n\n\n\n\n [Up / Down]  Navigate");
            Console.WriteLine("\n [+ / -]      Increase / Decrease values");
            Console.WriteLine("\n [Esc]        Apply settings and go back");

            ConsoleKeyInfo cki = new ConsoleKeyInfo();
            int currentSelection = 0;

            int[] tempValues = new int[]
            {
                (int)this.consoleSettings.PlayerThemes[Game.FirstPlayerID],
                (int)this.consoleSettings.PlayerThemes[Game.SecondPlayerID]
            };

            while (cki.Key != ConsoleKey.Escape)
            {
                Console.SetCursorPosition(0, 2);

                if (currentSelection == 0)
                {
                    Console.Write("\n     [ * ]  Color of Player 1's stone: ");

                    Console.ForegroundColor = (ConsoleColor)tempValues[0];
                    Console.Write("####");
                    Console.ForegroundColor = ConsoleColor.Gray;

                    Console.WriteLine(" ({0})  [{1} - {2}]", tempValues[0], 1, (int)ConsoleColor.White);

                    Console.Write("\n     [   ]  Color of Player 2's stone: ");

                    Console.ForegroundColor = (ConsoleColor)tempValues[1];
                    Console.Write("####");
                    Console.ForegroundColor = ConsoleColor.Gray;

                    Console.WriteLine(" ({0})  [{1} - {2}]", tempValues[1], 1, (int)ConsoleColor.White);
                }
                else
                {
                    Console.Write("\n     [   ]  Color of Player 1's stone: ");

                    Console.ForegroundColor = (ConsoleColor)tempValues[0];
                    Console.Write("####");
                    Console.ForegroundColor = ConsoleColor.Gray;

                    Console.WriteLine(" ({0})  [{1} - {2}]", tempValues[0], 1, (int)ConsoleColor.White);

                    Console.Write("\n     [ * ]  Color of Player 2's stone: ");

                    Console.ForegroundColor = (ConsoleColor)tempValues[1];
                    Console.Write("####");
                    Console.ForegroundColor = ConsoleColor.Gray;

                    Console.WriteLine(" ({0})  [{1} - {2}]", tempValues[1], 1, (int)ConsoleColor.White);
                }

                cki = Console.ReadKey(true);

                switch (cki.Key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.DownArrow:
                        if (currentSelection == 0)
                        {
                            currentSelection = 1;
                        }
                        else
                        {
                            currentSelection = 0;
                        }

                        break;
                    case ConsoleKey.OemPlus:
                        if ((currentSelection == 0 && tempValues[0] < 15) ||
                            (currentSelection == 1 && tempValues[1] < 15))
                        {
                            tempValues[currentSelection]++;
                        }

                        break;
                    case ConsoleKey.OemMinus:
                        if ((currentSelection == 0 && tempValues[0] > 1) ||
                            (currentSelection == 1 && tempValues[1] > 1))
                        {
                            tempValues[currentSelection]--;
                        }

                        break;
                }
            }

            this.consoleSettings.SetThemeForPlayer(Game.FirstPlayerID, (ConsoleColor)tempValues[0]);
            this.consoleSettings.SetThemeForPlayer(Game.SecondPlayerID, (ConsoleColor)tempValues[1]);

            if (this.OnConsoleSettingsChanged != null)
            {
                this.OnConsoleSettingsChanged(this.consoleSettings);
            }
        }

        /// <summary>
        /// Waits for input which can be used during a game.
        /// </summary>
        private void WaitForControlCommands()
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    if (this.duringGame)
                    {
                        this.HandleControlCommands(Console.ReadKey());
                    }
                }

                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Looks if the user input is a valid command which can be used during a game.
        /// </summary>
        /// <param name="cki">Contains information about the user input.</param>
        private void HandleControlCommands(ConsoleKeyInfo cki)
        {
            switch (cki.Key)
            {
                case ConsoleKey.N:
                    if (cki.Modifiers == ConsoleModifiers.Control)
                    {
                        if (this.PromptForExit())
                        {
                            if (this.OnNewGameRequested != null)
                            {
                                this.OnNewGameRequested();
                            }
                        }
                        else
                        {
                            this.DrawTopMenuDuringGame();
                        }
                    }
                    else if (this.cancelRequest)
                    {
                        this.cancelRequest = false;

                        this.DrawTopMenuDuringGame();
                    }

                    break;
                case ConsoleKey.Y:
                    if (this.cancelRequest)
                    {
                        this.cancelRequest = false;

                        if (this.OnGameCanceled != null)
                        {
                            this.OnGameCanceled();
                        }
                    }

                    break;
            }
        }
    }
}
