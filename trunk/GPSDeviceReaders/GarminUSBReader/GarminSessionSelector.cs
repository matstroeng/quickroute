using System;
using System.Windows.Forms;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
  public partial class GarminSessionSelector : Form
  {
    private GarminUSBReader garminUSBReader;

    public GarminSessionSelector()
    {
      InitializeComponent();
    }

    public GarminUSBReader GarminUSBReader
    {
      get { return garminUSBReader; }
      set
      {
        garminUSBReader = value;
        sessionsComboBox.DataSource = garminUSBReader.GarminDevice.Sessions;
      }
    }

    public GarminSession SelectedSession
    {
      get { return sessionsComboBox.SelectedItem as GarminSession; }
      set { sessionsComboBox.SelectedItem = value; }
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

    private void readGPSUnit_Click(object sender, EventArgs e)
    {
      using (var progressIndicator = new ProgressIndicator(garminUSBReader))
      {
        garminUSBReader.StartReadA1000Protocol();
        progressIndicator.ShowDialog();
      }
    }
  }
}