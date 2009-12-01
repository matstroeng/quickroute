namespace QuickRoute.UI.Forms
{
  partial class ExceptionMessageBox
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionMessageBox));
      this.stackTrace = new System.Windows.Forms.TextBox();
      this.showStackTrace = new System.Windows.Forms.Button();
      this.ok = new System.Windows.Forms.Button();
      this.errorIcon = new System.Windows.Forms.Label();
      this.errorMessage = new System.Windows.Forms.Label();
      this.hideStackTrace = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // stackTrace
      // 
      resources.ApplyResources(this.stackTrace, "stackTrace");
      this.stackTrace.Name = "stackTrace";
      this.stackTrace.ReadOnly = true;
      // 
      // showStackTrace
      // 
      resources.ApplyResources(this.showStackTrace, "showStackTrace");
      this.showStackTrace.Name = "showStackTrace";
      this.showStackTrace.UseVisualStyleBackColor = true;
      this.showStackTrace.Click += new System.EventHandler(this.showStackTrace_Click);
      // 
      // ok
      // 
      resources.ApplyResources(this.ok, "ok");
      this.ok.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.ok.Name = "ok";
      this.ok.UseVisualStyleBackColor = true;
      this.ok.Click += new System.EventHandler(this.ok_Click);
      // 
      // errorIcon
      // 
      resources.ApplyResources(this.errorIcon, "errorIcon");
      this.errorIcon.Name = "errorIcon";
      // 
      // errorMessage
      // 
      resources.ApplyResources(this.errorMessage, "errorMessage");
      this.errorMessage.Name = "errorMessage";
      // 
      // hideStackTrace
      // 
      resources.ApplyResources(this.hideStackTrace, "hideStackTrace");
      this.hideStackTrace.Name = "hideStackTrace";
      this.hideStackTrace.UseVisualStyleBackColor = true;
      this.hideStackTrace.Click += new System.EventHandler(this.hideStackTrace_Click);
      // 
      // ExceptionMessageBox
      // 
      this.AcceptButton = this.ok;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.ok;
      this.Controls.Add(this.hideStackTrace);
      this.Controls.Add(this.errorMessage);
      this.Controls.Add(this.errorIcon);
      this.Controls.Add(this.ok);
      this.Controls.Add(this.showStackTrace);
      this.Controls.Add(this.stackTrace);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ExceptionMessageBox";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Resize += new System.EventHandler(this.ExceptionMessageBox_Resize);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox stackTrace;
    private System.Windows.Forms.Button showStackTrace;
    private System.Windows.Forms.Button ok;
    private System.Windows.Forms.Label errorIcon;
    private System.Windows.Forms.Label errorMessage;
    private System.Windows.Forms.Button hideStackTrace;
  }
}