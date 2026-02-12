using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using SlotMachineAdminSystem.Models;

namespace SlotMachineAdminSystem.Services
{
    public class XmlDataManager
    {
        private readonly string playersFile = "Data/players.xml";
        private readonly string sessionsFile = "Data/sessions.xml";
        private readonly string settingsFile = "Data/settings.xml";

        public XmlDataManager()
        {
            Directory.CreateDirectory("Data");

            if (!File.Exists(playersFile))
                CreateEmptyPlayersFile();

            if (!File.Exists(sessionsFile))
                CreateEmptySessionsFile();

            if (!File.Exists(settingsFile))
                CreateDefaultSettingsFile();
        }

        #region Players

        public List<Player> LoadPlayers()
        {
            XDocument doc = XDocument.Load(playersFile);

            return doc.Root.Elements("Player")
                .Select(p => new Player
                {
                    Id = (int)p.Element("Id"),
                    Username = (string)p.Element("Username") ?? "Unknown",
                    Password = (string)p.Element("Password") ?? "password",
                    Balance = (decimal?)p.Element("Balance") ?? 0.00m,
                    LastActivity = (DateTime?)p.Element("LastActivity") ?? DateTime.Now,
                    TotalWin = (decimal?)p.Element("TotalWin") ?? 0.00m,
                    TotalDeposit = (decimal?)p.Element("TotalDeposit") ?? 0,
                    TotalWithdraw = (decimal?)p.Element("TotalWithdraw") ?? 0,
                }).ToList();
        }
        public void SavePlayers(List<Player> players)
        {
            XDocument doc = new XDocument
                (
                new XElement("Players",
                    players.Select(p =>
                        new XElement("Player",
                            new XElement("Id", p.Id),
                            new XElement("Username", p.Username),
                            new XElement("Password", p.Password),
                            new XElement("Balance", p.Balance),
                            new XElement("LastActivity", p.LastActivity),
                            new XElement("TotalWin", p.TotalWin),
                            new XElement("TotalDeposit", p.TotalDeposit),
                            new XElement("TotalWithdraw", p.TotalWithdraw)
                        )
                    )
                )
            );

            doc.Save(playersFile);
        }


        private void CreateEmptyPlayersFile()
        {
            new XDocument(new XElement("Players")).Save(playersFile);
        }

        #endregion

        #region GameSessions

        public List<GameSession> LoadSessions()
        {
            XDocument doc = XDocument.Load(sessionsFile);

            return doc.Root.Elements("Session")
                .Select(s => new GameSession
                {
                    PlayerId = (int)s.Element("PlayerId"),
                    Bet = (decimal)s.Element("Bet"),
                    WinAmount = (decimal)s.Element("WinAmount"),
                    Date = (DateTime)s.Element("Date")
                }).ToList();
        }

        public void SaveSessions(List<GameSession> sessions)
        {
            XDocument doc = new XDocument(
                new XElement("Sessions",
                    sessions.Select(s =>
                        new XElement("Session",
                            new XElement("PlayerId", s.PlayerId),
                            new XElement("Bet", s.Bet),
                            new XElement("WinAmount", s.WinAmount),
                            new XElement("Date", s.Date)
                        ))
                )
            );

            doc.Save(sessionsFile);
        }

        private void CreateEmptySessionsFile()
        {
            new XDocument(new XElement("Sessions")).Save(sessionsFile);
        }

        #endregion

        #region Settings

        public ProbabilitySettings LoadSettings()
        {
            XDocument doc = XDocument.Load(settingsFile);

            return new ProbabilitySettings
            {
                WinningProbability = (int)doc.Root.Element("WinningProbability"),
                AdminPassword = (string)doc.Root.Element("AdminPassword")
            };
        }

        public void SaveSettings(ProbabilitySettings settings)
        {
            XDocument doc = new XDocument(
                new XElement("Settings",
                    new XElement("WinningProbability", settings.WinningProbability),
                    new XElement("AdminPassword", settings.AdminPassword)
                )
            );

            doc.Save(settingsFile);
        }

        private void CreateDefaultSettingsFile()
        {
            SaveSettings(new ProbabilitySettings
            {
                WinningProbability = 30,
                AdminPassword = "admin123"
            });
        }
    }
}
#endregion