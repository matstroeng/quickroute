using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Windows.Forms;

namespace QuickRoute.UI.Forms
{
  public partial class SelectUICultureForm : Form
  {
    private readonly List<CultureInfo> availableCultures = new List<CultureInfo>();

    public SelectUICultureForm()
    {
      InitializeComponent();

      var culturesString = ConfigurationManager.AppSettings.Get("cultures");
      foreach (var culture in culturesString.Split(';'))
      {
        availableCultures.Add(new CultureInfo(culture));
      }

      uiCultureDropdown.DataSource = availableCultures;
      uiCultureDropdown.DisplayMember = "NativeName";
    }

    public CultureInfo UiCulture
    {
      get { return uiCultureDropdown.SelectedItem as CultureInfo; }
      set
      {
        uiCultureDropdown.SelectedItem = value;
        if (uiCultureDropdown.SelectedItem == null) uiCultureDropdown.SelectedItem = availableCultures[0];
      }
    }

    private void ok_Click(object sender, System.EventArgs e)
    {
      DialogResult = System.Windows.Forms.DialogResult.OK; 
      Close();
    }

    private void SelectUICultureForm_Load(object sender, System.EventArgs e)
    {

    }
  }
}
