using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using Microsoft.Msagl.Drawing;      // MSAGL rajzoláshoz
using Microsoft.Msagl.GraphViewerGdi; // GViewer vezérlőhöz
using System.Text;                  // StringBuilder-hez (opcionális, de tisztább lehet)

namespace HotcakesProductViewer // Vagy a te projekted névtér neve
{
    // Ezt az osztályt ide teheted, a Form2 osztályon KÍVÜLRE, de a névtéren BELÜLRE:


    public partial class Form2 : Form // Itt a te formod neve
    {
        private string _connectionString = null;
        private List<ProductGraphData> _allProductGraphData = new List<ProductGraphData>();

        // Property Nevek (Pontosan egyezzenek a DB-ben lévőkkel!)
        private const string PROP_NAME_ERA = "Korszak_Tag";
        private const string PROP_NAME_FRONT = "Konfliktus_Tag";
        private const string PROP_NAME_TERRAIN = "Terep_Tag";

        // Ezt a metódust a Form2.cs osztályodba másold be:

        /// <summary>
        /// Lekérdezi a Terep_Tag (PropertyId = 11) egyedi értékeit
        /// az adatbázisból és betölti őket a megadott CheckedListBox-ba.
        /// </summary>
        /// <param name="targetListBox">A CheckedListBox, amit fel kell tölteni (pl. this.chkLstTerrainFilter).</param>
        /// <param name="defaultCheckedState">Az elemek alapértelmezett bepipáltsági állapota.</param>
        public void PopulateTerrainFilterFromDb(CheckedListBox targetListBox, bool defaultCheckedState = true)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                LoadConnectionString();
                if (string.IsNullOrEmpty(_connectionString))
                {
                    MessageBox.Show("Hiányzó adatbázis kapcsolat a terep szűrő feltöltéséhez.", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            HashSet<string> distinctTerrainValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string sqlQuery = @"
        SELECT PropertyValue
        FROM [dbo].[hcc_ProductPropertyValue] -- ADATBÁZISNEVET CSERÉLD, HA KELL (pl. [MyDNNDatabase]...)
        WHERE PropertyId = 11  -- Fixen a Terep_Tag ID-ja
          AND PropertyValue IS NOT NULL
          AND PropertyValue <> '-'
          AND PropertyValue <> '';";

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string pipeSeparatedValues = reader["PropertyValue"]?.ToString();
                                if (!string.IsNullOrWhiteSpace(pipeSeparatedValues))
                                {
                                    string[] individualTags = pipeSeparatedValues.Split('|');
                                    foreach (string tag in individualTags)
                                    {
                                        string trimmedTag = tag.Trim();
                                        if (!string.IsNullOrEmpty(trimmedTag))
                                        {
                                            distinctTerrainValues.Add(trimmedTag);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                targetListBox.Items.Clear();
                foreach (string value in distinctTerrainValues.OrderBy(v => v)) // Rendezve adjuk hozzá
                {
                    targetListBox.Items.Add(value, defaultCheckedState);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a tereptípusok lekérdezése közben: {ex.Message}", "Adatbázis Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                targetListBox.Items.Clear(); // Hiba esetén is ürítjük
            }
        }

        public Form2() // Konstruktor neve is Form2
        {
            InitializeComponent();
            LoadConnectionString();
        }

        private void Form2_Load(object sender, EventArgs e) // Eseménykezelő neve is Form2-re utal
        {
            if (this.chkLstTerrainFilter != null) // Ellenőrizzük, hogy a vezérlő létezik-e
            {
                PopulateTerrainFilterFromDb(this.chkLstTerrainFilter, false); // Alapértelmezetten ne legyen semmi bepipálva
            }
            else
            {
                MessageBox.Show("A chkLstTerrainFilter vezérlő nem található a formon! A terep szűrés nem fog működni.", "Konfigurációs Hiba", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            // LoadAndDisplayGraphData(); // <-- EZT A SORT TÖRÖLJÜK VAGY KOMMENTELJÜK KI
            // Kezdetben üres gráf vagy semmi megjelenítése
            _allProductGraphData.Clear();
            BuildAndDisplayGraph();
            this.Text = "Termék Kapcsolati Gráf (Válasszon szűrőt és kattintson az Alkalmaz gombra)";
        }

        private void LoadConnectionString()
        {
            try
            {
                _connectionString = ConfigurationManager.ConnectionStrings["HotcakesDb"]?.ConnectionString;
                if (string.IsNullOrEmpty(_connectionString))
                {
                    MessageBox.Show("A 'HotcakesDb' connection string nem található vagy üres az App.config fájlban. A program nem tud működni.", "Konfigurációs Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            catch (ConfigurationErrorsException ex)
            {
                MessageBox.Show($"Hiba az App.config olvasása közben: {ex.Message}", "Konfigurációs Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _connectionString = null;
                this.Close();
            }
        }

        private void LoadAndDisplayGraphData()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                MessageBox.Show("Hiányzó adatbázis kapcsolat miatt a gráf nem tölthető be.", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. Kiválasztott tereptípusok összegyűjtése
            List<string> selectedTerrains = new List<string>();
            if (this.chkLstTerrainFilter != null) // Győződjünk meg róla, hogy létezik a vezérlő
            {
                foreach (object item in chkLstTerrainFilter.CheckedItems)
                {
                    selectedTerrains.Add(item.ToString());
                }
            }

            // Ha vannak elemek a listában, de nincs egy sem kiválasztva, jelezzük és ne töltsünk.
            if (this.chkLstTerrainFilter != null && this.chkLstTerrainFilter.Items.Count > 0 && selectedTerrains.Count == 0)
            {
                MessageBox.Show("Kérjük, válasszon ki legalább egy tereptípust a szűréshez!", "Szűrési feltétel hiányzik", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _allProductGraphData.Clear(); // Korábbi adatok törlése
                BuildAndDisplayGraph();       // Üres gráf megjelenítése
                this.Text = "Termék Kapcsolati Gráf (Válasszon szűrőt)";
                return;
            }


            this.Text = "Gráf betöltése...";
            this.Cursor = Cursors.WaitCursor;

            // GViewer gViewer = this.Controls.OfType<GViewer>().FirstOrDefault();
            // if (gViewer != null) gViewer.Graph = null; // Korábbi gráf törlése a BuildAndDisplayGraph-ban lesz

            // 2. SQL lekérdezés dinamikus módosítása
            // Használjunk egy placeholder-t az SQL stringben a dinamikus résznek
            string sqlQueryTemplate = @"
    WITH KorszakCTE AS (
        SELECT ProductBvin, PropertyValue
        FROM [MyDNNDatabase].[dbo].[hcc_ProductPropertyValue]
        WHERE PropertyId = 8 -- Korszak_Tag
          AND PropertyValue IS NOT NULL AND PropertyValue <> '-' AND PropertyValue <> ''
          AND PropertyValue NOT LIKE '%(Civil)%' AND PropertyValue NOT LIKE '%(Fiktív)%'
    ),
    KonfliktusCTE AS (
        SELECT ProductBvin, PropertyValue
        FROM [MyDNNDatabase].[dbo].[hcc_ProductPropertyValue]
        WHERE PropertyId = 9 -- Konfliktus_Tag
          AND PropertyValue IS NOT NULL AND PropertyValue <> '-' AND PropertyValue <> ''
          AND PropertyValue NOT LIKE '%(Fiktív)%' AND PropertyValue NOT LIKE '%(Civil)%'
    ),
    TerepCTE AS (
        SELECT ProductBvin, PropertyValue
        FROM [MyDNNDatabase].[dbo].[hcc_ProductPropertyValue]
        WHERE PropertyId = 11 -- Terep_Tag
          AND PropertyValue IS NOT NULL AND PropertyValue <> '-' AND PropertyValue <> ''
          AND PropertyValue NOT LIKE '%(Fiktív)%' AND PropertyValue NOT LIKE '%(Civil)%'
    )
    SELECT
         TOP 100
        a.bvin,
        a.SKU,
        b.Title AS ProductName,
        k.PropertyValue AS EraTagString,
        f.PropertyValue AS FrontTagString, 
        t.PropertyValue AS TerrainTagString
    FROM
        [MyDNNDatabase].[dbo].[hcc_Product] a
    JOIN
        [MyDNNDatabase].[dbo].[hcc_SearchObjects] b ON a.bvin = b.ObjectId
    LEFT JOIN
        KorszakCTE k ON k.ProductBvin = a.bvin
    LEFT JOIN
        KonfliktusCTE f ON f.ProductBvin = a.bvin
    LEFT JOIN
        TerepCTE t ON t.ProductBvin = a.bvin
    WHERE
        a.Status = 1 
        AND k.PropertyValue IS NOT NULL
        AND f.PropertyValue IS NOT NULL
        AND t.PropertyValue IS NOT NULL
        {TERRAIN_FILTER_PLACEHOLDER} -- Ezt cseréljük le
    ORDER BY
        b.Title;";

            string terrainFilterConditionSql = "";
            if (selectedTerrains.Any())
            {
                // Fontos: Az SQL injection elleni védelem itt minimális (csak aposztróf csere).
                // Éles rendszerben parametrikus lekérdezés vagy alaposabb tisztítás javasolt.
                var terrainLikes = selectedTerrains.Select(terrain => $"t.PropertyValue LIKE '%{terrain.Replace("'", "''")}%'");
                terrainFilterConditionSql = $"AND ({string.Join(" OR ", terrainLikes)})";
            }
            // Ha nincs kiválasztott terep (és a lista nem üres, amit már fentebb kezeltünk),
            // akkor a placeholder üres string-re cserélődik, és nem történik extra szűrés a LIKE alapján,
            // csak az "AND t.PropertyValue IS NOT NULL" marad.

            string sqlQuery = sqlQueryTemplate.Replace("{TERRAIN_FILTER_PLACEHOLDER}", terrainFilterConditionSql);

            _allProductGraphData.Clear();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        // A paraméterek itt feleslegesnek tűnnek, mert az SQL query nem használja őket.
                        // command.Parameters.AddWithValue("@TargetStoreId", 1);
                        // command.Parameters.AddWithValue("@EraTagName", PROP_NAME_ERA);
                        // command.Parameters.AddWithValue("@FrontTagName", PROP_NAME_FRONT);
                        // command.Parameters.AddWithValue("@TerrainTagName", PROP_NAME_TERRAIN);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var pgd = new ProductGraphData
                                {
                                    Bvin = reader["bvin"] as Guid? ?? Guid.Empty,
                                    Sku = reader["Sku"]?.ToString(),
                                    ProductName = reader["ProductName"]?.ToString(),
                                };
                                pgd.EraTags = ProductGraphData.ParseTags(reader["EraTagString"]?.ToString());
                                pgd.FrontTags = ProductGraphData.ParseTags(reader["FrontTagString"]?.ToString());
                                pgd.TerrainTags = ProductGraphData.ParseTags(reader["TerrainTagString"]?.ToString());
                                _allProductGraphData.Add(pgd);
                            }
                        }
                    }
                }

                BuildAndDisplayGraph();
                this.Text = $"Termék Kapcsolati Gráf ({_allProductGraphData.Count} termék)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a gráf adatainak betöltése közben: {ex.Message}\n{ex.StackTrace}", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Text = "Hiba a gráf betöltésekor";
                _allProductGraphData.Clear(); // Hiba esetén is ürítjük
                BuildAndDisplayGraph();     // És üres gráfot jelenítünk meg
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void BuildAndDisplayGraph()
        {
            GViewer gViewer = this.Controls.OfType<GViewer>().FirstOrDefault();
            if (gViewer == null)
            {
                // Ha a Designer-ben adtad hozzá a GViewer-t, akkor a neve pl. gViewer1 lehet.
                // Akkor itt this.gViewer1.Graph = null; -t használj, ha van ilyen tagváltozó.
                // A jelenlegi kód feltételezi, hogy dinamikusan keresed.
                // Ha biztosan van egy gViewer nevű komponens a formon, akkor pl. 'this.gViewerControlName.Graph = null;'
                // A biztonság kedvéért, ha nem találja, ne crasheljen.
                Control foundCtrl = this.Controls.Find("gViewer", true).FirstOrDefault(); // Keressük a gViewer nevű vezérlőt
                if (foundCtrl is GViewer)
                {
                    gViewer = (GViewer)foundCtrl;
                }
                else
                {
                    // Ha továbbra sincs, és a Controls.OfType sem találta, akkor lehet, hogy tényleg nincs.
                    // De ha te hoztad létre a designerben, akkor annak a nevét kell itt használni.
                    // Pl. ha a neve 'graphDisplayControl' a designerben:
                    // if (this.graphDisplayControl != null) this.graphDisplayControl.Graph = null; else return;
                    // Mivel a kódodban is this.Controls.OfType<GViewer>().First() volt, ezt használjuk.
                    if (this.Controls.OfType<GViewer>().FirstOrDefault() == null)
                    {
                        MessageBox.Show("A GViewer vezérlő nem található a Form2-n. Kérlek, add hozzá a Designerben.", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    gViewer = this.Controls.OfType<GViewer>().First();
                }
            }

            gViewer.Graph = null; // Korábbi gráf törlése

            Graph graph = new Graph("termekek_graf");

            graph.LayoutAlgorithmSettings.NodeSeparation = 70;  // Növelt érték a csomópontok közötti vízszintes térközhöz
            

            foreach (var productData in _allProductGraphData)
            {
                if (productData.Bvin == Guid.Empty) continue;
                var node = graph.AddNode(productData.Bvin.ToString());
                node.LabelText = productData.ProductName ?? "Ismeretlen Termék";
                node.UserData = productData;
            }

            for (int i = 0; i < _allProductGraphData.Count; i++)
            {
                if (_allProductGraphData[i].Bvin == Guid.Empty) continue;
                for (int j = i + 1; j < _allProductGraphData.Count; j++)
                {
                    if (_allProductGraphData[j].Bvin == Guid.Empty) continue;

                    ProductGraphData productA = _allProductGraphData[i];
                    ProductGraphData productB = _allProductGraphData[j];

                    bool eraMatch = productA.EraTags.Overlaps(productB.EraTags);
                    bool frontMatch = productA.FrontTags.Overlaps(productB.FrontTags);
                    bool terrainMatch = productA.TerrainTags.Overlaps(productB.TerrainTags);

                    if (eraMatch && frontMatch && terrainMatch)
                    {
                        graph.AddEdge(productA.Bvin.ToString(), productB.Bvin.ToString());
                    }
                }
            }

            gViewer.Graph = graph;

            gViewer.ObjectUnderMouseCursorChanged -= GViewer_ObjectUnderMouseCursorChanged;
            gViewer.ObjectUnderMouseCursorChanged += GViewer_ObjectUnderMouseCursorChanged;
        }

        private void GViewer_ObjectUnderMouseCursorChanged(object sender, ObjectUnderMouseCursorChangedEventArgs e)
        {
            GViewer gViewer = sender as GViewer;
            if (gViewer == null) return;

            if (e.OldObject != null && e.OldObject.DrawingObject is Node oldNode)
            {
                oldNode.Label.FontColor = Microsoft.Msagl.Drawing.Color.Black;
            }
            if (e.NewObject != null && e.NewObject.DrawingObject is Node newNode)
            {
                newNode.Label.FontColor = Microsoft.Msagl.Drawing.Color.Blue;
                if (newNode.UserData is ProductGraphData pd)
                {
                    gViewer.SetToolTip(new ToolTip(), $"Termék: {pd.ProductName}\nSKU: {pd.Sku}\nKorszak: {string.Join(", ", pd.EraTags)}\nFront: {string.Join(", ", pd.FrontTags)}\nTerep: {string.Join(", ", pd.TerrainTags)}");
                }
            }
            else
            {
                gViewer.SetToolTip(new ToolTip(), "");
            }
            gViewer.Invalidate();
        }

        // Ez az eseménykezelő üres volt, ha nincs rá szükség, törölhető.
        // private void Form2_Load_1(object sender, EventArgs e)
        // {
        // }

        private void btnApplyTerrainFilter_Click(object sender, EventArgs e)
        {
            // Ellenőrizzük, hogy a chkLstTerrainFilter létezik-e, mielőtt adatokat próbálnánk betölteni.
            if (this.chkLstTerrainFilter == null)
            {
                MessageBox.Show("A terep szűrő (chkLstTerrainFilter) nem található. A szűrés nem alkalmazható.", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LoadAndDisplayGraphData(); // Gráf betöltése és megjelenítése a szűrők alapján
        }
    }

    public class ProductGraphData
    {
        public Guid Bvin { get; set; }
        public string Sku { get; set; }
        public string ProductName { get; set; }
        public HashSet<string> EraTags { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> FrontTags { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> TerrainTags { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public static HashSet<string> ParseTags(string pipeSeparatedTags)
        {
            if (string.IsNullOrWhiteSpace(pipeSeparatedTags))
            {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
            return new HashSet<string>(pipeSeparatedTags.Split('|').Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag)), StringComparer.OrdinalIgnoreCase);
        }
    }
}