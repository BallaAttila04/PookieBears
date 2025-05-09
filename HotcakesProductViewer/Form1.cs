using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;

namespace HotcakesProductViewer
{
    public partial class Form1 : Form
    {
        private Guid? _selectedProductBvin = null;
        private string _connectionString = null;
        private int _eraPropertyId = -1;
        private int _frontPropertyId = -1;
        private int _terrainPropertyId = -1;

        // Property Nevek (Pontosan egyezzenek a DB-ben lévőkkel!)
        private const string PROP_NAME_ERA = "Korszak_Tag";
        private const string PROP_NAME_FRONT = "Front_Tag";
        private const string PROP_NAME_TERRAIN = "Terep_Tag";

        private const int FIXED_TAG_ROW_COUNT = 10; // Fix sorok száma a DataGridView-ben

        // DataTable a címkék DataGridView-jéhez
        private DataTable _tagsDataTable;

        public Form1()
        {
            InitializeComponent();
            LoadConnectionString();
            ConfigureProductGrid(); // A terméklistához
            InitializeTagsDataGridView(); // Az új címke DataGridView-hez
            ClearAndDisableTagEditor(); // Kezdetben a szerkesztő (most a DGV) le van tiltva
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadPropertyIds();
            LoadProducts();
        }

        private void LoadConnectionString()
        {
            try
            {
                _connectionString = ConfigurationManager.ConnectionStrings["HotcakesDb"]?.ConnectionString;
                if (string.IsNullOrEmpty(_connectionString))
                {
                    MessageBox.Show("A 'HotcakesDb' connection string nem található vagy üres az App.config fájlban.", "Konfigurációs Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    loadProductsButton.Enabled = false;
                    if (btnSearch != null) btnSearch.Enabled = false;
                    if (btnSaveTags != null) btnSaveTags.Enabled = false;
                }
            }
            catch (ConfigurationErrorsException ex)
            {
                MessageBox.Show($"Hiba az App.config olvasása közben: {ex.Message}", "Konfigurációs Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _connectionString = null; // Hiba esetén nullázzuk
                loadProductsButton.Enabled = false;
                if (btnSearch != null) btnSearch.Enabled = false;
                if (btnSaveTags != null) btnSaveTags.Enabled = false;
            }
        }

        private void LoadPropertyIds()
        {
            if (string.IsNullOrEmpty(_connectionString)) return;

            _eraPropertyId = GetPropertyId(PROP_NAME_ERA);
            _frontPropertyId = GetPropertyId(PROP_NAME_FRONT);
            _terrainPropertyId = GetPropertyId(PROP_NAME_TERRAIN);

            if (_eraPropertyId == -1 || _frontPropertyId == -1 || _terrainPropertyId == -1)
            {
                MessageBox.Show($"Egy vagy több szükséges terméktulajdonság ({PROP_NAME_ERA}, {PROP_NAME_FRONT}, {PROP_NAME_TERRAIN}) nem található az adatbázisban (hcc_ProductProperty tábla, StoreId=1). A címkeszerkesztés nem fog működni.", "Konfigurációs Hiba", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (btnSaveTags != null) btnSaveTags.Enabled = false; // Mentés letiltása, ha nincs ID
            }
            else
            {
                if (btnSaveTags != null) btnSaveTags.Enabled = true; // Engedélyezzük, ha megvannak az ID-k
            }
        }

        private int GetPropertyId(string propertyName)
        {
            if (string.IsNullOrEmpty(_connectionString)) return -1;
            string sqlQuery = "SELECT TOP 1 Id FROM [dbo].[hcc_ProductProperty] WHERE PropertyName = @PropertyName AND StoreId = 1;"; // Visszatettem a StoreId=1 szűrést
            int propertyId = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.Add(new SqlParameter("@PropertyName", SqlDbType.NVarChar) { Value = propertyName });
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value) { propertyId = Convert.ToInt32(result); }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a(z) '{propertyName}' Property ID lekérdezése közben: {ex.Message}", "Adatbázis Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return propertyId;
        }

        private void ConfigureProductGrid()
        {
            // Ez változatlanul a termékeket listázó DataGridView-t konfigurálja
            productsDataGridView.SelectionChanged -= productsDataGridView_SelectionChanged;
            productsDataGridView.AutoGenerateColumns = false;
            productsDataGridView.Columns.Clear();
            productsDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "SkuColumn", DataPropertyName = "Sku", HeaderText = "SKU", Width = 120 });
            productsDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "NameColumn", DataPropertyName = "ProductName", HeaderText = "Product Name", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            productsDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "PriceColumn", DataPropertyName = "SitePrice", HeaderText = "Price", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            productsDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "BvinColumn", DataPropertyName = "bvin", HeaderText = "Bvin", Visible = false });
            productsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            productsDataGridView.MultiSelect = false;
            productsDataGridView.SelectionChanged += productsDataGridView_SelectionChanged;
        }

        private void InitializeTagsDataGridView()
        {
            _tagsDataTable = new DataTable("ProductTags");
            // Oszlopok hozzáadása a DataTable-hez a Property Nevek alapján
            _tagsDataTable.Columns.Add(PROP_NAME_ERA, typeof(string));
            _tagsDataTable.Columns.Add(PROP_NAME_FRONT, typeof(string));
            _tagsDataTable.Columns.Add(PROP_NAME_TERRAIN, typeof(string));

            // Feltöltjük 10 üres sorral
            for (int i = 0; i < FIXED_TAG_ROW_COUNT; i++)
            {
                _tagsDataTable.Rows.Add(_tagsDataTable.NewRow());
            }

            // DataGridView beállítása
            dgvTags.DataSource = _tagsDataTable; // Tegyük fel, a DGV neve 'dgvTags'
            dgvTags.AllowUserToAddRows = false; // A sorok fixek
            dgvTags.AllowUserToDeleteRows = false; // A sorok fixek

            // Oszlopok megjelenítési nevei (opcionális)
            if (dgvTags.Columns[PROP_NAME_ERA] != null) dgvTags.Columns[PROP_NAME_ERA].HeaderText = "Korszak Címkék";
            if (dgvTags.Columns[PROP_NAME_FRONT] != null) dgvTags.Columns[PROP_NAME_FRONT].HeaderText = "Front Címkék";
            if (dgvTags.Columns[PROP_NAME_TERRAIN] != null) dgvTags.Columns[PROP_NAME_TERRAIN].HeaderText = "Terep Címkék";
        }


        private void LoadProducts(string searchTerm = null)
        {
            if (string.IsNullOrEmpty(_connectionString)) return;
            string baseQuery = @"SELECT p.bvin, p.Sku, n.Title as ProductName, p.SitePrice FROM [dbo].[hcc_Product] p JOIN [dbo].[hcc_SearchObjects] n ON p.bvin = n.ObjectId WHERE p.StoreId = 1 AND p.Status = 1 ";
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (!string.IsNullOrWhiteSpace(searchTerm)) { baseQuery += " AND (n.Title LIKE @SearchTerm OR p.Sku LIKE @SearchTerm) "; parameters.Add(new SqlParameter("@SearchTerm", SqlDbType.NVarChar) { Value = $"%{searchTerm}%" }); }
            baseQuery += " ORDER BY n.Title;"; string sqlQuery = baseQuery;
            DataTable productTable = new DataTable();
            this.Cursor = Cursors.WaitCursor;
            loadProductsButton.Enabled = false; if (btnSearch != null) btnSearch.Enabled = false;
            productsDataGridView.DataSource = null;
            ClearAndDisableTagEditor();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    if (parameters.Any()) { command.Parameters.AddRange(parameters.ToArray()); }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command)) { adapter.Fill(productTable); }
                }
                productsDataGridView.DataSource = productTable;
                this.Text = $"Product Tagger (DB) - {productTable.Rows.Count} products loaded";
            }
            catch (Exception ex) { MessageBox.Show($"Hiba a termékek lekérdezése közben: {ex.Message}", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { this.Cursor = Cursors.Default; loadProductsButton.Enabled = true; if (btnSearch != null) btnSearch.Enabled = true; }
        }

        private void loadProductsButton_Click(object sender, EventArgs e) { LoadProducts(); }
        private void btnSearch_Click(object sender, EventArgs e) { LoadProducts(txtSearch.Text); }

        private void productsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (productsDataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = productsDataGridView.SelectedRows[0];
                if (selectedRow.DataBoundItem is DataRowView rowView)
                {
                    if (rowView.Row.Table.Columns.Contains("bvin") && rowView["bvin"] != DBNull.Value)
                    {
                        _selectedProductBvin = (Guid)rowView["bvin"];
                        string productName = rowView.Row.Table.Columns.Contains("ProductName") ? rowView["ProductName"].ToString() : "N/A";
                        lblSelectedProduct.Text = $"Kiválasztva: {productName}";
                        dgvTags.Enabled = true; // Engedélyezzük a címke DGV-t
                        btnSaveTags.Enabled = _eraPropertyId != -1 && _frontPropertyId != -1 && _terrainPropertyId != -1; // Mentés gomb csak ha vannak Property ID-k
                        LoadTagsForProduct(_selectedProductBvin.Value);
                    }
                    else { ClearAndDisableTagEditor(); }
                }
                else { ClearAndDisableTagEditor(); }
            }
            else { ClearAndDisableTagEditor(); }
        }

        // --- Módosított Címke Kezelés ---
        private void ClearAndDisableTagEditor()
        {
            _selectedProductBvin = null;
            if (lblSelectedProduct != null) lblSelectedProduct.Text = "Nincs kiválasztott termék";

            // Ürítjük a DataTable sorait
            if (_tagsDataTable != null)
            {
                foreach (DataRow row in _tagsDataTable.Rows)
                {
                    row[PROP_NAME_ERA] = DBNull.Value;
                    row[PROP_NAME_FRONT] = DBNull.Value;
                    row[PROP_NAME_TERRAIN] = DBNull.Value;
                }
            }
            if (dgvTags != null) dgvTags.Enabled = false;
            if (btnSaveTags != null) btnSaveTags.Enabled = false;
        }

        private void LoadTagsForProduct(Guid productBvin)
        {
            // Először ürítjük a DataTable sorait, hogy csak az aktuális termék címkéi legyenek benne
            foreach (DataRow row in _tagsDataTable.Rows)
            {
                row[PROP_NAME_ERA] = DBNull.Value;
                row[PROP_NAME_FRONT] = DBNull.Value;
                row[PROP_NAME_TERRAIN] = DBNull.Value;
            }

            if (string.IsNullOrEmpty(_connectionString)) return;

            // Lekérdezzük a 3 propertyhez tartozó | elválasztott stringeket
            var propertyValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            string sqlQuery = @"
                SELECT prop.PropertyName, pv.PropertyValue
                FROM [dbo].[hcc_ProductPropertyValue] pv
                JOIN [dbo].[hcc_ProductProperty] prop ON pv.PropertyId = prop.Id
                WHERE pv.ProductBvin = @ProductBvin
                  AND prop.PropertyName IN (@PropNameEra, @PropNameFront, @PropNameTerrain);";
            // AND pv.StoreId = 1 -- Ezt kivettük, mert a PropertyDef-ben StoreId=1 van a GetPropertyId-ban
            // AND prop.StoreId = 1 -- Ezt is, a GetPropertyId miatt

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@ProductBvin", SqlDbType.UniqueIdentifier) { Value = productBvin });
                        command.Parameters.Add(new SqlParameter("@PropNameEra", SqlDbType.NVarChar) { Value = PROP_NAME_ERA });
                        command.Parameters.Add(new SqlParameter("@PropNameFront", SqlDbType.NVarChar) { Value = PROP_NAME_FRONT });
                        command.Parameters.Add(new SqlParameter("@PropNameTerrain", SqlDbType.NVarChar) { Value = PROP_NAME_TERRAIN });

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string propertyName = reader["PropertyName"]?.ToString();
                                string propertyValue = reader["PropertyValue"]?.ToString();
                                if (!string.IsNullOrEmpty(propertyName))
                                {
                                    propertyValues[propertyName] = propertyValue; // Felülírjuk, ha több van (nem kellene)
                                }
                            }
                        }
                    }
                }

                // Stringek szétbontása és DataTable feltöltése
                FillDataTableWithTags(PROP_NAME_ERA, propertyValues.ContainsKey(PROP_NAME_ERA) ? propertyValues[PROP_NAME_ERA] : "");
                FillDataTableWithTags(PROP_NAME_FRONT, propertyValues.ContainsKey(PROP_NAME_FRONT) ? propertyValues[PROP_NAME_FRONT] : "");
                FillDataTableWithTags(PROP_NAME_TERRAIN, propertyValues.ContainsKey(PROP_NAME_TERRAIN) ? propertyValues[PROP_NAME_TERRAIN] : "");

                dgvTags.Refresh(); // Frissítjük a DataGridView megjelenítését
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a termék címkéinek betöltése közben: {ex.Message}", "Adatbázis Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearAndDisableTagEditor();
            }
        }

        private void FillDataTableWithTags(string columnName, string pipeSeparatedTags)
        {
            if (string.IsNullOrWhiteSpace(pipeSeparatedTags)) return;

            string[] tags = pipeSeparatedTags.Split('|').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToArray();

            for (int i = 0; i < tags.Length && i < FIXED_TAG_ROW_COUNT; i++)
            {
                _tagsDataTable.Rows[i][columnName] = tags[i];
            }
        }

        private void btnSaveTags_Click(object sender, EventArgs e)
        {
            if (!_selectedProductBvin.HasValue)
            {
                MessageBox.Show("Nincs kiválasztva termék a mentéshez.", "Hiba", MessageBoxButtons.OK);
                return;
            }
            if (_eraPropertyId == -1 || _frontPropertyId == -1 || _terrainPropertyId == -1)
            {
                MessageBox.Show($"A szükséges terméktulajdonság ID-k nincsenek betöltve. Mentés sikertelen.", "Konfigurációs Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(_connectionString)) return;

            // Adatok összegyűjtése a DataGridView-ből és összefűzése | jellel
            string eraTagsJoined = GetTagsFromColumn(PROP_NAME_ERA);
            string frontTagsJoined = GetTagsFromColumn(PROP_NAME_FRONT);
            string terrainTagsJoined = GetTagsFromColumn(PROP_NAME_TERRAIN);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlTransaction transaction = null;
                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    // Régi értékek törlése (csak a 3 kezelt propertyre)
                    string deleteSql = "DELETE FROM [dbo].[hcc_ProductPropertyValue] WHERE ProductBvin = @ProductBvin AND PropertyId IN (@EraId, @FrontId, @TerrainId);";
                    // AND StoreId = 1; -- Ezt is kivesszük, ha a Property definíció nem StoreId specifikus
                    using (SqlCommand deleteCmd = new SqlCommand(deleteSql, connection, transaction))
                    {
                        deleteCmd.Parameters.Add(new SqlParameter("@ProductBvin", SqlDbType.UniqueIdentifier) { Value = _selectedProductBvin.Value });
                        deleteCmd.Parameters.Add(new SqlParameter("@EraId", SqlDbType.Int) { Value = _eraPropertyId });
                        deleteCmd.Parameters.Add(new SqlParameter("@FrontId", SqlDbType.Int) { Value = _frontPropertyId });
                        deleteCmd.Parameters.Add(new SqlParameter("@TerrainId", SqlDbType.Int) { Value = _terrainPropertyId });
                        deleteCmd.ExecuteNonQuery();
                    }

                    // Új (összefűzött) értékek beszúrása, ha nem üresek
                    SavePropertyValues(connection, transaction, _selectedProductBvin.Value, _eraPropertyId, eraTagsJoined);
                    SavePropertyValues(connection, transaction, _selectedProductBvin.Value, _frontPropertyId, frontTagsJoined);
                    SavePropertyValues(connection, transaction, _selectedProductBvin.Value, _terrainPropertyId, terrainTagsJoined);

                    transaction.Commit();
                    MessageBox.Show("Címkék sikeresen mentve!", "Mentés", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // LoadTagsForProduct(_selectedProductBvin.Value); // Frissítés, ha kell
                }
                catch (Exception ex)
                {
                    try { transaction?.Rollback(); } catch { /* Rollback hiba */ }
                    MessageBox.Show($"Hiba történt a címkék mentése közben: {ex.Message}", "Mentési Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally { if (connection.State == ConnectionState.Open) { connection.Close(); } }
            }
        }

        private string GetTagsFromColumn(string columnName)
        {
            List<string> tags = new List<string>();
            for (int i = 0; i < FIXED_TAG_ROW_COUNT; i++)
            {
                // Ügyeljünk arra, hogy a DataGridView cella értéke ne legyen null vagy DBNull
                object cellValue = _tagsDataTable.Rows[i][columnName];
                if (cellValue != null && cellValue != DBNull.Value)
                {
                    string tag = cellValue.ToString().Trim();
                    if (!string.IsNullOrEmpty(tag))
                    {
                        tags.Add(tag);
                    }
                }
            }
            return string.Join("|", tags);
        }

        private void SavePropertyValues(SqlConnection connection, SqlTransaction transaction, Guid productBvin, int propertyId, string joinedTags)
        {
            // Csak akkor szúrjuk be, ha van mit (azaz a joinedTags nem üres)
            // Vagy ha az a logika, hogy üres stringet is mentünk, akkor ez az if nem kell
            if (!string.IsNullOrEmpty(joinedTags))
            {
                string insertSql = "INSERT INTO [dbo].[hcc_ProductPropertyValue] (ProductBvin, PropertyId, PropertyValue) VALUES (@ProductBvin, @PropertyId, @PropertyValue);";
                // StoreId = 1; -- Ezt is kivesszük, ha nem Store specifikus a property érték mentése
                using (SqlCommand insertCmd = new SqlCommand(insertSql, connection, transaction))
                {
                    insertCmd.Parameters.Add(new SqlParameter("@ProductBvin", SqlDbType.UniqueIdentifier) { Value = productBvin });
                    insertCmd.Parameters.Add(new SqlParameter("@PropertyId", SqlDbType.Int) { Value = propertyId });
                    insertCmd.Parameters.Add(new SqlParameter("@PropertyValue", SqlDbType.NVarChar) { Value = joinedTags }); // Vagy a DB-nek megfelelő típus!
                    insertCmd.ExecuteNonQuery();
                }
            }
            // Ha az a logika, hogy ha a joinedTags üres, akkor is kell egy sor üres PropertyValue-val,
            // akkor az if nélkül kellene futtatni az INSERT-et, vagy egy UPDATE-et, ha már létezik a sor.
            // A jelenlegi "DELETE ALL then INSERT non-empty" ezt implicit módon kezeli (ha üres, nem lesz sor).
        }
    }
}