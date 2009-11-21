namespace QuickRoute.BusinessEntities.Importers.Polar.ProTrainer
{
  partial class PersonSessionSelector
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PersonSessionSelector));
      this.ok = new System.Windows.Forms.Button();
      this.personLabel = new System.Windows.Forms.Label();
      this.personsComboBox = new System.Windows.Forms.ComboBox();
      this.cancel = new System.Windows.Forms.Button();
      this.sessionsComboBox = new System.Windows.Forms.ComboBox();
      this.sessionLabel = new System.Windows.Forms.Label();
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
      // personLabel
      // 
      this.personLabel.AccessibleDescription = null;
      this.personLabel.AccessibleName = null;
      resources.ApplyResources(this.personLabel, "personLabel");
      this.personLabel.Font = null;
      this.personLabel.Name = "personLabel";
      // 
      // personsComboBox
      // 
      this.personsComboBox.AccessibleDescription = null;
      this.personsComboBox.AccessibleName = null;
      resources.ApplyResources(this.personsComboBox, "personsComboBox");
      this.personsComboBox.BackgroundImage = null;
      this.personsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.personsComboBox.Font = null;
      this.personsComboBox.FormattingEnabled = true;
      this.personsComboBox.Name = "personsComboBox";
      this.personsComboBox.SelectedIndexChanged += new System.EventHandler(this.personsComboBox_SelectedIndexChanged);
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
      // sessionLabel
      // 
      this.sessionLabel.AccessibleDescription = null;
      this.sessionLabel.AccessibleName = null;
      resources.ApplyResources(this.sessionLabel, "sessionLabel");
      this.sessionLabel.Font = null;
      this.sessionLabel.Name = "sessionLabel";
      // 
      // PersonSessionSelector
      // 
      this.AcceptButton = this.ok;
      this.AccessibleDescription = null;
      this.AccessibleName = null;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = null;
      this.CancelButton = this.cancel;
      this.Controls.Add(this.sessionsComboBox);
      this.Controls.Add(this.sessionLabel);
      this.Controls.Add(this.cancel);
      this.Controls.Add(this.personsComboBox);
      this.Controls.Add(this.personLabel);
      this.Controls.Add(this.ok);
      this.Font = null;
      this.Icon = null;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "PersonSessionSelector";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button ok;
    private System.Windows.Forms.Label personLabel;
    private System.Windows.Forms.ComboBox personsComboBox;
    private System.Windows.Forms.Button cancel;
    private System.Windows.Forms.ComboBox sessionsComboBox;
    private System.Windows.Forms.Label sessionLabel;

  }
}