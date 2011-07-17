using System;
using System.Windows.Forms;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
  public partial class ProgressIndicator : Form
  {
    private readonly GarminUSBReader garminUSBReader;
    
    public ProgressIndicator(GarminUSBReader garminUSBReader)
    {
      InitializeComponent();
      this.garminUSBReader = garminUSBReader;
      status.Text = "";
    }

    protected override void OnLoad(EventArgs e)
    {
      BindEventHandlers();
      base.OnLoad(e);
    }

    private void BindEventHandlers()
    {
      garminUSBReader.Progress += GarminUSBReader_Progress;
      garminUSBReader.Completed += GarminUSBReader_Completed;
      garminUSBReader.Error += GarminUSBReader_Error;
    }

    private void UnbindEventHandlers()
    {
      garminUSBReader.Progress -= GarminUSBReader_Progress;
      garminUSBReader.Completed -= GarminUSBReader_Completed;
      garminUSBReader.Error -= GarminUSBReader_Error;
    }

    private void GarminUSBReader_Error(Exception ex)
    {
      if (InvokeRequired)
      {
        BeginInvoke(new ErrorDelegate(GarminUSBReader_Error), ex);
        return;
      }
      UnbindEventHandlers();
      MessageBox.Show(this, ex.Message, Strings.USBReadError, MessageBoxButtons.OK, MessageBoxIcon.Error);
      DialogResult = DialogResult.Abort;
    }

    private void GarminUSBReader_Progress(ReadType readType, int step, int maxSteps, double partCompleted)
    {
      if (InvokeRequired)
      {
        BeginInvoke(new ProgressDelegate(GarminUSBReader_Progress), readType, step, maxSteps, partCompleted);
        return;
      }
      status.Text = string.Format(Strings.Status, step, maxSteps, GetReadTypeString(readType));
      progressBar.Value = (int)(100 * partCompleted);
    }

    private void GarminUSBReader_Completed()
    {
      if (InvokeRequired)
      {
        BeginInvoke(new CompletedDelegate(GarminUSBReader_Completed));
        return;
      }
      UnbindEventHandlers();
      DialogResult = DialogResult.OK;
      Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      garminUSBReader.CancelRead();
      DialogResult = DialogResult.Cancel;
    }

    private static string GetReadTypeString(ReadType readType)
    {
      switch (readType)
      {
        case ReadType.ProductData:
          return Strings.ProductData;
        case ReadType.Runs:
          return Strings.Runs;
        case ReadType.Laps:
          return Strings.Laps;
        case ReadType.Tracks:
          return Strings.Tracks;
      }
      return "";
    }
    
  }
}