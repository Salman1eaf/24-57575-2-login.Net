using System;
using System.Drawing;
using System.Windows.Forms;

namespace _24_57575_2_login
{
    public class RegisterForm : Form
    {
        private Label lblUser;
        private Label lblPass;
        private Label lblConfirm;
        private TextBox txtUser;
        private TextBox txtPass;
        private TextBox txtConfirm;
        private TextBox txtEmail;
        private Button btnSubmit;
        private Button btnCancel;

        public RegisterForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Register";
            this.ClientSize = new Size(420, 320);
            this.StartPosition = FormStartPosition.CenterParent;

            lblUser = new Label { Text = "Username:", Location = new Point(20, 20), AutoSize = true };
            txtUser = new TextBox { Location = new Point(20, 40), Width = 360 };

            lblPass = new Label { Text = "Password:", Location = new Point(20, 80), AutoSize = true };
            txtPass = new TextBox { Location = new Point(20, 100), Width = 360, UseSystemPasswordChar = true };

            lblConfirm = new Label { Text = "Confirm Password:", Location = new Point(20, 140), AutoSize = true };
            txtConfirm = new TextBox { Location = new Point(20, 160), Width = 360, UseSystemPasswordChar = true };

            var lblEmail = new Label { Text = "Email:", Location = new Point(20, 200), AutoSize = true };
            txtEmail = new TextBox { Location = new Point(20, 220), Width = 360 };

            btnSubmit = new Button { Text = "Register", Location = new Point(200, 260), Width = 90 };
            btnCancel = new Button { Text = "Cancel", Location = new Point(300, 260), Width = 90 };

            btnSubmit.Click += BtnSubmit_Click;
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblUser, txtUser, lblPass, txtPass, lblConfirm, txtConfirm, lblEmail, txtEmail, btnSubmit, btnCancel });
        }

        private void BtnSubmit_Click(object? sender, EventArgs e)
        {
            var user = txtUser.Text.Trim();
            var pass = txtPass.Text;
            var confirm = txtConfirm.Text;
            var email = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(confirm) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please fill all fields.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (pass.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (pass != confirm)
            {
                MessageBox.Show("Passwords do not match.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!email.Contains("@"))
            {
                MessageBox.Show("Please enter a valid email.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var db = new DatabaseHelper();
                if (db.UsernameExists(user))
                {
                    MessageBox.Show("Username already taken.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var hash = DatabaseHelper.ComputeSha256Hash(pass);
                db.CreateUser(user, hash, email, null);
                MessageBox.Show("Registration successful.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Registration failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
