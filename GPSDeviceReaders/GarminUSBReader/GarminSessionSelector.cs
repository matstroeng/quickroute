using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
  public partial class GarminSessionSelector : Form
  {
    public GarminSessionSelector(IEnumerable<GarminSessionHeader> sessionHeaders)
    {
      InitializeComponent();
      sessionsComboBox.DataSource = sessionHeaders;
    }

    public GarminSessionHeader SelectedSessionHeader
    {
      get { return sessionsComboBox.SelectedItem as GarminSessionHeader; }
    }

    private void ok_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
      Close();
    }

    private void cancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }
  }
}