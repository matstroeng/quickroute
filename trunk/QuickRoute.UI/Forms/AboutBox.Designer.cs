namespace QuickRoute.UI.Forms
{
  partial class AboutBox
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
      this.quickRouteLabel = new System.Windows.Forms.Label();
      this.versionLabel = new System.Windows.Forms.Label();
      this.developerLabel = new System.Windows.Forms.Label();
      this.closeButton = new System.Windows.Forms.Button();
      this.websiteLink = new System.Windows.Forms.LinkLabel();
      this.translationLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // quickRouteLabel
      // 
      resources.ApplyResources(this.quickRouteLabel, "quickRouteLabel");
      this.quickRouteLabel.BackColor = System.Drawing.Color.Transparent;
      this.quickRouteLabel.Name = "quickRouteLabel";
      // 
      // versionLabel
      // 
      resources.ApplyResources(this.versionLabel, "versionLabel");
      this.versionLabel.BackColor = System.Drawing.Color.Transparent;
      this.versionLabel.Name = "versionLabel";
      // 
      // developerLabel
      // 
      resources.ApplyResources(this.developerLabel, "developerLabel");
      this.developerLabel.BackColor = System.Drawing.Color.Transparent;
      this.developerLabel.Name = "developerLabel";
      // 
      // closeButton
      // 
      resources.ApplyResources(this.closeButton, "closeButton");
      this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.closeButton.Name = "closeButton";
      this.closeButton.UseVisualStyleBackColor = true;
      this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
      // 
      // websiteLink
      // 
      resources.ApplyResources(this.websiteLink, "websiteLink");
      this.websiteLink.BackColor = System.Drawing.Color.Transparent;
      this.websiteLink.Name = "websiteLink";
      this.websiteLink.TabStop = true;
      this.websiteLink.UseCompatibleTextRendering = true;
      this.websiteLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.websiteLink_LinkClicked);
      // 
      // translationLabel
      // 
      resources.ApplyResources(this.translationLabel, "translationLabel");
      this.translationLabel.BackColor = System.Drawing.Color.Transparent;
      this.translationLabel.Name = "translationLabel";
      // 
      // AboutBox
      // 
      this.AcceptButton = this.closeButton;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.closeButton;
      this.Controls.Add(this.translationLabel);
      this.Controls.Add(this.websiteLink);
      this.Controls.Add(this.closeButton);
      this.Controls.Add(this.developerLabel);
      this.Controls.Add(this.versionLabel);
      this.Controls.Add(this.quickRouteLabel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AboutBox";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label quickRouteLabel;
    private System.Windows.Forms.Label versionLabel;
    private System.Windows.Forms.Label developerLabel;
    private System.Windows.Forms.Button closeButton;
    private System.Windows.Forms.LinkLabel websiteLink;
    private System.Windows.Forms.Label translationLabel;
  }
}