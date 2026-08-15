namespace RE4_PS2_TPL_Manager.Dialog
{
    partial class DialogGetIndex
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogGetIndex));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.spinIndexForReplace = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.btnConfirmIndex = new System.Windows.Forms.Button();
            this.btnOpenNewInstance = new System.Windows.Forms.Button();
            this.lblTplCount = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinIndexForReplace)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(407, 31);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(127, 7);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(158, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Multiple textures detected";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panel2.Controls.Add(this.lblTplCount);
            this.panel2.Controls.Add(this.btnConfirmIndex);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.spinIndexForReplace);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(3, 69);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(407, 135);
            this.panel2.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(148, 59);
            this.label3.Name = "label3";
            this.label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label3.Size = new System.Drawing.Size(42, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Index:";
            // 
            // spinIndexForReplace
            // 
            this.spinIndexForReplace.Location = new System.Drawing.Point(192, 57);
            this.spinIndexForReplace.Name = "spinIndexForReplace";
            this.spinIndexForReplace.Size = new System.Drawing.Size(62, 20);
            this.spinIndexForReplace.TabIndex = 1;
            this.spinIndexForReplace.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(34, 5);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label2.Size = new System.Drawing.Size(345, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "Please select which index is the texture you want to import";
            // 
            // btnConfirmIndex
            // 
            this.btnConfirmIndex.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirmIndex.Location = new System.Drawing.Point(151, 83);
            this.btnConfirmIndex.Name = "btnConfirmIndex";
            this.btnConfirmIndex.Size = new System.Drawing.Size(103, 23);
            this.btnConfirmIndex.TabIndex = 3;
            this.btnConfirmIndex.Text = "Confirm";
            this.btnConfirmIndex.UseVisualStyleBackColor = true;
            this.btnConfirmIndex.Click += new System.EventHandler(this.btnConfirmIndex_Click);
            // 
            // btnOpenNewInstance
            // 
            this.btnOpenNewInstance.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenNewInstance.Location = new System.Drawing.Point(278, 228);
            this.btnOpenNewInstance.Name = "btnOpenNewInstance";
            this.btnOpenNewInstance.Size = new System.Drawing.Size(122, 23);
            this.btnOpenNewInstance.TabIndex = 4;
            this.btnOpenNewInstance.Text = "I don\'t know the index";
            this.btnOpenNewInstance.UseVisualStyleBackColor = true;
            this.btnOpenNewInstance.Click += new System.EventHandler(this.btnOpenNewInstance_Click);
            // 
            // lblTplCount
            // 
            this.lblTplCount.AutoSize = true;
            this.lblTplCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTplCount.ForeColor = System.Drawing.Color.White;
            this.lblTplCount.Location = new System.Drawing.Point(260, 59);
            this.lblTplCount.Name = "lblTplCount";
            this.lblTplCount.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTplCount.Size = new System.Drawing.Size(0, 16);
            this.lblTplCount.TabIndex = 4;
            // 
            // DialogGetIndex
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(412, 263);
            this.Controls.Add(this.btnOpenNewInstance);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DialogGetIndex";
            this.Text = "RE4 PS2 TPL Manager";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinIndexForReplace)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown spinIndexForReplace;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnConfirmIndex;
        private System.Windows.Forms.Button btnOpenNewInstance;
        public System.Windows.Forms.Label lblTplCount;
    }
}