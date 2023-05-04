namespace RE4_PS2_TPL_Manager
{
    partial class FrmMain
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createEmptyTPLFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTPLFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.removeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllMipmapsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exportAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.convertAllToPNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertAllToBMPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertAllToTGAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.increaseAllTo256ColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decreaseAllTo16ColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compileFromFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.convertAndImportBMPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.fixBrokenTPLexperimentalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.includeMipmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.showHiddenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.disableDuplicatePromptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usabilityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.problemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.creditsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.btnCreateNewFile = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.texturePreview = new System.Windows.Forms.PictureBox();
            this.table = new System.Windows.Forms.DataGridView();
            this.ctxMenuTable = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.testeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bMPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tGAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeMipmapsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.increaseColorDepthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decreaseColorDepthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.lblStatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.spinHue = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.spinPixelate = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.spinBrightness = new System.Windows.Forms.NumericUpDown();
            this.spinSharpen = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.spinSaturation = new System.Windows.Forms.NumericUpDown();
            this.spinContrast = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnLayerOverlay = new System.Windows.Forms.Button();
            this.btnLayerAddMask = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.btnFilterGray = new System.Windows.Forms.Button();
            this.btnFilterInvert = new System.Windows.Forms.Button();
            this.btnFilterColorful = new System.Windows.Forms.Button();
            this.btnFilterComic = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnUpscale = new System.Windows.Forms.Button();
            this.btnDownscale = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnFlipY = new System.Windows.Forms.Button();
            this.btnFlipX = new System.Windows.Forms.Button();
            this.btnRotate = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.ctxPreviewImage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.swapTextureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pNGToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.bMPToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tGAToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.extendViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.animatedViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.texturePreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.table)).BeginInit();
            this.ctxMenuTable.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinHue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinPixelate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinSharpen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinSaturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinContrast)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.ctxPreviewImage.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.textureToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.creditsToolStripMenuItem});
            this.menuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(951, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createEmptyTPLFileToolStripMenuItem,
            this.openTPLFileToolStripMenuItem,
            this.refreshTableToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveFileToolStripMenuItem,
            this.toolStripSeparator4,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // createEmptyTPLFileToolStripMenuItem
            // 
            this.createEmptyTPLFileToolStripMenuItem.Name = "createEmptyTPLFileToolStripMenuItem";
            this.createEmptyTPLFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.createEmptyTPLFileToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.createEmptyTPLFileToolStripMenuItem.Text = "Create empty .TPL file";
            this.createEmptyTPLFileToolStripMenuItem.Click += new System.EventHandler(this.createEmptyTPLFileToolStripMenuItem_Click);
            // 
            // openTPLFileToolStripMenuItem
            // 
            this.openTPLFileToolStripMenuItem.Name = "openTPLFileToolStripMenuItem";
            this.openTPLFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openTPLFileToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.openTPLFileToolStripMenuItem.Text = "Open .TPL file";
            this.openTPLFileToolStripMenuItem.Click += new System.EventHandler(this.openTPLFileToolStripMenuItem_Click);
            // 
            // refreshTableToolStripMenuItem
            // 
            this.refreshTableToolStripMenuItem.Name = "refreshTableToolStripMenuItem";
            this.refreshTableToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.refreshTableToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.refreshTableToolStripMenuItem.Text = "Refresh table";
            this.refreshTableToolStripMenuItem.Click += new System.EventHandler(this.refreshTableToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(229, 6);
            // 
            // saveFileToolStripMenuItem
            // 
            this.saveFileToolStripMenuItem.Name = "saveFileToolStripMenuItem";
            this.saveFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveFileToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.saveFileToolStripMenuItem.Text = "Save file";
            this.saveFileToolStripMenuItem.Click += new System.EventHandler(this.saveFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(229, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // textureToolStripMenuItem
            // 
            this.textureToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewTexturesToolStripMenuItem,
            this.toolStripSeparator5,
            this.removeAllToolStripMenuItem,
            this.removeAllMipmapsToolStripMenuItem,
            this.toolStripSeparator2,
            this.exportAllToolStripMenuItem,
            this.exportSelectedToolStripMenuItem,
            this.toolStripSeparator6,
            this.convertAllToPNGToolStripMenuItem,
            this.convertAllToBMPToolStripMenuItem,
            this.convertAllToTGAToolStripMenuItem,
            this.toolStripSeparator12,
            this.increaseAllTo256ColorsToolStripMenuItem,
            this.decreaseAllTo16ColorsToolStripMenuItem});
            this.textureToolStripMenuItem.Name = "textureToolStripMenuItem";
            this.textureToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.textureToolStripMenuItem.Text = "Texture";
            // 
            // addNewTexturesToolStripMenuItem
            // 
            this.addNewTexturesToolStripMenuItem.Name = "addNewTexturesToolStripMenuItem";
            this.addNewTexturesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this.addNewTexturesToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.addNewTexturesToolStripMenuItem.Text = "Add new textures";
            this.addNewTexturesToolStripMenuItem.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(271, 6);
            // 
            // removeAllToolStripMenuItem
            // 
            this.removeAllToolStripMenuItem.Name = "removeAllToolStripMenuItem";
            this.removeAllToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.removeAllToolStripMenuItem.Text = "Remove all";
            this.removeAllToolStripMenuItem.Click += new System.EventHandler(this.removeAllToolStripMenuItem_Click);
            // 
            // removeAllMipmapsToolStripMenuItem
            // 
            this.removeAllMipmapsToolStripMenuItem.Name = "removeAllMipmapsToolStripMenuItem";
            this.removeAllMipmapsToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.removeAllMipmapsToolStripMenuItem.Text = "Remove all mipmaps";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(271, 6);
            // 
            // exportAllToolStripMenuItem
            // 
            this.exportAllToolStripMenuItem.Name = "exportAllToolStripMenuItem";
            this.exportAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.E)));
            this.exportAllToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.exportAllToolStripMenuItem.Text = "Extract all";
            this.exportAllToolStripMenuItem.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // exportSelectedToolStripMenuItem
            // 
            this.exportSelectedToolStripMenuItem.Name = "exportSelectedToolStripMenuItem";
            this.exportSelectedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.exportSelectedToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.exportSelectedToolStripMenuItem.Text = "Extract selected";
            this.exportSelectedToolStripMenuItem.Click += new System.EventHandler(this.extractSelectedToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(271, 6);
            // 
            // convertAllToPNGToolStripMenuItem
            // 
            this.convertAllToPNGToolStripMenuItem.Name = "convertAllToPNGToolStripMenuItem";
            this.convertAllToPNGToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.convertAllToPNGToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.convertAllToPNGToolStripMenuItem.Text = "Convert all to PNG";
            this.convertAllToPNGToolStripMenuItem.Click += new System.EventHandler(this.convertAllToPNGToolStripMenuItem_Click);
            // 
            // convertAllToBMPToolStripMenuItem
            // 
            this.convertAllToBMPToolStripMenuItem.Name = "convertAllToBMPToolStripMenuItem";
            this.convertAllToBMPToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.convertAllToBMPToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.convertAllToBMPToolStripMenuItem.Text = "Convert all to BMP";
            this.convertAllToBMPToolStripMenuItem.Click += new System.EventHandler(this.convertAllToBMPToolStripMenuItem_Click);
            // 
            // convertAllToTGAToolStripMenuItem
            // 
            this.convertAllToTGAToolStripMenuItem.Name = "convertAllToTGAToolStripMenuItem";
            this.convertAllToTGAToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.convertAllToTGAToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.convertAllToTGAToolStripMenuItem.Text = "Convert all to TGA";
            this.convertAllToTGAToolStripMenuItem.Click += new System.EventHandler(this.convertAllToTGAToolStripMenuItem_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(271, 6);
            // 
            // increaseAllTo256ColorsToolStripMenuItem
            // 
            this.increaseAllTo256ColorsToolStripMenuItem.Name = "increaseAllTo256ColorsToolStripMenuItem";
            this.increaseAllTo256ColorsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.I)));
            this.increaseAllTo256ColorsToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.increaseAllTo256ColorsToolStripMenuItem.Text = "Increase all to 256 colors";
            this.increaseAllTo256ColorsToolStripMenuItem.Click += new System.EventHandler(this.increaseAllTo256ColorsToolStripMenuItem_Click);
            // 
            // decreaseAllTo16ColorsToolStripMenuItem
            // 
            this.decreaseAllTo16ColorsToolStripMenuItem.Name = "decreaseAllTo16ColorsToolStripMenuItem";
            this.decreaseAllTo16ColorsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D)));
            this.decreaseAllTo16ColorsToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.decreaseAllTo16ColorsToolStripMenuItem.Text = "Decrease all to 16 colors";
            this.decreaseAllTo16ColorsToolStripMenuItem.Click += new System.EventHandler(this.decreaseAllTo16ColorsToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.compileFromFolderToolStripMenuItem,
            this.toolStripSeparator8,
            this.convertAndImportBMPToolStripMenuItem,
            this.toolStripSeparator11,
            this.fixBrokenTPLexperimentalToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // compileFromFolderToolStripMenuItem
            // 
            this.compileFromFolderToolStripMenuItem.Name = "compileFromFolderToolStripMenuItem";
            this.compileFromFolderToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.compileFromFolderToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.compileFromFolderToolStripMenuItem.Text = "Compile from folder";
            this.compileFromFolderToolStripMenuItem.Click += new System.EventHandler(this.btnCompileFromFolder_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(228, 6);
            // 
            // convertAndImportBMPToolStripMenuItem
            // 
            this.convertAndImportBMPToolStripMenuItem.Name = "convertAndImportBMPToolStripMenuItem";
            this.convertAndImportBMPToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.convertAndImportBMPToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.convertAndImportBMPToolStripMenuItem.Text = "Convert BMP to TPL";
            this.convertAndImportBMPToolStripMenuItem.Click += new System.EventHandler(this.convertAndImportBMPToolStripMenuItem_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(228, 6);
            // 
            // fixBrokenTPLexperimentalToolStripMenuItem
            // 
            this.fixBrokenTPLexperimentalToolStripMenuItem.Name = "fixBrokenTPLexperimentalToolStripMenuItem";
            this.fixBrokenTPLexperimentalToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.fixBrokenTPLexperimentalToolStripMenuItem.Text = "Fix broken TPL (experimental)";
            this.fixBrokenTPLexperimentalToolStripMenuItem.Click += new System.EventHandler(this.fixBrokenTPLexperimentalToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.includeMipmapToolStripMenuItem,
            this.toolStripSeparator3,
            this.showHiddenToolStripMenuItem,
            this.toolStripSeparator7,
            this.disableDuplicatePromptToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // includeMipmapToolStripMenuItem
            // 
            this.includeMipmapToolStripMenuItem.Checked = true;
            this.includeMipmapToolStripMenuItem.CheckOnClick = true;
            this.includeMipmapToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.includeMipmapToolStripMenuItem.Name = "includeMipmapToolStripMenuItem";
            this.includeMipmapToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.includeMipmapToolStripMenuItem.Text = "Include Mipmap (experimental)";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(238, 6);
            // 
            // showHiddenToolStripMenuItem
            // 
            this.showHiddenToolStripMenuItem.CheckOnClick = true;
            this.showHiddenToolStripMenuItem.Name = "showHiddenToolStripMenuItem";
            this.showHiddenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.H)));
            this.showHiddenToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.showHiddenToolStripMenuItem.Text = "Show Hidden";
            this.showHiddenToolStripMenuItem.Click += new System.EventHandler(this.showHiddenToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(238, 6);
            // 
            // disableDuplicatePromptToolStripMenuItem
            // 
            this.disableDuplicatePromptToolStripMenuItem.CheckOnClick = true;
            this.disableDuplicatePromptToolStripMenuItem.Name = "disableDuplicatePromptToolStripMenuItem";
            this.disableDuplicatePromptToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.disableDuplicatePromptToolStripMenuItem.Text = "Disable duplicate prompt";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.usabilityToolStripMenuItem,
            this.problemsToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // usabilityToolStripMenuItem
            // 
            this.usabilityToolStripMenuItem.Name = "usabilityToolStripMenuItem";
            this.usabilityToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.usabilityToolStripMenuItem.Text = "Usability";
            this.usabilityToolStripMenuItem.Click += new System.EventHandler(this.usabilityToolStripMenuItem_Click);
            // 
            // problemsToolStripMenuItem
            // 
            this.problemsToolStripMenuItem.Name = "problemsToolStripMenuItem";
            this.problemsToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.problemsToolStripMenuItem.Text = "Problems?";
            this.problemsToolStripMenuItem.Click += new System.EventHandler(this.problemsToolStripMenuItem_Click);
            // 
            // creditsToolStripMenuItem
            // 
            this.creditsToolStripMenuItem.Name = "creditsToolStripMenuItem";
            this.creditsToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.creditsToolStripMenuItem.Text = "Credits";
            this.creditsToolStripMenuItem.Click += new System.EventHandler(this.creditsToolStripMenuItem_Click);
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(203, 310);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(135, 29);
            this.btnOpenFile.TabIndex = 10;
            this.btnOpenFile.Text = "Open .TPL file";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // btnCreateNewFile
            // 
            this.btnCreateNewFile.Location = new System.Drawing.Point(203, 272);
            this.btnCreateNewFile.Name = "btnCreateNewFile";
            this.btnCreateNewFile.Size = new System.Drawing.Size(135, 29);
            this.btnCreateNewFile.TabIndex = 9;
            this.btnCreateNewFile.Text = "Create empty .TPL file";
            this.btnCreateNewFile.UseVisualStyleBackColor = true;
            this.btnCreateNewFile.Click += new System.EventHandler(this.btnCreateNewFile_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(572, 27);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(374, 21);
            this.panel2.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(372, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Texture Preview";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // texturePreview
            // 
            this.texturePreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.texturePreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.texturePreview.Location = new System.Drawing.Point(572, 54);
            this.texturePreview.Name = "texturePreview";
            this.texturePreview.Size = new System.Drawing.Size(374, 247);
            this.texturePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.texturePreview.TabIndex = 5;
            this.texturePreview.TabStop = false;
            this.texturePreview.MouseDown += new System.Windows.Forms.MouseEventHandler(this.texturePreview_MouseDown);
            // 
            // table
            // 
            this.table.AllowDrop = true;
            this.table.AllowUserToAddRows = false;
            this.table.AllowUserToDeleteRows = false;
            this.table.AllowUserToResizeRows = false;
            this.table.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.table.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.table.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.table.Location = new System.Drawing.Point(0, 27);
            this.table.Name = "table";
            this.table.Size = new System.Drawing.Size(566, 501);
            this.table.TabIndex = 4;
            this.table.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.table_CellClick);
            this.table.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.table_CellEnter);
            this.table.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.table_CellMouseDown);
            this.table.DragDrop += new System.Windows.Forms.DragEventHandler(this.table_DragDrop);
            this.table.DragEnter += new System.Windows.Forms.DragEventHandler(this.table_DragEnter);
            // 
            // ctxMenuTable
            // 
            this.ctxMenuTable.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testeToolStripMenuItem,
            this.replaceToolStripMenuItem,
            this.duplicateToolStripMenuItem,
            this.convertToolStripMenuItem,
            this.toolStripSeparator9,
            this.removeToolStripMenuItem,
            this.removeMipmapsToolStripMenuItem,
            this.toolStripSeparator10,
            this.increaseColorDepthToolStripMenuItem,
            this.decreaseColorDepthToolStripMenuItem});
            this.ctxMenuTable.Name = "contextMenuStrip1";
            this.ctxMenuTable.Size = new System.Drawing.Size(186, 192);
            // 
            // testeToolStripMenuItem
            // 
            this.testeToolStripMenuItem.Name = "testeToolStripMenuItem";
            this.testeToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.testeToolStripMenuItem.Text = "Extract";
            this.testeToolStripMenuItem.Click += new System.EventHandler(this.extractToolStripMenuItem_Click);
            // 
            // replaceToolStripMenuItem
            // 
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.replaceToolStripMenuItem.Text = "Replace";
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
            // 
            // duplicateToolStripMenuItem
            // 
            this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
            this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.duplicateToolStripMenuItem.Text = "Duplicate";
            this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
            // 
            // convertToolStripMenuItem
            // 
            this.convertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pNGToolStripMenuItem,
            this.bMPToolStripMenuItem,
            this.tGAToolStripMenuItem});
            this.convertToolStripMenuItem.Name = "convertToolStripMenuItem";
            this.convertToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.convertToolStripMenuItem.Text = "Convert";
            // 
            // pNGToolStripMenuItem
            // 
            this.pNGToolStripMenuItem.Name = "pNGToolStripMenuItem";
            this.pNGToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.pNGToolStripMenuItem.Text = "PNG";
            this.pNGToolStripMenuItem.Click += new System.EventHandler(this.pNGToolStripMenuItem_Click);
            // 
            // bMPToolStripMenuItem
            // 
            this.bMPToolStripMenuItem.Name = "bMPToolStripMenuItem";
            this.bMPToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.bMPToolStripMenuItem.Text = "BMP";
            this.bMPToolStripMenuItem.Click += new System.EventHandler(this.bMPToolStripMenuItem_Click);
            // 
            // tGAToolStripMenuItem
            // 
            this.tGAToolStripMenuItem.Name = "tGAToolStripMenuItem";
            this.tGAToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.tGAToolStripMenuItem.Text = "TGA";
            this.tGAToolStripMenuItem.Click += new System.EventHandler(this.tGAToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(182, 6);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // removeMipmapsToolStripMenuItem
            // 
            this.removeMipmapsToolStripMenuItem.Name = "removeMipmapsToolStripMenuItem";
            this.removeMipmapsToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.removeMipmapsToolStripMenuItem.Text = "Remove mipmaps";
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(182, 6);
            // 
            // increaseColorDepthToolStripMenuItem
            // 
            this.increaseColorDepthToolStripMenuItem.Name = "increaseColorDepthToolStripMenuItem";
            this.increaseColorDepthToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.increaseColorDepthToolStripMenuItem.Text = "Increase color depth";
            this.increaseColorDepthToolStripMenuItem.Click += new System.EventHandler(this.increaseColorDepthToolStripMenuItem_Click);
            // 
            // decreaseColorDepthToolStripMenuItem
            // 
            this.decreaseColorDepthToolStripMenuItem.Name = "decreaseColorDepthToolStripMenuItem";
            this.decreaseColorDepthToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.decreaseColorDepthToolStripMenuItem.Text = "Decrease color depth";
            this.decreaseColorDepthToolStripMenuItem.Click += new System.EventHandler(this.decreaseColorDepthToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.lblStatusText});
            this.statusStrip1.Location = new System.Drawing.Point(0, 531);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(951, 22);
            this.statusStrip1.TabIndex = 12;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // lblStatusText
            // 
            this.lblStatusText.BackColor = System.Drawing.Color.Transparent;
            this.lblStatusText.Name = "lblStatusText";
            this.lblStatusText.Size = new System.Drawing.Size(0, 17);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(572, 307);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(374, 21);
            this.panel1.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(372, 19);
            this.label2.TabIndex = 0;
            this.label2.Text = "Editor";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(50)))));
            this.groupBox1.Controls.Add(this.panel3);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(176, 188);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Adjustments";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel3.AutoScroll = true;
            this.panel3.Controls.Add(this.spinHue);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.spinPixelate);
            this.panel3.Controls.Add(this.label11);
            this.panel3.Controls.Add(this.spinBrightness);
            this.panel3.Controls.Add(this.spinSharpen);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.spinSaturation);
            this.panel3.Controls.Add(this.spinContrast);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Location = new System.Drawing.Point(4, 19);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(166, 163);
            this.panel3.TabIndex = 8;
            // 
            // spinHue
            // 
            this.spinHue.Location = new System.Drawing.Point(88, 161);
            this.spinHue.Maximum = new decimal(new int[] {
            359,
            0,
            0,
            0});
            this.spinHue.Name = "spinHue";
            this.spinHue.Size = new System.Drawing.Size(57, 20);
            this.spinHue.TabIndex = 11;
            this.spinHue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.spinHue.ValueChanged += new System.EventHandler(this.spinHue_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 163);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Hue";
            // 
            // spinPixelate
            // 
            this.spinPixelate.Location = new System.Drawing.Point(88, 131);
            this.spinPixelate.Name = "spinPixelate";
            this.spinPixelate.Size = new System.Drawing.Size(57, 20);
            this.spinPixelate.TabIndex = 9;
            this.spinPixelate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.spinPixelate.ValueChanged += new System.EventHandler(this.spinPixelate_ValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(16, 133);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(44, 13);
            this.label11.TabIndex = 8;
            this.label11.Text = "Pixelate";
            // 
            // spinBrightness
            // 
            this.spinBrightness.Location = new System.Drawing.Point(88, 9);
            this.spinBrightness.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.spinBrightness.Name = "spinBrightness";
            this.spinBrightness.Size = new System.Drawing.Size(57, 20);
            this.spinBrightness.TabIndex = 1;
            this.spinBrightness.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.spinBrightness.ValueChanged += new System.EventHandler(this.spinBrightness_ValueChanged);
            // 
            // spinSharpen
            // 
            this.spinSharpen.Location = new System.Drawing.Point(88, 100);
            this.spinSharpen.Name = "spinSharpen";
            this.spinSharpen.Size = new System.Drawing.Size(57, 20);
            this.spinSharpen.TabIndex = 7;
            this.spinSharpen.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.spinSharpen.ValueChanged += new System.EventHandler(this.spinSharpen_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Brightness";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 102);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Sharpen";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Contrast";
            // 
            // spinSaturation
            // 
            this.spinSaturation.Location = new System.Drawing.Point(88, 70);
            this.spinSaturation.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.spinSaturation.Name = "spinSaturation";
            this.spinSaturation.Size = new System.Drawing.Size(57, 20);
            this.spinSaturation.TabIndex = 5;
            this.spinSaturation.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.spinSaturation.ValueChanged += new System.EventHandler(this.spinSaturation_ValueChanged);
            // 
            // spinContrast
            // 
            this.spinContrast.Location = new System.Drawing.Point(88, 40);
            this.spinContrast.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.spinContrast.Name = "spinContrast";
            this.spinContrast.Size = new System.Drawing.Size(57, 20);
            this.spinContrast.TabIndex = 3;
            this.spinContrast.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.spinContrast.ValueChanged += new System.EventHandler(this.spinContrast_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 72);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Saturation";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.groupBox2.Controls.Add(this.panel5);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(186, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(181, 188);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Properties";
            // 
            // panel5
            // 
            this.panel5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel5.AutoScroll = true;
            this.panel5.Controls.Add(this.btnLayerOverlay);
            this.panel5.Controls.Add(this.btnLayerAddMask);
            this.panel5.Controls.Add(this.label9);
            this.panel5.Controls.Add(this.btnFilterGray);
            this.panel5.Controls.Add(this.btnFilterInvert);
            this.panel5.Controls.Add(this.btnFilterColorful);
            this.panel5.Controls.Add(this.btnFilterComic);
            this.panel5.Controls.Add(this.label8);
            this.panel5.Controls.Add(this.btnReset);
            this.panel5.Controls.Add(this.btnUpscale);
            this.panel5.Controls.Add(this.btnDownscale);
            this.panel5.Controls.Add(this.button1);
            this.panel5.Controls.Add(this.btnFlipY);
            this.panel5.Controls.Add(this.btnFlipX);
            this.panel5.Controls.Add(this.btnRotate);
            this.panel5.Location = new System.Drawing.Point(7, 19);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(168, 163);
            this.panel5.TabIndex = 9;
            // 
            // btnLayerOverlay
            // 
            this.btnLayerOverlay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLayerOverlay.ForeColor = System.Drawing.Color.Black;
            this.btnLayerOverlay.Location = new System.Drawing.Point(244, 123);
            this.btnLayerOverlay.Name = "btnLayerOverlay";
            this.btnLayerOverlay.Size = new System.Drawing.Size(75, 23);
            this.btnLayerOverlay.TabIndex = 22;
            this.btnLayerOverlay.Text = "Overlay";
            this.btnLayerOverlay.UseVisualStyleBackColor = true;
            this.btnLayerOverlay.Click += new System.EventHandler(this.btnLayerOverlay_Click);
            // 
            // btnLayerAddMask
            // 
            this.btnLayerAddMask.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLayerAddMask.ForeColor = System.Drawing.Color.Black;
            this.btnLayerAddMask.Location = new System.Drawing.Point(171, 123);
            this.btnLayerAddMask.Name = "btnLayerAddMask";
            this.btnLayerAddMask.Size = new System.Drawing.Size(67, 23);
            this.btnLayerAddMask.TabIndex = 21;
            this.btnLayerAddMask.Text = "Add mask";
            this.btnLayerAddMask.UseVisualStyleBackColor = true;
            this.btnLayerAddMask.Click += new System.EventHandler(this.btnLayerAddMask_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(220, 102);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(33, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "Layer";
            // 
            // btnFilterGray
            // 
            this.btnFilterGray.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFilterGray.ForeColor = System.Drawing.Color.Black;
            this.btnFilterGray.Location = new System.Drawing.Point(244, 32);
            this.btnFilterGray.Name = "btnFilterGray";
            this.btnFilterGray.Size = new System.Drawing.Size(75, 23);
            this.btnFilterGray.TabIndex = 19;
            this.btnFilterGray.Text = "Gray";
            this.btnFilterGray.UseVisualStyleBackColor = true;
            this.btnFilterGray.Click += new System.EventHandler(this.btnFilterGray_Click);
            // 
            // btnFilterInvert
            // 
            this.btnFilterInvert.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFilterInvert.ForeColor = System.Drawing.Color.Black;
            this.btnFilterInvert.Location = new System.Drawing.Point(244, 66);
            this.btnFilterInvert.Name = "btnFilterInvert";
            this.btnFilterInvert.Size = new System.Drawing.Size(75, 23);
            this.btnFilterInvert.TabIndex = 18;
            this.btnFilterInvert.Text = "Invert";
            this.btnFilterInvert.UseVisualStyleBackColor = true;
            this.btnFilterInvert.Click += new System.EventHandler(this.btnFilterInvert_Click);
            // 
            // btnFilterColorful
            // 
            this.btnFilterColorful.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFilterColorful.ForeColor = System.Drawing.Color.Black;
            this.btnFilterColorful.Location = new System.Drawing.Point(171, 66);
            this.btnFilterColorful.Name = "btnFilterColorful";
            this.btnFilterColorful.Size = new System.Drawing.Size(67, 23);
            this.btnFilterColorful.TabIndex = 17;
            this.btnFilterColorful.Text = "Colorful";
            this.btnFilterColorful.UseVisualStyleBackColor = true;
            this.btnFilterColorful.Click += new System.EventHandler(this.btnFilterColorful_Click);
            // 
            // btnFilterComic
            // 
            this.btnFilterComic.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFilterComic.ForeColor = System.Drawing.Color.Black;
            this.btnFilterComic.Location = new System.Drawing.Point(171, 32);
            this.btnFilterComic.Name = "btnFilterComic";
            this.btnFilterComic.Size = new System.Drawing.Size(67, 23);
            this.btnFilterComic.TabIndex = 16;
            this.btnFilterComic.Text = "Comic";
            this.btnFilterComic.UseVisualStyleBackColor = true;
            this.btnFilterComic.Click += new System.EventHandler(this.btnFilterComic_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(220, 11);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Filters";
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.ForeColor = System.Drawing.Color.Black;
            this.btnReset.Location = new System.Drawing.Point(83, 86);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 14;
            this.btnReset.Text = "Reset all";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnUpscale
            // 
            this.btnUpscale.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUpscale.ForeColor = System.Drawing.Color.Black;
            this.btnUpscale.Location = new System.Drawing.Point(83, 18);
            this.btnUpscale.Name = "btnUpscale";
            this.btnUpscale.Size = new System.Drawing.Size(75, 23);
            this.btnUpscale.TabIndex = 13;
            this.btnUpscale.Text = "Upscale";
            this.btnUpscale.UseVisualStyleBackColor = true;
            this.btnUpscale.Click += new System.EventHandler(this.btnUpscale_Click);
            // 
            // btnDownscale
            // 
            this.btnDownscale.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDownscale.ForeColor = System.Drawing.Color.Black;
            this.btnDownscale.Location = new System.Drawing.Point(83, 52);
            this.btnDownscale.Name = "btnDownscale";
            this.btnDownscale.Size = new System.Drawing.Size(75, 23);
            this.btnDownscale.TabIndex = 12;
            this.btnDownscale.Text = "Downscale";
            this.btnDownscale.UseVisualStyleBackColor = true;
            this.btnDownscale.Click += new System.EventHandler(this.btnDownscale_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.PaleTurquoise;
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Location = new System.Drawing.Point(10, 123);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(148, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "Apply changes";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.btnApplyChanges_Click_1);
            // 
            // btnFlipY
            // 
            this.btnFlipY.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFlipY.ForeColor = System.Drawing.Color.Black;
            this.btnFlipY.Location = new System.Drawing.Point(10, 86);
            this.btnFlipY.Name = "btnFlipY";
            this.btnFlipY.Size = new System.Drawing.Size(67, 23);
            this.btnFlipY.TabIndex = 10;
            this.btnFlipY.Text = "Flip Y";
            this.btnFlipY.UseVisualStyleBackColor = true;
            this.btnFlipY.Click += new System.EventHandler(this.btnFlipY_Click);
            // 
            // btnFlipX
            // 
            this.btnFlipX.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFlipX.ForeColor = System.Drawing.Color.Black;
            this.btnFlipX.Location = new System.Drawing.Point(10, 52);
            this.btnFlipX.Name = "btnFlipX";
            this.btnFlipX.Size = new System.Drawing.Size(67, 23);
            this.btnFlipX.TabIndex = 9;
            this.btnFlipX.Text = "Flip X";
            this.btnFlipX.UseVisualStyleBackColor = true;
            this.btnFlipX.Click += new System.EventHandler(this.btnFlipX_Click);
            // 
            // btnRotate
            // 
            this.btnRotate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRotate.ForeColor = System.Drawing.Color.Black;
            this.btnRotate.Location = new System.Drawing.Point(10, 18);
            this.btnRotate.Name = "btnRotate";
            this.btnRotate.Size = new System.Drawing.Size(67, 23);
            this.btnRotate.TabIndex = 8;
            this.btnRotate.Text = "Rotate";
            this.btnRotate.UseVisualStyleBackColor = true;
            this.btnRotate.Click += new System.EventHandler(this.btnRotate_Click_1);
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.Controls.Add(this.groupBox1);
            this.panel4.Controls.Add(this.groupBox2);
            this.panel4.Location = new System.Drawing.Point(572, 334);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(373, 194);
            this.panel4.TabIndex = 15;
            // 
            // ctxPreviewImage
            // 
            this.ctxPreviewImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.swapTextureToolStripMenuItem,
            this.toolStripSeparator13,
            this.saveAsToolStripMenuItem,
            this.extendViewToolStripMenuItem,
            this.animatedViewToolStripMenuItem});
            this.ctxPreviewImage.Name = "ctxPreviewImage";
            this.ctxPreviewImage.Size = new System.Drawing.Size(181, 120);
            // 
            // swapTextureToolStripMenuItem
            // 
            this.swapTextureToolStripMenuItem.Name = "swapTextureToolStripMenuItem";
            this.swapTextureToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.swapTextureToolStripMenuItem.Text = "Swap texture";
            this.swapTextureToolStripMenuItem.Click += new System.EventHandler(this.swapTextureToolStripMenuItem_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(177, 6);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pNGToolStripMenuItem1,
            this.bMPToolStripMenuItem1,
            this.tGAToolStripMenuItem1});
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveAsToolStripMenuItem.Text = "Save as";
            // 
            // pNGToolStripMenuItem1
            // 
            this.pNGToolStripMenuItem1.Name = "pNGToolStripMenuItem1";
            this.pNGToolStripMenuItem1.Size = new System.Drawing.Size(99, 22);
            this.pNGToolStripMenuItem1.Text = "PNG";
            this.pNGToolStripMenuItem1.Click += new System.EventHandler(this.pNGToolStripMenuItem1_Click);
            // 
            // bMPToolStripMenuItem1
            // 
            this.bMPToolStripMenuItem1.Name = "bMPToolStripMenuItem1";
            this.bMPToolStripMenuItem1.Size = new System.Drawing.Size(99, 22);
            this.bMPToolStripMenuItem1.Text = "BMP";
            this.bMPToolStripMenuItem1.Click += new System.EventHandler(this.bMPToolStripMenuItem1_Click);
            // 
            // tGAToolStripMenuItem1
            // 
            this.tGAToolStripMenuItem1.Name = "tGAToolStripMenuItem1";
            this.tGAToolStripMenuItem1.Size = new System.Drawing.Size(99, 22);
            this.tGAToolStripMenuItem1.Text = "TGA";
            this.tGAToolStripMenuItem1.Click += new System.EventHandler(this.tGAToolStripMenuItem1_Click);
            // 
            // extendViewToolStripMenuItem
            // 
            this.extendViewToolStripMenuItem.Name = "extendViewToolStripMenuItem";
            this.extendViewToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
            this.extendViewToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.extendViewToolStripMenuItem.Text = "Extend view";
            this.extendViewToolStripMenuItem.Click += new System.EventHandler(this.extendViewToolStripMenuItem_Click);
            // 
            // animatedViewToolStripMenuItem
            // 
            this.animatedViewToolStripMenuItem.Name = "animatedViewToolStripMenuItem";
            this.animatedViewToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.animatedViewToolStripMenuItem.Text = "Animated view";
            this.animatedViewToolStripMenuItem.Click += new System.EventHandler(this.animatedViewToolStripMenuItem_Click);
            // 
            // FrmMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(951, 553);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.texturePreview);
            this.Controls.Add(this.btnOpenFile);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.btnCreateNewFile);
            this.Controls.Add(this.table);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmMain";
            this.Text = "RE4 PS2 TPL Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.texturePreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.table)).EndInit();
            this.ctxMenuTable.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinHue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinPixelate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinSharpen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinSaturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinContrast)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.ctxPreviewImage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openTPLFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem includeMipmapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.DataGridView table;
        private System.Windows.Forms.ToolStripMenuItem textureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exportAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showHiddenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem addNewTexturesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compileFromFolderToolStripMenuItem;
        private System.Windows.Forms.PictureBox texturePreview;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem convertAllToPNGToolStripMenuItem;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip ctxMenuTable;
        private System.Windows.Forms.ToolStripMenuItem testeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem duplicateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pNGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bMPToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem convertAndImportBMPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem creditsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem disableDuplicatePromptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAllMipmapsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem increaseColorDepthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decreaseColorDepthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem removeMipmapsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem fixBrokenTPLexperimentalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createEmptyTPLFileToolStripMenuItem;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.Button btnCreateNewFile;
        private System.Windows.Forms.StatusStrip statusStrip1;
        public System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusText;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown spinSharpen;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown spinSaturation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown spinContrast;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown spinBrightness;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.NumericUpDown spinPixelate;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button btnRotate;
        private System.Windows.Forms.Button btnFlipY;
        private System.Windows.Forms.Button btnFlipX;
        private System.Windows.Forms.ToolStripMenuItem increaseAllTo256ColorsToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem refreshTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripMenuItem decreaseAllTo16ColorsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip ctxPreviewImage;
        private System.Windows.Forms.ToolStripMenuItem swapTextureToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pNGToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem bMPToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem usabilityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem problemsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tGAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tGAToolStripMenuItem1;
        private System.Windows.Forms.Button btnUpscale;
        private System.Windows.Forms.Button btnDownscale;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.ToolStripMenuItem convertAllToBMPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertAllToTGAToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown spinHue;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnFilterGray;
        private System.Windows.Forms.Button btnFilterInvert;
        private System.Windows.Forms.Button btnFilterColorful;
        private System.Windows.Forms.Button btnFilterComic;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnLayerOverlay;
        private System.Windows.Forms.Button btnLayerAddMask;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ToolStripMenuItem extendViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem animatedViewToolStripMenuItem;
    }
}

