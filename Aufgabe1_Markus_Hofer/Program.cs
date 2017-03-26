//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>The user can use this program to play the game Connect Four against the computer or another human, either local or over the network.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe1_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The user can use this program to play the game Connect Four against the computer or another human, either local or over the network.
    /// </summary>
    public class Program
    {
        /// <summary> Saves settings for console appearance, like color of the stones. </summary>
        private static ConsoleSettings consoleSettings;

        /// <summary> Saves settings for the game, like amount of rows and columns. </summary>
        private static Settings gameSettings;

        /// <summary> Will be used to display the game on the console. </summary>
        private static ConsoleDisplay display;

        /// <summary> Will be used to get input from the user. </summary>
        private static ConsoleUserInput consoleUserInput;

        /// <summary> The map of a game. </summary>
        private static GameMap map;

        /// <summary> Is needed for creating new games. </summary>
        private static Game game;

        /// <summary> Is needed for deciding which player starts the game. </summary>
        private static Random rand;

        /// <summary> The client can look for and connect to a server. </summary>
        private static GameClient client;

        /// <summary> The server offers the hosting of connect four games in the network. </summary>
        private static GameServer server;

        /// <summary>
        /// This method represents the entry point of the program.
        /// </summary>
        /// <param name="args">Array of command line arguments.</param>
        private static void Main(string[] args)
        {
            Console.WindowHeight = 40;

            rand = new Random();

            map = new GameMap();

            consoleSettings = new ConsoleSettings();

            gameSettings = new Settings();

            display = new ConsoleDisplay(consoleSettings);
            display.SetMapPosition(1, 7);

            game = new Game(map, display);

            game.OnGameOver += Game_OnGameOver;
            game.OnGameCanceled += Game_OnGameCanceled;

            consoleUserInput = new ConsoleUserInput(map, display.MapRenderer);

            consoleUserInput.OnGameCanceled += ConsoleUserInput_OnGameCanceled;
            consoleUserInput.OnNewGameRequested += ConsoleUserInput_OnNewGameRequested;

            consoleUserInput.OnNewOfflineSinglePlayerGameRequested += ConsoleUserInput_OnNewOfflineSinglePlayerGameRequested;
            consoleUserInput.OnNewOfflineMultiPlayerGameRequested += ConsoleUserInput_OnNewOfflineMultiPlayerGameRequested;

            consoleUserInput.OnGameServerCreationRequested += ConsoleUserInput_OnGameServerCreationRequested;
            consoleUserInput.OnGameServerJoiningRequested += ConsoleUserInput_OnGameServerJoiningRequested;
            consoleUserInput.OnGameServerSearchRequested += ConsoleUserInput_OnGameServerSearchRequested;

            consoleUserInput.OnSettingsChanged += ConsoleUserInput_OnSettingsChanged;
            consoleUserInput.OnConsoleSettingsChanged += ConsoleUserInput_OnConsoleSettingsChanged;

            server = new GameServer();
            server.OnPlayerJoinedHostedGame += Server_OnPlayerJoinedHostedGame;
            server.OnNetworkErrorOccurred += Server_OnNetworkErrorOccurred;

            client = new GameClient();
            client.OnGameEstablished += Client_OnGameEstablished;
            client.OnNetworkErrorOccurred += Client_OnNetworkErrorOccurred;

            consoleUserInput.DrawMainMenu();
        }

        /// <summary>
        /// Gets called when the user changed some settings of the console appearance.
        /// </summary>
        /// <param name="settings">The settings, which have been applied by the user.</param>
        private static void ConsoleUserInput_OnConsoleSettingsChanged(ConsoleSettings settings)
        {
            foreach (int id in settings.PlayerThemes.Keys)
            {
                consoleSettings.SetThemeForPlayer(id, settings.PlayerThemes[id]);
            }
        }

        /// <summary>
        /// Gets called when the user changed some settings of the game.
        /// </summary>
        /// <param name="settings">The settings, which have been applied by the user.</param>
        private static void ConsoleUserInput_OnSettingsChanged(Settings settings)
        {
            gameSettings.SetAmountOfGameMapRows(settings.AmountGameMapRows);
            gameSettings.SetAmountOfGameMapColumns(settings.AmountGameMapColumns);
        }

        /// <summary>
        /// Gets called when a game has been canceled without the intention of the user.
        /// </summary>
        /// <param name="reason">The reason, why the game has been canceled.</param>
        private static void Game_OnGameCanceled(Game.GameAbortReason reason)
        {
            consoleUserInput.SetIsDuringGame(false);

            Console.SetCursorPosition(0, 1);
            Console.Write("                                                                         ");

            Console.SetCursorPosition(1, 1);

            switch (reason)
            {
                case Game.GameAbortReason.Abort_due_to_connection_loss:
                    Console.WriteLine(" The connection to an online player has been lost!\n  Press [Enter] to continue...");

                    break;
            }

            Console.ReadLine();

            consoleUserInput.DrawMainMenu();
        }

        /// <summary>
        /// Gets called when the user wants to create a new game server.
        /// </summary>
        private static void ConsoleUserInput_OnGameServerCreationRequested()
        {
            Console.Clear();

            Console.WriteLine("\n > New game server has been created!");

            Console.WriteLine("\n > Waiting for a player to start the game...");

            Console.WriteLine("\n Press [Esc] to stop the server.");

            server.CreateNewGame(gameSettings);

            while (server.IsSending)
            {
                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    {
                        server.StopWaiting();

                        Console.WriteLine("\n > Server stopped...");

                        System.Threading.Thread.Sleep(1000);

                        consoleUserInput.DrawNewGameOptions();
                    }
                }

                System.Threading.Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Gets called when the user wants to join a server direct by entering an IP address.
        /// </summary>
        private static void ConsoleUserInput_OnGameServerJoiningRequested()
        {
            Console.Clear();

            Console.WriteLine("\n Enter the IP address of the server you want to connect to: ");
            Console.WriteLine(" To cancel, just enter whitespaces! ");

            string ip = "temp";

            while (!(Protocol.IsValidIPAddress(ip) || string.IsNullOrWhiteSpace(ip)))
            {
                Console.Write("\n > Address: ");

                ip = Console.ReadLine();
            }

            if (!string.IsNullOrWhiteSpace(ip))
            {
                client.Connect(ip);
            }
            else
            {
                consoleUserInput.DrawNewGameOptions();
            }
        }

        /// <summary>
        /// Gets called when the user wants to search for an open game in the network.
        /// </summary>
        private static void ConsoleUserInput_OnGameServerSearchRequested()
        {
            Console.Clear();

            Console.WriteLine("\n > Searching for an open game...");

            Console.WriteLine("\n Press [Esc] to stop searching for an open game.");

            client.SearchForOpenGames();

            while (client.IsSearching)
            {
                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    {
                        client.StopSearching();

                        Console.WriteLine("\n > The search has been canceled...");

                        System.Threading.Thread.Sleep(1000);

                        consoleUserInput.DrawNewGameOptions();
                    }
                }

                System.Threading.Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Gets called when the client connected successful to a server.
        /// </summary>
        /// <param name="player">The player, who hosted the game.</param>
        /// <param name="settings">The settings, which will be used for this game.</param>
        private static void Client_OnGameEstablished(OnlinePlayer player, Settings settings)
        {
            Console.WriteLine("\n > Open game found! To start the game, press [Enter].");

            while (Console.ReadKey().Key != ConsoleKey.Enter)
            {
            }

            // StartNewGame(new AI(Game.FirstPlayerID, player, map), player, settings);
            StartNewGame(new Human(Game.FirstPlayerID, consoleUserInput), player, settings);
        }

        /// <summary>
        /// Gets called when the client failed to connect to a server.
        /// </summary>
        /// <param name="error">The reason, why the client failed.</param>
        private static void Client_OnNetworkErrorOccurred(Protocol.ErrorType error)
        {
            Console.WriteLine("\n > A Network error occurred!");

            switch (error)
            {
                case Protocol.ErrorType.Client_broadcast_failed:
                    Console.WriteLine("\n > The client was unable to listen to the network.");

                    break;
                case Protocol.ErrorType.Client_connection_failed:
                    Console.WriteLine("\n > The client was unable to connect to the server.");

                    break;
            }

            Console.WriteLine("\n > Press [Enter] to continue...");

            while (Console.ReadKey().Key != ConsoleKey.Enter)
            {
            }

            consoleUserInput.DrawNewGameOptions();
        }

        /// <summary>
        /// Gets called when a client joined the hosted game of the server.
        /// </summary>
        /// <param name="player">The player, who has joined the game.</param>
        /// <param name="settings">The settings, which will be used for this game.</param>
        private static void Server_OnPlayerJoinedHostedGame(OnlinePlayer player, Settings settings)
        {
            Console.WriteLine("\n > Player found! To start the game, press [Enter].");

            while (Console.ReadKey().Key != ConsoleKey.Enter)
            {
            }

            // StartNewGame(new AI(Game.FirstPlayerID, player, map), player, settings);
            StartNewGame(new Human(Game.FirstPlayerID, consoleUserInput), player, settings);
        }

        /// <summary>
        /// Gets called when the hosting of a game server failed.
        /// </summary>
        /// <param name="error">The reason, why the hosting failed.</param>
        private static void Server_OnNetworkErrorOccurred(Protocol.ErrorType error)
        {
            Console.WriteLine("\n > A Network error occurred!");

            switch (error)
            {
                case Protocol.ErrorType.Server_broadcast_failed:
                    Console.WriteLine("\n > The server was unable to host the server.");

                    break;
                case Protocol.ErrorType.Server_connection_failed:
                    Console.WriteLine("\n > The server was unable to maintain the connection to a client.");

                    break;
            }

            Console.WriteLine("\n > Press [Enter] to continue...");

            while (Console.ReadKey().Key != ConsoleKey.Enter)
            {
            }

            consoleUserInput.DrawNewGameOptions();
        }

        /// <summary>
        /// Gets called when the user wants to start a new offline game with one human player.
        /// </summary>
        private static void ConsoleUserInput_OnNewOfflineMultiPlayerGameRequested()
        {
            StartNewGame(new Human(Game.FirstPlayerID, consoleUserInput), new Human(Game.SecondPlayerID, consoleUserInput), gameSettings);
        }

        /// <summary>
        /// Gets called when the user wants to start a new offline game with two human players.
        /// </summary>
        private static void ConsoleUserInput_OnNewOfflineSinglePlayerGameRequested()
        {
            Player player1 = new Human(Game.FirstPlayerID, consoleUserInput);
            StartNewGame(player1, new AI(Game.SecondPlayerID, player1, map), gameSettings);
        }

        /// <summary>
        /// Starts a new game with the given players and settings.
        /// </summary>
        /// <param name="player1">The first given player of the game.</param>
        /// <param name="player2">The second player of the game.</param>
        /// <param name="settings">The settings, which will be used for this game.</param>
        private static void StartNewGame(Player player1, Player player2, Settings settings)
        {
            consoleUserInput.SetIsDuringGame(true);

            Console.Clear();

            consoleUserInput.DrawTopMenuDuringGame();

            if (player2 is OnlinePlayer)
            {
                player1.StartGame(!player2.IsBeginning);
            }
            else
            {
                if (rand.Next(0, 100) % 2 == 0)
                {
                    player1.StartGame(false);
                    player2.StartGame(true);
                }
                else
                {
                    player1.StartGame(true);
                    player2.StartGame(false);
                }
            }
            
            if (player1.IsBeginning)
            {
                game.CreateNewGame(player1, player2, settings);
            }
            else
            {
                game.CreateNewGame(player2, player1, settings);
            }
        }

        /// <summary>
        /// Gets called when a game is over. If there is no winner, it is a tie.
        /// </summary>
        /// <param name="winner">The winner of the game.</param>
        private static void Game_OnGameOver(Player winner)
        {
            consoleUserInput.SetIsDuringGame(false);

            Console.SetCursorPosition(0, 1);
            Console.Write("                                                                         ");

            Console.SetCursorPosition(1, 1);

            if (winner == null)
            {
                Console.WriteLine("The game ended with a tie! Press [Enter] to continue...");
            }
            else
            {
                Console.WriteLine("The winner of the game is player {0}! Press [Enter] to continue...", winner.ID);
            }

            Console.ReadLine();

            consoleUserInput.DrawMainMenu();
        }

        /// <summary>
        /// Gets called when the user wants to start a new game. This can happen during a running game or in the main menu.
        /// </summary>
        private static void ConsoleUserInput_OnNewGameRequested()
        {
            consoleUserInput.SetIsDuringGame(false);

            if (game.IsRunning)
            {
                game.StopGame();
            }

            consoleUserInput.DrawNewGameOptions();
        }

        /// <summary>
        /// Gets called when the user cancels a running game.
        /// </summary>
        private static void ConsoleUserInput_OnGameCanceled()
        {
            consoleUserInput.SetIsDuringGame(false);

            if (game.IsRunning)
            {
                game.StopGame();
            }

            consoleUserInput.DrawMainMenu();
        }
    }
}
