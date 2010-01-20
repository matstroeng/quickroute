namespace QuickRoute.UI.Forms
{
  partial class SelectUICultureForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectUICultureForm));
      this.uiCultureDropdown = new System.Windows.Forms.ComboBox();
      this.ok = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // uiCultureDropdown
      // 
      this.uiCultureDropdown.AccessibleDescription = null;
      this.uiCultureDropdown.AccessibleName = null;
      resources.ApplyResources(this.uiCultureDropdown, "uiCultureDropdown");
      this.uiCultureDropdown.BackgroundImage = null;
      this.uiCultureDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.uiCultureDropdown.Font = null;
      this.uiCultureDropdown.FormattingEnabled = true;
      this.uiCultureDropdown.Name = "uiCultureDropdown";
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
      // SelectUICultureForm
      // 
      this.AcceptButton = this.ok;
      this.AccessibleDescription = null;
      this.AccessibleName = null;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = null;
      this.Controls.Add(this.ok);
      this.Controls.Add(this.uiCultureDropdown);
      this.Font = null;
      this.Icon = null;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "SelectUICultureForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Load += new System.EventHandler(this.SelectUICultureForm_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ComboBox uiCultureDropdown;
    private System.Windows.Forms.Button ok;
  }
}