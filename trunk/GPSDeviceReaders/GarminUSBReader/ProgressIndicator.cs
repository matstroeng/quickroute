using System;
using System.Windows.Forms;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
    public partial class ProgressIndicator : Form
    {
        private readonly GarminUSBReader garminUSBReader;
        public ProgressIndicator(GarminUSBReader oGarminUSB)
        {
            InitializeComponent();
            garminUSBReader = oGarminUSB;
            garminUSBReader.USBProgressChanged += _GarminUSB_USBProgressChanged;
            garminUSBReader.USBReadCompleted += _GarminUSB_USBReadCompleted;
            garminUSBReader.USBReadError += _GarminUSB_USBReadError;
            status.Text = "";
        }

        void _GarminUSB_USBReadError(Exception e)
        {
            if (InvokeRequired)
            {
                object[] arg = new object[1];
                arg[0] = e;
                BeginInvoke(new USBReadErrorDelegate(_GarminUSB_USBReadError), arg);
                return;
            }
            MessageBox.Show(this, e.Message, Strings.USBReadError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.DialogResult = DialogResult.Abort;
        }
        void _GarminUSB_USBProgressChanged(string type, int stepNr, int stepMax, int percent)
        {
            try
            {
                if (InvokeRequired)
                {
                    object[] arg = new object[4];
                    arg[0] = type;
                    arg[1] = stepNr;
                    arg[2] = stepMax;
                    arg[3] = percent;
                    BeginInvoke(new USBProgressDelegate(_GarminUSB_USBProgressChanged), arg);
                    return;
                }
                status.Text = string.Format(Strings.Status, stepNr, stepMax, GetReadTypeString(type));
                progressBar.Value = percent;
            }
            catch (Exception) {}
        }
        void _GarminUSB_USBReadCompleted()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new USBReadCompletedDelegate(_GarminUSB_USBReadCompleted));
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            garminUSBReader.Cancel();
            this.DialogResult = DialogResult.Cancel;
        }

        private static string GetReadTypeString(string readType)
        {
          switch (readType)
          {
            case "product data":
              return Strings.ProductData;
            case "date and time":
              return Strings.DateAndTime;
            case "almanac":
              return Strings.Almanac;
            case "position":
              return Strings.Position;
            case "runs":
              return Strings.Runs;
            case "laps":
              return Strings.Laps;
            case "tracks":
              return Strings.Tracks;
          }
          return "";
        }


    }
}