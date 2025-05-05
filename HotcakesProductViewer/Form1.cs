using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Required for App.config settings
using System.Configuration;

// Required namespaces based on Hotcakes documentation
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.CommerceDTO.v1.Client;

namespace HotcakesProductViewer // Use the namespace of your project
{
    public partial class Form1 : Form
    {
        private Api apiClient; // The Hotcakes API client proxy

        public Form1()
        {
            InitializeComponent();
            InitializeApiClient();
            ConfigureProductGrid();
        }

        /// <summary>
        /// Reads configuration and initializes the Hotcakes API client.
        /// </summary>
        private void InitializeApiClient()
        {
            try
            {
                string apiUrl = ConfigurationManager.AppSettings["HotcakesApiBaseUrl"];
                string apiKey = ConfigurationManager.AppSettings["HotcakesApiKey"];

                // Basic validation for configuration
                if (string.IsNullOrWhiteSpace(apiUrl) || apiUrl == "http://YOURDOMAIN.COM" ||
                    string.IsNullOrWhiteSpace(apiKey) || apiKey == "YOUR-API-KEY")
                {
                    MessageBox.Show("API URL or Key is not configured correctly in App.config.\nPlease update it and restart the application.",
                                    "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    apiClient = null; // Prevent further API calls
                    loadProductsButton.Enabled = false; // Disable button
                    return;
                }

                apiClient = new Api(apiUrl, apiKey);
            }
            catch (ConfigurationErrorsException ex)
            {
                MessageBox.Show($"Error reading configuration from App.config: {ex.Message}",
                                "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                apiClient = null;
                loadProductsButton.Enabled = false;
            }
            catch (Exception ex) // Catch other potential errors during init
            {
                MessageBox.Show($"An unexpected error occurred during API initialization: {ex.Message}",
                               "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                apiClient = null;
                loadProductsButton.Enabled = false;
            }
        }

        /// <summary>
        /// Sets up the columns for the DataGridView.
        /// </summary>
        private void ConfigureProductGrid()
        {
            productsDataGridView.AutoGenerateColumns = false; // We define columns manually
            productsDataGridView.Columns.Clear();

            // Add columns for relevant ProductDTO properties
            // DataPropertyName MUST match the exact property name in ProductDTO
            productsDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SkuColumn",
                DataPropertyName = "Sku", // Property name in ProductDTO
                HeaderText = "SKU",
                Width = 120
            });
            productsDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NameColumn",
                DataPropertyName = "ProductName", // Property name in ProductDTO
                HeaderText = "Product Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill // Take remaining space
            });
            productsDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PriceColumn",
                DataPropertyName = "SitePrice", // Property name in ProductDTO
                HeaderText = "Price",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } // Format as number
            });
            productsDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "BvinColumn",
                DataPropertyName = "Bvin", // Property name in ProductDTO
                HeaderText = "Bvin",
                Visible = false // Typically hidden
            });
        }

        /// <summary>
        /// Handles the click event for the Load Products button.
        /// </summary>
        private void loadProductsButton_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

        /// <summary>
        /// Calls the Hotcakes API to find all products and displays them in the grid.
        /// </summary>
        private async void LoadProducts() // Mark as async for potential future await
        {
            if (apiClient == null)
            {
                MessageBox.Show("API Client is not initialized. Check App.config.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Provide visual feedback
            this.Cursor = Cursors.WaitCursor;
            loadProductsButton.Enabled = false;
            productsDataGridView.DataSource = null; // Clear previous data

            try
            {
                // Call the API endpoint as per documentation
                // Using Task.Run to avoid blocking the UI thread for the network call
                ApiResponse<List<ProductDTO>> response = await Task.Run(() => apiClient.ProductsFindAll());
                // Check for API-level errors
                if (response.Errors != null && response.Errors.Any())
                {
                    string errorMsg = string.Join("\n", response.Errors.Select(err => $"{err.Code}: {err.Description}"));
                    MessageBox.Show($"Failed to load products:\n{errorMsg}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    productsDataGridView.DataSource = null; // Ensure grid is empty on error
                }
                else if (response.Content != null)
                {
                    // Success: Bind the list of products to the DataGridView
                    productsDataGridView.DataSource = response.Content;
                    this.Text = $"Product Viewer - {response.Content.Count} products loaded"; // Update title bar
                }
                else
                {
                    // Success, but no content (empty list)
                    productsDataGridView.DataSource = null;
                    this.Text = "Product Viewer - No products found";
                    MessageBox.Show("Successfully connected, but no products were found in the store.", "Empty Catalog", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex) // Catch unexpected errors (network issues, etc.)
            {
                MessageBox.Show($"Hiba típusa: {ex.GetType()}\nÜzenet: {ex.Message}\nBelső hiba: {ex.InnerException?.Message}\n\nStackTrace: {ex.StackTrace}",
                                "Részletes Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                productsDataGridView.DataSource = null;
            }
            finally
            {
                // Restore UI state
                this.Cursor = Cursors.Default;
                loadProductsButton.Enabled = true;
            }
        }
    }
}