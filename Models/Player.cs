using System;

namespace SlotMachineAdminSystem.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public decimal Balance { get; set; }
        public DateTime LastActivity { get; set; }
        public decimal TotalWin { get; set; }
        public decimal TotalDeposit { get; set; }
        public decimal TotalWithdraw { get; set; }


        public Player() { }

        public Player(int id, string username, string password, decimal balance)
        {
            Id = id;
            Username = username;
            Password = password;
            Balance = balance;
            LastActivity = DateTime.Now;
            TotalWin = 0;
        }
    }
}
