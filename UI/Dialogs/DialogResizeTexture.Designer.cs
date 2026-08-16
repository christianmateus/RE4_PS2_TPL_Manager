namespace RE4_PS2_TPL_Manager
{
    partial class DialogResizeTexture
    {
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.lblBefore = new System.Windows.Forms.Label();
            this.lblAfter = new System.Windows.Forms.Label();
            this.previewBefore = new RE4_PS2_TPL_Manager.TexturePreviewBox();
            this.previewAfter = new RE4_PS2_TPL_Manager.TexturePreviewBox();
            this.lblWidth = new System.Windows.Forms.Label();
            this.lblHeight = new System.Windows.Forms.Label();
            this.numWidth = new System.Windows.Forms.NumericUpDown();
            this.numHeight = new System.Windows.Forms.NumericUpDown();
            this.chkKeepAspect = new System.Windows.Forms.CheckBox();
            this.lblSize = new System.Windows.Forms.Label();
            this.btnHalf = new System.Windows.Forms.Button();
            this.btnOriginal = new System.Windows.Forms.Button();
            this.btnDouble = new System.Windows.Forms.Button();
            this.lblResampling = new System.Windows.Forms.Label();
            this.cmbResampling = new System.Windows.Forms.ComboBox();
            this.lblSummary = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnResize = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.previewBefore)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.previewAfter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // lblBefore
            // 
            this.lblBefore.Location = new System.Drawing.Point(14, 10);
            this.lblBefore.Name = "lblBefore";
            this.lblBefore.Size = new System.Drawing.Size(360, 20);
            this.lblBefore.TabIndex = 0;
            this.lblBefore.Text = "Before";
            this.lblBefore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAfter
            // 
            this.lblAfter.Location = new System.Drawing.Point(386, 10);
            this.lblAfter.Name = "lblAfter";
            this.lblAfter.Size = new System.Drawing.Size(360, 20);
            this.lblAfter.TabIndex = 1;
            this.lblAfter.Text = "After";
            this.lblAfter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // previewBefore
            // 
            this.previewBefore.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.previewBefore.Location = new System.Drawing.Point(14, 32);
            this.previewBefore.Name = "previewBefore";
            this.previewBefore.Size = new System.Drawing.Size(360, 220);
            this.previewBefore.ShowNavigationHint = false;
            this.previewBefore.TabIndex = 2;
            this.previewBefore.TabStop = false;
            // 
            // previewAfter
            // 
            this.previewAfter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.previewAfter.Location = new System.Drawing.Point(386, 32);
            this.previewAfter.Name = "previewAfter";
            this.previewAfter.Size = new System.Drawing.Size(360, 220);
            this.previewAfter.ShowNavigationHint = false;
            this.previewAfter.TabIndex = 3;
            this.previewAfter.TabStop = false;
            // 
            // lblWidth
            // 
            this.lblWidth.AutoSize = true;
            this.lblWidth.Location = new System.Drawing.Point(14, 272);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(39, 15);
            this.lblWidth.TabIndex = 4;
            this.lblWidth.Text = "Width";
            // 
            // numWidth
            // 
            this.numWidth.Location = new System.Drawing.Point(60, 268);
            this.numWidth.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
            this.numWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numWidth.Name = "numWidth";
            this.numWidth.Size = new System.Drawing.Size(76, 23);
            this.numWidth.TabIndex = 5;
            this.numWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numWidth.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.numWidth.ValueChanged += new System.EventHandler(this.numWidth_ValueChanged);
            // 
            // lblHeight
            // 
            this.lblHeight.AutoSize = true;
            this.lblHeight.Location = new System.Drawing.Point(150, 272);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(43, 15);
            this.lblHeight.TabIndex = 6;
            this.lblHeight.Text = "Height";
            // 
            // numHeight
            // 
            this.numHeight.Location = new System.Drawing.Point(200, 268);
            this.numHeight.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
            this.numHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numHeight.Name = "numHeight";
            this.numHeight.Size = new System.Drawing.Size(76, 23);
            this.numHeight.TabIndex = 7;
            this.numHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numHeight.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.numHeight.ValueChanged += new System.EventHandler(this.numHeight_ValueChanged);
            // 
            // chkKeepAspect
            // 
            this.chkKeepAspect.AutoSize = true;
            this.chkKeepAspect.Checked = true;
            this.chkKeepAspect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkKeepAspect.Location = new System.Drawing.Point(292, 270);
            this.chkKeepAspect.Name = "chkKeepAspect";
            this.chkKeepAspect.Size = new System.Drawing.Size(119, 19);
            this.chkKeepAspect.TabIndex = 8;
            this.chkKeepAspect.Text = "Keep aspect ratio";
            this.chkKeepAspect.UseVisualStyleBackColor = true;
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(14, 306);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(27, 15);
            this.lblSize.TabIndex = 9;
            this.lblSize.Text = "Size";
            // 
            // btnHalf
            // 
            this.btnHalf.Location = new System.Drawing.Point(60, 300);
            this.btnHalf.Name = "btnHalf";
            this.btnHalf.Size = new System.Drawing.Size(72, 27);
            this.btnHalf.TabIndex = 10;
            this.btnHalf.Text = "½ Size";
            this.btnHalf.UseVisualStyleBackColor = true;
            this.btnHalf.Click += new System.EventHandler(this.btnHalf_Click);
            // 
            // btnOriginal
            // 
            this.btnOriginal.Location = new System.Drawing.Point(138, 300);
            this.btnOriginal.Name = "btnOriginal";
            this.btnOriginal.Size = new System.Drawing.Size(72, 27);
            this.btnOriginal.TabIndex = 11;
            this.btnOriginal.Text = "Original";
            this.btnOriginal.UseVisualStyleBackColor = true;
            this.btnOriginal.Click += new System.EventHandler(this.btnOriginal_Click);
            // 
            // btnDouble
            // 
            this.btnDouble.Location = new System.Drawing.Point(216, 300);
            this.btnDouble.Name = "btnDouble";
            this.btnDouble.Size = new System.Drawing.Size(72, 27);
            this.btnDouble.TabIndex = 12;
            this.btnDouble.Text = "2× Size";
            this.btnDouble.UseVisualStyleBackColor = true;
            this.btnDouble.Click += new System.EventHandler(this.btnDouble_Click);
            // 
            // lblResampling
            // 
            this.lblResampling.AutoSize = true;
            this.lblResampling.Location = new System.Drawing.Point(492, 306);
            this.lblResampling.Name = "lblResampling";
            this.lblResampling.Size = new System.Drawing.Size(68, 15);
            this.lblResampling.TabIndex = 13;
            this.lblResampling.Text = "Resampling";
            // 
            // cmbResampling
            // 
            this.cmbResampling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbResampling.FormattingEnabled = true;
            this.cmbResampling.Items.AddRange(new object[] {
            "Nearest Neighbor",
            "Bilinear",
            "Bicubic"});
            this.cmbResampling.Location = new System.Drawing.Point(568, 302);
            this.cmbResampling.Name = "cmbResampling";
            this.cmbResampling.Size = new System.Drawing.Size(140, 23);
            this.cmbResampling.TabIndex = 14;
            this.cmbResampling.SelectedIndexChanged += new System.EventHandler(this.cmbResampling_SelectedIndexChanged);
            // 
            // lblSummary
            // 
            this.lblSummary.Location = new System.Drawing.Point(14, 342);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(430, 27);
            this.lblSummary.TabIndex = 15;
            this.lblSummary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(590, 337);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(74, 29);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnResize
            // 
            this.btnResize.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnResize.Location = new System.Drawing.Point(670, 337);
            this.btnResize.Name = "btnResize";
            this.btnResize.Size = new System.Drawing.Size(76, 29);
            this.btnResize.TabIndex = 17;
            this.btnResize.Text = "Resize";
            this.btnResize.UseVisualStyleBackColor = true;
            // 
            // DialogResizeTexture
            // 
            this.AcceptButton = this.btnResize;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(760, 380);
            this.Controls.Add(this.btnResize);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblSummary);
            this.Controls.Add(this.cmbResampling);
            this.Controls.Add(this.lblResampling);
            this.Controls.Add(this.btnDouble);
            this.Controls.Add(this.btnOriginal);
            this.Controls.Add(this.btnHalf);
            this.Controls.Add(this.lblSize);
            this.Controls.Add(this.chkKeepAspect);
            this.Controls.Add(this.numHeight);
            this.Controls.Add(this.lblHeight);
            this.Controls.Add(this.numWidth);
            this.Controls.Add(this.lblWidth);
            this.Controls.Add(this.previewAfter);
            this.Controls.Add(this.previewBefore);
            this.Controls.Add(this.lblAfter);
            this.Controls.Add(this.lblBefore);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogResizeTexture";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Resize Texture";
            ((System.ComponentModel.ISupportInitialize)(this.previewBefore)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.previewAfter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblBefore;
        private System.Windows.Forms.Label lblAfter;
        private RE4_PS2_TPL_Manager.TexturePreviewBox previewBefore;
        private RE4_PS2_TPL_Manager.TexturePreviewBox previewAfter;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.NumericUpDown numWidth;
        private System.Windows.Forms.NumericUpDown numHeight;
        private System.Windows.Forms.CheckBox chkKeepAspect;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.Button btnHalf;
        private System.Windows.Forms.Button btnOriginal;
        private System.Windows.Forms.Button btnDouble;
        private System.Windows.Forms.Label lblResampling;
        private System.Windows.Forms.ComboBox cmbResampling;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnResize;
    }
}
