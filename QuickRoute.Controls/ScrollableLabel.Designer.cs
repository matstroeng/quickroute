namespace QuickRoute.Controls
{
  partial class ScrollableLabel
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
      this.scrollbar = new System.Windows.Forms.VScrollBar();
      this.SuspendLayout();
      // 
      // scrollbar
      // 
      this.scrollbar.Dock = System.Windows.Forms.DockStyle.Right;
      this.scrollbar.Location = new System.Drawing.Point(590, 0);
      this.scrollbar.Name = "scrollbar";
      this.scrollbar.Size = new System.Drawing.Size(19, 59);
      this.scrollbar.TabIndex = 0;
      this.scrollbar.ValueChanged += new System.EventHandler(this.scrollbar_ValueChanged);
      this.scrollbar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollbar_Scroll);
      // 
      // ScrollableLabel
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.scrollbar);
      this.Name = "ScrollableLabel";
      this.Size = new System.Drawing.Size(609, 59);
      this.LocationChanged += new System.EventHandler(this.ScrollableLabel_LocationChanged);
      this.Paint += new System.Windows.Forms.PaintEventHandler(this.ScrollableLabel_Paint);
      this.Resize += new System.EventHandler(this.ScrollableLabel_Resize);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.VScrollBar scrollbar;
  }
}
