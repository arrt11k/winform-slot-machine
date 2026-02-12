using System;

namespace SlotMachineAdminSystem.Models
{
    public class GameSession
    {
        public int PlayerId { get; set; }
        public decimal Bet { get; set; }
        public decimal WinAmount { get; set; }
        public DateTime Date { get; set; }

        public bool IsWin => WinAmount > 0;
    }
}
