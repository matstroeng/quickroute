using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using QuickRoute.Resources;

namespace QuickRoute.UI.Forms
{
  public partial class ImageManipulator : Form
  {
    public Image OriginalImage { get; private set; }
    public Image TransformedImage { get; private set; }

    private bool preventDraw = true;

    private double scale;
    private int rotation; // 90 degree steps
    
    // corners are the pixels just outside the cropping, in image coordinates
    private Point cropRectangleCorner1;
    private Point cropRectangleCorner2;

    private const int borderWidth = 1;
    private const int closeThreshold = 12;

    private Graphics backBufferGraphics;
    private Image backBufferImage;
    private Graphics canvasGraphics;

    // inclusive borders
    private int canvasWidth;
    private int canvasHeight;
    private int canvasX;
    private int canvasY;

    private MouseDownMode? mouseDownMode = null;
    private List<CroppingBorder> croppingBordersToChange;
    private Point moveCroppingOrigo;
    private Point moveCroppingCorner1;
    private Point moveCroppingCorner2;
    private Point scrollImageOrigo;
    private Point scrollImageOriginalScrollValue;

    private readonly SolidBrush cropBrush;
    private readonly Pen cropPen;
    private readonly Brush borderBrush;

    public ImageManipulator(Image image) : this(image, 1, 0, null)
    {
    }

    public ImageManipulator(Image image, double scale, double rotationInDegrees, Rectangle? cropRectangle)
    {
      borderBrush = new SolidBrush(Color.Black);
      cropBrush = new SolidBrush(Color.FromArgb(128, Color.Black));
      cropPen = new Pen(Color.Red);
      OriginalImage = image;

      this.scale = scale;
      rotation = (int)Math.Round(rotationInDegrees/90);
      TransformImage();

      if(cropRectangle != null)
      {
        cropRectangleCorner1 = new Point(cropRectangle.Value.Left - 1, cropRectangle.Value.Top - 1);
        cropRectangleCorner2 = new Point(cropRectangle.Value.Right, cropRectangle.Value.Bottom);
      }
      else
      {
        CroppingActive = false;
      }


      InitializeComponent();

      FormatScaleTextboxValue();
      UpdateSizeInPixelsLabel();
    }

    ~ImageManipulator()
    {
      borderBrush.Dispose();
      cropBrush.Dispose();
      cropPen.Dispose();
    }

    #region Event handlers

    private void ImageManipulator_Load(object sender, EventArgs e)
    {
      preventDraw = false;
      CreateCanvas();
      Draw();
    }
   
    private void ImageManipulator_Resize(object sender, EventArgs e)
    {
      CreateCanvas();
    }

    private void ImageManipulator_Paint(object sender, PaintEventArgs e)
    {
      Draw();
    }

    private void ImageManipulator_MouseDown(object sender, MouseEventArgs e)
    {
      var imageLocation = ToImageLocation(e.Location);
      if (e.Button == MouseButtons.Left)
      {
        var closeBorders = GetCloseCroppingBorders(imageLocation);
        if (closeBorders.Count > 0)
        {
          mouseDownMode = MouseDownMode.ResizeCropping;
          croppingBordersToChange = closeBorders;
        }
        else if(IsInsideCroppingRectangle(e.Location))
        {
          mouseDownMode = MouseDownMode.MoveCropping;
          moveCroppingOrigo = imageLocation;
          moveCroppingCorner1 = cropRectangleCorner1;
          moveCroppingCorner2 = cropRectangleCorner2;
        }
        else
        {
          mouseDownMode = MouseDownMode.CreateCropping;
          cropRectangleCorner1 = imageLocation;
          cropRectangleCorner2 = imageLocation;
        }
        Cursor = GetCursorFromMouseLocation(e.Location, closeBorders);
        UpdateSizeInPixelsLabel();
      }
      else if (e.Button == MouseButtons.Right)
      {
        mouseDownMode = MouseDownMode.ScrollImage;
        scrollImageOrigo = e.Location;
        scrollImageOriginalScrollValue = new Point(ScrollX.Value, ScrollY.Value);
        Cursor = Cursors.Hand;
      }
    }

