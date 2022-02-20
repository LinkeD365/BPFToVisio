
namespace LinkeD365.BPFToVisio
{
    partial class BPFToVisioCtl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCreateVisio = new System.Windows.Forms.ToolStripButton();
            this.gvBPFs = new System.Windows.Forms.DataGridView();
            this.toolStripMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvBPFs)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.tssSeparator1,
            this.btnCreateVisio});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(559, 31);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(86, 28);
            this.tsbClose.Text = "Close this tool";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // tssSeparator1
            // 
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // btnCreateVisio
            // 
            this.btnCreateVisio.Image = global::LinkeD365.BPFToVisio.Properties.Resources.visio_icon;
            this.btnCreateVisio.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCreateVisio.Name = "btnCreateVisio";
            this.btnCreateVisio.Size = new System.Drawing.Size(97, 28);
            this.btnCreateVisio.Text = "Create Visio";
            this.btnCreateVisio.Click += new System.EventHandler(this.btnCreateVisio_Click);
            // 
            // gvBPFs
            // 
            this.gvBPFs.AllowUserToAddRows = false;
            this.gvBPFs.AllowUserToDeleteRows = false;
            this.gvBPFs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvBPFs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvBPFs.Location = new System.Drawing.Point(0, 31);
            this.gvBPFs.Name = "gvBPFs";
            this.gvBPFs.ReadOnly = true;
            this.gvBPFs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvBPFs.Size = new System.Drawing.Size(559, 269);
            this.gvBPFs.TabIndex = 5;
            // 
            // BPFToVisioCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gvBPFs);
            this.Controls.Add(this.toolStripMenu);
            this.Name = "BPFToVisioCtl";
            this.Size = new System.Drawing.Size(559, 300);
            this.ConnectionUpdated += new XrmToolBox.Extensibility.PluginControlBase.ConnectionUpdatedHandler(this.BPFToVisioCtl_ConnectionUpdated);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvBPFs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripSeparator tssSeparator1;
        private System.Windows.Forms.DataGridView gvBPFs;
        private System.Windows.Forms.ToolStripButton btnCreateVisio;
    }
}
