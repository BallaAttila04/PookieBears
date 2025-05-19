namespace HotcakesProductViewer
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.gViewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.chkLstTerrainFilter = new System.Windows.Forms.CheckedListBox();
            this.btnApplyTerrainFilter = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // gViewer
            // 
            this.gViewer.ArrowheadLength = 10D;
            this.gViewer.AsyncLayout = false;
            this.gViewer.AutoScroll = true;
            this.gViewer.BackwardEnabled = false;
            this.gViewer.BuildHitTree = true;
            this.gViewer.CurrentLayoutMethod = Microsoft.Msagl.GraphViewerGdi.LayoutMethod.UseSettingsOfTheGraph;
            this.gViewer.EdgeInsertButtonVisible = true;
            this.gViewer.FileName = "";
            this.gViewer.ForwardEnabled = false;
            this.gViewer.Graph = null;
            this.gViewer.InsertingEdge = false;
            this.gViewer.LayoutAlgorithmSettingsButtonVisible = true;
            this.gViewer.LayoutEditingEnabled = true;
            this.gViewer.Location = new System.Drawing.Point(217, 12);
            this.gViewer.LooseOffsetForRouting = 0.25D;
            this.gViewer.MouseHitDistance = 0.05D;
            this.gViewer.Name = "gViewer";
            this.gViewer.NavigationVisible = true;
            this.gViewer.NeedToCalculateLayout = true;
            this.gViewer.OffsetForRelaxingInRouting = 0.6D;
            this.gViewer.PaddingForEdgeRouting = 8D;
            this.gViewer.PanButtonPressed = false;
            this.gViewer.SaveAsImageEnabled = true;
            this.gViewer.SaveAsMsaglEnabled = true;
            this.gViewer.SaveButtonVisible = true;
            this.gViewer.SaveGraphButtonVisible = true;
            this.gViewer.SaveInVectorFormatEnabled = true;
            this.gViewer.Size = new System.Drawing.Size(1429, 634);
            this.gViewer.TabIndex = 0;
            this.gViewer.TightOffsetForRouting = 0.125D;
            this.gViewer.ToolBarIsVisible = true;
            this.gViewer.Transform = ((Microsoft.Msagl.Core.Geometry.Curves.PlaneTransformation)(resources.GetObject("gViewer.Transform")));
            this.gViewer.UndoRedoButtonsVisible = true;
            this.gViewer.WindowZoomButtonPressed = false;
            this.gViewer.ZoomF = 1D;
            this.gViewer.ZoomWindowThreshold = 0.05D;
            // 
            // chkLstTerrainFilter
            // 
            this.chkLstTerrainFilter.FormattingEnabled = true;
            this.chkLstTerrainFilter.Location = new System.Drawing.Point(12, 12);
            this.chkLstTerrainFilter.Name = "chkLstTerrainFilter";
            this.chkLstTerrainFilter.Size = new System.Drawing.Size(199, 319);
            this.chkLstTerrainFilter.TabIndex = 1;
            // 
            // btnApplyTerrainFilter
            // 
            this.btnApplyTerrainFilter.Location = new System.Drawing.Point(12, 337);
            this.btnApplyTerrainFilter.Name = "btnApplyTerrainFilter";
            this.btnApplyTerrainFilter.Size = new System.Drawing.Size(75, 23);
            this.btnApplyTerrainFilter.TabIndex = 2;
            this.btnApplyTerrainFilter.Text = "Gráf betöltése";
            this.btnApplyTerrainFilter.UseVisualStyleBackColor = true;
            this.btnApplyTerrainFilter.Click += new System.EventHandler(this.btnApplyTerrainFilter_Click);
            // 
            // Form2
            // 
            this.ClientSize = new System.Drawing.Size(1658, 667);
            this.Controls.Add(this.btnApplyTerrainFilter);
            this.Controls.Add(this.chkLstTerrainFilter);
            this.Controls.Add(this.gViewer);
            this.Name = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);

        }


        #endregion

        private Microsoft.Msagl.GraphViewerGdi.GViewer gViewer;
        private System.Windows.Forms.ToolTip ToolTip;
        private System.Windows.Forms.CheckedListBox chkLstTerrainFilter;
        private System.Windows.Forms.Button btnApplyTerrainFilter;
    }
}