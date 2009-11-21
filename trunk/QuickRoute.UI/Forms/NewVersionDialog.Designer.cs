namespace QuickRoute.UI.Forms
{
  partial class NewVersionDialog
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewVersionDialog));
      this.goToDownloadPage = new System.Windows.Forms.Button();
      this.dontRemindMe = new System.Windows.Forms.CheckBox();
      this.close = new System.Windows.Forms.Button();
      this.message = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // goToDownloadPage
      // 
      this.goToDownloadPage.AccessibleDescription = null;
      this.goToDownloadPage.AccessibleName = null;
      resources.ApplyResources(this.goToDownloadPage, "goToDownloadPage");
      this.goToDownloadPage.BackgroundImage = null;
      this.goToDownloadPage.Font = null;
      this.goToDownloadPage.Name = "goToDownloadPage";
      this.goToDownloadPage.UseVisualStyleBackColor = true;
      this.goToDownloadPage.Click += new System.EventHandler(this.goToDownloadPage_Click);
      // 
      // dontRemindMe
      // 
      this.dontRemindMe.AccessibleDescription = null;
      this.dontRemindMe.AccessibleName = null;
      resources.ApplyResources(this.dontRemindMe, "dontRemindMe");
      this.dontRemindMe.BackgroundImage = null;
      this.dontRemindMe.Font = null;
      this.dontRemindMe.Name = "dontRemindMe";
      this.dontRemindMe.UseVisualStyleBackColor = true;
      // 
      // close
      // 
      this.close.AccessibleDescription = null;
      this.close.AccessibleName = null;
      resources.ApplyResources(this.close, "close");
      this.close.BackgroundImage = null;
      this.close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.close.Font = null;
      this.close.Name = "close";
      this.close.UseVisualStyleBackColor = true;
      this.close.Click += new System.EventHandler(this.close_Click);
      // 
      // message
      // 
      this.message.AcceptsReturn = true;
      this.message.AccessibleDescription = null;
      this.message.AccessibleName = null;
      resources.ApplyResources(this.message, "message");
      this.message.BackgroundImage = null;
      this.message.Font = null;
      this.message.Name = "message";
      this.message.ReadOnly = true;
      // 
      // NewVersionDialog
      // 
      this.AcceptButton = this.close;
      this.AccessibleDescription = null;
      this.AccessibleName = null;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = null;
      this.CancelButton = this.close;
      this.Controls.Add(this.message);
      this.Controls.Add(this.dontRemindMe);
      this.Controls.Add(this.close);
      this.Controls.Add(this.goToDownloadPage);
      this.Font = null;
      this.Icon = null;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "NewVersionDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button goToDownloadPage;
    private System.Windows.Forms.CheckBox dontRemindMe;
    private System.Windows.Forms.Button close;
    private System.Windows.Forms.TextBox message;
  }
}