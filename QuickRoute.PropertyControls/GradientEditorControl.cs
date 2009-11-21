using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;
using System.Windows.Forms.Design;
using QuickRoute.Controls.Forms;

namespace QuickRoute.PropertyControls
{
  public partial class GradientEditorControl : UserControl
  {
    private Padding gradientPadding = new Padding(6, 1, 6, 17);
    private Size colorEntryMarkerSize = new Size(11, 16);
    private Rectangle gradientRectangle;
    private GradientColorEntry activeGradientColorEntry;
    private bool draggingGradientColorEntryNow = false;
    private Bitmap gradientPanelBackBuffer;
    private Gradient gradient;
    private IWindowsFormsEditorService editorService;

    public Gradient Gradient
    {
      get { return gradient; }
      set { gradient = value; }
    }

    public GradientEditorControl(Gradient gradient, IWindowsFormsEditorService editorService)
    {
      this.gradient = gradient;
      this.editorService = editorService;
      InitializeComponent();
    }

    #region Private methods

    private void CreateGradientRectangle()
    {
      gradientRectangle = new Rectangle(gradientPadding.Left, gradientPadding.Top, gradientPanel.Width - gradientPadding.Horizontal, gradientPanel.Height - gradientPadding.Vertical);
    }

    private void CreateGradientBackBuffer()
    {
      gradientPanelBackBuffer = new Bitmap(gradientPanel.Width, gradientPanel.Height);
    }

    private void DrawGradient()
    {
      Graphics graphics = Graphics.FromImage(gradientPanelBackBuffer);
      graphics.Clear(gradientPanel.BackColor);
      if (gradient != null)
      {
        Gradient.FillCheckerboardRectangle(graphics, gradientRectangle, 8);
        gradient.Draw(graphics, gradientRectangle, 0.0, 1.0, Gradient.Direction.Horizontal);
        graphics.DrawRectangle(Pens.Black, new Rectangle(gradientRectangle.Left - 1, gradientRectangle.Top - 1, gradientRectangle.Width + 2, gradientRectangle.Height + 1));

        DrawColorEntryMarkers();
      }
      graphics.Dispose();
    }

    private void DrawColorEntryMarkers()
    {
      Graphics graphics = Graphics.FromImage(gradientPanelBackBuffer);
      Brush b = new SolidBrush(gradientPanel.BackColor);
      graphics.FillRectangle(b, new Rectangle(0, gradientRectangle.Bottom + 1, gradientPanel.Width, gradientPanel.Height - gradientRectangle.Bottom));
      b.Dispose();

      int x;
      foreach (GradientColorEntry colorEntry in gradient.ColorEntries)
      {
        if (!colorEntry.Equals(activeGradientColorEntry))
        {
          x = gradientRectangle.Left + (int)(colorEntry.Location * gradientRectangle.Width);
          DrawColorEntryMarker(graphics, new Point(x, gradientRectangle.Bottom + 1), colorEntryMarkerSize, colorEntry.Color, Color.Black);
        }
      }
      if (activeGradientColorEntry != null)
      {
        x = gradientRectangle.Left + (int)(activeGradientColorEntry.Location * gradientRectangle.Width);
        DrawColorEntryMarker(graphics, new Point(x, gradientRectangle.Bottom + 1), colorEntryMarkerSize, activeGradientColorEntry.Color, Color.Blue);
      }
      graphics.Dispose();
    }

    private void CopyBackBufferToScreen()
    {
      // copy back buffer to screen
      gradientPanel.CreateGraphics().DrawImageUnscaled(gradientPanelBackBuffer, 0, 0);
    }

    private void DrawColorEntryMarker(Graphics graphics, Point point, Size size, Color color, Color borderColor)
    {
      Point[] markerPoints = new Point[5];
      markerPoints[0] = new Point(point.X, point.Y);
      markerPoints[1] = new Point(point.X + size.Width / 2, point.Y + size.Width / 2);
      markerPoints[2] = new Point(point.X + size.Width / 2, point.Y + size.Height - 1);
      markerPoints[3] = new Point(point.X - size.Width / 2, point.Y + size.Height - 1);
      markerPoints[4] = new Point(point.X - size.Width / 2, point.Y + size.Width / 2);

      Brush b = new SolidBrush(borderColor);
      graphics.FillPolygon(b, markerPoints);
      b.Dispose();
      Pen p = new Pen(borderColor);
      graphics.DrawPolygon(p, markerPoints);
      p.Dispose();

      Rectangle rect = new Rectangle(point.X - size.Width / 2 + 1, point.Y + size.Width / 2 + 1, size.Width - 2, size.Height - size.Width / 2 - 2);

      Gradient.FillCheckerboardRectangle(graphics, rect, rect.Width / 3);
      b = new SolidBrush(color);
      graphics.FillRectangle(b, rect);
      b.Dispose();
    }

