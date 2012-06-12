namespace QuickRoute.UI.Forms
{
  partial class ColorRangeIntervalEditor
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorRangeIntervalEditor));
      this.intervalStartTextbox = new System.Windows.Forms.TextBox();
      this.intervalStartLabel = new System.Windows.Forms.Label();
      this.intervalEndLabel = new System.Windows.Forms.Label();
      this.intervalEndTextbox = new System.Windows.Forms.TextBox();
      this.ok = new System.Windows.Forms.Button();
      this.cancel = new System.Windows.Forms.Button();
      this.infoLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // intervalStartTextbox
      // 
      resources.ApplyResources(this.intervalStartTextbox, "intervalStartTextbox");
      this.intervalStartTextbox.Name = "intervalStartTextbox";
      this.intervalStartTextbox.TextChanged += new System.EventHandler(this.intervalStartTextbox_TextChanged);
      this.intervalStartTextbox.Enter += new System.EventHandler(this.IntervalStartTextbox_Enter);
      this.intervalStartTextbox.Leave += new System.EventHandler(this.IntervalStartTextbox_Leave);
      // 
      // intervalStartLabel
      // 
      resources.ApplyResources(this.intervalStartLabel, "intervalStartLabel");
      this.intervalStartLabel.Name = "intervalStartLabel";
      // 
      // intervalEndLabel
      // 
      resources.ApplyResources(this.intervalEndLabel, "intervalEndLabel");
      this.intervalEndLabel.Name = "intervalEndLabel";
      // 
      // intervalEndTextbox
      // 
      resources.ApplyResources(this.intervalEndTextbox, "intervalEndTextbox");
      this.intervalEndTextbox.Name = "intervalEndTextbox";
      this.intervalEndTextbox.TextChanged += new System.EventHandler(this.intervalEndTextbox_TextChanged);
      this.intervalEndTextbox.Enter += new System.EventHandler(this.IntervalEndTextbox_Enter);
      this.intervalEndTextbox.Leave += new System.EventHandler(this.IntervalEndTextbox_Leave);
      // 
      // ok
      // 
      resources.ApplyResources(this.ok, "ok");
      this.ok.Name = "ok";
      this.ok.UseVisualStyleBackColor = true;
      this.ok.Click += new System.EventHandler(this.OK_Click);
      // 
      // cancel
      // 
      resources.ApplyResources(this.cancel, "cancel");
      this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancel.Name = "cancel";
      this.cancel.UseVisualStyleBackColor = true;
      this.cancel.Click += new System.EventHandler(this.Cancel_Click);
      // 
      // infoLabel
      // 
      resources.ApplyResources(this.infoLabel, "infoLabel");
      this.infoLabel.BackColor = System.Drawing.SystemColors.Info;
      this.infoLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.infoLabel.Name = "infoLabel";
      // 
      // ColorRangeIntervalEditor
      // 
      this.AcceptButton = this.ok;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancel;
      this.Controls.Add(this.infoLabel);
      this.Controls.Add(this.cancel);
      this.Controls.Add(this.ok);
      this.Controls.Add(this.intervalEndLabel);
      this.Controls.Add(this.intervalEndTextbox);
      this.Controls.Add(this.intervalStartLabel);
      this.Controls.Add(this.intervalStartTextbox);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ColorRangeIntervalEditor";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox intervalStartTextbox;
    private System.Windows.Forms.Label intervalStartLabel;
    private System.Windows.Forms.Label intervalEndLabel;
    private System.Windows.Forms.TextBox intervalEndTextbox;
    private System.Windows.Forms.Button ok;
    private System.Windows.Forms.Button cancel;
    private System.Windows.Forms.Label infoLabel;
  }
}