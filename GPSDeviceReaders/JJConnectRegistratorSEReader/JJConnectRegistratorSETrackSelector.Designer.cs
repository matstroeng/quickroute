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
          System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JJConnectRegistratorSETrackSelector));
          this.cbSelectedTrack = new System.Windows.Forms.ComboBox();
          this.lblSelectedTarck = new System.Windows.Forms.Label();
          this.btnOk = new System.Windows.Forms.Button();
          this.btnCancel = new System.Windows.Forms.Button();
          this.btnScan = new System.Windows.Forms.Button();
          this.SuspendLayout();
          // 
          // cbSelectedTrack
          // 
          this.cbSelectedTrack.AccessibleDescription = null;
          this.cbSelectedTrack.AccessibleName = null;
          resources.ApplyResources(this.cbSelectedTrack, "cbSelectedTrack");
          this.cbSelectedTrack.BackgroundImage = null;
          this.cbSelectedTrack.Font = null;
          this.cbSelectedTrack.FormattingEnabled = true;
          this.cbSelectedTrack.Name = "cbSelectedTrack";
          // 
          // lblSelectedTarck
          // 
          this.lblSelectedTarck.AccessibleDescription = null;
          this.lblSelectedTarck.AccessibleName = null;
          resources.ApplyResources(this.lblSelectedTarck, "lblSelectedTarck");
          this.lblSelectedTarck.Font = null;
          this.lblSelectedTarck.Name = "lblSelectedTarck";
          // 
          // btnOk
          // 
          this.btnOk.AccessibleDescription = null;
          this.btnOk.AccessibleName = null;
          resources.ApplyResources(this.btnOk, "btnOk");
          this.btnOk.BackgroundImage = null;
          this.btnOk.Font = null;
          this.btnOk.Name = "btnOk";
          this.btnOk.UseVisualStyleBackColor = true;
          this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
          // 
          // btnCancel
          // 
          this.btnCancel.AccessibleDescription = null;
          this.btnCancel.AccessibleName = null;
          resources.ApplyResources(this.btnCancel, "btnCancel");
          this.btnCancel.BackgroundImage = null;
          this.btnCancel.Font = null;
          this.btnCancel.Name = "btnCancel";
          this.btnCancel.UseVisualStyleBackColor = true;
          this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
          // 
          // btnScan
          // 
          this.btnScan.AccessibleDescription = null;
          this.btnScan.AccessibleName = null;
          resources.ApplyResources(this.btnScan, "btnScan");
          this.btnScan.BackgroundImage = null;
          this.btnScan.Font = null;
          this.btnScan.Name = "btnScan";
          this.btnScan.UseVisualStyleBackColor = true;
          this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
          // 
          // JJConnectRegistratorSETrackSelector
          // 
          this.AcceptButton = this.btnOk;
          this.AccessibleDescription = null;
          this.AccessibleName = null;
          resources.ApplyResources(this, "$this");
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.BackgroundImage = null;
          this.Controls.Add(this.btnCancel);
          this.Controls.Add(this.btnScan);
          this.Controls.Add(this.btnOk);
          this.Controls.Add(this.lblSelectedTarck);
          this.Controls.Add(this.cbSelectedTrack);
          this.Font = null;
          this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
          this.Icon = null;
          this.Name = "JJConnectRegistratorSETrackSelector";
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