    private GradientColorEntry GetGradientColorEntryFromLocation(int x)
    {
      int tmpX;
      foreach (GradientColorEntry colorEntry in gradient.ColorEntries)
      {
        tmpX = gradientRectangle.Left + (int)(colorEntry.Location * gradientRectangle.Width);
        if (Math.Abs(x - tmpX) <= colorEntryMarkerSize.Width / 2)
        {
          return colorEntry;
        }
      }
      return null;
    }

    private double GetColorEntryLocationFromX(int x)
    {
      return Math.Min(1, Math.Max(0, (double)(x - gradientRectangle.Left) / (gradientRectangle.Width)));
    }

    #endregion

    #region Event handlers

    private void gradientPanel_Resize(object sender, EventArgs e)
    {
      CreateGradientRectangle();
      CreateGradientBackBuffer();
      DrawGradient();
    }

    private void gradientPanel_Paint(object sender, PaintEventArgs e)
    {
      CopyBackBufferToScreen();
    }

    private void gradientPanel_MouseMove(object sender, MouseEventArgs e)
    {
      if (gradient == null) return;
      if (draggingGradientColorEntryNow)
      {
        // drag gradient color entry
        activeGradientColorEntry.Location = GetColorEntryLocationFromX(e.X);
        DrawGradient();
        CopyBackBufferToScreen();
      }
      else
      {
        // check if mouse pointer is over gradient entry marker
        GradientColorEntry previousActiveGradientColorEntry = activeGradientColorEntry;
        activeGradientColorEntry = GetGradientColorEntryFromLocation(e.X);
        if ((previousActiveGradientColorEntry != null && !previousActiveGradientColorEntry.Equals(activeGradientColorEntry))
            ||
            (activeGradientColorEntry != null && !activeGradientColorEntry.Equals(previousActiveGradientColorEntry)))
        {
          DrawColorEntryMarkers();
          CopyBackBufferToScreen();
        }
      }
    }

    private void gradientPanel_MouseDown(object sender, MouseEventArgs e)
    {
      if (gradient == null) return;
      activeGradientColorEntry = GetGradientColorEntryFromLocation(e.X);
      if (activeGradientColorEntry != null)
      {
        if (e.Button == MouseButtons.Left)
        {
          // drag
          draggingGradientColorEntryNow = true;
        }
        else if (e.Button == MouseButtons.Right)
        {
          // delete
          gradient.ColorEntries.Remove(activeGradientColorEntry);
        }
      }
      else
      {
        if (e.Button == MouseButtons.Left)
        {
          // add
          double location = GetColorEntryLocationFromX(e.X);
          Color color = gradient.GetColor(location);
          gradient.ColorEntries.Add(new GradientColorEntry(color, location));
        }
      }
      DrawGradient();
      CopyBackBufferToScreen();
    }

    private void gradientPanel_MouseUp(object sender, MouseEventArgs e)
    {
      if (gradient == null) return;
      draggingGradientColorEntryNow = false;
      DrawGradient();
      CopyBackBufferToScreen();
    }

    private void gradientPanel_MouseLeave(object sender, EventArgs e)
    {
      if (gradient == null) return;
      activeGradientColorEntry = null;
      DrawColorEntryMarkers();
    }

    private void gradientPanel_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (gradient == null) return;
      activeGradientColorEntry = GetGradientColorEntryFromLocation(e.X);
      if (activeGradientColorEntry != null)
      {
        GradientColorEntry colorEntry = activeGradientColorEntry;
        using (var cc = new ColorChooser { Color = colorEntry.Color })
        {
          if (cc.ShowDialog() == DialogResult.OK)
          {
            colorEntry.Color = cc.Color;
            DrawGradient();
            CopyBackBufferToScreen();
          }
        }
      }
    }

    private void gradientNameTextbox_TextChanged(object sender, EventArgs e)
    {
      gradient.Name = gradientNameTextbox.Text;
    }

    private void gradientNameTextbox_Enter(object sender, EventArgs e)
    {
      gradientNameTextbox.SelectAll();
    }

    #endregion

  }
}
