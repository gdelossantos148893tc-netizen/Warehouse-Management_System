using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using WMsystem48;

namespace Warehouse_Management_System
{
    public partial class Warehouses : Form
    {
        private readonly string connectionString =
            @"Data Source=GERALDKIMDELOSS\SQLEXPRESS;
              Initial Catalog=WMSystem;
              Integrated Security=True;
              TrustServerCertificate=True;";

        private int selectedWarehouseID = 0;

        public Warehouses()
        {
            InitializeComponent();

            button1.Click -= button1_Click;
            button1.Click += button1_Click;

            treeView1.AfterSelect -= treeView1_AfterSelect;
            treeView1.AfterSelect += treeView1_AfterSelect;

            this.Load -= Warehouses_Load;
            this.Load += Warehouses_Load;
        }

        private void Warehouses_Load(object sender, EventArgs e)
        {
            treeView1.BackColor = Color.FromArgb(30, 40, 88);
            treeView1.ForeColor = Color.White;
            treeView1.Font = new Font("Segoe UI", 9F);
            treeView1.BorderStyle = BorderStyle.FixedSingle;
            treeView1.HideSelection = false;
            treeView1.FullRowSelect = true;

            LoadWarehouseTree();

            panel3.Controls.Clear();
            selectedWarehouseID = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowAddWarehouseForm();
        }

