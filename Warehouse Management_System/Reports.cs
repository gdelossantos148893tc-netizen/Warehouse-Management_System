using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using WMsystem48;

namespace Warehouse_Management_System
{
    public partial class Reports : Form
    {
        private string connectionString =
            @"Data Source=GERALDKIMDELOSS\SQLEXPRESS;
              Initial Catalog=WMSystem;
              Integrated Security=True;
              TrustServerCertificate=True;";

        private DataTable reportsTable = new DataTable();

        public Reports()
        {
            InitializeComponent();

            // FORM LOAD
            this.Load -= Reports_Load;
            this.Load += Reports_Load;

            // BUTTON EVENTS
            btnDashboard.Click -= btnDashboard_Click;
            btnDashboard.Click += btnDashboard_Click;

            btnProducts.Click -= btnProducts_Click;
            btnProducts.Click += btnProducts_Click;

            btnWarehouse.Click -= btnWarehouse_Click;
            btnWarehouse.Click += btnWarehouse_Click;

            btnReports.Click -= btnReports_Click;
            btnReports.Click += btnReports_Click;

            btnLogout.Click -= btnLogout_Click;
            btnLogout.Click += btnLogout_Click;

            button1.Click -= button1_Click;
            button1.Click += button1_Click;

            // GRID
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;

            dataGridView1.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dataGridView1.MultiSelect = false;

            SetGridColumns();
        }

        // =====================================================
        // FORM LOAD
        // =====================================================

        private void Reports_Load(object sender, EventArgs e)
        {
            LoadReports();
        }

        // =====================================================
        // LOAD REPORT DATA
        // =====================================================

        private void LoadReports()
        {
            try
            {
                string query = @"
                    SELECT
                        p.ProductCode AS CODE,
                        p.ProductName AS NAME,
                        ISNULL(c.CategoryName, '') AS CATEGORY,

                        CASE
                            WHEN w.WarehouseName = 'Main Warehouse'
                            THEN
                                'Main / Floor ' +
                                ISNULL(
                                    CAST(f.FloorNumber AS VARCHAR(10)),
                                    ''
                                ) +
                                ' / ' +
                                ISNULL(b.BinCode, '')

                            ELSE
                                ISNULL(w.WarehouseName, '') +
                                ' / Floor ' +
                                ISNULL(
                                    CAST(f.FloorNumber AS VARCHAR(10)),
                                    ''
                                ) +
                                ' / ' +
                                ISNULL(b.BinCode, '')
                        END AS LOCATION,

                        ISNULL(p.Quantity, 0) AS QTY,

                        ISNULL(p.UnitPrice, 0) AS [UNIT PRICE]

                    FROM dbo.Products p

                    LEFT JOIN dbo.Categories c
                        ON p.CategoryID = c.CategoryID

                    LEFT JOIN dbo.Warehouses w
                        ON p.WarehouseID = w.WarehouseID

                    LEFT JOIN dbo.Floors f
                        ON p.FloorID = f.FloorID

                    LEFT JOIN dbo.Bins b
                        ON p.BinID = b.BinID

                    ORDER BY p.ProductID DESC;
                ";

                DataTable newTable =
                    new DataTable();

                using (SqlConnection con =
                    new SqlConnection(connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(query, con))
                using (SqlDataAdapter da =
                    new SqlDataAdapter(cmd))
                {
                    con.Open();

                    da.Fill(newTable);
                }

                reportsTable = newTable;

                // REMOVE OLD DATA
                dataGridView1.DataSource = null;

                // DISPLAY NEW DATA
                dataGridView1.DataSource =
                    reportsTable;

                SetGridColumns();

                dataGridView1.Refresh();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "SQL DATABASE ERROR!\n\n" +
                    ex.Message,
                    "Reports Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "ERROR LOADING REPORTS!\n\n" +
                    ex.ToString(),
                    "Reports Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // GRID COLUMNS
        // =====================================================

        private void SetGridColumns()
        {
            // IMPORTANT:
            // AutoGenerateColumns = FALSE
            // So only Designer columns are used.

            if (dataGridView1.Columns.Count < 6)
                return;

            // COLUMN 1
            dataGridView1.Columns[0].DataPropertyName =
                "CODE";

            dataGridView1.Columns[0].HeaderText =
                "CODE";

            // COLUMN 2
            dataGridView1.Columns[1].DataPropertyName =
                "NAME";

            dataGridView1.Columns[1].HeaderText =
                "NAME";

            // COLUMN 3
            dataGridView1.Columns[2].DataPropertyName =
                "CATEGORY";

            dataGridView1.Columns[2].HeaderText =
                "CATEGORY";

            // COLUMN 4
            dataGridView1.Columns[3].DataPropertyName =
                "LOCATION";

            dataGridView1.Columns[3].HeaderText =
                "LOCATION";

            // COLUMN 5
            dataGridView1.Columns[4].DataPropertyName =
                "QTY";

            dataGridView1.Columns[4].HeaderText =
                "QUANTITY";

            dataGridView1.Columns[4]
                .DefaultCellStyle.Format = "N0";

            // COLUMN 6
            dataGridView1.Columns[5].DataPropertyName =
                "UNIT PRICE";

            dataGridView1.Columns[5].HeaderText =
                "UNIT PRICE";

            dataGridView1.Columns[5]
                .DefaultCellStyle.Format = "N2";

            // SHOW ONLY FIRST 6 COLUMNS
            for (int i = 0;
                 i < dataGridView1.Columns.Count;
                 i++)
            {
                if (i < 6)
                    dataGridView1.Columns[i].Visible = true;
                else
                    dataGridView1.Columns[i].Visible = false;
            }

            dataGridView1.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
        }

        // =====================================================
        // OPEN CRYSTAL REPORT
        // =====================================================

        private void button1_Click(
            object sender,
            EventArgs e)
        {
            Viewer report = new Viewer();
            report.Show();
        }

        // =====================================================
        // DASHBOARD
        // =====================================================

        private void btnDashboard_Click(
            object sender,
            EventArgs e)
        {
            Dashboard dashboard =
                new Dashboard();

            dashboard.Show();

            this.Hide();
        }

        // =====================================================
        // PRODUCTS
        // =====================================================

        private void btnProducts_Click(
            object sender,
            EventArgs e)
        {
            product productForm =
                new product();

            productForm.Show();

            this.Hide();
        }

        // =====================================================
        // WAREHOUSE
        // =====================================================

        private void btnWarehouse_Click(
            object sender,
            EventArgs e)
        {
            Warehouses warehouses =
                new Warehouses();

            warehouses.Show();

            this.Hide();
        }

        // =====================================================
        // REPORTS
        // =====================================================

        private void btnReports_Click(
            object sender,
            EventArgs e)
        {
            LoadReports();
        }

        // =====================================================
        // LOGOUT
        // =====================================================

        private void btnLogout_Click(
            object sender,
            EventArgs e)
        {
            DialogResult result =
                MessageBox.Show(
                    "Are you sure you want to logout?",
                    "Logout",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Form1 login =
                    new Form1();

                login.Show();

                this.Hide();
            }
        }

        // =====================================================
        // GRID CLICK
        // =====================================================

        private void dataGridView1_CellContentClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
        }
    }
}