    private void ImageManipulator_MouseUp(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        UpdateCornerLocation(e.Location);

        NormalizeCropRectangle();
        if (cropRectangleCorner1.X >= cropRectangleCorner2.X - 1 || cropRectangleCorner1.Y >= cropRectangleCorner2.Y - 1) CroppingActive = false;
        UpdateSizeInPixelsLabel();
      }
      else if(e.Button == MouseButtons.Right)
      {
        var imageLocation = ToImageLocation(e.Location);
        var closeBorders = GetCloseCroppingBorders(imageLocation);
        Cursor = GetCursorFromMouseLocation(e.Location, closeBorders);
      }
      mouseDownMode = null;
      Draw();
    }

    private void ImageManipulator_MouseMove(object sender, MouseEventArgs e)
    {
      if (mouseDownMode != null)
      {
        switch(mouseDownMode.Value)
        {
          case MouseDownMode.CreateCropping:
          case MouseDownMode.MoveCropping:
          case MouseDownMode.ResizeCropping:
            UpdateCornerLocation(e.Location);
            if (!IsInsideCanvas(e.Location))
            {
              // scroll if outside canvas
              var x = ScrollX.Value;
              var y = ScrollY.Value;
              if (e.X < canvasX) x -= ScrollX.SmallChange;
              if (e.Y < canvasY) y -= ScrollY.SmallChange;
              if (e.X > canvasX + canvasWidth) x += ScrollX.SmallChange;
              if (e.Y > canvasY + canvasHeight) y += ScrollY.SmallChange;
              ScrollX.Value = Math.Min(ScrollX.Maximum - ScrollX.LargeChange, Math.Max(ScrollX.Minimum, x));
              ScrollY.Value = Math.Min(ScrollY.Maximum - ScrollY.LargeChange, Math.Max(ScrollY.Minimum, y));
            }
            UpdateSizeInPixelsLabel();
            break;

          case MouseDownMode.ScrollImage:
            preventDraw = true;
            var scrollX = scrollImageOriginalScrollValue.X + scrollImageOrigo.X - e.Location.X;
            var scrollY = scrollImageOriginalScrollValue.Y + scrollImageOrigo.Y - e.Location.Y;
            ScrollX.Value = Math.Min(Math.Max(scrollX, 0), ScrollX.Maximum - ScrollX.LargeChange);
            ScrollY.Value = Math.Min(Math.Max(scrollY, 0), ScrollY.Maximum - ScrollY.LargeChange);
            preventDraw = false;
            break;
        }

        Draw();
      }
      else
      {
        var closeBorders = GetCloseCroppingBorders(ToImageLocation(e.Location));
        Cursor = GetCursorFromMouseLocation(e.Location, closeBorders);
      }
    }

    private void ImageManipulator_MouseWheel(object sender, MouseEventArgs e)
    {
      if(IsInsideCanvas(e.Location))
      {
        var units = e.Delta / 120;
        if ((ModifierKeys & Keys.Control) == Keys.Control)
        {
          // rescale
          ChangeScale(scale + units * 0.1);
          return;
        }

        if (ScrollY.Enabled && (ModifierKeys & Keys.Shift) != Keys.Shift)
        {
          ScrollY.Value = Math.Max(ScrollY.Minimum, Math.Min(ScrollY.Maximum - ScrollY.LargeChange, ScrollY.Value - units * ScrollY.SmallChange));
        }
        else if(ScrollX.Enabled)
        {
          ScrollX.Value = Math.Max(ScrollX.Minimum, Math.Min(ScrollX.Maximum - ScrollX.LargeChange, ScrollX.Value - units * ScrollX.SmallChange));
        }
      }
    }

    private void ScrollY_ValueChanged(object sender, EventArgs e)
    {
      Draw();
    }

    private void ScrollX_ValueChanged(object sender, EventArgs e)
    {
      Draw();
    }

    private void SizeInPercent_Leave(object sender, EventArgs e)
    {
      var text = ScaleTextbox.Text.Replace("%", "");
      double percent;
      if (double.TryParse(text, out percent))
      {
        ChangeScale(percent / 100);
      }
      FormatScaleTextboxValue();
    }

    private void SizeInPercent_Enter(object sender, EventArgs e)
    {
      ScaleTextbox.SelectAll();
    }

