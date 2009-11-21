using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;


/* MinHeight
 * MaxHeight
 * 
 * 
*/

namespace QuickRoute.Controls
{
  public partial class ScrollableLabel : UserControl
  {
    private SizeF textSize;
    private int minHeight = 16;
    private int maxHeight = 32;
    private bool scrollbarVisible;
    private bool resizingNow;

    public ScrollableLabel()
    {
      InitializeComponent();
    }

    public override string Text
    {
      get
      {
        return base.Text;
      }
      set
      {
        base.Text = value;
        MeasureText();
        DrawText();
      }
    }

    public override bool AutoSize
    {
      get
      {
        return base.AutoSize;
      }
      set
      {
        base.AutoSize = value;
      }
    }

    public int MinHeight
    {
      get { return minHeight; }
      set { minHeight = value; }
    }

    public int MaxHeight
    {
      get { return maxHeight; }
      set { maxHeight = value; }
    }

    private void DrawText()
    {
      Graphics g = CreateGraphics();
      g.Clear(BackColor);
      g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
      g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
      RectangleF rect = new RectangleF(
        (float)Padding.Left,
        (float)(Padding.Top - (scrollbarVisible ? scrollbar.Value : 0)),
        textSize.Width,
        textSize.Height);
      Brush b = new SolidBrush(ForeColor);
      g.DrawString(Text, Font, b, rect);
      b.Dispose();
      g.Dispose();
    }

    private void MeasureText()
    {
      Graphics g = CreateGraphics();
      // measure the string
      // without scrollbar
      SizeF characterSize = g.MeasureString("A", Font);
      SizeF textSizeWithoutScrollbar = g.MeasureString(Text, Font, Width - Padding.Horizontal);
      // with scrollbar
      SizeF textSizeWithScrollbar = g.MeasureString(Text, Font, Width - Padding.Horizontal - scrollbar.Width);

      // do we need to use a scrollbar?
      scrollbarVisible = (int)textSizeWithoutScrollbar.Height > maxHeight;
      if (scrollbarVisible)
      {
        textSize = textSizeWithScrollbar;
      }
      else
      {
        textSize = textSizeWithoutScrollbar;
      }

      int proposedHeight = Padding.Vertical + (int)Math.Ceiling(textSize.Height);
      proposedHeight = Math.Min(Math.Max(proposedHeight, minHeight), maxHeight);
      resizingNow = true;
      Height = proposedHeight;
      resizingNow = false;

      if (scrollbarVisible)
      {
        scrollbar.Maximum = (int)textSizeWithScrollbar.Height;
        scrollbar.LargeChange = maxHeight;
        scrollbar.SmallChange = (int)characterSize.Height;
      }
      
      g.Dispose();

      scrollbar.Visible = scrollbarVisible;
      scrollbar.Left = Width - scrollbar.Width;
      scrollbar.Height = Height;
    }

    private void ScrollableLabel_Paint(object sender, PaintEventArgs e)
    {
      DrawText();
    }

    private void ScrollableLabel_Resize(object sender, EventArgs e)
    {
      if (!resizingNow)
      {
        MeasureText();
        DrawText();
      }
    }

    private void scrollbar_Scroll(object sender, ScrollEventArgs e)
    {
      DrawText();
    }

    private void scrollbar_ValueChanged(object sender, EventArgs e)
    {
      DrawText();
    }

    private void ScrollableLabel_LocationChanged(object sender, EventArgs e)
    {
      DrawText();
    }




  }

}
