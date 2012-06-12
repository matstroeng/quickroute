using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace QuickRoute.UI.Forms
{

  public class ColorChooser : System.Windows.Forms.Form
	{
		internal System.Windows.Forms.Label lblBlue;
		internal System.Windows.Forms.Label lblGreen;
		internal System.Windows.Forms.Label lblRed;
		internal System.Windows.Forms.Label lblBrightness;
		internal System.Windows.Forms.Label lblSaturation;
		internal System.Windows.Forms.Label lblHue;
		internal System.Windows.Forms.HScrollBar hsbBlue;
		internal System.Windows.Forms.HScrollBar hsbGreen;
		internal System.Windows.Forms.HScrollBar hsbRed;
		internal System.Windows.Forms.HScrollBar hsbBrightness;
		internal System.Windows.Forms.HScrollBar hsbSaturation;
		internal System.Windows.Forms.HScrollBar hsbHue;
		internal System.Windows.Forms.Button btnCancel;
		internal System.Windows.Forms.Button btnOK;
		internal System.Windows.Forms.Label Label3;
		internal System.Windows.Forms.Label Label7;
		internal System.Windows.Forms.Panel pnlColor;
		internal System.Windows.Forms.Label Label6;
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.Label Label5;
		internal System.Windows.Forms.Panel pnlSelectedColor;
		internal System.Windows.Forms.Panel pnlBrightness;
		internal System.Windows.Forms.Label Label2;
    internal Label lblAlpha;
    internal HScrollBar hsbAlpha;
    internal Label label8;
    internal Panel pnlAlpha;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ColorChooser()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorChooser));
      this.lblBlue = new System.Windows.Forms.Label();
      this.lblGreen = new System.Windows.Forms.Label();
      this.lblRed = new System.Windows.Forms.Label();
      this.lblBrightness = new System.Windows.Forms.Label();
      this.lblSaturation = new System.Windows.Forms.Label();
      this.lblHue = new System.Windows.Forms.Label();
      this.hsbBlue = new System.Windows.Forms.HScrollBar();
      this.hsbGreen = new System.Windows.Forms.HScrollBar();
      this.hsbRed = new System.Windows.Forms.HScrollBar();
      this.hsbBrightness = new System.Windows.Forms.HScrollBar();
      this.hsbSaturation = new System.Windows.Forms.HScrollBar();
      this.hsbHue = new System.Windows.Forms.HScrollBar();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnOK = new System.Windows.Forms.Button();
      this.Label3 = new System.Windows.Forms.Label();
      this.Label7 = new System.Windows.Forms.Label();
      this.pnlColor = new System.Windows.Forms.Panel();
      this.Label6 = new System.Windows.Forms.Label();
      this.Label1 = new System.Windows.Forms.Label();
      this.Label5 = new System.Windows.Forms.Label();
      this.pnlSelectedColor = new System.Windows.Forms.Panel();
      this.pnlBrightness = new System.Windows.Forms.Panel();
      this.Label2 = new System.Windows.Forms.Label();
      this.lblAlpha = new System.Windows.Forms.Label();
      this.hsbAlpha = new System.Windows.Forms.HScrollBar();
      this.label8 = new System.Windows.Forms.Label();
      this.pnlAlpha = new System.Windows.Forms.Panel();
      this.SuspendLayout();
      // 
      // lblBlue
      // 
      resources.ApplyResources(this.lblBlue, "lblBlue");
      this.lblBlue.Name = "lblBlue";
      // 
      // lblGreen
      // 
      resources.ApplyResources(this.lblGreen, "lblGreen");
      this.lblGreen.Name = "lblGreen";
      // 
      // lblRed
      // 
      resources.ApplyResources(this.lblRed, "lblRed");
      this.lblRed.Name = "lblRed";
      // 
      // lblBrightness
      // 
      resources.ApplyResources(this.lblBrightness, "lblBrightness");
      this.lblBrightness.Name = "lblBrightness";
      // 
      // lblSaturation
      // 
      resources.ApplyResources(this.lblSaturation, "lblSaturation");
      this.lblSaturation.Name = "lblSaturation";
      // 
      // lblHue
      // 
      resources.ApplyResources(this.lblHue, "lblHue");
      this.lblHue.Name = "lblHue";
      // 
      // hsbBlue
      // 
      resources.ApplyResources(this.hsbBlue, "hsbBlue");
      this.hsbBlue.LargeChange = 1;
      this.hsbBlue.Maximum = 255;
      this.hsbBlue.Name = "hsbBlue";
      this.hsbBlue.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HandleARGBScroll);
      // 
      // hsbGreen
      // 
      resources.ApplyResources(this.hsbGreen, "hsbGreen");
      this.hsbGreen.LargeChange = 1;
      this.hsbGreen.Maximum = 255;
      this.hsbGreen.Name = "hsbGreen";
      this.hsbGreen.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HandleARGBScroll);
      // 
      // hsbRed
      // 
      resources.ApplyResources(this.hsbRed, "hsbRed");
      this.hsbRed.LargeChange = 1;
      this.hsbRed.Maximum = 255;
      this.hsbRed.Name = "hsbRed";
      this.hsbRed.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HandleARGBScroll);
      // 
      // hsbBrightness
      // 
      resources.ApplyResources(this.hsbBrightness, "hsbBrightness");
      this.hsbBrightness.LargeChange = 1;
      this.hsbBrightness.Maximum = 255;
      this.hsbBrightness.Name = "hsbBrightness";
      this.hsbBrightness.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HandleHSVScroll);
      // 
      // hsbSaturation
      // 
      resources.ApplyResources(this.hsbSaturation, "hsbSaturation");
      this.hsbSaturation.LargeChange = 1;
      this.hsbSaturation.Maximum = 255;
      this.hsbSaturation.Name = "hsbSaturation";
      this.hsbSaturation.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HandleHSVScroll);
      // 
      // hsbHue
      // 
      resources.ApplyResources(this.hsbHue, "hsbHue");
      this.hsbHue.LargeChange = 1;
      this.hsbHue.Maximum = 255;
      this.hsbHue.Name = "hsbHue";
      this.hsbHue.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HandleHSVScroll);
      // 
      // btnCancel
      // 
      resources.ApplyResources(this.btnCancel, "btnCancel");
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Name = "btnCancel";
      // 
      // btnOK
      // 
      resources.ApplyResources(this.btnOK, "btnOK");
      this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnOK.Name = "btnOK";
      // 
      // Label3
      // 
      resources.ApplyResources(this.Label3, "Label3");
      this.Label3.Name = "Label3";
      // 
      // Label7
      // 
      resources.ApplyResources(this.Label7, "Label7");
      this.Label7.Name = "Label7";
      // 
      // pnlColor
      // 
      resources.ApplyResources(this.pnlColor, "pnlColor");
      this.pnlColor.Name = "pnlColor";
      // 
      // Label6
      // 
      resources.ApplyResources(this.Label6, "Label6");
      this.Label6.Name = "Label6";
      // 
      // Label1
      // 
      resources.ApplyResources(this.Label1, "Label1");
      this.Label1.Name = "Label1";
      // 
      // Label5
      // 
      resources.ApplyResources(this.Label5, "Label5");
      this.Label5.Name = "Label5";
      // 
      // pnlSelectedColor
      // 
      resources.ApplyResources(this.pnlSelectedColor, "pnlSelectedColor");
      this.pnlSelectedColor.Name = "pnlSelectedColor";
      // 
      // pnlBrightness
      // 
      resources.ApplyResources(this.pnlBrightness, "pnlBrightness");
      this.pnlBrightness.Name = "pnlBrightness";
      // 
      // Label2
      // 
      resources.ApplyResources(this.Label2, "Label2");
      this.Label2.Name = "Label2";
      // 
      // lblAlpha
      // 
      resources.ApplyResources(this.lblAlpha, "lblAlpha");
      this.lblAlpha.Name = "lblAlpha";
      // 
      // hsbAlpha
      // 
      resources.ApplyResources(this.hsbAlpha, "hsbAlpha");
      this.hsbAlpha.LargeChange = 1;
      this.hsbAlpha.Maximum = 255;
      this.hsbAlpha.Name = "hsbAlpha";
      this.hsbAlpha.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HandleARGBScroll);
      // 
      // label8
      // 
      resources.ApplyResources(this.label8, "label8");
      this.label8.Name = "label8";
      // 
      // pnlAlpha
      // 
      resources.ApplyResources(this.pnlAlpha, "pnlAlpha");
      this.pnlAlpha.Name = "pnlAlpha";
      // 
      // ColorChooser
      // 
      this.AcceptButton = this.btnOK;
      resources.ApplyResources(this, "$this");
      this.CancelButton = this.btnCancel;
      this.Controls.Add(this.pnlAlpha);
      this.Controls.Add(this.lblAlpha);
      this.Controls.Add(this.hsbAlpha);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.lblBlue);
      this.Controls.Add(this.lblGreen);
      this.Controls.Add(this.lblRed);
      this.Controls.Add(this.lblBrightness);
      this.Controls.Add(this.lblSaturation);
      this.Controls.Add(this.lblHue);
      this.Controls.Add(this.hsbBlue);
      this.Controls.Add(this.hsbGreen);
      this.Controls.Add(this.hsbRed);
      this.Controls.Add(this.hsbBrightness);
      this.Controls.Add(this.hsbSaturation);
      this.Controls.Add(this.hsbHue);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnOK);
      this.Controls.Add(this.Label3);
      this.Controls.Add(this.Label7);
      this.Controls.Add(this.pnlColor);
      this.Controls.Add(this.Label6);
      this.Controls.Add(this.Label1);
      this.Controls.Add(this.Label5);
      this.Controls.Add(this.pnlSelectedColor);
      this.Controls.Add(this.pnlBrightness);
      this.Controls.Add(this.Label2);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ColorChooser";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Load += new System.EventHandler(this.ColorChooser2_Load);
      this.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorChooser2_Paint);
      this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HandleMouse);
      this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HandleMouse);
      this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseUp);
      this.ResumeLayout(false);

		}
		#endregion

		private enum ChangeStyle
		{
			MouseMove,
			ARGB,
			AHSV,
			None
		}

		private ChangeStyle changeType = ChangeStyle.None;
		private Point selectedPoint;

		private ColorWheel myColorWheel;
		private ColorHandler.ARGB ARGB;
		private ColorHandler.AHSV AHSV;

		private void ColorChooser2_Load(object sender, System.EventArgs e)
		{
			// Turn on double-buffering, so the form looks better. 
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);

			// These properties are set in design view, as well, but they
			// have to be set to false in order for the Paint
			// event to be able to display their contents.
			// Never hurts to make sure they're invisible.
			pnlSelectedColor.Visible = false;
      pnlBrightness.Visible = false;
      pnlAlpha.Visible = false;
      pnlColor.Visible = false;

			// Calculate the coordinates of the three
			// required regions on the form.
			Rectangle SelectedColorRectangle =  new Rectangle(pnlSelectedColor.Location, pnlSelectedColor.Size);
      Rectangle BrightnessRectangle = new Rectangle(pnlBrightness.Location, pnlBrightness.Size);
      Rectangle AlphaRectangle = new Rectangle(pnlAlpha.Location, pnlAlpha.Size);
      Rectangle ColorRectangle = new Rectangle(pnlColor.Location, pnlColor.Size);

			// Export the new ColorWheel class, indicating
			// the locations of the color wheel itself, the
			// brightness area, and the position of the selected color.
      myColorWheel = new ColorWheel(ColorRectangle, BrightnessRectangle, AlphaRectangle, SelectedColorRectangle);
			myColorWheel.ColorChanged += this.myColorWheel_ColorChanged;

			// Set the RGB and HSV values 
			// of the NumericUpDown controls.
			SetARGB(ARGB);
			SetAHSV(AHSV);		
		}

		private void HandleMouse(object sender,  MouseEventArgs e)
		{
			// If you have the left mouse button down, 
			// then update the selectedPoint value and 
			// force a repaint of the color wheel.
			if ( e.Button == MouseButtons.Left ) 
			{
				changeType = ChangeStyle.MouseMove;
				selectedPoint = new Point(e.X, e.Y);
				this.Invalidate();
			}
		}

		private void frmMain_MouseUp(object sender,  MouseEventArgs e)
		{
			myColorWheel.SetMouseUp();
			changeType = ChangeStyle.None;
		}

		private void SetARGBLabels(ColorHandler.ARGB ARGB) 
		{
      RefreshText(lblAlpha, ARGB.Alpha);
      RefreshText(lblRed, ARGB.Red);
      RefreshText(lblBlue, ARGB.Blue);
			RefreshText(lblGreen, ARGB.Green);
		}

		private void SetAHSVLabels(ColorHandler.AHSV AHSV) 
		{
			RefreshText(lblHue, AHSV.Hue);
			RefreshText(lblSaturation, AHSV.Saturation);
			RefreshText(lblBrightness, AHSV.Value);
		}

		private void SetARGB(ColorHandler.ARGB ARGB) 
		{
			// Update the RGB values on the form.
      RefreshValue(hsbAlpha, ARGB.Alpha);
      RefreshValue(hsbRed, ARGB.Red);
      RefreshValue(hsbBlue, ARGB.Blue);
			RefreshValue(hsbGreen, ARGB.Green);
			SetARGBLabels(ARGB);
			}

		private void SetAHSV( ColorHandler.AHSV AHSV) 
		{
			// Update the HSV values on the form.
			RefreshValue(hsbHue, AHSV.Hue);
			RefreshValue(hsbSaturation, AHSV.Saturation);
			RefreshValue(hsbBrightness, AHSV.Value);
			SetAHSVLabels(AHSV);
			}

		private void RefreshValue(HScrollBar hsb, int value) 
		{
			hsb.Value = value;
		}

		private void RefreshText(Label lbl, int value) 
		{
			lbl.Text = value.ToString();
		}

		public Color Color  
		{
			// Get or set the color to be
			// displayed in the color wheel.
			get 
			{
				return myColorWheel.Color;
			}

			set 
			{
				// Indicate the color change type. Either RGB or HSV
				// will cause the color wheel to update the position
				// of the pointer.
				changeType = ChangeStyle.ARGB;
				ARGB = new ColorHandler.ARGB(value.A, value.R, value.G, value.B);
				AHSV = ColorHandler.ARGBtoAHSV(ARGB);
			}
		}

		private void myColorWheel_ColorChanged(object sender,  ColorChangedEventArgs e)  
		{
			SetARGB(e.ARGB);
			SetAHSV(e.AHSV);
		}

		private void HandleHSVScroll(object sender,  ScrollEventArgs e)  
			// If the H, S, or V values change, use this 
			// code to update the RGB values and invalidate
			// the color wheel (so it updates the pointers).
			// Check the isInUpdate flag to avoid recursive events
			// when you update the NumericUpdownControls.
		{
			changeType = ChangeStyle.AHSV;
			AHSV = new ColorHandler.AHSV(hsbAlpha.Value, hsbHue.Value, hsbSaturation.Value, hsbBrightness.Value);
			SetARGB(ColorHandler.AHSVtoARGB(AHSV));
			SetAHSVLabels(AHSV);
			this.Invalidate();
		}

		private void HandleARGBScroll(object sender, ScrollEventArgs e)
		{
			// If the R, G, or B values change, use this 
			// code to update the HSV values and invalidate
			// the color wheel (so it updates the pointers).
			// Check the isInUpdate flag to avoid recursive events
			// when you update the NumericUpdownControls.
			changeType = ChangeStyle.ARGB;
			ARGB = new ColorHandler.ARGB(hsbAlpha.Value, hsbRed.Value, hsbGreen.Value, hsbBlue.Value);
			SetAHSV(ColorHandler.ARGBtoAHSV(ARGB));
			SetARGBLabels(ARGB);
			this.Invalidate();
		}

		private void ColorChooser2_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			// Depending on the circumstances, force a repaint
			// of the color wheel passing different information.
			switch (changeType)
			{
				case ChangeStyle.AHSV:
					myColorWheel.Draw(e.Graphics, AHSV);
					break;
				case ChangeStyle.MouseMove:
				case ChangeStyle.None:
					myColorWheel.Draw(e.Graphics, selectedPoint);
					break;
				case ChangeStyle.ARGB:
					myColorWheel.Draw(e.Graphics, ARGB);
					break;
			}
		}
	}

  public class ColorWheel : IDisposable
  {

    // These resources should be disposed
    // of when you're done with them.
    private Graphics g;
    private Region colorRegion;
    private Region brightnessRegion;
    private Region alphaRegion;
    private Bitmap colorImage;

    public EventHandler<ColorChangedEventArgs> ColorChanged;

    // Keep track of the current mouse state. 
    public enum MouseState
    {
      MouseUp,
      ClickOnColor,
      DragInColor,
      ClickOnBrightness,
      DragInBrightness,
      ClickOnAlpha,
      DragInAlpha,
      ClickOutsideRegion,
      DragOutsideRegion,
    }
    private MouseState currentState = MouseState.MouseUp;

    // The code needs to convert back and forth between 
    // degrees and radians. There are 2*PI radians in a 
    // full circle, and 360 degrees. This constant allows
    // you to convert back and forth.
    private const double DEGREES_PER_RADIAN = 180.0 / Math.PI;

    // COLOR_COUNT represents the number of distinct colors
    // used to create the circular gradient. Its value 
    // is somewhat arbitrary -- change this to 6, for 
    // example, to see what happens. 1536 (6 * 256) seems 
    // a good compromise -- it's enough to get a full 
    // range of colors, but it doesn't overwhelm the processor
    // attempting to generate the image. The color wheel
    // contains 6 sections, and each section displays 
    // 256 colors. Seems like a reasonable compromise.
    private const int COLOR_COUNT = 6 * 256;

    private Point centerPoint;
    private int radius;

    private Rectangle colorRectangle;
    private Rectangle brightnessRectangle;
    private Rectangle alphaRectangle;
    private Rectangle selectedColorRectangle;
    private int brightnessX;
    private double brightnessScaling;
    private int alphaX;
    private double alphaScaling;

    // selectedColor is the actual value selected
    // by the user. fullColor is the same color, 
    // with its brightness set to 255.
    private Color selectedColor = Color.White;
    private Color fullColor;

    private ColorHandler.ARGB ARGB;
    private ColorHandler.AHSV AHSV;

    // Locations for the two "pointers" on the form.
    private Point colorPoint;
    private Point brightnessPoint;
    private Point alphaPoint;

    private int brightness;
    private int brightnessMin;
    private int brightnessMax;

    private int alpha;
    private int alphaMin;
    private int alphaMax;

    public ColorWheel(Rectangle colorRectangle, Rectangle brightnessRectangle, Rectangle alphaRectangle, Rectangle selectedColorRectangle)
    {

      // Caller must provide locations for color wheel
      // (colorRectangle), brightness "strip" (brightnessRectangle),
      // alpha "strip" (alphaRectangle)
      // and location to display selected color (selectedColorRectangle).

      using (GraphicsPath path = new GraphicsPath())
      {
        // Store away locations for later use. 
        this.colorRectangle = colorRectangle;
        this.brightnessRectangle = brightnessRectangle;
        this.alphaRectangle = alphaRectangle;
        this.selectedColorRectangle = selectedColorRectangle;

        // Calculate the center of the circle.
        // Start with the location, then offset
        // the point by the radius.
        // Use the smaller of the width and height of
        // the colorRectangle value.
        this.radius = (int)Math.Min(colorRectangle.Width, colorRectangle.Height) / 2;
        this.centerPoint = colorRectangle.Location;
        this.centerPoint.Offset(radius, radius);

        // Start the pointer in the center.
        this.colorPoint = this.centerPoint;

        // Export a region corresponding to the color circle.
        // Code uses this later to determine if a specified
        // point is within the region, using the IsVisible 
        // method.
        path.AddEllipse(colorRectangle);
        colorRegion = new Region(path);

        // set the range for the brightness selector.
        this.brightnessMin = this.brightnessRectangle.Top;
        this.brightnessMax = this.brightnessRectangle.Bottom;

        // Export a region corresponding to the
        // brightness rectangle, with a little extra 
        // "breathing room". 

        path.AddRectangle(new Rectangle(brightnessRectangle.Left, brightnessRectangle.Top - 10, brightnessRectangle.Width + 10, brightnessRectangle.Height + 20));
        // Export region corresponding to brightness
        // rectangle. Later code uses this to 
        // determine if a specified point is within
        // the region, using the IsVisible method.
        brightnessRegion = new Region(path);

        // Set the location for the brightness indicator "marker".
        // Also calculate the scaling factor, scaling the height
        // to be between 0 and 255. 
        brightnessX = brightnessRectangle.Left + brightnessRectangle.Width;
        brightnessScaling = (double)255 / (brightnessMax - brightnessMin);

        // Calculate the location of the brightness
        // pointer. Assume it's at the highest position.
        brightnessPoint = new Point(brightnessX, brightnessMax);


        // set the range for the alpha selector.
        this.alphaMin = this.alphaRectangle.Top;
        this.alphaMax = this.alphaRectangle.Bottom;

        // Export a region corresponding to the
        // alpha rectangle, with a little extra 
        // "breathing room". 

        path.AddRectangle(new Rectangle(alphaRectangle.Left, alphaRectangle.Top - 10, alphaRectangle.Width + 10, alphaRectangle.Height + 20));
        // Export region corresponding to alpha
        // rectangle. Later code uses this to 
        // determine if a specified point is within
        // the region, using the IsVisible method.
        alphaRegion = new Region(path);

        // Set the location for the alpha indicator "marker".
        // Also calculate the scaling factor, scaling the height
        // to be between 0 and 255. 
        alphaX = alphaRectangle.Left + alphaRectangle.Width;
        alphaScaling = (double)255 / (alphaMax - alphaMin);

        // Calculate the location of the alpha
        // pointer. Assume it's at the highest position.
        alphaPoint = new Point(alphaX, alphaMax);


        // Export the bitmap that contains the circular gradient.
        CreateGradient();
      }
    }

    protected void OnColorChanged(ColorHandler.ARGB RGB, ColorHandler.AHSV AHSV)
    {
      ColorChangedEventArgs e = new ColorChangedEventArgs(RGB, AHSV);
      ColorChanged(this, e);
    }

    public Color Color
    {
      get
      {
        return selectedColor;
      }
    }

    void IDisposable.Dispose()
    {
      // Dispose of graphic resources
      if (colorImage != null)
        colorImage.Dispose();
      if (colorRegion != null)
        colorRegion.Dispose();
      if (brightnessRegion != null)
        brightnessRegion.Dispose();
      if (alphaRegion != null)
        alphaRegion.Dispose();
      if (g != null)
        g.Dispose();
    }

    public void SetMouseUp()
    {
      // Indicate that the user has
      // released the mouse.
      currentState = MouseState.MouseUp;
    }

    public void Draw(Graphics g, ColorHandler.AHSV AHSV)
    {
      // Given AHSV values, update the screen.
      this.g = g;
      this.AHSV = AHSV;
      CalcCoordsAndUpdate(this.AHSV);
      UpdateDisplay();
    }

    public void Draw(Graphics g, ColorHandler.ARGB ARGB)
    {
      // Given RGB values, calculate AHSV and then update the screen.
      this.g = g;
      this.AHSV = ColorHandler.ARGBtoAHSV(ARGB);
      CalcCoordsAndUpdate(this.AHSV);
      UpdateDisplay();
    }

    public void Draw(Graphics g, Point mousePoint)
    {
      // You've moved the mouse. 
      // Now update the screen to match.

      double distance;
      int degrees;
      Point delta;
      Point newColorPoint;
      Point newBrightnessPoint;
      Point newAlphaPoint;
      Point newPoint;

      // Keep track of the previous color pointer point, 
      // so you can put the mouse there in case the 
      // user has clicked outside the circle.
      newColorPoint = colorPoint;
      newBrightnessPoint = brightnessPoint;
      newAlphaPoint = alphaPoint;

      // Store this away for later use.
      this.g = g;

      if (currentState == MouseState.MouseUp)
      {
        if (!mousePoint.IsEmpty)
        {
          if (colorRegion.IsVisible(mousePoint))
          {
            // Is the mouse point within the color circle?
            // If so, you just clicked on the color wheel.
            currentState = MouseState.ClickOnColor;
          }
          else if (brightnessRegion.IsVisible(mousePoint))
          {
            // Is the mouse point within the brightness area?
            // You clicked on the brightness area.
            currentState = MouseState.ClickOnBrightness;
          }
          else if (alphaRegion.IsVisible(mousePoint))
          {
            // Is the mouse point within the alpha area?
            // You clicked on the alpha area.
            currentState = MouseState.ClickOnAlpha;
          }
          else
          {
            // Clicked outside the color, brightness and alpha 
            // regions. In that case, just put the 
            // pointers back where they were.
            currentState = MouseState.ClickOutsideRegion;
          }
        }
      }

      switch (currentState)
      {
        case MouseState.ClickOnBrightness:
        case MouseState.DragInBrightness:
          // Calculate new color information
          // based on the brightness, which may have changed.
          newPoint = mousePoint;
          if (newPoint.Y < brightnessMin)
          {
            newPoint.Y = brightnessMin;
          }
          else if (newPoint.Y > brightnessMax)
          {
            newPoint.Y = brightnessMax;
          }
          newBrightnessPoint = new Point(brightnessX, newPoint.Y);
          brightness = (int)((brightnessMax - newPoint.Y) * brightnessScaling);
          AHSV.Value = brightness;
          ARGB = ColorHandler.AHSVtoARGB(AHSV);
          break;

        case MouseState.ClickOnAlpha:
        case MouseState.DragInAlpha:
          // Calculate new color information
          // based on the alpha, which may have changed.
          newPoint = mousePoint;
          if (newPoint.Y < alphaMin)
          {
            newPoint.Y = alphaMin;
          }
          else if (newPoint.Y > alphaMax)
          {
            newPoint.Y = alphaMax;
          }
          newAlphaPoint = new Point(alphaX, newPoint.Y);
          alpha = (int)((alphaMax - newPoint.Y) * alphaScaling);
          AHSV.Alpha = alpha;
          ARGB = ColorHandler.AHSVtoARGB(AHSV);
          break;

        case MouseState.ClickOnColor:
        case MouseState.DragInColor:
          // Calculate new color information
          // based on selected color, which may have changed.
          newColorPoint = mousePoint;

          // Calculate x and y distance from the center,
          // and then calculate the angle corresponding to the
          // new location.
          delta = new Point(
            mousePoint.X - centerPoint.X, mousePoint.Y - centerPoint.Y);
          degrees = CalcDegrees(delta);

          // Calculate distance from the center to the new point 
          // as a fraction of the radius. Use your old friend, 
          // the Pythagorean theorem, to calculate this value.
          distance = Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y) / radius;

          if (currentState == MouseState.DragInColor)
          {
            if (distance > 1)
            {
              // Mouse is down, and outside the circle, but you 
              // were previously dragging in the color circle. 
              // What to do?
              // In that case, move the point to the edge of the 
              // circle at the correct angle.
              distance = 1;
              newColorPoint = GetPoint(degrees, radius, centerPoint);
            }
          }

          // Calculate the new AHSV and RGB values.
          AHSV.Hue = (int)(degrees * 255 / 360);
          AHSV.Saturation = (int)(distance * 255);
          AHSV.Value = brightness;
          ARGB = ColorHandler.AHSVtoARGB(AHSV);
          fullColor = ColorHandler.AHSVtoColor(AHSV.Alpha, AHSV.Hue, AHSV.Saturation, 255);
          break;
      }
      selectedColor = ColorHandler.AHSVtoColor(AHSV);

      // Raise an event back to the parent form,
      // so the form can update any UI it's using 
      // to display selected color values.
      OnColorChanged(ARGB, AHSV);

      // On the way out, set the new state.
      switch (currentState)
      {
        case MouseState.ClickOnBrightness:
          currentState = MouseState.DragInBrightness;
          break;
        case MouseState.ClickOnAlpha:
          currentState = MouseState.DragInAlpha;
          break;
        case MouseState.ClickOnColor:
          currentState = MouseState.DragInColor;
          break;
        case MouseState.ClickOutsideRegion:
          currentState = MouseState.DragOutsideRegion;
          break;
      }

      // Store away the current points for next time.
      colorPoint = newColorPoint;
      brightnessPoint = newBrightnessPoint;
      alphaPoint = newAlphaPoint;

      // Draw the gradients and points. 
      UpdateDisplay();
    }

    private Point CalcBrightnessPoint(int brightness)
    {
      // Take the value for brightness (0 to 255), scale to the 
      // scaling used in the brightness bar, then add the value 
      // to the bottom of the bar. return the correct point at which 
      // to display the brightness pointer.
      return new Point(brightnessX,
        (int)(brightnessMax - brightness / brightnessScaling));
    }

    private Point CalcAlphaPoint(int alpha)
    {
      // Take the value for alpha (0 to 255), scale to the 
      // scaling used in the alpha bar, then add the value 
      // to the bottom of the bar. return the correct point at which 
      // to display the alpha pointer.
      return new Point(alphaX,
        (int)(alphaMax - alpha / alphaScaling));
    }

    private void UpdateDisplay()
    {
      // Update the gradients, and place the 
      // pointers correctly based on colors and 
      // brightness.

      using (Brush selectedBrush = new SolidBrush(selectedColor))
      {
        // Draw the saved color wheel image.
        g.DrawImage(colorImage, colorRectangle);

        // Draw the "selected color" rectangle.
        DrawSelectedColorRectangle(selectedBrush);

        // Draw the "brightness" rectangle.
        DrawBrightnessLinearGradient(fullColor);

        // Draw the "alpha" rectangle.
        DrawAlphaLinearGradient(selectedColor);

        // Draw the three pointers.
        DrawColorPointer(colorPoint);
        DrawBrightnessPointer(brightnessPoint);
        DrawAlphaPointer(alphaPoint);
      }
    }

    private void CalcCoordsAndUpdate(ColorHandler.AHSV AHSV)
    {
      // Convert color to real-world coordinates and then calculate
      // the various points. AHSV.Hue represents the degrees (0 to 360), 
      // AHSV.Saturation represents the radius. 
      // This procedure doesn't draw anything--it simply 
      // updates class-level variables. The UpdateDisplay
      // procedure uses these values to update the screen.

      // Given the angle (AHSV.Hue), and distance from 
      // the center (AHSV.Saturation), and the center, 
      // calculate the point corresponding to 
      // the selected color, on the color wheel.
      colorPoint = GetPoint((double)AHSV.Hue / 255 * 360,
        (double)AHSV.Saturation / 255 * radius,
        centerPoint);

      // Given the brightness (AHSV.value), calculate the 
      // point corresponding to the brightness indicator.
      brightnessPoint = CalcBrightnessPoint(AHSV.Value);

      // Store information about the selected color.
      brightness = AHSV.Value;
      selectedColor = ColorHandler.AHSVtoColor(AHSV);
      ARGB = ColorHandler.AHSVtoARGB(AHSV);

      // The full color is the same as AHSV, except that the 
      // brightness is set to full (255). This is the top-most
      // color in the brightness gradient.
      fullColor = ColorHandler.AHSVtoColor(AHSV.Alpha, AHSV.Hue, AHSV.Saturation, 255);

      // Given the brightness (AHSV.value), calculate the 
      // point corresponding to the alpha indicator.
      alphaPoint = CalcAlphaPoint(AHSV.Alpha);

      // Store information about the selected color.
      alpha = AHSV.Alpha;

    }

    private void DrawSelectedColorRectangle(Brush brush)
    {
      // Make checkerboard boxes as a background
      int x = selectedColorRectangle.Left;
      int y = selectedColorRectangle.Top;
      int size = 8;
      int brushIndex = 0;
      int rowIndex = 0;
      SolidBrush[] checkerboardBrushes = new SolidBrush[] { (SolidBrush)Brushes.LightGray, (SolidBrush)Brushes.White };
      while (y <= selectedColorRectangle.Bottom)
      {
        while (x <= selectedColorRectangle.Right)
        {
          g.FillRectangle(checkerboardBrushes[brushIndex], x, y, Math.Min(size, selectedColorRectangle.Right - x), Math.Min(size, selectedColorRectangle.Bottom - y));
          x += size;
          brushIndex = (brushIndex + 1) % 2;
        }
        x = selectedColorRectangle.Left;
        rowIndex += 1;
        brushIndex = rowIndex % 2;
        y += size;
      }
      g.FillRectangle(brush, selectedColorRectangle);
    }

    private void DrawBrightnessLinearGradient(Color TopColor)
    {
      // Given the top color, draw a linear gradient
      // ranging from black to the top opaque color. Use the 
      // brightness rectangle as the area to fill.
      TopColor = Color.FromArgb(255, TopColor);
      using (LinearGradientBrush lgb =
               new LinearGradientBrush(brightnessRectangle, TopColor,
               Color.Black, LinearGradientMode.Vertical))
      {
        g.FillRectangle(lgb, brightnessRectangle);
      }
    }

    private void DrawAlphaLinearGradient(Color color)
    {
      // Make checkerboard boxes as a background
      int x = alphaRectangle.Left;
      int y = alphaRectangle.Top;
      int size = 8;
      int brushIndex = 0;
      int rowIndex = 0;
      SolidBrush[] checkerboardBrushes = new SolidBrush[] { (SolidBrush)Brushes.LightGray, (SolidBrush)Brushes.White };
      while (y <= alphaRectangle.Bottom)
      {
        while (x <= alphaRectangle.Right)
        {
          g.FillRectangle(checkerboardBrushes[brushIndex], x, y, Math.Min(size, alphaRectangle.Right - x), Math.Min(size, alphaRectangle.Bottom - y));
          x += size;
          brushIndex = (brushIndex + 1) % 2;
        }
        x = alphaRectangle.Left;
        rowIndex += 1;
        brushIndex = rowIndex % 2;
        y += size;
      }

      // Given the color, draw a linear gradient
      // ranging from opaque to transparent. Use the 
      // alpha rectangle as the area to fill.
      using (LinearGradientBrush lgb =
               new LinearGradientBrush(alphaRectangle, Color.FromArgb(255, color),
               Color.FromArgb(0, color), LinearGradientMode.Vertical))
      {
        g.FillRectangle(lgb, alphaRectangle);
      }
    }

    private int CalcDegrees(Point pt)
    {
      int degrees;

      if (pt.X == 0)
      {
        // The point is on the y-axis. Determine whether 
        // it's above or below the x-axis, and return the 
        // corresponding angle. Note that the orientation of the
        // y-coordinate is backwards. That is, A positive Y value 
        // indicates a point BELOW the x-axis.
        if (pt.Y > 0)
        {
          degrees = 270;
        }
        else
        {
          degrees = 90;
        }
      }
      else
      {
        // This value needs to be multiplied
        // by -1 because the y-coordinate
        // is opposite from the normal direction here.
        // That is, a y-coordinate that's "higher" on 
        // the form has a lower y-value, in this coordinate
        // system. So everything's off by a factor of -1 when
        // performing the ratio calculations.
        degrees = (int)(-Math.Atan((double)pt.Y / pt.X) * DEGREES_PER_RADIAN);

        // If the x-coordinate of the selected point
        // is to the left of the center of the circle, you 
        // need to add 180 degrees to the angle. ArcTan only
        // gives you a value on the right-hand side of the circle.
        if (pt.X < 0)
        {
          degrees += 180;
        }

        // Ensure that the return value is 
        // between 0 and 360.
        degrees = (degrees + 360) % 360;
      }
      return degrees;
    }

    private void CreateGradient()
    {
      // Export a new PathGradientBrush, supplying
      // an array of points created by calling
      // the GetPoints method.
      using (PathGradientBrush pgb =
        new PathGradientBrush(GetPoints(radius, new Point(radius, radius))))
      {
        // Set the various properties. Note the SurroundColors
        // property, which contains an array of points, 
        // in a one-to-one relationship with the points
        // that created the gradient.
        pgb.CenterColor = Color.White;
        pgb.CenterPoint = new PointF(radius, radius);
        pgb.SurroundColors = GetColors();

        // Export a new bitmap containing
        // the color wheel gradient, so the 
        // code only needs to do all this 
        // work once. Later code uses the bitmap
        // rather than recreating the gradient.
        colorImage = new Bitmap(
          colorRectangle.Width, colorRectangle.Height,
          PixelFormat.Format32bppArgb);

        using (Graphics newGraphics =
                 Graphics.FromImage(colorImage))
        {
          newGraphics.FillEllipse(pgb, 0, 0,
            colorRectangle.Width, colorRectangle.Height);
        }
      }
    }

    private Color[] GetColors()
    {
      // Export an array of COLOR_COUNT
      // colors, looping through all the 
      // hues between 0 and 255, broken
      // into COLOR_COUNT intervals. AHSV is
      // particularly well-suited for this, 
      // because the only value that changes
      // as you create colors is the Hue.
      Color[] Colors = new Color[COLOR_COUNT];

      for (int i = 0; i <= COLOR_COUNT - 1; i++)
        Colors[i] = ColorHandler.AHSVtoColor(255, (int)((double)(i * 255) / COLOR_COUNT), 255, 255);
      return Colors;
    }

    private Point[] GetPoints(double radius, Point centerPoint)
    {
      // Generate the array of points that describe
      // the locations of the COLOR_COUNT colors to be 
      // displayed on the color wheel.
      Point[] Points = new Point[COLOR_COUNT];

      for (int i = 0; i <= COLOR_COUNT - 1; i++)
        Points[i] = GetPoint((double)(i * 360) / COLOR_COUNT, radius, centerPoint);
      return Points;
    }

    private Point GetPoint(double degrees, double radius, Point centerPoint)
    {
      // Given the center of a circle and its radius, along
      // with the angle corresponding to the point, find the coordinates. 
      // In other words, conver  t from polar to rectangular coordinates.
      double radians = degrees / DEGREES_PER_RADIAN;

      return new Point((int)(centerPoint.X + Math.Floor(radius * Math.Cos(radians))),
        (int)(centerPoint.Y - Math.Floor(radius * Math.Sin(radians))));
    }

    private void DrawColorPointer(Point pt)
    {
      // Given a point, draw the color selector. 
      // The constant SIZE represents half
      // the width -- the square will be twice
      // this value in width and height.
      const int SIZE = 3;
      g.DrawRectangle(Pens.Black,
        pt.X - SIZE, pt.Y - SIZE, SIZE * 2, SIZE * 2);
    }

    private void DrawBrightnessPointer(Point pt)
    {
      // Draw a triangle for the 
      // brightness indicator that "points"
      // at the provided point.
      const int HEIGHT = 10;
      const int WIDTH = 7;

      Point[] Points = new Point[3];
      Points[0] = pt;
      Points[1] = new Point(pt.X + WIDTH, pt.Y + HEIGHT / 2);
      Points[2] = new Point(pt.X + WIDTH, pt.Y - HEIGHT / 2);
      g.FillPolygon(Brushes.Black, Points);
    }

    private void DrawAlphaPointer(Point pt)
    {
      // Draw a triangle for the 
      // alpha indicator that "points"
      // at the provided point.
      const int HEIGHT = 10;
      const int WIDTH = 7;

      Point[] Points = new Point[3];
      Points[0] = pt;
      Points[1] = new Point(pt.X + WIDTH, pt.Y + HEIGHT / 2);
      Points[2] = new Point(pt.X + WIDTH, pt.Y - HEIGHT / 2);
      g.FillPolygon(Brushes.Black, Points);
    }
  }

  public class ColorHandler
  {
    // Handle conversions between RGB and HSV    
    // (and Color types, as well).

    public struct ARGB
    {
      // All values are between 0 and 255.
      public int Alpha;
      public int Red;
      public int Green;
      public int Blue;

      public ARGB(int A, int R, int G, int B)
      {
        Alpha = A;
        Red = R;
        Green = G;
        Blue = B;
      }

      public override string ToString()
      {
        return String.Format("({0}, {1}, {2}, {3})", Alpha, Red, Green, Blue);
      }
    }

    public struct AHSV
    {
      // All values are between 0 and 255.
      public int Alpha;
      public int Hue;
      public int Saturation;
      public int Value;

      public AHSV(int A, int H, int S, int V)
      {
        Alpha = A;
        Hue = H;
        Saturation = S;
        Value = V;
      }

      public override string ToString()
      {
        return String.Format("({0}, {1}, {2}, {3})", Alpha, Hue, Saturation, Value);
      }
    }

    public static ARGB AHSVtoARGB(int A, int H, int S, int V)
    {
      // H, S, and V must all be between 0 and 255.
      return AHSVtoARGB(new AHSV(A, H, S, V));
    }

    public static Color AHSVtoColor(AHSV ahsv)
    {
      ARGB ARGB = AHSVtoARGB(ahsv);
      return Color.FromArgb(ARGB.Alpha, ARGB.Red, ARGB.Green, ARGB.Blue);
    }

    public static Color AHSVtoColor(int A, int H, int S, int V)
    {
      return AHSVtoColor(new AHSV(A, H, S, V));
    }

    public static ARGB AHSVtoARGB(AHSV AHSV)
    {
      // HSV contains values scaled as in the color wheel:
      // that is, all from 0 to 255. 

      // for ( this code to work, HSV.Hue needs
      // to be scaled from 0 to 360 (it//s the angle of the selected
      // point within the circle). HSV.Saturation and HSV.value must be 
      // scaled to be between 0 and 1.

      double h;
      double s;
      double v;

      double r = 0;
      double g = 0;
      double b = 0;

      // Scale Hue to be between 0 and 360. Saturation
      // and value scale to be between 0 and 1.
      h = ((double)AHSV.Hue / 255 * 360) % 360;
      s = (double)AHSV.Saturation / 255;
      v = (double)AHSV.Value / 255;

      if (s == 0)
      {
        // If s is 0, all colors are the same.
        // This is some flavor of gray.
        r = v;
        g = v;
        b = v;
      }
      else
      {
        double p;
        double q;
        double t;

        double fractionalSector;
        int sectorNumber;
        double sectorPos;

        // The color wheel consists of 6 sectors.
        // Figure out which sector you//re in.
        sectorPos = h / 60;
        sectorNumber = (int)(Math.Floor(sectorPos));

        // get the fractional part of the sector.
        // That is, how many degrees into the sector
        // are you?
        fractionalSector = sectorPos - sectorNumber;

        // Calculate values for the three axes
        // of the color. 
        p = v * (1 - s);
        q = v * (1 - (s * fractionalSector));
        t = v * (1 - (s * (1 - fractionalSector)));

        // Assign the fractional colors to r, g, and b
        // based on the sector the angle is in.
        switch (sectorNumber)
        {
          case 0:
            r = v;
            g = t;
            b = p;
            break;

          case 1:
            r = q;
            g = v;
            b = p;
            break;

          case 2:
            r = p;
            g = v;
            b = t;
            break;

          case 3:
            r = p;
            g = q;
            b = v;
            break;

          case 4:
            r = t;
            g = p;
            b = v;
            break;

          case 5:
            r = v;
            g = p;
            b = q;
            break;
        }
      }
      // return an RGB structure, with values scaled
      // to be between 0 and 255.
      return new ARGB(AHSV.Alpha, (int)(r * 255), (int)(g * 255), (int)(b * 255));
    }

    public static AHSV ARGBtoAHSV(ARGB ARGB)
    {
      // In this function, R, G, and B values must be scaled 
      // to be between 0 and 1.
      // HSV.Hue will be a value between 0 and 360, and 
      // HSV.Saturation and value are between 0 and 1.
      // The code must scale these to be between 0 and 255 for
      // the purposes of this application.

      double min;
      double max;
      double delta;

      double r = (double)ARGB.Red / 255;
      double g = (double)ARGB.Green / 255;
      double b = (double)ARGB.Blue / 255;

      double h;
      double s;
      double v;

      min = Math.Min(Math.Min(r, g), b);
      max = Math.Max(Math.Max(r, g), b);
      v = max;
      delta = max - min;
      if (max == 0 || delta == 0)
      {
        // R, G, and B must be 0, or all the same.
        // In this case, S is 0, and H is undefined.
        // Using H = 0 is as good as any...
        s = 0;
        h = 0;
      }
      else
      {
        s = delta / max;
        if (r == max)
        {
          // Between Yellow and Magenta
          h = (g - b) / delta;
        }
        else if (g == max)
        {
          // Between Cyan and Yellow
          h = 2 + (b - r) / delta;
        }
        else
        {
          // Between Magenta and Cyan
          h = 4 + (r - g) / delta;
        }

      }
      // Scale h to be between 0 and 360. 
      // This may require adding 360, if the value
      // is negative.
      h *= 60;
      if (h < 0)
      {
        h += 360;
      }

      // Scale to the requirements of this 
      // application. All values are between 0 and 255.
      return new AHSV(ARGB.Alpha, (int)(h / 360 * 255), (int)(s * 255), (int)(v * 255));
    }
  }

  public class ColorChangedEventArgs : EventArgs
  {

    private ColorHandler.ARGB mRGB;
    private ColorHandler.AHSV mHSV;

    public ColorChangedEventArgs(ColorHandler.ARGB RGB, ColorHandler.AHSV HSV)
    {
      mRGB = RGB;
      mHSV = HSV;
    }

    public ColorHandler.ARGB ARGB
    {
      get
      {
        return mRGB;
      }
    }

    public ColorHandler.AHSV AHSV
    {
      get
      {
        return mHSV;
      }
    }
  }
}