    private void RotateCounterclockwise_Click(object sender, EventArgs e)
    {
      ChangeRotation((rotation + 3) % 4); // to keep between 0 and 3
      TransformImage();
      CreateCanvas();
      Draw();
    }

    private void RotateClockwise_Click(object sender, EventArgs e)
    {
      ChangeRotation((rotation + 1) % 4); // to keep between 0 and 3
      TransformImage();
      CreateCanvas();
      Draw();
    }

    private void OK_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
      Close();
    }

    private void Cancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

    #endregion
  
    #region Public properties
  
    public Image ManipulatedImage
    {
      get
      {
        var croppingRectangle = ImageCropRectangle ?? new Rectangle(0, 0, TransformedImage.Width, TransformedImage.Height);
        var image = new Bitmap(croppingRectangle.Width, croppingRectangle.Height);
        using(var g = Graphics.FromImage(image))
        {
          g.DrawImage(TransformedImage, 0, 0, croppingRectangle, GraphicsUnit.Pixel);
        }
        return image;
      }
    }

    public double ImageRotationInDegrees
    {
      get { return 90 * rotation; }
    }

    public double ImageScale
    {
      get { return scale; }
    }

    public Rectangle? ImageCropRectangle
    {
      get
      {
        if(!CroppingActive) return null;
        var c1 = cropRectangleCorner1;
        var c2 = cropRectangleCorner2;
        c1.Offset(new Point(1, 1));
        //c2.Offset(new Point(-1, -1));
        return GetRectangleFromCorners(c1, c2);
      }
    }

    #endregion

    private void TransformImage()
    {
      // create the working copy of the image
      TransformedImage = new Bitmap((int)(scale * OriginalImage.Width), (int)(scale * OriginalImage.Height));

      // resize...
      using (var g = Graphics.FromImage(TransformedImage))
      {
        var destRect = new Rectangle(0, 0, TransformedImage.Width, TransformedImage.Height);
        var srcRect = new Rectangle(0, 0, OriginalImage.Width, OriginalImage.Height);
        g.DrawImage(OriginalImage, destRect, srcRect, GraphicsUnit.Pixel);
      }
      // ...and rotate
      switch (rotation % 4)
      {
        case 1:
          TransformedImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
          break;
        case 2:
          TransformedImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
          break;
        case 3:
          TransformedImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
          break;
      }
    }

    private void CreateCanvas()
    {
      canvasX = 0;
      canvasY = ToolStrip.Bottom + 1;
      canvasWidth = Math.Min(Math.Max(ScrollY.Left, 0), TransformedImage.Width + 2 * borderWidth);
      canvasHeight = Math.Min(Math.Max(ScrollX.Top - ToolStrip.Bottom - 1, 0), TransformedImage.Height + 2 * borderWidth);

      // create back buffer
      if (backBufferImage != null) backBufferImage.Dispose();
      backBufferImage = new Bitmap(Math.Min(TransformedImage.Width + 2 * borderWidth, canvasWidth), Math.Min(TransformedImage.Height + 2 * borderWidth, canvasHeight));
      if (backBufferGraphics != null) backBufferGraphics.Dispose();
      backBufferGraphics = Graphics.FromImage(backBufferImage);
      if (canvasGraphics != null) canvasGraphics.Dispose();
      canvasGraphics = CreateGraphics();
      canvasGraphics.Clear(BackColor);

      // setup scrollbars
      ScrollX.Enabled = canvasWidth - 2 * borderWidth < TransformedImage.Width;
      ScrollY.Enabled = canvasHeight - 2 * borderWidth < TransformedImage.Height;

      ScrollX.Maximum = TransformedImage.Width;
      ScrollY.Maximum = TransformedImage.Height;
      ScrollX.LargeChange = canvasWidth - 2 * borderWidth;
      ScrollY.LargeChange = canvasHeight - 2 * borderWidth;
      preventDraw = true;
      if (ScrollX.Value + ScrollX.LargeChange > ScrollX.Maximum) ScrollX.Value = ScrollX.Maximum - ScrollX.LargeChange;
      if (ScrollY.Value + ScrollY.LargeChange > ScrollY.Maximum) ScrollY.Value = ScrollY.Maximum - ScrollY.LargeChange;
      preventDraw = false;
      ScrollX.SmallChange = 32;
      ScrollY.SmallChange = 32;

      if (!ScrollX.Enabled) ScrollX.Value = 0;
      if (!ScrollY.Enabled) ScrollY.Value = 0;
    }

    private void Draw()
    {
      if (preventDraw) return;

      // draw image
      var destRect = new Rectangle(borderWidth, borderWidth, Math.Min(TransformedImage.Width, canvasWidth - 2 * borderWidth), Math.Min(TransformedImage.Height, canvasHeight - 2 * borderWidth));
      var srcRect = new Rectangle(ScrollX.Value, ScrollY.Value, destRect.Width, destRect.Height);
      backBufferGraphics.DrawImage(TransformedImage, destRect, srcRect, GraphicsUnit.Pixel);
      
      // draw border
      backBufferGraphics.FillRectangle(borderBrush, 0, 0, canvasWidth, borderWidth);
      backBufferGraphics.FillRectangle(borderBrush, 0, 0, borderWidth, canvasHeight);
      backBufferGraphics.FillRectangle(borderBrush, canvasWidth - borderWidth, 0, borderWidth, canvasHeight);
      backBufferGraphics.FillRectangle(borderBrush, 0, canvasHeight - borderWidth, canvasWidth, borderWidth);

      // draw cropping
      if (CroppingActive)
      {
        var r = GetRectangleFromCorners(ToCanvasLocation(cropRectangleCorner1), ToCanvasLocation(cropRectangleCorner2));
        backBufferGraphics.DrawRectangle(cropPen, r);
        backBufferGraphics.FillRectangle(cropBrush, borderWidth, borderWidth, canvasWidth - 2 * borderWidth, r.Top - borderWidth);
        backBufferGraphics.FillRectangle(cropBrush, borderWidth, r.Bottom + 1, canvasWidth - 2 * borderWidth, canvasHeight - r.Bottom - borderWidth);
        backBufferGraphics.FillRectangle(cropBrush, borderWidth, r.Top, r.Left - borderWidth, r.Height + 1);
        backBufferGraphics.FillRectangle(cropBrush, r.Right + 1, r.Top, canvasWidth - borderWidth - r.Right - 1, r.Height + 1);
      }

      // transfer back buffer to front
      canvasGraphics.DrawImageUnscaled(backBufferImage, canvasX, canvasY);

    }

    private void ChangeScale(double newScale)
    {
      newScale = Math.Min(1, Math.Max(0.1, newScale));
      var oldScale = scale;
      var factor = newScale / oldScale;
      scale = newScale;

      FormatScaleTextboxValue();

      if (factor == 1) return;
      
      var outsideAtLeft = cropRectangleCorner1.X == -1;
      var outsideAtRight = cropRectangleCorner2.X == TransformedImage.Width;
      var outsideAtTop = cropRectangleCorner1.Y == -1;
      var outsideAtBottom = cropRectangleCorner2.Y == TransformedImage.Height;

      cropRectangleCorner1 = new Point((int)(factor * cropRectangleCorner1.X), (int)(factor * cropRectangleCorner1.Y));
      cropRectangleCorner2 = new Point((int)(factor * cropRectangleCorner2.X), (int)(factor * cropRectangleCorner2.Y));

      if (outsideAtLeft) cropRectangleCorner1.X = -1;
      if (outsideAtRight) cropRectangleCorner2.X = (int)(newScale * OriginalImage.Width);
      if (outsideAtTop) cropRectangleCorner1.Y = -1;
      if (outsideAtBottom) cropRectangleCorner2.Y = (int)(newScale * OriginalImage.Height);

      UpdateSizeInPixelsLabel();

      TransformImage();
      CreateCanvas();
      Draw();
    }

    private void ChangeRotation(int newRotation)
    {
      var oldRotation = rotation;
      rotation = newRotation;
      var rotationChange = (newRotation - oldRotation + 4) % 4;

      var c1 = cropRectangleCorner1;
      var c2 = cropRectangleCorner2;
      var w = TransformedImage.Width - 1;
      var h = TransformedImage.Height - 1;
      switch(rotationChange)
      {
        case 1:
          cropRectangleCorner1 = new Point(h - c1.Y, c1.X);
          cropRectangleCorner2 = new Point(h - c2.Y, c2.X);
          break;
        case 2:
          cropRectangleCorner1 = new Point(w - c1.X, h - c1.Y);
          cropRectangleCorner2 = new Point(w - c2.X, h - c2.Y);
          break;
        case 3:
          cropRectangleCorner1 = new Point(c1.Y, w - c1.X);
          cropRectangleCorner2 = new Point(c2.Y, w - c2.X);
          break;
      }
      NormalizeCropRectangle();
      UpdateSizeInPixelsLabel();

      TransformImage();
      CreateCanvas();
      Draw();
    }

    private void UpdateCornerLocation(Point location)
    {
      if (mouseDownMode == null) return; // should never happen

      var imageLocation = ToImageLocation(location);
      switch (mouseDownMode.Value)
      {
        case MouseDownMode.CreateCropping:
          cropRectangleCorner2 = imageLocation;
          break;
        case MouseDownMode.ResizeCropping:
          if (croppingBordersToChange.Contains(CroppingBorder.X1)) cropRectangleCorner1.X = imageLocation.X;
          if (croppingBordersToChange.Contains(CroppingBorder.X2)) cropRectangleCorner2.X = imageLocation.X;
          if (croppingBordersToChange.Contains(CroppingBorder.Y1)) cropRectangleCorner1.Y = imageLocation.Y;
          if (croppingBordersToChange.Contains(CroppingBorder.Y2)) cropRectangleCorner2.Y = imageLocation.Y;
          break;
        case MouseDownMode.MoveCropping:
          var diff = new Point(imageLocation.X - moveCroppingOrigo.X, imageLocation.Y - moveCroppingOrigo.Y);
          if (moveCroppingCorner1.X + diff.X < -1) diff.X = -moveCroppingCorner1.X - 1;
          if (moveCroppingCorner2.X + diff.X > TransformedImage.Width) diff.X = TransformedImage.Width - moveCroppingCorner2.X;
          if (moveCroppingCorner1.Y + diff.Y < -1) diff.Y = -moveCroppingCorner1.Y - 1;
          if (moveCroppingCorner2.Y + diff.Y > TransformedImage.Height) diff.Y = TransformedImage.Height - moveCroppingCorner2.Y;
          cropRectangleCorner1 = moveCroppingCorner1;
          cropRectangleCorner1.Offset(diff);
          cropRectangleCorner2 = moveCroppingCorner2;
          cropRectangleCorner2.Offset(diff);
          break;
      }
    }

    private Cursor GetCursorFromMouseLocation(Point location, List<CroppingBorder> closeBorders)
    {
      if (!IsInsideCanvas(location))
      {
        // outside canvas
        return Cursors.Default;
      }
      
      // resize cursor?
      if (closeBorders.Count > 0)
      {
        if ((closeBorders.Contains(CroppingBorder.X1) && closeBorders.Contains(CroppingBorder.Y1)) ||
            (closeBorders.Contains(CroppingBorder.X2) && closeBorders.Contains(CroppingBorder.Y2))) return Cursors.SizeNWSE;
        if ((closeBorders.Contains(CroppingBorder.X1) && closeBorders.Contains(CroppingBorder.Y2)) ||
            (closeBorders.Contains(CroppingBorder.X2) && closeBorders.Contains(CroppingBorder.Y1))) return Cursors.SizeNESW;
        if (closeBorders.Contains(CroppingBorder.X1) || closeBorders.Contains(CroppingBorder.X2)) return Cursors.SizeWE;
        return Cursors.SizeNS;
      }

      if(IsInsideCroppingRectangle(location))
      {
        // inside the cropping rectangle
        return Cursors.SizeAll;
      }

      // not close to any border
      return Cursors.Cross;
    }

    private bool IsInsideCanvas(Point location)
    {
      return !(location.X < canvasX ||
               location.Y < canvasY ||
               location.X > canvasX + canvasWidth ||
               location.Y > canvasY + canvasHeight);
    }

    private bool IsInsideCroppingRectangle(Point location)
    {
      if (!CroppingActive) return false;
      var imageLocation = ToImageLocation(location);
      return !(imageLocation.X <= cropRectangleCorner1.X ||
               imageLocation.Y <= cropRectangleCorner1.Y ||
               imageLocation.X >= cropRectangleCorner2.X ||
               imageLocation.Y >= cropRectangleCorner2.Y);
    }

    private bool CroppingActive
    {
      get
      {
        return !(cropRectangleCorner1.X == -1 &&
                 cropRectangleCorner1.Y == -1 &&
                 cropRectangleCorner2.X == TransformedImage.Width &&
                 cropRectangleCorner2.Y == TransformedImage.Height);
      }
      set
      {
        if (!value)
        {
          cropRectangleCorner1 = new Point(-1, -1);
          cropRectangleCorner2 = new Point(TransformedImage.Width, TransformedImage.Height);
        }
      }
    }

    private int CropWidth
    {
      get { return Math.Max(0, Math.Abs(cropRectangleCorner2.X - cropRectangleCorner1.X) - 1); }
    }

    private int CropHeight
    {
      get { return Math.Max(0, Math.Abs(cropRectangleCorner2.Y - cropRectangleCorner1.Y) - 1); }
    }

    private List<CroppingBorder> GetCloseCroppingBorders(Point imageLocation)
    {
      var result = new List<CroppingBorder>();

      var x1Distance = Math.Abs(imageLocation.X - cropRectangleCorner1.X);
      var x2Distance = Math.Abs(imageLocation.X - cropRectangleCorner2.X);
      var y1Distance = Math.Abs(imageLocation.Y - cropRectangleCorner1.Y);
      var y2Distance = Math.Abs(imageLocation.Y - cropRectangleCorner2.Y);

      if (!(cropRectangleCorner1.Y - imageLocation.Y > closeThreshold || imageLocation.Y - cropRectangleCorner2.Y > closeThreshold))
      {
        if (x1Distance < closeThreshold && x1Distance <= x2Distance)
        {
          result.Add(CroppingBorder.X1);
        }
        else if (x2Distance < closeThreshold)
        {
          result.Add(CroppingBorder.X2);
        }
      }

      if (!(cropRectangleCorner1.X - imageLocation.X > closeThreshold || imageLocation.X - cropRectangleCorner2.X > closeThreshold))
      {
        if (y1Distance < closeThreshold && y1Distance <= y2Distance)
        {
          result.Add(CroppingBorder.Y1);
        }
        else if (y2Distance < closeThreshold)
        {
          result.Add(CroppingBorder.Y2);
        }
      }
      return result;
    }

    private static Rectangle GetRectangleFromCorners(Point c1, Point c2)
    {
      var topLeft = new Point(Math.Min(c1.X, c2.X), Math.Min(c1.Y, c2.Y));
      var bottomRight = new Point(Math.Max(c1.X, c2.X), Math.Max(c1.Y, c2.Y));
      return new Rectangle(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
    }

    private Point ToImageLocation(Point formLocation)
    {
      return new Point(Math.Min(Math.Max(-1, formLocation.X - canvasX - borderWidth + ScrollX.Value), TransformedImage.Width),
                       Math.Min(Math.Max(-1, formLocation.Y - canvasY - borderWidth + ScrollY.Value), TransformedImage.Height));
    }

    private Point ToCanvasLocation(Point imageLocation)
    {
      return new Point(imageLocation.X - ScrollX.Value + borderWidth,
                       imageLocation.Y - ScrollY.Value + borderWidth);
    }

    private void NormalizeCropRectangle()
    {
      var rect = GetRectangleFromCorners(cropRectangleCorner1, cropRectangleCorner2);
      cropRectangleCorner1 = new Point(rect.Left, rect.Top);
      cropRectangleCorner2 = new Point(rect.Right, rect.Bottom);
    }

    private void FormatScaleTextboxValue()
    {
      ScaleTextbox.Text = string.Format("{0:p0}", scale);
    }

    private void UpdateSizeInPixelsLabel()
    {
      SizeInPixelsLabel.Text = string.Format("{0} x {1} {2}", CropWidth, CropHeight, Strings.Pixels);
      Application.DoEvents();
    }

    #region Enums
    
    private enum CroppingBorder
    {
      X1,
      X2,
      Y1,
      Y2
    }

    private enum MouseDownMode
    {
      CreateCropping,
      ResizeCropping,
      MoveCropping,
      ScrollImage
    }

    #endregion
  }
}
