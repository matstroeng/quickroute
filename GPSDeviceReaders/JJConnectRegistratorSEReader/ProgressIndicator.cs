using System;
using System.Windows.Forms;

namespace QuickRoute.GPSDeviceReaders.JJConnectRegistratorSEReader
{
    public partial class ProgressIndicator : Form
    {
        private readonly JJConnectRegistratorSEReader _regSEReader;
        public ProgressIndicator(JJConnectRegistratorSEReader regSEReader)
        {
            InitializeComponent();
            _regSEReader = regSEReader;
            regSEReader.ProgressChanged += _RegSE_ProgressChanged;
            regSEReader.ReadCompleted += _RegSE_ReadCompleted;
            regSEReader.ReadError += _RegSE_ReadError;
            status.Text = "";
        }

        public JJConnectRegistratorSEReader RegSEReader
        {
            get { return _regSEReader; }
        }

        void _RegSE_ReadError(Exception e)
        {
            if (InvokeRequired)
            {
                var arg = new object[1];
                arg[0] = e;
                BeginInvoke(new ReadErrorDelegate(_RegSE_ReadError), arg);
                return;
            }
            MessageBox.Show(this, e.Message, "Read error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            DialogResult = DialogResult.Abort;
        }
        void _RegSE_ProgressChanged(string message, int percent)
        {
            try
            {
                if (InvokeRequired)
                {
                    var arg = new object[2];
                    arg[0] = message;
                    arg[1] = percent;
                    BeginInvoke(new ProgressDelegate(_RegSE_ProgressChanged), arg);
                    return;
                }
                status.Text = message;
                progressBar.Value = percent;
            }
            catch (Exception)
            {
                // do nothing
            }
        }
        void _RegSE_ReadCompleted()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ReadCompletedDelegate(_RegSE_ReadCompleted));
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _regSEReader.Cancel();
            DialogResult = DialogResult.Cancel;
        }
    }
}