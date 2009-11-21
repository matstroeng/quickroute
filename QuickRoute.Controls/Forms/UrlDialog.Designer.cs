namespace QuickRoute.Controls.Forms
{
  partial class UrlDialog
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UrlDialog));
      this.ok = new System.Windows.Forms.Button();
      this.cancel = new System.Windows.Forms.Button();
      this.uiInformation = new System.Windows.Forms.Label();
      this.urls = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // ok
      // 
      this.ok.AccessibleDescription = null;
      this.ok.AccessibleName = null;
      resources.ApplyResources(this.ok, "ok");
      this.ok.BackgroundImage = null;
      this.ok.Font = null;
      this.ok.Name = "ok";
      this.ok.UseVisualStyleBackColor = true;
      this.ok.Click += new System.EventHandler(this.ok_Click);
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
      this.cancel.Click += new System.EventHandler(this.cancel_Click);
      // 
      // uiInformation
      // 
      this.uiInformation.AccessibleDescription = null;
      this.uiInformation.AccessibleName = null;
      resources.ApplyResources(this.uiInformation, "uiInformation");
      this.uiInformation.BackColor = System.Drawing.SystemColors.Info;
      this.uiInformation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.uiInformation.Font = null;
      this.uiInformation.Name = "uiInformation";
      // 
      // urls
      // 
      this.urls.AcceptsReturn = true;
      this.urls.AccessibleDescription = null;
      this.urls.AccessibleName = null;
      resources.ApplyResources(this.urls, "urls");
      this.urls.BackgroundImage = null;
      this.urls.Font = null;
      this.urls.Name = "urls";
      this.urls.Enter += new System.EventHandler(this.urls_Enter);
      // 
      // UrlDialog
      // 
      this.AcceptButton = this.ok;
      this.AccessibleDescription = null;
      this.AccessibleName = null;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = null;
      this.CancelButton = this.cancel;
      this.Controls.Add(this.urls);
      this.Controls.Add(this.uiInformation);
      this.Controls.Add(this.cancel);
      this.Controls.Add(this.ok);
      this.Font = null;
      this.Icon = null;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "UrlDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Load += new System.EventHandler(this.UrlDialog_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button ok;
    private System.Windows.Forms.Button cancel;
    private System.Windows.Forms.Label uiInformation;
    private System.Windows.Forms.TextBox urls;
  }
}