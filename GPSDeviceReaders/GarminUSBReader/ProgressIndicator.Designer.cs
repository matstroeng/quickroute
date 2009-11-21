namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
    partial class ProgressIndicator
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
          System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressIndicator));
          this.status = new System.Windows.Forms.Label();
          this.progressBar = new System.Windows.Forms.ProgressBar();
          this.cancel = new System.Windows.Forms.Button();
          this.SuspendLayout();
          // 
          // status
          // 
          this.status.AccessibleDescription = null;
          this.status.AccessibleName = null;
          resources.ApplyResources(this.status, "status");
          this.status.Font = null;
          this.status.Name = "status";
          // 
          // progressBar
          // 
          this.progressBar.AccessibleDescription = null;
          this.progressBar.AccessibleName = null;
          resources.ApplyResources(this.progressBar, "progressBar");
          this.progressBar.BackgroundImage = null;
          this.progressBar.Font = null;
          this.progressBar.Name = "progressBar";
          // 
          // cancel
          // 
          this.cancel.AccessibleDescription = null;
          this.cancel.AccessibleName = null;
          resources.ApplyResources(this.cancel, "cancel");
          this.cancel.BackgroundImage = null;
          this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
          this.cancel.Font = null;
          this.cancel.Name = "cancel";
          this.cancel.UseVisualStyleBackColor = true;
          this.cancel.Click += new System.EventHandler(this.btnCancel_Click);
          // 
          // ProgressIndicator
          // 
          this.AccessibleDescription = null;
          this.AccessibleName = null;
          resources.ApplyResources(this, "$this");
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.BackgroundImage = null;
          this.CancelButton = this.cancel;
          this.ControlBox = false;
          this.Controls.Add(this.cancel);
          this.Controls.Add(this.progressBar);
          this.Controls.Add(this.status);
          this.Font = null;
          this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
          this.Icon = null;
          this.MaximizeBox = false;
          this.MinimizeBox = false;
          this.Name = "ProgressIndicator";
          this.ShowInTaskbar = false;
          this.ResumeLayout(false);
          this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label status;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button cancel;
    }
}