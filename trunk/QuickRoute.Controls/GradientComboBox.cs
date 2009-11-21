using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;
using System.Drawing;

namespace QuickRoute.Controls
{
  public class GradientComboBox : ComboBox
  {

    public GradientComboBox()
    {
      DrawMode = DrawMode.OwnerDrawFixed;
    }

    protected override void OnDrawItem(DrawItemEventArgs ea)
    {
      if (ea.Index != -1)
      {
        GradientListItem item = (GradientListItem)Items[ea.Index];
        Brush b = new SolidBrush(BackColor);
        ea.Graphics.FillRectangle(b, ea.Bounds);
        b.Dispose();

        Bitmap backBufferBitmap = new Bitmap(ea.Bounds.Width, ea.Bounds.Height);
        Graphics backBufferGraphics = Graphics.FromImage(backBufferBitmap);

        item.Gradient.Draw(backBufferGraphics, new Rectangle(0,0,backBufferBitmap.Width,backBufferBitmap.Height), 0, 1, Gradient.Direction.Horizontal);

        ea.Graphics.DrawImage(backBufferBitmap, ea.Bounds.Location);

        backBufferBitmap.Dispose();
        backBufferGraphics.Dispose();

        ea.DrawFocusRectangle();

        base.OnDrawItem(ea);
      }
    }
  }

  public class GradientListItem
  {
    private Gradient gradient;
    private string fileName;

    public GradientListItem(Gradient gradient, string fileName)
    {
      this.gradient = gradient;
      this.fileName = fileName;
    }

    public Gradient Gradient
    {
      get { return gradient; }
      set { gradient = value; }
    }

    public string FileName
    {
      get { return fileName; }
      set { fileName = value; }
    }
  }
}
