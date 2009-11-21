namespace QuickRoute.Controls.Forms
{
  partial class RouteCutForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RouteCutForm));
      this.cancel = new System.Windows.Forms.Button();
      this.ok = new System.Windows.Forms.Button();
      this.timeLabel = new System.Windows.Forms.Label();
      this.timeTextbox = new System.Windows.Forms.TextBox();
      this.cutBefore = new System.Windows.Forms.RadioButton();
      this.cutAfter = new System.Windows.Forms.RadioButton();
      this.SuspendLayout();
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
      this.cancel.Click += new System.EventHandler(this.Cancel_Click);
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
      this.ok.Click += new System.EventHandler(this.OK_Click);
      // 
      // timeLabel
      // 
      this.timeLabel.AccessibleDescription = null;
      this.timeLabel.AccessibleName = null;
      resources.ApplyResources(this.timeLabel, "timeLabel");
      this.timeLabel.Font = null;
      this.timeLabel.Name = "timeLabel";
      // 
      // timeTextbox
      // 
      this.timeTextbox.AccessibleDescription = null;
      this.timeTextbox.AccessibleName = null;
      resources.ApplyResources(this.timeTextbox, "timeTextbox");
      this.timeTextbox.BackgroundImage = null;
      this.timeTextbox.Font = null;
      this.timeTextbox.Name = "timeTextbox";
      this.timeTextbox.Leave += new System.EventHandler(this.TimeTextbox_Leave);
      this.timeTextbox.Enter += new System.EventHandler(this.TimeTextbox_Enter);
      // 
      // cutBefore
      // 
      this.cutBefore.AccessibleDescription = null;
      this.cutBefore.AccessibleName = null;
      resources.ApplyResources(this.cutBefore, "cutBefore");
      this.cutBefore.BackgroundImage = null;
      this.cutBefore.Font = null;
      this.cutBefore.Name = "cutBefore";
      this.cutBefore.TabStop = true;
      this.cutBefore.UseVisualStyleBackColor = true;
      // 
      // cutAfter
      // 
      this.cutAfter.AccessibleDescription = null;
      this.cutAfter.AccessibleName = null;
      resources.ApplyResources(this.cutAfter, "cutAfter");
      this.cutAfter.BackgroundImage = null;
      this.cutAfter.Font = null;
      this.cutAfter.Name = "cutAfter";
      this.cutAfter.TabStop = true;
      this.cutAfter.UseVisualStyleBackColor = true;
      // 
      // RouteCutForm
      // 
      this.AcceptButton = this.ok;
      this.AccessibleDescription = null;
      this.AccessibleName = null;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = null;
      this.CancelButton = this.cancel;
      this.Controls.Add(this.cutAfter);
      this.Controls.Add(this.cutBefore);
      this.Controls.Add(this.timeTextbox);
      this.Controls.Add(this.timeLabel);
      this.Controls.Add(this.cancel);
      this.Controls.Add(this.ok);
      this.Font = null;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Icon = null;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "RouteCutForm";
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
    private System.Windows.Forms.RadioButton cutBefore;
    private System.Windows.Forms.RadioButton cutAfter;

  }
}