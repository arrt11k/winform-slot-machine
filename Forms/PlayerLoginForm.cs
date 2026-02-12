using System;
using System.Linq;
using System.Windows.Forms;
using SlotMachineAdminSystem.Models;
using SlotMachineAdminSystem.Services;

namespace SlotMachineAdminSystem.Forms
{
    public partial class PlayerLoginForm : Form
    {
        private readonly XmlDataManager dataManager;

        public Player AuthorizedPlayer { get; private set; }

        public PlayerLoginForm()
        {
            InitializeComponent();
            dataManager = new XmlDataManager();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            lblMessage.Text = string.Empty;

            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text;

            var players = dataManager.LoadPlayers();
            var player = players.FirstOrDefault(p =>
                p.Username == username &&
                p.Password == password);

            if (player == null)
            {
                lblMessage.Text = "Invalid username or password.";
                return;
            }

            AuthorizedPlayer = player;

            // Hide login form while the main game window is open
            Hide();

            // Open the main form for the authorized player
            using (var mainForm = new MainForm(AuthorizedPlayer))
            {
                mainForm.ShowDialog();
            }

            // Reset fields and show login form again
            txtUsername.Clear();
            txtPassword.Clear();
            chkShowPassword.Checked = false;
            Show();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            lblMessage.Text = string.Empty;

            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblMessage.Text = "Please enter a username and password.";
                return;
            }

            var players = dataManager.LoadPlayers();

            if (players.Any(p => p.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                lblMessage.Text = "This username is already taken.";
                return;
            }

            int newId = players.Count == 0 ? 1 : players.Max(p => p.Id) + 1;

            var newPlayer = new Player(
                newId,
                username,
                password,
                100
            );

            players.Add(newPlayer);
            dataManager.SavePlayers(players);

            lblMessage.Text = "Registration successful.";

            // Clear fields after registration
            txtUsername.Clear();
            txtPassword.Clear();
            chkShowPassword.Checked = false;
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            using (var adminLogin = new AdminLoginForm())
            {
                adminLogin.ShowDialog();

                if (adminLogin.IsAuthorized)
                {
                    using (var adminPanel = new AdminPanelForm())
                    {
                        adminPanel.ShowDialog();
                    }
                }
            }
        }
    }
}
