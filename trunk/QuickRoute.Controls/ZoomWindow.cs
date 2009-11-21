using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;

namespace QuickRoute.Controls
{
  public partial class ZoomWindow : UserControl
  {
    private PointD center;
    private double zoom = 1;

    public PointD Center
    {
      get { return center; }
      set { center = value; }
    }

    public double Zoom
    {
      get { return zoom; }
      set { zoom = value; }
    }


    public ZoomWindow()
    {
      InitializeComponent();
    }

    public void Draw(Document document)
    {
      Image image = document.Map.Image;

      Graphics g = this.CreateGraphics();
      RectangleF destRect = new RectangleF(0F, 0F, (float)this.Width, (float)this.Height);
      RectangleF srcRect = new RectangleF(
        (float)(center.X - this.Width / zoom / 2),
        (float)(center.Y - this.Height / zoom / 2),
        (float)(this.Width / zoom), 
        (float)(this.Height / zoom));

      g.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
    }
  }
}
