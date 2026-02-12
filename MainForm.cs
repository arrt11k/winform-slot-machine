using SlotMachineAdminSystem.Models;
using SlotMachineAdminSystem.Services;
using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace SlotMachineAdminSystem
{
    public partial class MainForm : Form
    {
        private Player currentPlayer;
        private decimal currentWin;              // Win amount from the last spin
        private readonly GameMachine gameMachine;
        private readonly XmlDataManager dataManager;

        private Symbol[] finalSymbols;

        // Reel stopping logic (stop reels one by one)
        private int spinStep = 0;                // current timer tick
        private const int MaxSpinSteps = 15;     // total ticks before stopping
        private readonly int reel1StopStep = 10; // left reel stops at tick 10
        private readonly int reel2StopStep = 12; // middle reel stops at tick 12
        private readonly int reel3StopStep = 14; // right reel stops at tick 14

        private readonly SoundPlayer spinSound;
        private readonly SoundPlayer winSound;
        private bool soundEnabled = true;

        public MainForm(Player player)
        {
            InitializeComponent();

            dataManager = new XmlDataManager();
            gameMachine = new GameMachine(dataManager);

            spinSound = new SoundPlayer("Resources/spin.wav");
            winSound = new SoundPlayer("Resources/win.wav");

            currentPlayer = player;
            UpdateUI();
            UpdateSoundButtonText();
        }

        private void btnSpin_Click(object sender, EventArgs e)
        {
            try
            {
                lblResult.Text = string.Empty;

                decimal bet;
                if (!decimal.TryParse(txtBet.Text, out bet))
                {
                    MessageBox.Show("Please enter a valid bet amount.", "Invalid input",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = gameMachine.Spin(currentPlayer, bet);

                finalSymbols = result.Symbols;
                currentWin = result.WinAmount;

                spinStep = 0;

                if (soundEnabled)
                    spinSound.PlayLooping();

                spinTimer.Start();

                // Persist and refresh UI after a successful spin
                SavePlayer();
                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SavePlayer()
        {
            var players = dataManager.LoadPlayers();
            var index = players.FindIndex(p => p.Id == currentPlayer.Id);

            if (index >= 0)
            {
                players[index] = currentPlayer;
            }
            else
            {
                // If player is not found, add to list (safety fallback)
                players.Add(currentPlayer);
            }

            dataManager.SavePlayers(players);
        }

        private void UpdateUI()
        {
            if (currentPlayer != null)
            {
                lblBalance.Text = string.Format("Balance: {0} €", currentPlayer.Balance);
            }
        }

        private void spinTimer_Tick(object sender, EventArgs e)
        {
            if (finalSymbols == null || finalSymbols.Length < 3)
                return;

            // Spin preview until each reel stops
            if (spinStep < MaxSpinSteps)
            {
                // Left reel
                picSlot1.Image = Image.FromFile(
                    spinStep < reel1StopStep
                        ? gameMachine.SpinPreviewSymbol().ImagePath
                        : finalSymbols[0].ImagePath);

                // Middle reel
                picSlot2.Image = Image.FromFile(
                    spinStep < reel2StopStep
                        ? gameMachine.SpinPreviewSymbol().ImagePath
                        : finalSymbols[1].ImagePath);

                // Right reel
                picSlot3.Image = Image.FromFile(
                    spinStep < reel3StopStep
                        ? gameMachine.SpinPreviewSymbol().ImagePath
                        : finalSymbols[2].ImagePath);

                spinStep++;
            }
            else
            {
                spinTimer.Stop();
                spinStep = 0;

                ShowFinalResult();
            }
        }

        private void ShowFinalResult()
        {
            if (finalSymbols == null || finalSymbols.Length < 3)
                return;

            spinSound.Stop();

            picSlot1.Image = Image.FromFile(finalSymbols[0].ImagePath);
            picSlot2.Image = Image.FromFile(finalSymbols[1].ImagePath);
            picSlot3.Image = Image.FromFile(finalSymbols[2].ImagePath);

            if (currentWin > 0)
            {
                lblResult.Text = string.Format("You won: {0} €", currentWin);

                if (soundEnabled)
                    winSound.Play();
            }
            else
            {
                lblResult.Text = "Try again!";
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSound_Click(object sender, EventArgs e)
        {
            soundEnabled = !soundEnabled;

            if (!soundEnabled)
            {
                spinSound.Stop();
                winSound.Stop();
            }

            UpdateSoundButtonText();
        }

        private void UpdateSoundButtonText()
        {
            btnSound.Text = soundEnabled ? "🔊 Sound" : "🔇 Sound";
        }

        private void btnDeposit_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter deposit amount:",
                "Deposit",
                "10");

            decimal amount;
            if (decimal.TryParse(input, out amount))
            {
                if (amount <= 0)
                {
                    MessageBox.Show("Amount must be greater than 0.", "Invalid amount",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                currentPlayer.Balance += amount;
                currentPlayer.TotalDeposit += amount;

                SavePlayer();
                UpdateUI();

                MessageBox.Show(string.Format("Balance updated by {0} €.", amount), "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (!string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Invalid number.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnWithdraw_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter withdraw amount:",
                "Withdraw",
                "10");

            decimal amount;
            if (decimal.TryParse(input, out amount))
            {
                if (amount <= 0)
                {
                    MessageBox.Show("Amount must be greater than 0.", "Invalid amount",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (currentPlayer.Balance < amount)
                {
                    MessageBox.Show("Insufficient balance.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                currentPlayer.Balance -= amount;
                currentPlayer.TotalWithdraw += amount;

                SavePlayer();
                UpdateUI();

                MessageBox.Show(string.Format("Withdrawn: {0} €.", amount), "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (!string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Invalid number.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Optional: remove if not used in Designer
        private void lblBalance_Click(object sender, EventArgs e)
        {
        }
    }
}
