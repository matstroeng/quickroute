namespace QuickRoute.Controls
{
  partial class ColorRangeIntervalSlider
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.sliderPanel = new System.Windows.Forms.Panel();
      this.SuspendLayout();
      // 
      // sliderPanel
      // 
      this.sliderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sliderPanel.Location = new System.Drawing.Point(0, 0);
      this.sliderPanel.Name = "sliderPanel";
      this.sliderPanel.Size = new System.Drawing.Size(288, 34);
      this.sliderPanel.TabIndex = 0;
      this.sliderPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ColorRangeIntervalSlider_MouseDown);
      this.sliderPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ColorRangeIntervalSlider_MouseMove);
      this.sliderPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.SliderPanel_Paint);
      this.sliderPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ColorRangeIntervalSlider_MouseUp);
      // 
      // colorRangeIntervalSlider
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Font = new System.Drawing.Font("Calibri", 8F);
      this.Controls.Add(this.sliderPanel);
      this.Name = "colorRangeIntervalSlider";
      this.Size = new System.Drawing.Size(288, 34);
      this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ColorRangeIntervalSlider_MouseDown);
      this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ColorRangeIntervalSlider_MouseMove);
      this.Resize += new System.EventHandler(this.ColorRangeIntervalSlider_Resize);
      this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ColorRangeIntervalSlider_MouseUp);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel sliderPanel;
  }
}
