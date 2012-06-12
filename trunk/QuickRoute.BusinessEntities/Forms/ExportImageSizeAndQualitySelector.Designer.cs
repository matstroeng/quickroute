namespace QuickRoute.BusinessEntities.Forms
{
  partial class ExportImageSizeAndQualitySelector
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportImageSizeAndQualitySelector));
      this.ok = new System.Windows.Forms.Button();
      this.cancel = new System.Windows.Forms.Button();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.imageQualityLabel = new System.Windows.Forms.Label();
      this.sizeLabel = new System.Windows.Forms.Label();
      this.panel1 = new System.Windows.Forms.Panel();
      this.sizeInPixels = new System.Windows.Forms.Label();
      this.percentualSizeComboBox = new System.Windows.Forms.ComboBox();
      this.imageQualityComboBox = new System.Windows.Forms.ComboBox();
      this.tableLayoutPanel1.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // ok
      // 
      resources.ApplyResources(this.ok, "ok");
      this.ok.Name = "ok";
      this.ok.UseVisualStyleBackColor = true;
      this.ok.Click += new System.EventHandler(this.ok_Click);
      // 
      // cancel
      // 
      resources.ApplyResources(this.cancel, "cancel");
      this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancel.Name = "cancel";
      this.cancel.UseVisualStyleBackColor = true;
      this.cancel.Click += new System.EventHandler(this.cancel_Click);
      // 
      // tableLayoutPanel1
      // 
      resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
      this.tableLayoutPanel1.Controls.Add(this.imageQualityLabel, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.sizeLabel, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.imageQualityComboBox, 1, 1);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      // 
      // imageQualityLabel
      // 
      resources.ApplyResources(this.imageQualityLabel, "imageQualityLabel");
      this.imageQualityLabel.Name = "imageQualityLabel";
      // 
      // sizeLabel
      // 
      resources.ApplyResources(this.sizeLabel, "sizeLabel");
      this.sizeLabel.Name = "sizeLabel";
      // 
      // panel1
      // 
      resources.ApplyResources(this.panel1, "panel1");
      this.panel1.Controls.Add(this.sizeInPixels);
      this.panel1.Controls.Add(this.percentualSizeComboBox);
      this.panel1.Name = "panel1";
      // 
      // sizeInPixels
      // 
      resources.ApplyResources(this.sizeInPixels, "sizeInPixels");
      this.sizeInPixels.Name = "sizeInPixels";
      // 
      // percentualSizeComboBox
      // 
      resources.ApplyResources(this.percentualSizeComboBox, "percentualSizeComboBox");
      this.percentualSizeComboBox.FormattingEnabled = true;
      this.percentualSizeComboBox.Items.AddRange(new object[] {
            resources.GetString("percentualSizeComboBox.Items"),
            resources.GetString("percentualSizeComboBox.Items1"),
            resources.GetString("percentualSizeComboBox.Items2"),
            resources.GetString("percentualSizeComboBox.Items3"),
            resources.GetString("percentualSizeComboBox.Items4"),
            resources.GetString("percentualSizeComboBox.Items5")});
      this.percentualSizeComboBox.Name = "percentualSizeComboBox";
      this.percentualSizeComboBox.TextChanged += new System.EventHandler(this.percentualSizeComboBox_TextChanged);
      this.percentualSizeComboBox.Leave += new System.EventHandler(this.percentualSizeComboBox_Leave);
      // 
      // imageQualityComboBox
      // 
      resources.ApplyResources(this.imageQualityComboBox, "imageQualityComboBox");
      this.imageQualityComboBox.FormattingEnabled = true;
      this.imageQualityComboBox.Items.AddRange(new object[] {
            resources.GetString("imageQualityComboBox.Items"),
            resources.GetString("imageQualityComboBox.Items1"),
            resources.GetString("imageQualityComboBox.Items2"),
            resources.GetString("imageQualityComboBox.Items3"),
            resources.GetString("imageQualityComboBox.Items4"),
            resources.GetString("imageQualityComboBox.Items5"),
            resources.GetString("imageQualityComboBox.Items6"),
            resources.GetString("imageQualityComboBox.Items7"),
            resources.GetString("imageQualityComboBox.Items8"),
            resources.GetString("imageQualityComboBox.Items9")});
      this.imageQualityComboBox.Name = "imageQualityComboBox";
      this.imageQualityComboBox.Leave += new System.EventHandler(this.imageQualityComboBox_Leave);
      // 
      // ExportImageSizeAndQualitySelector
      // 
      this.AcceptButton = this.ok;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancel;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Controls.Add(this.cancel);
      this.Controls.Add(this.ok);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ExportImageSizeAndQualitySelector";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.Button ok;
    protected System.Windows.Forms.Button cancel;
    protected System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    protected System.Windows.Forms.Label sizeLabel;
    protected System.Windows.Forms.ComboBox percentualSizeComboBox;
    protected System.Windows.Forms.Panel panel1;
    protected System.Windows.Forms.Label sizeInPixels;
    protected System.Windows.Forms.Label imageQualityLabel;
    protected System.Windows.Forms.ComboBox imageQualityComboBox;
  }
}