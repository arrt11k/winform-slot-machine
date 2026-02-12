using SlotMachineAdminSystem.Models;
using SlotMachineAdminSystem.Services;
using System;
using System.Linq;
using System.Windows.Forms;

namespace SlotMachineAdminSystem.Forms
{
    public partial class AdminPanelForm : Form
    {
        private readonly XmlDataManager dataManager;
        private ProbabilitySettings settings;

        public AdminPanelForm()
        {
            InitializeComponent();

            dataManager = new XmlDataManager();

            InitializeSortOptions();
            InitializePlayersGrid();

            LoadSettings();
            LoadPlayers();
        }

        private void InitializeSortOptions()
        {
            cmbSortBy.Items.Clear();
            cmbSortBy.Items.Add("Username (A–Z)");
            cmbSortBy.Items.Add("Balance (Ascending)");
            cmbSortBy.Items.Add("Balance (Descending)");
            cmbSortBy.Items.Add("Total Win");
            cmbSortBy.Items.Add("Last Activity");
            cmbSortBy.SelectedIndex = 0;
        }

        private void InitializePlayersGrid()
        {
            dgvPlayers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPlayers.MultiSelect = true;
            dgvPlayers.AutoGenerateColumns = false;

            if (dgvPlayers.Columns.Count == 0)
            {
                dgvPlayers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Username",
                    DataPropertyName = "Username"
                });

                dgvPlayers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Balance",
                    DataPropertyName = "Balance"
                });

                dgvPlayers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Total Win",
                    DataPropertyName = "TotalWin"
                });

                dgvPlayers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Last Activity",
                    DataPropertyName = "LastActivity"
                });
            }
        }

        private void LoadSettings()
        {
            settings = dataManager.LoadSettings();
            numWinProbability.Value = settings.WinningProbability;
        }

        private void LoadPlayers()
        {
            dgvPlayers.DataSource = null;
            dgvPlayers.DataSource = dataManager.LoadPlayers();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            settings.WinningProbability = (int)numWinProbability.Value;
            dataManager.SaveSettings(settings);
            MessageBox.Show("Settings saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            settings.AdminPassword = txtNewPassword.Text;
            dataManager.SaveSettings(settings);

            txtNewPassword.Clear();
            MessageBox.Show("Password updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtNewPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        private void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            if (dgvPlayers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select at least one player to delete.", "No selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                "Are you sure you want to delete the selected player(s)?",
                "Confirm deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            var players = dataManager.LoadPlayers();

            foreach (DataGridViewRow row in dgvPlayers.SelectedRows)
            {
                if (row.DataBoundItem is Player player)
                    players.RemoveAll(p => p.Id == player.Id);
            }

            dataManager.SavePlayers(players);
            LoadPlayers();

            MessageBox.Show("Selected player(s) deleted.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            var players = dataManager.LoadPlayers();

            switch (cmbSortBy.SelectedItem?.ToString())
            {
                case "Username (A–Z)":
                    players = players.OrderBy(p => p.Username).ToList();
                    break;

                case "Balance (Ascending)":
                    players = players.OrderBy(p => p.Balance).ToList();
                    break;

                case "Balance (Descending)":
                    players = players.OrderByDescending(p => p.Balance).ToList();
                    break;

                case "Total Win":
                    players = players.OrderByDescending(p => p.TotalWin).ToList();
                    break;

                case "Last Activity":
                    players = players.OrderByDescending(p => p.LastActivity).ToList();
                    break;
            }

            dgvPlayers.DataSource = null;
            dgvPlayers.DataSource = players;
        }
    }
}
