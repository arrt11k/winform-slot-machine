using SlotMachineAdminSystem.Forms;
using System;
using System.Windows.Forms;

namespace SlotMachineAdminSystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new PlayerLoginForm()); // starte login
        }
    }
}
