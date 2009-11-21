using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using QuickRoute.PropertyControls;

namespace QuickRoute.Controls
{
  public partial class GradientViewer : UserControl
  {
    private IWindowsFormsEditorService editorService = null;
    private Gradient gradient;

    // This constructor takes a Gradient value from the
    // design-time environment, which will be used to display
    // the initial state.
    public GradientViewer(Gradient gradient, IWindowsFormsEditorService editorService)
    {
      // This call is required by the designer.
      InitializeComponent();

      // Cache the light shape value provided by the 
      // design-time environment.
      this.gradient = gradient;

      // Cache the reference to the editor service.
      this.editorService = editorService;

    }

    // Gradient is the property for which this control provides
    // a custom user interface in the Properties window.
    [Category("Gradient")]
    [Browsable(true)]
    [EditorAttribute(typeof(GradientEditor), typeof(UITypeEditor))]
    public Gradient Gradient
    {
      get
      {
        return this.gradient;
      }
      set
      {
        this.gradient = value;
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      gradient.Draw(this.CreateGraphics(), this.Bounds, 0, 1, Gradient.Direction.Horizontal); 
    }

  }

  internal class GradientEditor : UITypeEditor
  {

    IWindowsFormsEditorService editorService;

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    {
      return UITypeEditorEditStyle.DropDown;
    }

    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    {
      if (provider != null)
      {
        editorService =
            provider.GetService(
            typeof(IWindowsFormsEditorService))
            as IWindowsFormsEditorService;
      }

      if (editorService != null)
      {
        GradientEditorControl editorControl = new GradientEditorControl((Gradient)value, editorService);
        editorService.DropDownControl(editorControl);
      }

      return value;
    }

    // This method indicates to the design environment that
    // the type editor will paint additional content in the
    // LightShape entry in the PropertyGrid.
    public override bool GetPaintValueSupported(ITypeDescriptorContext context)
    {
      return true;
    }

    // This method paints a graphical representation of the 
    // selected value of the LightShpae property.
    public override void PaintValue(PaintValueEventArgs e)
    {
      Gradient gradient = (Gradient)e.Value;
      gradient.Draw(e.Graphics, e.Bounds, 0, 1, Gradient.Direction.Horizontal);
    }

  }
}
