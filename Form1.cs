using System;
using System.Drawing;
using System.Windows.Forms;

namespace _24_57575_2_login
{
    public partial class Form1 : Form
    {
        private int failedAttempts = 0;

        private void HandleFailedLogin(string message)
        {
            failedAttempts++;
            MessageBox.Show(message, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            if (failedAttempts >= 3)
            {
                // disable login button on too many failures
                this.btnLogin.Enabled = false;
                MessageBox.Show("Too many failed attempts. Login disabled.", "Locked", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        public void ClearForLogout()
        {
            this.txtUsername.Clear();
            this.txtPassword.Clear();
            this.txtUsername.Focus();
            this.failedAttempts = 0;
            this.btnLogin.Enabled = true;
        }
        public Form1()
        {
            InitializeComponent();
            // adjust positions now that sizes are known
            CenterPanel();
        }

        private void CenterPanel()
        {
            if (this.pnlCard != null)
            {
                this.pnlCard.Left = (this.ClientSize.Width - this.pnlCard.Width) / 2;
                this.pnlCard.Top = (this.ClientSize.Height - this.pnlCard.Height) / 2;
                // center inner title and logo
                this.picLogo.Left = (this.pnlCard.Width - this.picLogo.Width) / 2;
                this.lblTitle.Left = (this.pnlCard.Width - this.lblTitle.PreferredWidth) / 2;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var user = txtUsername.Text.Trim();
            var pass = txtPassword.Text;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Please enter username and password.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var db = new DatabaseHelper();
                var record = db.GetUserByUsername(user);
                if (record == null)
                {
                    HandleFailedLogin("Invalid credentials.");
                    return;
                }

                var hash = DatabaseHelper.ComputeSha256Hash(pass);
                if (!string.Equals(hash, record.PasswordHash, StringComparison.OrdinalIgnoreCase))
                {
                    HandleFailedLogin("Invalid credentials.");
                    return;
                }

                // Success: open HomeForm
                failedAttempts = 0;
                var home = new HomeForm(db, record, this);
                home.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            using var reg = new RegisterForm();
            reg.ShowDialog(this);
        }

        private void btnTestDb_Click(object sender, EventArgs e)
        {
            try
            {
                var db = new DatabaseHelper();
                if (db.TestConnection(out var err))
                    MessageBox.Show("Database connection successful.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show($"Database connection failed: {err}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Test connection error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

