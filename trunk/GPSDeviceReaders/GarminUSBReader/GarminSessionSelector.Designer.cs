namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
  partial class GarminSessionSelector
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GarminSessionSelector));
        this.ok = new System.Windows.Forms.Button();
        this.activityLabel = new System.Windows.Forms.Label();
        this.sessionsComboBox = new System.Windows.Forms.ComboBox();
        this.cancel = new System.Windows.Forms.Button();
        this.readGPSUnit = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // ok
        // 
        resources.ApplyResources(this.ok, "ok");
        this.ok.Name = "ok";
        this.ok.UseVisualStyleBackColor = true;
        this.ok.Click += new System.EventHandler(this.ok_Click);
        // 
        // activityLabel
        // 
        resources.ApplyResources(this.activityLabel, "activityLabel");
        this.activityLabel.Name = "activityLabel";
        // 
        // sessionsComboBox
        // 
        this.sessionsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.sessionsComboBox.FormattingEnabled = true;
        resources.ApplyResources(this.sessionsComboBox, "sessionsComboBox");
        this.sessionsComboBox.Name = "sessionsComboBox";
        // 
        // cancel
        // 
        this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        resources.ApplyResources(this.cancel, "cancel");
        this.cancel.Name = "cancel";
        this.cancel.UseVisualStyleBackColor = true;
        this.cancel.Click += new System.EventHandler(this.cancel_Click);
        // 
        // readGPSUnit
        // 
        resources.ApplyResources(this.readGPSUnit, "readGPSUnit");
        this.readGPSUnit.Name = "readGPSUnit";
        this.readGPSUnit.UseVisualStyleBackColor = true;
        this.readGPSUnit.Click += new System.EventHandler(this.readGPSUnit_Click);
        // 
        // GarminSessionSelector
        // 
        this.AcceptButton = this.ok;
        resources.ApplyResources(this, "$this");
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.CancelButton = this.cancel;
        this.Controls.Add(this.readGPSUnit);
        this.Controls.Add(this.cancel);
        this.Controls.Add(this.sessionsComboBox);
        this.Controls.Add(this.activityLabel);
        this.Controls.Add(this.ok);
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "GarminSessionSelector";
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
    private System.Windows.Forms.Button readGPSUnit;

  }
}