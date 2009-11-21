namespace QuickRoute.UI.Forms
{
  partial class SplashScreen
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
      this.applicationName = new System.Windows.Forms.Label();
      this.version = new System.Windows.Forms.Label();
      this.website = new System.Windows.Forms.LinkLabel();
      this.donate = new System.Windows.Forms.LinkLabel();
      this.developers = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // applicationName
      // 
      this.applicationName.AutoSize = true;
      this.applicationName.Location = new System.Drawing.Point(108, 40);
      this.applicationName.Name = "applicationName";
      this.applicationName.Size = new System.Drawing.Size(35, 13);
      this.applicationName.TabIndex = 0;
      this.applicationName.Text = "label1";
      // 
      // version
      // 
      this.version.AutoSize = true;
      this.version.Location = new System.Drawing.Point(108, 97);
      this.version.Name = "version";
      this.version.Size = new System.Drawing.Size(35, 13);
      this.version.TabIndex = 1;
      this.version.Text = "label2";
      // 
      // website
      // 
      this.website.AutoSize = true;
      this.website.Location = new System.Drawing.Point(108, 163);
      this.website.Name = "website";
      this.website.Size = new System.Drawing.Size(55, 13);
      this.website.TabIndex = 2;
      this.website.TabStop = true;
      this.website.Text = "linkLabel1";
      // 
      // donate
      // 
      this.donate.AutoSize = true;
      this.donate.Location = new System.Drawing.Point(108, 212);
      this.donate.Name = "donate";
      this.donate.Size = new System.Drawing.Size(55, 13);
      this.donate.TabIndex = 3;
      this.donate.TabStop = true;
      this.donate.Text = "linkLabel1";
      // 
      // developers
      // 
      this.developers.AutoSize = true;
      this.developers.Location = new System.Drawing.Point(108, 134);
      this.developers.Name = "developers";
      this.developers.Size = new System.Drawing.Size(35, 13);
      this.developers.TabIndex = 4;
      this.developers.Text = "label2";
      // 
      // SplashScreen
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(292, 262);
      this.ControlBox = false;
      this.Controls.Add(this.developers);
      this.Controls.Add(this.donate);
      this.Controls.Add(this.website);
      this.Controls.Add(this.version);
      this.Controls.Add(this.applicationName);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.HelpButton = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "SplashScreen";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "SplashScreen";
      this.Load += new System.EventHandler(this.SplashScreen_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label applicationName;
    private System.Windows.Forms.Label version;
    private System.Windows.Forms.LinkLabel website;
    private System.Windows.Forms.LinkLabel donate;
    private System.Windows.Forms.Label developers;
  }
}