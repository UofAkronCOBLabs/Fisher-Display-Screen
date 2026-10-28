namespace Display_test
{
    partial class InActivityWindow
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
            this.lblText = new System.Windows.Forms.Label();
            this.btnYes = new System.Windows.Forms.Button();
            this.tlpIAW = new System.Windows.Forms.TableLayoutPanel();
            this.tlpIAW.SuspendLayout();
            this.SuspendLayout();
            this.lblText.AutoSize = true;
            this.lblText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblText.Font = new System.Drawing.Font("Microsoft Sans Serif", 27F, System.Drawing.FontStyle.Bold);
            this.lblText.ForeColor = System.Drawing.Color.Transparent;
            this.lblText.Location = new System.Drawing.Point(3, 0);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(394, 100);
            this.lblText.TabIndex = 0;
            this.lblText.Text = "Still Watching?";
            this.lblText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblText.Click += new System.EventHandler(this.lblText_Click);
            this.btnYes.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnYes.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnYes.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnYes.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            this.btnYes.ForeColor = System.Drawing.Color.Black;
            this.btnYes.Location = new System.Drawing.Point(110, 130);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(180, 39);
            this.btnYes.TabIndex = 1;
            this.btnYes.Text = "Yes";
            this.btnYes.UseVisualStyleBackColor = false;
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            this.tlpIAW.ColumnCount = 1;
            this.tlpIAW.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpIAW.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpIAW.Controls.Add(this.lblText, 0, 0);
            this.tlpIAW.Controls.Add(this.btnYes, 0, 1);
            this.tlpIAW.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpIAW.Location = new System.Drawing.Point(0, 0);
            this.tlpIAW.Name = "tlpIAW";
            this.tlpIAW.RowCount = 2;
            this.tlpIAW.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpIAW.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpIAW.Size = new System.Drawing.Size(400, 200);
            this.tlpIAW.TabIndex = 2;
            this.tlpIAW.Click += new System.EventHandler(this.InActivityWindow_Click);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(0, 76, 157);
            this.ClientSize = new System.Drawing.Size(400, 200);
            this.Controls.Add(this.tlpIAW);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "InActivityWindow";
            this.ShowInTaskbar = false;
            this.Text = "Inactivity";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.InActivityWindow_Load);
            this.Click += new System.EventHandler(this.InActivityWindow_Click);
            this.Leave += new System.EventHandler(this.InActivityWindow_Leave);
            this.Resize += new System.EventHandler(this.autoScale);
            this.tlpIAW.ResumeLayout(false);
            this.tlpIAW.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.TableLayoutPanel tlpIAW;
    }
}
