using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace _24_57575_2_login
{
    public class HomeForm : Form
    {
        private readonly DatabaseHelper _db;
        private readonly UserRecord _user;
        private readonly Form1 _loginForm;
        private Label lblWelcome;
        private Button btnLogout;
        private DataGridView dgvUsers;

        public HomeForm(DatabaseHelper db, UserRecord user, Form1 loginForm)
        {
            _db = db;
            _user = user;
            _loginForm = loginForm;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Home";
            this.ClientSize = new Size(700, 450);

            lblWelcome = new Label { Text = $"Welcome, {_user.FullName ?? _user.Username}", Location = new Point(20, 20), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold) };
            btnLogout = new Button { Text = "Logout", Location = new Point(600, 16), Width = 70 };
            btnLogout.Click += BtnLogout_Click;

            dgvUsers = new DataGridView { Location = new Point(20, 60), Size = new Size(650, 350), ReadOnly = true, AllowUserToAddRows = false };

            this.Controls.AddRange(new Control[] { lblWelcome, btnLogout, dgvUsers });
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormClosing += HomeForm_FormClosing;
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                var dt = _db.GetUsersDataTable();
                dgvUsers.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load users: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLogout_Click(object? sender, EventArgs e)
        {
            this.Close();
            _loginForm.ClearForLogout();
        }

        private void HomeForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            _loginForm.Show();
        }
    }
}
