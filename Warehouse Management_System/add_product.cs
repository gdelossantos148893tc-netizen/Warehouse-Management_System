using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Warehouse_Management_System
{
    public partial class add_product : Form
    {
        public add_product()
        {
            InitializeComponent();
        }

        // =========================================================
        // FORM LOAD
        // =========================================================
        private void add_product_Load(object sender, EventArgs e)
        {
            LoadWarehouses();
        }

        // =========================================================
        // LOAD WAREHOUSES
        // =========================================================
        private void LoadWarehouses()
        {
            try
            {
                string query = @"
                    SELECT
                        WarehouseID,
                        WarehouseName
                    FROM dbo.Warehouses
                    ORDER BY WarehouseName";

                using (SqlConnection con = Database.GetConnection())
                using (SqlDataAdapter da =
                    new SqlDataAdapter(query, con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbWarehouse.DataSource = dt;
                    cmbWarehouse.DisplayMember = "WarehouseName";
                    cmbWarehouse.ValueMember = "WarehouseID";
                    cmbWarehouse.SelectedIndex = -1;

                    cmbFloor.DataSource = null;
                    cmbBin.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading warehouses:\n\n" +
                    ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================================
        // WAREHOUSE CHANGED
        // =========================================================
        private void comboBox1_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            cmbFloor.DataSource = null;
            cmbBin.DataSource = null;

            if (cmbWarehouse.SelectedIndex == -1)
                return;

            try
            {
                DataRowView row =
                    cmbWarehouse.SelectedItem as DataRowView;

                if (row == null)
                    return;

                int warehouseID =
                    Convert.ToInt32(row["WarehouseID"]);

                LoadFloors(warehouseID);
            }
            catch
            {
                // Ignore temporary ComboBox events
            }
        }

        // =========================================================
        // LOAD FLOORS
        // =========================================================
        private void LoadFloors(int warehouseID)
        {
            try
            {
                string query = @"
                    SELECT
                        FloorID,
                        FloorName,
                        FloorNumber
                    FROM dbo.Floors
                    WHERE WarehouseID = @WarehouseID
                    ORDER BY FloorNumber";

                using (SqlConnection con =
                    Database.GetConnection())
                using (SqlCommand cmd =
                    new SqlCommand(query, con))
                using (SqlDataAdapter da =
                    new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.Add(
                        "@WarehouseID",
                        SqlDbType.Int
                    ).Value = warehouseID;

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbFloor.DataSource = dt;
                    cmbFloor.DisplayMember = "FloorName";
                    cmbFloor.ValueMember = "FloorID";
                    cmbFloor.SelectedIndex = -1;

                    cmbBin.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading floors:\n\n" +
                    ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================================
        // FLOOR CHANGED
        // =========================================================
        private void cmbFloor_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            cmbBin.DataSource = null;

            if (cmbFloor.SelectedIndex == -1)
                return;

            try
            {
                DataRowView row =
                    cmbFloor.SelectedItem as DataRowView;

                if (row == null)
                    return;

                int floorID =
                    Convert.ToInt32(row["FloorID"]);

                LoadBins(floorID);
            }
            catch
            {
                // Ignore temporary ComboBox events
            }
        }

        // =========================================================
        // LOAD BINS
        // =========================================================
        private void LoadBins(int floorID)
        {
            try
            {
                string query = @"
                    SELECT
                        BinID,
                        BinCode,
                        BinName,
                        BinCode + ' - ' + BinName AS BinDisplay
                    FROM dbo.Bins
                    WHERE FloorID = @FloorID
                    ORDER BY BinCode";

                using (SqlConnection con =
                    Database.GetConnection())
                using (SqlCommand cmd =
                    new SqlCommand(query, con))
                using (SqlDataAdapter da =
                    new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.Add(
                        "@FloorID",
                        SqlDbType.Int
                    ).Value = floorID;

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        cmbBin.DataSource = null;

                        MessageBox.Show(
                            "No BINs found for the selected Floor.",
                            "No BIN Available",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                        return;
                    }

                    cmbBin.DataSource = dt;
                    cmbBin.DisplayMember = "BinDisplay";
                    cmbBin.ValueMember = "BinID";
                    cmbBin.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading BINs:\n\n" +
                    ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================================
        // SAVE PRODUCT
        // =========================================================
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string productCode =
                    txtProductCode.Text.Trim();

                string productName =
                    txtProductName.Text.Trim();

                string categoryName =
                    txtCategory.Text.Trim();

                // =================================================
                // VALIDATION
                // =================================================

                if (productCode == "")
                {
                    MessageBox.Show(
                        "Please enter Product Code.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txtProductCode.Focus();
                    return;
                }

                if (productName == "")
                {
                    MessageBox.Show(
                        "Please enter Product Name.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txtProductName.Focus();
                    return;
                }

                if (categoryName == "")
                {
                    MessageBox.Show(
                        "Please enter Category.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txtCategory.Focus();
                    return;
                }

                if (cmbWarehouse.SelectedIndex == -1)
                {
                    MessageBox.Show(
                        "Please select a Warehouse.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    cmbWarehouse.Focus();
                    return;
                }

                if (cmbFloor.SelectedIndex == -1)
                {
                    MessageBox.Show(
                        "Please select a Floor.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    cmbFloor.Focus();
                    return;
                }

                if (cmbBin.SelectedIndex == -1)
                {
                    MessageBox.Show(
                        "Please select a BIN.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    cmbBin.Focus();
                    return;
                }

                // =================================================
                // UNIT PRICE
                // =================================================

                if (!decimal.TryParse(
                    txtUnitPrice.Text.Trim(),
                    out decimal unitPrice))
                {
                    MessageBox.Show(
                        "Please enter a valid Unit Price.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txtUnitPrice.Focus();
                    return;
                }

                if (unitPrice < 0)
                {
                    MessageBox.Show(
                        "Unit Price cannot be negative.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txtUnitPrice.Focus();
                    return;
                }

                // =================================================
                // QUANTITY
                // =================================================

                if (!int.TryParse(
                    txtQuantity.Text.Trim(),
                    out int quantity))
                {
                    MessageBox.Show(
                        "Please enter a valid Quantity.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txtQuantity.Focus();
                    return;
                }

                if (quantity < 0)
                {
                    MessageBox.Show(
                        "Quantity cannot be negative.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txtQuantity.Focus();
                    return;
                }

                // =================================================
                // GET SELECTED IDs
                // =================================================

                DataRowView warehouseRow =
                    cmbWarehouse.SelectedItem as DataRowView;

                DataRowView floorRow =
                    cmbFloor.SelectedItem as DataRowView;

                DataRowView binRow =
                    cmbBin.SelectedItem as DataRowView;

                if (warehouseRow == null ||
                    floorRow == null ||
                    binRow == null)
                {
                    MessageBox.Show(
                        "Please select Warehouse, Floor and BIN.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                int warehouseID =
                    Convert.ToInt32(
                        warehouseRow["WarehouseID"]);

                int floorID =
                    Convert.ToInt32(
                        floorRow["FloorID"]);

                int binID =
                    Convert.ToInt32(
                        binRow["BinID"]);

                // =================================================
                // DATABASE
                // =================================================

                using (SqlConnection con =
                    Database.GetConnection())
                {
                    con.Open();

                    using (SqlTransaction transaction =
                        con.BeginTransaction())
                    {
                        try
                        {
                            // =====================================
                            // CHECK FLOOR
                            // =====================================

                            string checkFloorQuery = @"
                                SELECT COUNT(*)
                                FROM dbo.Floors
                                WHERE FloorID = @FloorID
                                AND WarehouseID = @WarehouseID";

                            using (SqlCommand cmd =
                                new SqlCommand(
                                    checkFloorQuery,
                                    con,
                                    transaction))
                            {
                                cmd.Parameters.Add(
                                    "@FloorID",
                                    SqlDbType.Int
                                ).Value = floorID;

                                cmd.Parameters.Add(
                                    "@WarehouseID",
                                    SqlDbType.Int
                                ).Value = warehouseID;

                                int floorExists =
                                    Convert.ToInt32(
                                        cmd.ExecuteScalar());

                                if (floorExists == 0)
                                {
                                    throw new Exception(
                                        "The selected Floor does not belong " +
                                        "to the selected Warehouse."
                                    );
                                }
                            }

                            // =====================================
                            // CHECK BIN
                            // =====================================

                            string checkBinQuery = @"
                                SELECT COUNT(*)
                                FROM dbo.Bins
                                WHERE BinID = @BinID
                                AND FloorID = @FloorID";

                            using (SqlCommand cmd =
                                new SqlCommand(
                                    checkBinQuery,
                                    con,
                                    transaction))
                            {
                                cmd.Parameters.Add(
                                    "@BinID",
                                    SqlDbType.Int
                                ).Value = binID;

                                cmd.Parameters.Add(
                                    "@FloorID",
                                    SqlDbType.Int
                                ).Value = floorID;

                                int binExists =
                                    Convert.ToInt32(
                                        cmd.ExecuteScalar());

                                if (binExists == 0)
                                {
                                    throw new Exception(
                                        "The selected BIN does not belong " +
                                        "to the selected Floor."
                                    );
                                }
                            }

                            // =====================================
                            // CHECK PRODUCT CODE
                            // =====================================

                            string checkProductQuery = @"
                                SELECT COUNT(*)
                                FROM dbo.Products
                                WHERE ProductCode = @ProductCode";

                            using (SqlCommand cmd =
                                new SqlCommand(
                                    checkProductQuery,
                                    con,
                                    transaction))
                            {
                                cmd.Parameters.Add(
                                    "@ProductCode",
                                    SqlDbType.VarChar
                                ).Value = productCode;

                                int productExists =
                                    Convert.ToInt32(
                                        cmd.ExecuteScalar());

                                if (productExists > 0)
                                {
                                    throw new Exception(
                                        "Product Code already exists."
                                    );
                                }
                            }

                            // =====================================
                            // GET / CREATE CATEGORY
                            // =====================================

                            int categoryID;

                            string categoryQuery = @"
                                SELECT CategoryID
                                FROM dbo.Categories
                                WHERE CategoryName = @CategoryName";

                            using (SqlCommand cmd =
                                new SqlCommand(
                                    categoryQuery,
                                    con,
                                    transaction))
                            {
                                cmd.Parameters.Add(
                                    "@CategoryName",
                                    SqlDbType.VarChar
                                ).Value = categoryName;

                                object result =
                                    cmd.ExecuteScalar();

                                if (result != null)
                                {
                                    categoryID =
                                        Convert.ToInt32(result);
                                }
                                else
                                {
                                    string insertCategoryQuery = @"
                                        INSERT INTO dbo.Categories
                                        (CategoryName)
                                        OUTPUT INSERTED.CategoryID
                                        VALUES
                                        (@CategoryName)";

                                    using (SqlCommand insertCmd =
                                        new SqlCommand(
                                            insertCategoryQuery,
                                            con,
                                            transaction))
                                    {
                                        insertCmd.Parameters.Add(
                                            "@CategoryName",
                                            SqlDbType.VarChar
                                        ).Value = categoryName;

                                        categoryID =
                                            Convert.ToInt32(
                                                insertCmd.ExecuteScalar());
                                    }
                                }
                            }

                            // =====================================
                            // INSERT PRODUCT
                            // =====================================

                            string insertProductQuery = @"
                                INSERT INTO dbo.Products
                                (
                                    ProductCode,
                                    ProductName,
                                    CategoryID,
                                    WarehouseID,
                                    FloorID,
                                    BinID,
                                    UnitPrice,
                                    Quantity,
                                    Status,
                                    CreatedAt
                                )
                                VALUES
                                (
                                    @ProductCode,
                                    @ProductName,
                                    @CategoryID,
                                    @WarehouseID,
                                    @FloorID,
                                    @BinID,
                                    @UnitPrice,
                                    @Quantity,
                                    @Status,
                                    GETDATE()
                                )";

                            using (SqlCommand cmd =
                                new SqlCommand(
                                    insertProductQuery,
                                    con,
                                    transaction))
                            {
                                cmd.Parameters.Add(
                                    "@ProductCode",
                                    SqlDbType.VarChar
                                ).Value = productCode;

                                cmd.Parameters.Add(
                                    "@ProductName",
                                    SqlDbType.VarChar
                                ).Value = productName;

                                cmd.Parameters.Add(
                                    "@CategoryID",
                                    SqlDbType.Int
                                ).Value = categoryID;

                                cmd.Parameters.Add(
                                    "@WarehouseID",
                                    SqlDbType.Int
                                ).Value = warehouseID;

                                cmd.Parameters.Add(
                                    "@FloorID",
                                    SqlDbType.Int
                                ).Value = floorID;

                                cmd.Parameters.Add(
                                    "@BinID",
                                    SqlDbType.Int
                                ).Value = binID;

                                SqlParameter priceParameter =
                                    cmd.Parameters.Add(
                                        "@UnitPrice",
                                        SqlDbType.Decimal
                                    );

                                priceParameter.Precision = 18;
                                priceParameter.Scale = 2;
                                priceParameter.Value = unitPrice;

                                cmd.Parameters.Add(
                                    "@Quantity",
                                    SqlDbType.Int
                                ).Value = quantity;

                                cmd.Parameters.Add(
                                    "@Status",
                                    SqlDbType.VarChar
                                ).Value =
                                    quantity > 0
                                        ? "Available"
                                        : "Out of Stock";

                                cmd.ExecuteNonQuery();
                            }

                            // =====================================
                            // COMMIT
                            // =====================================

                            transaction.Commit();

                            MessageBox.Show(
                                "Product saved successfully!",
                                "Success",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );

                            // Tell the parent Product form
                            // that the save was successful.
                            this.DialogResult =
                                DialogResult.OK;

                            // Close Add Product
                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            MessageBox.Show(
                                "Error saving product:\n\n" +
                                ex.Message,
                                "Save Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Database Error:\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================================
        // BIN CHANGED
        // =========================================================
        private void cmbBin_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            // No code needed here for now.
        }

        // =========================================================
        // CANCEL
        // =========================================================
        private void btnCancel_Click(
            object sender,
            EventArgs e)
        {
            this.DialogResult =
                DialogResult.Cancel;

            this.Close();
        }
    }
}