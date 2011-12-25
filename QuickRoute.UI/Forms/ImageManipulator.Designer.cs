namespace QuickRoute.UI.Forms
{
  partial class ImageManipulator
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageManipulator));
      this.ToolStrip = new System.Windows.Forms.ToolStrip();
      this.RotateCounterclockwise = new System.Windows.Forms.ToolStripButton();
      this.RotateClockwise = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.SizeLabel = new System.Windows.Forms.ToolStripLabel();
      this.ScaleTextbox = new System.Windows.Forms.ToolStripTextBox();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.SizeInPixelsLabel = new System.Windows.Forms.ToolStripLabel();
      this.ScrollY = new System.Windows.Forms.VScrollBar();
      this.ScrollX = new System.Windows.Forms.HScrollBar();
      this.Cancel = new System.Windows.Forms.Button();
      this.OK = new System.Windows.Forms.Button();
      this.ToolStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // ToolStrip
      // 
      resources.ApplyResources(this.ToolStrip, "ToolStrip");
      this.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RotateCounterclockwise,
            this.RotateClockwise,
            this.toolStripSeparator1,
            this.SizeLabel,
            this.ScaleTextbox,
            this.toolStripSeparator2,
            this.SizeInPixelsLabel});
      this.ToolStrip.Name = "ToolStrip";
      // 
      // RotateCounterclockwise
      // 
      resources.ApplyResources(this.RotateCounterclockwise, "RotateCounterclockwise");
      this.RotateCounterclockwise.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.RotateCounterclockwise.Name = "RotateCounterclockwise";
      this.RotateCounterclockwise.Click += new System.EventHandler(this.RotateCounterclockwise_Click);
      // 
      // RotateClockwise
      // 
      resources.ApplyResources(this.RotateClockwise, "RotateClockwise");
      this.RotateClockwise.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.RotateClockwise.Name = "RotateClockwise";
      this.RotateClockwise.Click += new System.EventHandler(this.RotateClockwise_Click);
      // 
      // toolStripSeparator1
      // 
      resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      // 
      // SizeLabel
      // 
      resources.ApplyResources(this.SizeLabel, "SizeLabel");
      this.SizeLabel.Name = "SizeLabel";
      // 
      // ScaleTextbox
      // 
      resources.ApplyResources(this.ScaleTextbox, "ScaleTextbox");
      this.ScaleTextbox.Name = "ScaleTextbox";
      this.ScaleTextbox.Enter += new System.EventHandler(this.SizeInPercent_Enter);
      this.ScaleTextbox.Leave += new System.EventHandler(this.SizeInPercent_Leave);
      // 
      // toolStripSeparator2
      // 
      resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      // 
      // SizeInPixelsLabel
      // 
      resources.ApplyResources(this.SizeInPixelsLabel, "SizeInPixelsLabel");
      this.SizeInPixelsLabel.Name = "SizeInPixelsLabel";
      // 
      // ScrollY
      // 
      resources.ApplyResources(this.ScrollY, "ScrollY");
      this.ScrollY.Cursor = System.Windows.Forms.Cursors.Default;
      this.ScrollY.LargeChange = 100;
      this.ScrollY.Name = "ScrollY";
      this.ScrollY.SmallChange = 32;
      this.ScrollY.ValueChanged += new System.EventHandler(this.ScrollY_ValueChanged);
      // 
      // ScrollX
      // 
      resources.ApplyResources(this.ScrollX, "ScrollX");
      this.ScrollX.Cursor = System.Windows.Forms.Cursors.Default;
      this.ScrollX.LargeChange = 100;
      this.ScrollX.Name = "ScrollX";
      this.ScrollX.SmallChange = 32;
      this.ScrollX.ValueChanged += new System.EventHandler(this.ScrollX_ValueChanged);
      // 
      // Cancel
      // 
      resources.ApplyResources(this.Cancel, "Cancel");
      this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.Cancel.Name = "Cancel";
      this.Cancel.UseVisualStyleBackColor = true;
      this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
      // 
      // OK
      // 
      resources.ApplyResources(this.OK, "OK");
      this.OK.Name = "OK";
      this.OK.UseVisualStyleBackColor = true;
      this.OK.Click += new System.EventHandler(this.OK_Click);
      // 
      // ImageManipulator
      // 
      this.AcceptButton = this.OK;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.Cancel;
      this.Controls.Add(this.OK);
      this.Controls.Add(this.Cancel);
      this.Controls.Add(this.ToolStrip);
      this.Controls.Add(this.ScrollY);
      this.Controls.Add(this.ScrollX);
      this.Cursor = System.Windows.Forms.Cursors.Cross;
      this.KeyPreview = true;
      this.Name = "ImageManipulator";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Load += new System.EventHandler(this.ImageManipulator_Load);
      this.Paint += new System.Windows.Forms.PaintEventHandler(this.ImageManipulator_Paint);
      this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageManipulator_MouseDown);
      this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ImageManipulator_MouseMove);
      this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ImageManipulator_MouseUp);
      this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.ImageManipulator_MouseWheel);
      this.Resize += new System.EventHandler(this.ImageManipulator_Resize);
      this.ToolStrip.ResumeLayout(false);
      this.ToolStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ToolStrip ToolStrip;
    private System.Windows.Forms.VScrollBar ScrollY;
    private System.Windows.Forms.HScrollBar ScrollX;
    private System.Windows.Forms.ToolStripButton RotateCounterclockwise;
    private System.Windows.Forms.ToolStripButton RotateClockwise;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripLabel SizeLabel;
    private System.Windows.Forms.ToolStripTextBox ScaleTextbox;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripLabel SizeInPixelsLabel;
    private System.Windows.Forms.Button Cancel;
    private System.Windows.Forms.Button OK;
  }
}

