namespace QuickRoute.Controls.Forms
{
  partial class LapTimeForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LapTimeForm));
      this.cancel = new System.Windows.Forms.Button();
      this.ok = new System.Windows.Forms.Button();
      this.timeLabel = new System.Windows.Forms.Label();
      this.timeTextbox = new System.Windows.Forms.TextBox();
      this.timeTypeLabel = new System.Windows.Forms.Label();
      this.timeStyle = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // cancel
      // 
      resources.ApplyResources(this.cancel, "cancel");
      this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancel.Name = "cancel";
      this.cancel.UseVisualStyleBackColor = true;
      this.cancel.Click += new System.EventHandler(this.Cancel_Click);
      // 
      // ok
      // 
      resources.ApplyResources(this.ok, "ok");
      this.ok.Name = "ok";
      this.ok.UseVisualStyleBackColor = true;
      this.ok.Click += new System.EventHandler(this.OK_Click);
      // 
      // timeLabel
      // 
      resources.ApplyResources(this.timeLabel, "timeLabel");
      this.timeLabel.Name = "timeLabel";
      // 
      // timeTextbox
      // 
      resources.ApplyResources(this.timeTextbox, "timeTextbox");
      this.timeTextbox.Name = "timeTextbox";
      this.timeTextbox.Leave += new System.EventHandler(this.TimeTextbox_Leave);
      this.timeTextbox.Enter += new System.EventHandler(this.TimeTextbox_Enter);
      // 
      // timeTypeLabel
      // 
      resources.ApplyResources(this.timeTypeLabel, "timeTypeLabel");
      this.timeTypeLabel.Name = "timeTypeLabel";
      // 
      // timeStyle
      // 
      resources.ApplyResources(this.timeStyle, "timeStyle");
      this.timeStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.timeStyle.FormattingEnabled = true;
      this.timeStyle.Items.AddRange(new object[] {
            resources.GetString("timeStyle.Items"),
            resources.GetString("timeStyle.Items1")});
      this.timeStyle.Name = "timeStyle";
      // 
      // LapTimeForm
      // 
      this.AcceptButton = this.ok;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancel;
      this.Controls.Add(this.timeStyle);
      this.Controls.Add(this.timeTypeLabel);
      this.Controls.Add(this.timeTextbox);
      this.Controls.Add(this.timeLabel);
      this.Controls.Add(this.cancel);
      this.Controls.Add(this.ok);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "LapTimeForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button cancel;
    private System.Windows.Forms.Button ok;
    private System.Windows.Forms.Label timeLabel;
    private System.Windows.Forms.TextBox timeTextbox;
    private System.Windows.Forms.Label timeTypeLabel;
    private System.Windows.Forms.ComboBox timeStyle;

  }
}