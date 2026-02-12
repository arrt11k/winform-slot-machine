using SlotMachineAdminSystem.Models;
using System;
using System.Collections.Generic;

namespace SlotMachineAdminSystem.Services
{
    public class GameMachine
    {
        private readonly Random random = new Random();
        private readonly XmlDataManager dataManager;

        private List<Symbol> symbols;
        private ProbabilitySettings settings;

        public GameMachine(XmlDataManager xmlDataManager)
        {
            dataManager = xmlDataManager;
            settings = dataManager.LoadSettings();
            InitializeSymbols();
        }

        private void InitializeSymbols()
        {
            symbols = new List<Symbol>
            {
                new Symbol("Cherry", 2, "Resources/cherry.png"),
                new Symbol("Lemon", 3, "Resources/lemon.png"),
                new Symbol("Bell", 5, "Resources/bell.png"),
                new Symbol("Seven", 10, "Resources/seven.png")
            };
        }

        /// <summary>
        /// atgriež random simb
        /// </summary>
        public Symbol SpinPreviewSymbol()
        {
            return symbols[random.Next(symbols.Count)];
        }

        /// <summary>
        /// izdara griezinu
        /// </summary>
        public SpinResult Spin(Player player, decimal bet)
        {
            if (bet <= 0)
                throw new ArgumentException("Likmej jabūt vairak neka 0");

            if (player.Balance < bet)
                throw new InvalidOperationException("Nepietaks lidzeķļu");

            settings = dataManager.LoadSettings();

            bool isWinningSpin = random.Next(1, 101) <= settings.WinningProbability;

            Symbol s1, s2, s3;
            decimal winAmount = 0;

            if (isWinningSpin)
            {
                // garantets laimests
                Symbol winSymbol = symbols[random.Next(symbols.Count)];
                s1 = s2 = s3 = winSymbol;

                winAmount = bet * winSymbol.Multiplier;
            }
            else
            {
                // zaude- ramdom simboli
                s1 = symbols[random.Next(symbols.Count)];
                s2 = symbols[random.Next(symbols.Count)];
                s3 = symbols[random.Next(symbols.Count)];
            }

            player.Balance -= bet;       // nolasa bet
            player.Balance += winAmount; // +laimets
            player.TotalWin += winAmount;
            player.LastActivity = DateTime.Now;


            var players = dataManager.LoadPlayers();
            var index = players.FindIndex(p => p.Id == player.Id);
            if (index >= 0)
            {
                players[index] = player;
                dataManager.SavePlayers(players);
            }


            // saglaba sessiju
            SaveSession(player.Id, bet, winAmount);

            return new SpinResult
            {
                Symbols = new[] { s1, s2, s3 },
                WinAmount = winAmount,
                IsWin = winAmount > 0
            };
        }



        private void SaveSession(int playerId, decimal bet, decimal winAmount)
        {
            var sessions = dataManager.LoadSessions();

            sessions.Add(new GameSession
            {
                PlayerId = playerId,
                Bet = bet,
                WinAmount = winAmount,
                Date = DateTime.Now
            });

            dataManager.SaveSessions(sessions);
        }
    }

    /// <summary>
    /// Viena SPIN rezultāts
    /// </summary>
    public class SpinResult
    {
        public Symbol[] Symbols { get; set; }
        public decimal WinAmount { get; set; }
        public bool IsWin { get; set; }
    }
}
