using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;
using QuickRoute.Resources;

namespace QuickRoute.UI.Forms
{
  public partial class GradientEditor : Form
  {
    private Padding gradientPadding = new Padding(6, 1, 6, 17);
    private Size colorEntryMarkerSize = new Size(11, 16);
    private Rectangle gradientRectangle;
    private GradientColorEntry activeGradientColorEntry;
    private bool draggingGradientColorEntryNow = false;
    private List<Gradient> gradients = new List<Gradient>();
    private Bitmap gradientPanelBackBuffer;

    public GradientEditor()
    {
      InitializeComponent();
      gradientList.DataSource = gradients;
      CreateGradientRectangle();
      CreateGradientBackBuffer();
    }

    #region Public properties

    public Gradient CurrentGradient
    {
      get
      {
        if (gradientList.SelectedItem == null) return null;
        return (Gradient)gradientList.SelectedItem;
      }
      set
      {
        foreach (object item in gradientList.Items)
        {
          if (item.Equals(value))
          {
            gradientList.SelectedItem = item;
            return;
          }
        }
        gradientList.SelectedItem = null;
      }
    }

    public List<Gradient> Gradients
    {
      get { return gradients; }
      set 
      { 
        gradients = value;
        gradientList.DataSource = null;
        gradientList.DataSource = gradients;
      }
    }

    #endregion

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
      if (CurrentGradient != null)
      {
        Gradient.FillCheckerboardRectangle(graphics, gradientRectangle, 8);
        CurrentGradient.Draw(graphics, gradientRectangle, 0.0, 1.0, Gradient.Direction.Horizontal);
        
        graphics.DrawRectangle(Pens.Black, new Rectangle(gradientRectangle.Left - 1, gradientRectangle.Top - 1, gradientRectangle.Width + 2, gradientRectangle.Height + 1));

        DrawColorEntryMarkers();
        RefreshGradientList(true);
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
      foreach (GradientColorEntry colorEntry in CurrentGradient.ColorEntries)
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

    private void RefreshGradientList(bool refreshSelectedItemOnly)
    {
      if (refreshSelectedItemOnly)
      {
        if (gradientList.SelectedIndex >= 0) gradientList.Invalidate(gradientList.GetItemRectangle(gradientList.SelectedIndex));
      }
      else
      {
        gradientList.Refresh();
      }
    }

    private GradientColorEntry GetGradientColorEntryFromLocation(int x)
    {
      int tmpX;
      foreach (GradientColorEntry colorEntry in CurrentGradient.ColorEntries)
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

    private string CreateUniqueGradientName(string name)
    {
      int index = 1;
      string uniqueName = name;

      while (true)
      {
        bool unique = true;
        foreach (Gradient g in gradients)
        {
          if (uniqueName == g.Name)
          {
            unique = false;
            break;
          }
        }
        if (unique) return uniqueName;
        index++;
        uniqueName = name + " (" + index + ")";
      }
    }
#endregion

    #region Event handlers

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

    private void GradientPanel_Resize(object sender, EventArgs e)
    {
      CreateGradientRectangle();
      CreateGradientBackBuffer(); 
      DrawGradient();
    }

    private void GradientPanel_Paint(object sender, PaintEventArgs e)
    {
      CopyBackBufferToScreen();
    }

    private void GradientPanel_MouseMove(object sender, MouseEventArgs e)
    {
      if (CurrentGradient == null) return;
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

    private void GradientPanel_MouseDown(object sender, MouseEventArgs e)
    {
      if (CurrentGradient == null) return;
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
          CurrentGradient.ColorEntries.Remove(activeGradientColorEntry);
        }
      }
      else
      {
        if (e.Button == MouseButtons.Left)
        {
          // add
          double location = GetColorEntryLocationFromX(e.X);
          Color color = CurrentGradient.GetColor(location);
          CurrentGradient.ColorEntries.Add(new GradientColorEntry(color, location));
        }
      }
      DrawGradient();
      CopyBackBufferToScreen();
    }

    private void GradientPanel_MouseUp(object sender, MouseEventArgs e)
    {
      if (CurrentGradient == null) return;
      draggingGradientColorEntryNow = false;
      DrawGradient();
      CopyBackBufferToScreen();
    }

    private void GradientPanel_MouseLeave(object sender, EventArgs e)
    {
      if (CurrentGradient == null) return;
      activeGradientColorEntry = null;
      DrawColorEntryMarkers();
    }

    private void GradientPanel_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (CurrentGradient == null) return;
      activeGradientColorEntry = GetGradientColorEntryFromLocation(e.X);
      if (activeGradientColorEntry != null)
      {
        GradientColorEntry colorEntry = activeGradientColorEntry;
        using (var cc = new ColorChooser {Color = colorEntry.Color})
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

    private void GradientList_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (CurrentGradient != null) gradientNameTextbox.Text = CurrentGradient.Name;
      DrawGradient();
      gradientPanel.Refresh();

    }

    private void GradientNameTextbox_TextChanged(object sender, EventArgs e)
    {
      CurrentGradient.Name = gradientNameTextbox.Text;
      RefreshGradientList(true);
    }

    private void AddGradient_Click(object sender, EventArgs e)
    {
      Gradient newGradient = new Gradient();
      newGradient.Name = CreateUniqueGradientName(Strings.NewGradient);
      gradients.Add(newGradient);
      gradientList.DataSource = null;
      gradientList.DataSource = gradients;
      gradientList.SelectedIndex = gradients.Count - 1;
      deleteGradient.Enabled = (gradients.Count > 0);
      gradientNameTextbox.Focus();
    }

    private void DeleteGradient_Click(object sender, EventArgs e)
    {
      int index = gradientList.SelectedIndex;
      gradients.RemoveAt(index);
      gradientList.DataSource = gradients;
      if (index > gradients.Count - 1) index = gradients.Count - 1;
      gradientList.DataSource = null;
      gradientList.DataSource = gradients;
      gradientList.SelectedIndex = index;
      deleteGradient.Enabled = (gradients.Count > 0);
    }

    private void GradientNameTextbox_Enter(object sender, EventArgs e)
    {
      gradientNameTextbox.SelectAll();
    }

    private void GradientEditor_Activated(object sender, EventArgs e)
    {
      gradientNameTextbox.Focus();
    }
 
    #endregion

 }
}