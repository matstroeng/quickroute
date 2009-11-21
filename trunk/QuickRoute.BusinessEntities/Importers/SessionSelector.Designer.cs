namespace QuickRoute.BusinessEntities.Importers
{
  partial class SessionSelector
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SessionSelector));
      this.ok = new System.Windows.Forms.Button();
      this.activityLabel = new System.Windows.Forms.Label();
      this.sessionsComboBox = new System.Windows.Forms.ComboBox();
      this.cancel = new System.Windows.Forms.Button();
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
      // activityLabel
      // 
      this.activityLabel.AccessibleDescription = null;
      this.activityLabel.AccessibleName = null;
      resources.ApplyResources(this.activityLabel, "activityLabel");
      this.activityLabel.Font = null;
      this.activityLabel.Name = "activityLabel";
      // 
      // sessionsComboBox
      // 
      this.sessionsComboBox.AccessibleDescription = null;
      this.sessionsComboBox.AccessibleName = null;
      resources.ApplyResources(this.sessionsComboBox, "sessionsComboBox");
      this.sessionsComboBox.BackgroundImage = null;
      this.sessionsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.sessionsComboBox.Font = null;
      this.sessionsComboBox.FormattingEnabled = true;
      this.sessionsComboBox.Name = "sessionsComboBox";
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
      // SessionSelector
      // 
      this.AcceptButton = this.ok;
      this.AccessibleDescription = null;
      this.AccessibleName = null;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = null;
      this.CancelButton = this.cancel;
      this.Controls.Add(this.cancel);
      this.Controls.Add(this.sessionsComboBox);
      this.Controls.Add(this.activityLabel);
      this.Controls.Add(this.ok);
      this.Font = null;
      this.Icon = null;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "SessionSelector";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button ok;
    private System.Windows.Forms.Label activityLabel;
    private System.Windows.Forms.ComboBox sessionsComboBox;
    private System.Windows.Forms.Button cancel;

  }
}