        private void ShowAddWarehouseForm()
        {
            using (Form addForm = new Form())
            {
                addForm.Text = "Add Warehouse";
                addForm.StartPosition = FormStartPosition.CenterParent;
                addForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                addForm.MaximizeBox = false;
                addForm.MinimizeBox = false;
                addForm.ClientSize = new Size(430, 280);
                addForm.BackColor = Color.FromArgb(30, 40, 70);

                Label lblTitle = new Label();
                lblTitle.Text = "Add Warehouse";
                lblTitle.ForeColor = Color.White;
                lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
                lblTitle.Location = new Point(30, 20);
                lblTitle.AutoSize = true;

                Label lblWarehouseName = new Label();
                lblWarehouseName.Text = "Warehouse Name";
                lblWarehouseName.ForeColor = Color.White;
                lblWarehouseName.Location = new Point(30, 70);
                lblWarehouseName.AutoSize = true;

                TextBox txtWarehouseName = new TextBox();
                txtWarehouseName.Location = new Point(30, 95);
                txtWarehouseName.Size = new Size(370, 25);

                Label lblLocation = new Label();
                lblLocation.Text = "Location";
                lblLocation.ForeColor = Color.White;
                lblLocation.Location = new Point(30, 130);
                lblLocation.AutoSize = true;

                TextBox txtLocation = new TextBox();
                txtLocation.Location = new Point(30, 155);
                txtLocation.Size = new Size(370, 25);

                Button btnCancel = new Button();
                btnCancel.Text = "Cancel";
                btnCancel.Location = new Point(215, 205);
                btnCancel.Size = new Size(85, 35);
                btnCancel.BackColor = Color.FromArgb(50, 65, 100);
                btnCancel.ForeColor = Color.White;
                btnCancel.FlatStyle = FlatStyle.Flat;
                btnCancel.DialogResult = DialogResult.Cancel;

                Button btnSave = new Button();
                btnSave.Text = "Save Warehouse";
                btnSave.Location = new Point(310, 205);
                btnSave.Size = new Size(110, 35);
                btnSave.BackColor = Color.FromArgb(0, 180, 80);
                btnSave.ForeColor = Color.White;
                btnSave.FlatStyle = FlatStyle.Flat;

                addForm.Controls.Add(lblTitle);
                addForm.Controls.Add(lblWarehouseName);
                addForm.Controls.Add(txtWarehouseName);
                addForm.Controls.Add(lblLocation);
                addForm.Controls.Add(txtLocation);
                addForm.Controls.Add(btnCancel);
                addForm.Controls.Add(btnSave);

                addForm.AcceptButton = btnSave;
                addForm.CancelButton = btnCancel;

                btnSave.Click += (s, e) =>
                {
                    string warehouseName =
                        txtWarehouseName.Text.Trim();

                    string location =
                        txtLocation.Text.Trim();

                    if (string.IsNullOrWhiteSpace(warehouseName))
                    {
                        MessageBox.Show(
                            "Please enter Warehouse Name.",
                            "Required",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        txtWarehouseName.Focus();
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(location))
                    {
                        MessageBox.Show(
                            "Please enter Location.",
                            "Required",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        txtLocation.Focus();
                        return;
                    }

                    try
                    {
                        using (SqlConnection con =
                            new SqlConnection(connectionString))
                        {
                            con.Open();

                            string checkQuery = @"
                                SELECT COUNT(*)
                                FROM dbo.Warehouses
                                WHERE WarehouseName = @WarehouseName
                                AND Status = 'Active';";

                            using (SqlCommand checkCmd =
                                new SqlCommand(checkQuery, con))
                            {
                                checkCmd.Parameters.Add(
                                    "@WarehouseName",
                                    System.Data.SqlDbType.NVarChar, 255)
                                    .Value = warehouseName;

                                int count =
                                    Convert.ToInt32(
                                        checkCmd.ExecuteScalar());

                                if (count > 0)
                                {
                                    MessageBox.Show(
                                        "Warehouse already exists!",
                                        "Duplicate Warehouse",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);

                                    return;
                                }
                            }

                            string insertQuery = @"
                                INSERT INTO dbo.Warehouses
                                (
                                    WarehouseName,
                                    Location,
                                    Status
                                )
                                VALUES
                                (
                                    @WarehouseName,
                                    @Location,
                                    'Active'
                                );";

                            using (SqlCommand cmd =
                                new SqlCommand(insertQuery, con))
                            {
                                cmd.Parameters.Add(
                                    "@WarehouseName",
                                    System.Data.SqlDbType.NVarChar, 255)
                                    .Value = warehouseName;

                                cmd.Parameters.Add(
                                    "@Location",
                                    System.Data.SqlDbType.NVarChar, 255)
                                    .Value = location;

                                cmd.ExecuteNonQuery();
                            }
                        }

                        MessageBox.Show(
                            "Warehouse added successfully!",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        addForm.DialogResult = DialogResult.OK;
                        addForm.Close();

                        LoadWarehouseTree();

                        panel3.Controls.Clear();
                        selectedWarehouseID = 0;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Error adding warehouse:\n\n" +
                            ex.Message,
                            "Database Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                };

                addForm.ShowDialog(this);
            }
        }

        private void LoadWarehouseTree()
        {
            treeView1.BeginUpdate();

            try
            {
                treeView1.Nodes.Clear();

                string query = @"
                    SELECT
                        w.WarehouseID,
                        w.WarehouseName,
                        f.FloorID,
                        f.FloorNumber,
                        f.FloorName,
                        b.BinID,
                        b.BinCode,
                        b.BinName
                    FROM dbo.Warehouses w
                    LEFT JOIN dbo.Floors f
                        ON w.WarehouseID = f.WarehouseID
                    LEFT JOIN dbo.Bins b
                        ON f.FloorID = b.FloorID
                    WHERE w.Status = 'Active'
                    ORDER BY
                        w.WarehouseID,
                        f.FloorNumber,
                        b.BinCode;";

                using (SqlConnection con =
                    new SqlConnection(connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(query, con))
                {
                    con.Open();

                    using (SqlDataReader reader =
                        cmd.ExecuteReader())
                    {
                        TreeNode warehouseNode = null;
                        TreeNode floorNode = null;

                        int currentWarehouseID = -1;
                        int currentFloorID = -1;

                        while (reader.Read())
                        {
                            int warehouseID =
                                Convert.ToInt32(
                                    reader["WarehouseID"]);

                            if (warehouseID != currentWarehouseID)
                            {
                                warehouseNode =
                                    new TreeNode(
                                        reader["WarehouseName"].ToString());

                                warehouseNode.Tag = warehouseID;

                                treeView1.Nodes.Add(warehouseNode);

                                currentWarehouseID = warehouseID;
                                currentFloorID = -1;
                                floorNode = null;
                            }

                            if (reader["FloorID"] != DBNull.Value)
                            {
                                int floorID =
                                    Convert.ToInt32(
                                        reader["FloorID"]);

                                if (floorID != currentFloorID)
                                {
                                    floorNode =
                                        new TreeNode(
                                            "Floor " +
                                            reader["FloorNumber"] +
                                            " - " +
                                            reader["FloorName"]);

                                    floorNode.Tag = floorID;

                                    warehouseNode.Nodes.Add(floorNode);

                                    currentFloorID = floorID;
                                }

                                if (reader["BinID"] != DBNull.Value)
                                {
                                    TreeNode binNode =
                                        new TreeNode(
                                            reader["BinCode"] +
                                            " - " +
                                            reader["BinName"]);

                                    binNode.Tag =
                                        Convert.ToInt32(
                                            reader["BinID"]);

                                    floorNode.Nodes.Add(binNode);
                                }
                            }
                        }
                    }
                }

                treeView1.ExpandAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading warehouses:\n\n" +
                    ex.Message,
                    "Warehouse Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                treeView1.EndUpdate();
            }
        }

        private void treeView1_AfterSelect(
            object sender,
            TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            if (e.Node.Parent == null &&
                e.Node.Tag != null)
            {
                selectedWarehouseID =
                    Convert.ToInt32(e.Node.Tag);

                ShowWarehouseDetails(
                    selectedWarehouseID);
            }
        }

        private void ShowWarehouseDetails(
            int warehouseID)
        {
            if (panel3 == null)
                return;

            panel3.Controls.Clear();

            if (warehouseID <= 0)
                return;

            try
            {
                string warehouseName = "";
                string location = "";
                int totalFloors = 0;

                string query = @"
                    SELECT
                        w.WarehouseName,
                        w.Location,
                        (
                            SELECT COUNT(*)
                            FROM dbo.Floors f
                            WHERE f.WarehouseID =
                                  w.WarehouseID
                        ) AS TotalFloors
                    FROM dbo.Warehouses w
                    WHERE w.WarehouseID = @WarehouseID
                    AND w.Status = 'Active';";

                using (SqlConnection con =
                    new SqlConnection(connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(query, con))
                {
                    cmd.Parameters.Add(
                        "@WarehouseID",
                        System.Data.SqlDbType.Int)
                        .Value = warehouseID;

                    con.Open();

                    using (SqlDataReader reader =
                        cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return;

                        warehouseName =
                            reader["WarehouseName"].ToString();

                        location =
                            reader["Location"].ToString();

                        totalFloors =
                            Convert.ToInt32(
                                reader["TotalFloors"]);
                    }
                }

                Label lblTitle = new Label();
                lblTitle.Text = warehouseName;
                lblTitle.ForeColor = Color.White;
                lblTitle.Font =
                    new Font(
                        "Segoe UI",
                        20F,
                        FontStyle.Bold);
                lblTitle.Location = new Point(20, 20);
                lblTitle.AutoSize = true;

                panel3.Controls.Add(lblTitle);

                AddDetailLabel(
                    "WAREHOUSE NAME",
                    warehouseName,
                    75);

                AddDetailLabel(
                    "LOCATION",
                    location,
                    145);

                AddDetailLabel(
                    "TOTAL FLOORS",
                    totalFloors.ToString(),
                    220);

                if (warehouseName.Trim().Equals(
                    "Main Warehouse",
                    StringComparison.OrdinalIgnoreCase))
                {
                    ShowFloorOverview(warehouseID);
                }
                else
                {
                    AddDeleteButton(warehouseID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading warehouse details:\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void AddDetailLabel(
            string title,
            string value,
            int y)
        {
            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.ForeColor = Color.LightGray;
            lblTitle.Font =
                new Font("Segoe UI", 9F);
            lblTitle.Location =
                new Point(20, y);
            lblTitle.AutoSize = true;

            panel3.Controls.Add(lblTitle);

            Label lblValue = new Label();
            lblValue.Text = value;
            lblValue.ForeColor = Color.White;
            lblValue.Font =
                new Font(
                    "Segoe UI",
                    13F,
                    FontStyle.Bold);
            lblValue.Location =
                new Point(20, y + 22);
            lblValue.AutoSize = true;

            panel3.Controls.Add(lblValue);
        }

        private void ShowFloorOverview(int warehouseID)
        {
            Label lblOverview = new Label();
            lblOverview.Text = "FLOOR OVERVIEW";
            lblOverview.ForeColor = Color.LightGray;
            lblOverview.Font =
                new Font("Segoe UI", 9F);
            lblOverview.Location =
                new Point(20, 285);
            lblOverview.AutoSize = true;

            panel3.Controls.Add(lblOverview);

            int currentY = 310;

            try
            {
                string query = @"
                    SELECT
                        f.FloorName,
                        COUNT(b.BinID) AS TotalBins
                    FROM dbo.Floors f
                    LEFT JOIN dbo.Bins b
                        ON f.FloorID = b.FloorID
                    WHERE f.WarehouseID = @WarehouseID
                    GROUP BY
                        f.FloorID,
                        f.FloorName,
                        f.FloorNumber
                    ORDER BY f.FloorNumber;";

                using (SqlConnection con =
                    new SqlConnection(connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(query, con))
                {
                    cmd.Parameters.Add(
                        "@WarehouseID",
                        System.Data.SqlDbType.Int)
                        .Value = warehouseID;

                    con.Open();

                    using (SqlDataReader reader =
                        cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Label lblFloor = new Label();

                            lblFloor.Text =
                                reader["FloorName"]
                                .ToString()
                                .ToUpper();

                            lblFloor.ForeColor =
                                Color.LightGray;

                            lblFloor.Location =
                                new Point(20, currentY);

                            lblFloor.AutoSize = true;

                            panel3.Controls.Add(lblFloor);

                            Label lblBins = new Label();

                            lblBins.Text =
                                reader["TotalBins"] +
                                " BINS";

                            lblBins.ForeColor =
                                Color.White;

                            lblBins.Font =
                                new Font(
                                    "Segoe UI",
                                    13F,
                                    FontStyle.Bold);

                            lblBins.Location =
                                new Point(
                                    20,
                                    currentY + 18);

                            lblBins.AutoSize = true;

                            panel3.Controls.Add(lblBins);

                            currentY += 58;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading floor overview:\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void AddDeleteButton(int warehouseID)
        {
            Button deleteButton =
                new Button();

            deleteButton.Text = "Delete";
            deleteButton.Size =
                new Size(85, 35);

            deleteButton.Location =
                new Point(
                    Math.Max(
                        20,
                        panel3.Width - 105),
                    350);

            deleteButton.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            deleteButton.BackColor = Color.Red;
            deleteButton.ForeColor = Color.White;
            deleteButton.FlatStyle = FlatStyle.Flat;
            deleteButton.FlatAppearance.BorderSize = 0;
            deleteButton.Cursor = Cursors.Hand;

            deleteButton.Click +=
                (sender, e) =>
                {
                    DeleteWarehouse(
                        warehouseID);
                };

            panel3.Controls.Add(deleteButton);
        }

        private void DeleteWarehouse(int warehouseID)
        {
            try
            {
                string warehouseName = "";

                using (SqlConnection con =
                    new SqlConnection(connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(
                        @"SELECT WarehouseName
                          FROM dbo.Warehouses
                          WHERE WarehouseID = @WarehouseID
                          AND Status = 'Active';",
                        con))
                {
                    cmd.Parameters.Add(
                        "@WarehouseID",
                        System.Data.SqlDbType.Int)
                        .Value = warehouseID;

                    con.Open();

                    object result =
                        cmd.ExecuteScalar();

                    if (result == null)
                        return;

                    warehouseName =
                        result.ToString();
                }

                if (warehouseName.Trim().Equals(
                    "Main Warehouse",
                    StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "Main Warehouse cannot be deleted.",
                        "Not Allowed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                DialogResult confirm =
                    MessageBox.Show(
                        "Are you sure you want to delete:\n\n" +
                        warehouseName + "?",
                        "Delete Warehouse",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes)
                    return;

                using (SqlConnection con =
                    new SqlConnection(connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(
                        @"UPDATE dbo.Warehouses
                          SET Status = 'Inactive'
                          WHERE WarehouseID = @WarehouseID;",
                        con))
                {
                    cmd.Parameters.Add(
                        "@WarehouseID",
                        System.Data.SqlDbType.Int)
                        .Value = warehouseID;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show(
                    "Warehouse deleted successfully!",
                    "Deleted",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                selectedWarehouseID = 0;
                panel3.Controls.Clear();

                LoadWarehouseTree();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error deleting warehouse:\n\n" +
                    ex.Message,
                    "Delete Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnDashboard_Click(
            object sender,
            EventArgs e)
        {
            Dashboard dashboard =
                new Dashboard();

            dashboard.Show();
            this.Hide();
        }

        private void btnProducts_Click(
            object sender,
            EventArgs e)
        {
            product productForm =
                new product();

            productForm.Show();
            this.Hide();
        }

        private void btnWarehouse_Click(
            object sender,
            EventArgs e)
        {
            LoadWarehouseTree();
        }

        private void btnReports_Click(
            object sender,
            EventArgs e)
        {
            Reports reportsForm =
                new Reports();

            reportsForm.Show();
            this.Hide();
        }

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

        private void panel3_Paint(
            object sender,
            PaintEventArgs e)
        {
        }

        private void btnDelete_Click(
            object sender,
            EventArgs e)
        {
            if (selectedWarehouseID > 0)
            {
                DeleteWarehouse(
                    selectedWarehouseID);
            }
        }
    }
}