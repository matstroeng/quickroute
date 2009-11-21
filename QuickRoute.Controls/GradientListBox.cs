using System.Windows.Forms;
using QuickRoute.BusinessEntities;
using System.Drawing;

namespace QuickRoute.Controls
{
  public class GradientListBox : ListBox
  {
    private int nameWidth = 100;
    private int namePadding = 8;

    public GradientListBox()
    {
      DrawMode = DrawMode.OwnerDrawFixed;
      this.ItemHeight = 20;
    }

    public int NameWidth
    {
      get { return nameWidth; }
      set { nameWidth = value; }
    }

    public int NamePadding
    {
      get { return namePadding; }
      set { namePadding = value; }
    }

    protected override void OnDrawItem(DrawItemEventArgs ea)
    {
      if (ea.Index != -1 && Items.Count > 0)
      {
        Gradient item = (Gradient)Items[ea.Index];
        Bitmap backBufferBitmap = new Bitmap(ea.Bounds.Width, ea.Bounds.Height);
        Graphics backBufferGraphics = Graphics.FromImage(backBufferBitmap);


        backBufferGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        Brush b = new SolidBrush(BackColor);
        ea.Graphics.FillRectangle(b, ea.Bounds);
        b.Dispose();
        ea.DrawBackground();

        Rectangle rect = new Rectangle(4, 4, backBufferBitmap.Width - this.NameWidth-4, backBufferBitmap.Height-8);
        Gradient.FillCheckerboardRectangle(backBufferGraphics, rect, 3); 
        item.Draw(backBufferGraphics, rect, 0, 1, Gradient.Direction.Horizontal);
        backBufferGraphics.DrawRectangle(Pens.Black, new Rectangle(3, 3, backBufferBitmap.Width - this.NameWidth - 3, backBufferBitmap.Height - 7));

        Color textColor;
        if ((ea.State & DrawItemState.Selected).Equals(DrawItemState.Selected))
        {
          textColor = SystemColors.HighlightText;
        }
        else
        {
          textColor = this.ForeColor; 
        }

        Size textSize = TextRenderer.MeasureText(backBufferGraphics, item.Name, this.Font);
        Point textLocation = new Point(backBufferBitmap.Width - this.NameWidth + this.NamePadding, (ea.Bounds.Height - textSize.Height) / 2);
        TextRenderer.DrawText(backBufferGraphics, item.Name, this.Font, textLocation, textColor);

        ea.Graphics.DrawImage(backBufferBitmap, ea.Bounds.Location);

        backBufferBitmap.Dispose();
        backBufferGraphics.Dispose();

        ea.DrawFocusRectangle();

        base.OnDrawItem(ea);
      }
    }
  }

}
