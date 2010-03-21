namespace QuickRoute.GPSDeviceReaders.JJConnectRegistratorSEReader
{
    partial class JJConnectRegistratorSETrackSelector
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
            this.cbSelectedTrack = new System.Windows.Forms.ComboBox();
            this.lblSelectedTarck = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnScan = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbSelectedTrack
            // 
            this.cbSelectedTrack.FormattingEnabled = true;
            this.cbSelectedTrack.Location = new System.Drawing.Point(12, 29);
            this.cbSelectedTrack.Name = "cbSelectedTrack";
            this.cbSelectedTrack.Size = new System.Drawing.Size(473, 24);
            this.cbSelectedTrack.TabIndex = 12;
            // 
            // lblSelectedTarck
            // 
            this.lblSelectedTarck.AutoSize = true;
            this.lblSelectedTarck.Location = new System.Drawing.Point(12, 9);
            this.lblSelectedTarck.Name = "lblSelectedTarck";
            this.lblSelectedTarck.Size = new System.Drawing.Size(62, 17);
            this.lblSelectedTarck.TabIndex = 13;
            this.lblSelectedTarck.Text = "Session:";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(259, 70);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(110, 30);
            this.btnOk.TabIndex = 14;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(375, 70);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 30);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(87, 70);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(166, 30);
            this.btnScan.TabIndex = 1;
            this.btnScan.Text = "Re-read GPS unit";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // GlobalSatGH615MTrackSelector
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 116);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnScan);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblSelectedTarck);
            this.Controls.Add(this.cbSelectedTrack);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "GlobalSatGH615MTrackSelector";
            this.Text = "Select session";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbSelectedTrack;
        private System.Windows.Forms.Label lblSelectedTarck;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnScan;

    }
}