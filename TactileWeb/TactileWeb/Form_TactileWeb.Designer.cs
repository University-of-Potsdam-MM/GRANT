namespace TactileWeb
{
    partial class Form_TactileWeb
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtUIAInfo = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textUIAInfo
            // 
            this.txtUIAInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUIAInfo.BackColor = System.Drawing.SystemColors.Control;
            this.txtUIAInfo.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtUIAInfo.Location = new System.Drawing.Point(25, 70);
            this.txtUIAInfo.Multiline = true;
            this.txtUIAInfo.Name = "textUIAInfo";
            this.txtUIAInfo.ReadOnly = true;
            this.txtUIAInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtUIAInfo.Size = new System.Drawing.Size(490, 355);
            this.txtUIAInfo.TabIndex = 0;
            this.txtUIAInfo.WordWrap = false;
            // 
            // Form_TactileWeb
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1082, 453);
            this.Controls.Add(this.txtUIAInfo);
            this.Name = "Form_TactileWeb";
            this.Text = "TactileWeb";
            this.Load += new System.EventHandler(this.Form_TactileWeb_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

            // 
            // tmrRefresh
            //
            this.tmrRefresh = new System.Windows.Forms.Timer();

            this.tmrRefresh.Enabled = true;
            this.tmrRefresh.Interval = 200;
            this.tmrRefresh.Tick += new System.EventHandler(this.tmrRefresh_Tick);

        }

        #endregion

        private System.Windows.Forms.TextBox txtUIAInfo;

        private System.Windows.Forms.Timer tmrRefresh;

    }
}

