using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using WMsystem48;

namespace Warehouse_Management_System
{
    public partial class product : Form
    {
        private DataTable productsTable = new DataTable();

        private string connectionString =
            @"Data Source=GERALDKIMDELOSS\SQLEXPRESS;
              Initial Catalog=WMSystem;
              Integrated Security=True;
              TrustServerCertificate=True;";

        public product()
        {
            InitializeComponent();

            // IMPORTANT: Make sure Load event is connected
            this.Load -= product_Load;
            this.Load += product_Load;

            // GRID SETTINGS
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            SetGridColumns();

            // EVENTS
            txtSearch.TextChanged -= txtSearch_TextChanged;
            txtSearch.TextChanged += txtSearch_TextChanged;

            btnAdd.Click -= btnAdd_Click;
            btnAdd.Click += btnAdd_Click;

            btnDelete.Click -= btnDelete_Click;
            btnDelete.Click += btnDelete_Click;
        }

        // =====================================================
        // FORM LOAD
        // =====================================================

        private void product_Load(object sender, EventArgs e)
        {
            LoadProducts();
        }

        // =====================================================
        // LOAD PRODUCTS
        // =====================================================

        private void LoadProducts()
        {
            try
            {
                string query = @"
                    SELECT
                        p.ProductID,
                        p.ProductCode AS CODE,
                        p.ProductName AS NAME,
                        ISNULL(c.CategoryName, '') AS CATEGORY,

                        CASE
                            WHEN w.WarehouseName = 'Main Warehouse'
                            THEN
                                'Main / Floor ' +
                                ISNULL(CAST(f.FloorNumber AS VARCHAR(10)), '') +
                                ' / ' +
                                ISNULL(b.BinCode, '')
                            ELSE
                                ISNULL(w.WarehouseName, '') +
                                ' / Floor ' +
                                ISNULL(CAST(f.FloorNumber AS VARCHAR(10)), '') +
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

                DataTable newTable = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlCommand cmd =
                        new SqlCommand(query, con))
                    using (SqlDataAdapter da =
                        new SqlDataAdapter(cmd))
                    {
                        da.Fill(newTable);
                    }
                }

                productsTable = newTable;

                // REMOVE OLD DATA FIRST
                dataGridView1.DataSource = null;

                // SET NEW DATA
                dataGridView1.DataSource =
                    productsTable.DefaultView;

                SetGridColumns();

                dataGridView1.Refresh();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "SQL DATABASE ERROR!\n\n" +
                    ex.Message,
                    "Product Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "ERROR LOADING PRODUCTS!\n\n" +
                    ex.ToString(),
                    "Product Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // GRID COLUMNS
        // =====================================================

        private void SetGridColumns()
        {
            if (dataGridView1.Columns.Count < 6)
                return;

            dataGridView1.Columns[0].DataPropertyName = "CODE";
            dataGridView1.Columns[1].DataPropertyName = "NAME";
            dataGridView1.Columns[2].DataPropertyName = "CATEGORY";
            dataGridView1.Columns[3].DataPropertyName = "LOCATION";
            dataGridView1.Columns[4].DataPropertyName = "QTY";
            dataGridView1.Columns[5].DataPropertyName = "UNIT PRICE";

            dataGridView1.Columns[0].HeaderText = "CODE";
            dataGridView1.Columns[1].HeaderText = "NAME";
            dataGridView1.Columns[2].HeaderText = "CATEGORY";
            dataGridView1.Columns[3].HeaderText = "LOCATION";
            dataGridView1.Columns[4].HeaderText = "QUANTITY";
            dataGridView1.Columns[5].HeaderText = "UNIT PRICE";

            for (int i = 0; i < 6; i++)
            {
                dataGridView1.Columns[i].Visible = true;
            }

            dataGridView1.Columns[4]
                .DefaultCellStyle.Format = "N0";

            dataGridView1.Columns[5]
                .DefaultCellStyle.Format = "N2";

            dataGridView1.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
        }

        // =====================================================
        // SEARCH
        // =====================================================

        private void txtSearch_TextChanged(
            object sender,
            EventArgs e)
        {
            try
            {
                if (productsTable == null)
                    return;

                DataView view =
                    productsTable.DefaultView;

                string search =
                    txtSearch.Text.Trim();

                if (string.IsNullOrWhiteSpace(search))
                {
                    view.RowFilter = "";
                    return;
                }

                search = search
                    .Replace("'", "''")
                    .Replace("[", "[[]")
                    .Replace("%", "[%]")
                    .Replace("*", "[*]");

                view.RowFilter =
                    "CONVERT(CODE, 'System.String') LIKE '%" +
                    search +
                    "%' OR " +

                    "CONVERT(NAME, 'System.String') LIKE '%" +
                    search +
                    "%' OR " +

                    "CONVERT(CATEGORY, 'System.String') LIKE '%" +
                    search +
                    "%' OR " +

                    "CONVERT(LOCATION, 'System.String') LIKE '%" +
                    search +
                    "%' OR " +

                    "CONVERT(QTY, 'System.String') LIKE '%" +
                    search +
                    "%'";
            }
            catch
            {
                if (productsTable != null)
                {
                    productsTable.DefaultView.RowFilter = "";
                }
            }
        }

        // =====================================================
        // ADD PRODUCT
        // =====================================================

        private void btnAdd_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                using (add_product addProduct =
                    new add_product())
                {
                    DialogResult result =
                        addProduct.ShowDialog();

                    // ONLY REFRESH AFTER SUCCESSFUL SAVE
                    if (result == DialogResult.OK)
                    {
                        LoadProducts();

                        txtSearch.Clear();

                        dataGridView1.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "ERROR SA ADD PRODUCT!\n\n" +
                    ex.ToString(),
                    "Add Product Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // DELETE PRODUCT
        // =====================================================

        private void btnDelete_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow == null)
                {
                    MessageBox.Show(
                        "Please select a product first.",
                        "Delete Product",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                string productCode =
                    Convert.ToString(
                        dataGridView1.CurrentRow
                            .Cells[0].Value);

                string productName =
                    Convert.ToString(
                        dataGridView1.CurrentRow
                            .Cells[1].Value);

                if (string.IsNullOrWhiteSpace(productCode))
                {
                    MessageBox.Show(
                        "Invalid product.",
                        "Delete Product",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                DialogResult result =
                    MessageBox.Show(
                        "Delete this product?\n\n" +
                        "Code: " + productCode +
                        "\nName: " + productName,

                        "Confirm Delete",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                    return;

                string query = @"
                    DELETE FROM dbo.Products
                    WHERE ProductCode = @ProductCode;
                ";

                int rows;

                using (SqlConnection con =
                    new SqlConnection(connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(query, con))
                {
                    cmd.Parameters.Add(
                        "@ProductCode",
                        SqlDbType.NVarChar).Value =
                        productCode;

                    con.Open();

                    rows = cmd.ExecuteNonQuery();
                }

                if (rows > 0)
                {
                    MessageBox.Show(
                        "Product deleted successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadProducts();

                    txtSearch.Clear();
                }
                else
                {
                    MessageBox.Show(
                        "Product was not found.",
                        "Delete Product",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "ERROR DELETING PRODUCT!\n\n" +
                    ex.ToString(),
                    "Delete Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // DASHBOARD
        // =====================================================

        private void button1_Click(
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
            Reports reports =
                new Reports();

            reports.Show();

            this.Hide();
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

        private void btnReports_Click_1(object sender, EventArgs e)
        {
            Reports reports = new Reports();
            reports.Show();
            this.Hide();
        }
    }
}