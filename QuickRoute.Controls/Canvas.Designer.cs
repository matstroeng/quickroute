namespace QuickRoute.Controls
{
  partial class Canvas
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
      this.canvasPanel = new System.Windows.Forms.Panel();
      this.scrXPanel = new System.Windows.Forms.Panel();
      this.scrX = new System.Windows.Forms.HScrollBar();
      this.bottomRightPanel = new System.Windows.Forms.Panel();
      this.scrY = new System.Windows.Forms.VScrollBar();
      this.scrXPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // canvasPanel
      // 
      this.canvasPanel.BackColor = System.Drawing.SystemColors.AppWorkspace;
      this.canvasPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.canvasPanel.Location = new System.Drawing.Point(0, 0);
      this.canvasPanel.Name = "canvasPanel";
      this.canvasPanel.Size = new System.Drawing.Size(361, 216);
      this.canvasPanel.TabIndex = 7;
      this.canvasPanel.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.CanvasPanel_MouseWheel);
      this.canvasPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.CanvasPanel_Paint);
      this.canvasPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CanvasPanel_MouseMove);
      this.canvasPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CanvasPanel_MouseDown);
      this.canvasPanel.Resize += new System.EventHandler(this.CanvasPanel_Resize);
      this.canvasPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CanvasPanel_MouseUp);
      // 
      // scrXPanel
      // 
      this.scrXPanel.Controls.Add(this.scrX);
      this.scrXPanel.Controls.Add(this.bottomRightPanel);
      this.scrXPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.scrXPanel.Location = new System.Drawing.Point(0, 197);
      this.scrXPanel.Name = "scrXPanel";
      this.scrXPanel.Size = new System.Drawing.Size(361, 19);
      this.scrXPanel.TabIndex = 8;
      // 
      // scrX
      // 
      this.scrX.Dock = System.Windows.Forms.DockStyle.Fill;
      this.scrX.Location = new System.Drawing.Point(0, 0);
      this.scrX.Name = "scrX";
      this.scrX.Size = new System.Drawing.Size(342, 19);
      this.scrX.SmallChange = 10;
      this.scrX.TabIndex = 11;
      this.scrX.ValueChanged += new System.EventHandler(this.Scrollbar_ValueChanged);
      this.scrX.Scroll += new System.Windows.Forms.ScrollEventHandler(this.Scrollbar_Scroll);
      // 
      // bottomRightPanel
      // 
      this.bottomRightPanel.BackColor = System.Drawing.SystemColors.Control;
      this.bottomRightPanel.Dock = System.Windows.Forms.DockStyle.Right;
      this.bottomRightPanel.Location = new System.Drawing.Point(342, 0);
      this.bottomRightPanel.Name = "bottomRightPanel";
      this.bottomRightPanel.Size = new System.Drawing.Size(19, 19);
      this.bottomRightPanel.TabIndex = 10;
      // 
      // scrY
      // 
      this.scrY.Dock = System.Windows.Forms.DockStyle.Right;
      this.scrY.Location = new System.Drawing.Point(342, 0);
      this.scrY.Name = "scrY";
      this.scrY.Size = new System.Drawing.Size(19, 197);
      this.scrY.SmallChange = 10;
      this.scrY.TabIndex = 9;
      this.scrY.ValueChanged += new System.EventHandler(this.Scrollbar_ValueChanged);
      this.scrY.Scroll += new System.Windows.Forms.ScrollEventHandler(this.Scrollbar_Scroll);
      // 
      // Canvas
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.AppWorkspace;
      this.Controls.Add(this.scrY);
      this.Controls.Add(this.scrXPanel);
      this.Controls.Add(this.canvasPanel);
      this.Name = "Canvas";
      this.Size = new System.Drawing.Size(361, 216);
      this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.CanvasPanel_MouseWheel);
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Canvas_KeyDown);
      this.scrXPanel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel canvasPanel;
    private System.Windows.Forms.Panel scrXPanel;
    private System.Windows.Forms.HScrollBar scrX;
    private System.Windows.Forms.Panel bottomRightPanel;
    private System.Windows.Forms.VScrollBar scrY;
  }
}
