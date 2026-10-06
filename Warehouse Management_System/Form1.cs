
using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using Warehouse_Management_System;

namespace WMsystem48
{
    public partial class Form1 : Form
    {
        private string connectionString =
            @"Data Source=GERALDKIMDELOSS\SQLEXPRESS;
              Initial Catalog=loginapp;
              Integrated Security=True;
              TrustServerCertificate=True;";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                        SELECT COUNT(*)
                        FROM loginapp
                        WHERE username = @username
                        AND password = @password";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@username", txtUser.Text);
                        cmd.Parameters.AddWithValue("@password", txtPass.Text);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                        {
                            // Correct username and password
                          Dashboard dashboard = new Dashboard();
                            dashboard.Show();

                            // Hide login form
                            this.Hide();
                        }
                        else
                        {
                            // Wrong username or password
                            MessageBox.Show(
                                "Wrong username or password!",
                                "Login Failed",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                            txtPass.Clear();
                            txtPass.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Database Error:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void txtPass_TextChanged(object sender, EventArgs e)
        {
            if (checkBoxPass.Checked)
            {
                txtPass.PasswordChar = '\0';
            }
            else
            {
                txtPass.PasswordChar = '*';
            }
        }

        

        private void txtUser_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}

