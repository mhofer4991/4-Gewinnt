//-----------------------------------------------------------------------
// <copyright file="Game.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents a connect four game with two players.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe1_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents a connect four game with two players.
    /// </summary>
    public class Game
    {
        /// <summary> Gets the constant ID for the first player. </summary>
        public const int FirstPlayerID = 1;

        /// <summary> Gets the constant ID for the second player. </summary>
        public const int SecondPlayerID = 2;

        /// <summary> Renderer which will display the game. </summary>
        private IDisplay display;

        /// <summary> The player who begins the game. </summary>
        private Player beginningPlayer;

        /// <summary> The second player of this game. </summary>
        private Player secondPlayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="map">Map where the game will be played.</param>
        /// <param name="display">Renderer which will display the game.</param>
        public Game(GameMap map, IDisplay display)
        {
            this.Map = map;

            this.display = display;
            this.IsRunning = false;
        }

        /// <summary>
        /// Delegate for event OnGameOver.
        /// </summary>
        /// <param name="winner">The player who has won the game. If there is no winner, this variable is null.</param>
        public delegate void GameOver(Player winner);

        /// <summary>
        /// Delegate for event OnGameCanceled.
        /// </summary>
        /// <param name="reason">The reason for the abort of the game.</param>
        public delegate void GameCanceled(GameAbortReason reason);

        /// <summary>
        /// Gets called when the game ended.
        /// </summary>
        public event GameOver OnGameOver;

        /// <summary>
        /// Gets called when the game did not end, but got canceled.
        /// </summary>
        public event GameCanceled OnGameCanceled;

        /// <summary>
        /// This enumeration contains different reasons for the abort of a game.
        /// </summary>
        public enum GameAbortReason
        {
            /// <summary>
            /// Indicates that a player of an online game has gone offline.
            /// </summary>
            Abort_due_to_connection_loss = 1
        }

        /// <summary> Gets the map of the game. </summary>
        /// <value> The map of the game. </value>
        public GameMap Map { get; private set; }

        /// <summary> Gets a value indicating whether the game is running or not. </summary>
        /// <value> A value indicating whether the game is running or not. </value>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Creates and starts a new game.
        /// </summary>
        /// <param name="beginningPlayer">The player who begins the game.</param>
        /// <param name="secondPlayer">The second player of this game.</param>
        /// <param name="settings">Specific settings for this game.</param>
        public void CreateNewGame(Player beginningPlayer, Player secondPlayer, Settings settings)
        {
            this.IsRunning = true;
            this.beginningPlayer = beginningPlayer;
            this.secondPlayer = secondPlayer;

            this.Map.CreateNewGameMap(settings.AmountGameMapRows, settings.AmountGameMapColumns);

            this.display.ShowGameMap(this.Map);

            while (this.IsRunning)
            {
                this.HandleRoundOfPlayer(beginningPlayer, secondPlayer);

                this.HandleRoundOfPlayer(secondPlayer, beginningPlayer);
            }
        }

        /// <summary>
        /// Stops a running game.
        /// </summary>
        public void StopGame()
        {
            this.IsRunning = false;

            this.beginningPlayer.StopGame();
            this.secondPlayer.StopGame();
        }

        /// <summary>
        /// Handles the round of the given player.
        /// </summary>
        /// <param name="player">The given player who has a turn.</param>
        /// <param name="enemy">The enemy of the given player.</param>
        private void HandleRoundOfPlayer(Player player, Player enemy)
        {
            int move = player.GetMove();
                        
            if (this.IsRunning)
            {
                if (player is OnlinePlayer && !((OnlinePlayer)player).IsOnline)
                {
                    this.StopGame();

                    if (this.OnGameCanceled != null)
                    {
                        this.OnGameCanceled(GameAbortReason.Abort_due_to_connection_loss);
                    }
                }
                else
                {
                    enemy.NotifyAboutMove(move);

                    this.display.ShowAnimatedInsertion(this.Map, player.GetNewStone(), move);

                    this.Map.Insert(player.GetNewStone(), move);

                    if (this.Map.TeamIsWinning(player.GetNewStone()))
                    {
                        this.StopGame();

                        if (this.OnGameOver != null)
                        {
                            this.OnGameOver(player);
                        }
                    }
                    else if (this.Map.IsFull())
                    {
                        this.StopGame();

                        if (this.OnGameOver != null)
                        {
                            this.OnGameOver(null);
                        }
                    }
                }
            }
        }
    }
}
