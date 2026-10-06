
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using WMsystem48;

namespace Warehouse_Management_System
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();

            // Reload dashboard when returning to this form 
            this.Activated += Dashboard_Activated;
        }

        // ========================================================= 
        // DASHBOARD LOAD 
        // ========================================================= 
        private void Dashboard_Load(object sender, EventArgs e)
        {
            LoadDashboardData();
        }

        // ========================================================= 
        // DASHBOARD ACTIVATED 
        // ========================================================= 
        private void Dashboard_Activated(object sender, EventArgs e)
        {
            LoadDashboardData();
        }

        // ========================================================= 
        // LOAD DASHBOARD DATA 
        // ========================================================= 
        private void LoadDashboardData()
        {
            try
            {


                string query = @" 
                        SELECT 
                            (SELECT COUNT(*) 
                             FROM dbo.Warehouses 
                             WHERE Status = 'Active') 
                                AS WarehouseCount, 
 
                            (SELECT COUNT(*) 
                             FROM dbo.Products) 
                                AS ProductCount, 
 
                            (SELECT ISNULL(SUM(Quantity), 0) 
                             FROM dbo.Products) 
                                AS TotalQuantity, 
 
                            (SELECT COUNT(*) 
                             FROM dbo.Products 
                             WHERE Quantity <= 10) 
                                AS LowStockCount; 
                    ";

                using (SqlConnection con = Database.GetConnection())
                using (SqlCommand cmd = new SqlCommand(query, con))

                {
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int warehouseCount =
                                Convert.ToInt32(reader["WarehouseCount"]);

                            int productCount =
                                Convert.ToInt32(reader["ProductCount"]);

                            int totalQuantity =
                                Convert.ToInt32(reader["TotalQuantity"]);

                            int lowStockCount =
                                Convert.ToInt32(reader["LowStockCount"]);

                            UpdateDashboardLabels(
                                warehouseCount,
                                productCount,
                                totalQuantity,
                                lowStockCount
                            );
                        }
                    }
                }
            }

            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Error loading dashboard data:\n\n" +
                    ex.Message,
                    "Dashboard Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading dashboard:\n\n" +
                    ex.Message,
                    "Dashboard Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        private void UpdateDashboardLabels(
           int warehouseCount,
           int productCount,
           int totalQuantity,
           int lowStockCount)
        {
            List<Label> zeroLabels = GetAllLabels(this)
                .Where(lbl =>
                    lbl.Visible &&
                    lbl.Text.Trim() == "0")
                .OrderBy(lbl => lbl.Top)
                .ThenBy(lbl => lbl.Left)
                .ToList();

            if (zeroLabels.Count >= 4)
            {
                zeroLabels[0].Text =
                    warehouseCount.ToString();

                zeroLabels[1].Text =
                    productCount.ToString();

                zeroLabels[2].Text =
                    totalQuantity.ToString();

                zeroLabels[3].Text =
                    lowStockCount.ToString();
            }
        }
        private List<Label> GetAllLabels(Control parent)
        {
            List<Label> labels = new List<Label>();

            foreach (Control control in parent.Controls)
            {
                if (control is Label label)
                {
                    labels.Add(label);
                }

                if (control.HasChildren)
                {
                    labels.AddRange(GetAllLabels(control));
                }
            }

            return labels;
        }


        // ========================================================= 
        // DASHBOARD BUTTON 
        // ========================================================= 
        private void btnDashboard_Click(object sender, EventArgs e)
        {
            LoadDashboardData();
        }

        // ========================================================= 
        // PRODUCTS BUTTON 
        // ========================================================= 
        private void btnProducts_Click(object sender, EventArgs e)
        {
            product productsForm = new product();

            productsForm.Show();

            this.Hide();
        }

        // ========================================================= 
        // WAREHOUSE BUTTON 
        // ========================================================= 
        private void btnWarehouse_Click(object sender, EventArgs e)
        {
            Warehouses warehousesForm = new Warehouses();

            warehousesForm.Show();

            this.Hide();
        }

        // ========================================================= 
        // REPORTS BUTTON 
        // ========================================================= 
        private void btnReports_Click(object sender, EventArgs e)
        {
            Reports reportsForm = new Reports();

            reportsForm.Show();

            this.Hide();
        }

        // ========================================================= 
        // LOGOUT BUTTON 
        // ========================================================= 
        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to logout?",
                "Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                Form1 login = new Form1();

                login.Show();

                this.Hide();
            }
        }


        private void lblWarehouses_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void lblTotalProducts_Click(object sender, EventArgs e)
        {
        }

        private void lblTotalQuantity_Click(object sender, EventArgs e)
        {
        }

        private void lblLowStock_Click(object sender, EventArgs e)
        {
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
