using System;
using System.Windows.Forms;
using SlotMachineAdminSystem.Services;

namespace SlotMachineAdminSystem.Forms
{
    public partial class AdminLoginForm : Form
    {
        private readonly XmlDataManager dataManager;
        private string adminPassword;

        public bool IsAuthorized { get; private set; }

        public AdminLoginForm()
        {
            InitializeComponent();

            dataManager = new XmlDataManager();
            adminPassword = dataManager.LoadSettings().AdminPassword;

            IsAuthorized = false;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text == adminPassword)
            {
                IsAuthorized = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Nepareiza parole");
            }
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        private void AdminLoginForm_Load(object sender, EventArgs e)
        {

        }

        private void chkShowPassword_CheckedChanged_1(object sender, EventArgs e)
        {

        }
    }
}
