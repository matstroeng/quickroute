using System;
using System.Windows.Forms;

namespace QuickRoute.GPSDeviceReaders.GlobalSatGH615MReader
{
    public partial class ProgressIndicator : Form
    {
        private readonly GlobalSatGH615MReader _gsGH615MReader;
        public ProgressIndicator(GlobalSatGH615MReader gsGH615MReader)
        {
            InitializeComponent();
            _gsGH615MReader = gsGH615MReader;
            gsGH615MReader.ProgressChanged += _GH615M_ProgressChanged;
            gsGH615MReader.ReadCompleted += _GH615M_ReadCompleted;
            gsGH615MReader.ReadError += _GH615M_ReadError;
            status.Text = "";
        }

        public GlobalSatGH615MReader GsGH615MReader
        {
            get { return _gsGH615MReader; }
        }

        void _GH615M_ReadError(Exception e)
        {
            if (InvokeRequired)
            {
                var arg = new object[1];
                arg[0] = e;
                BeginInvoke(new ReadErrorDelegate(_GH615M_ReadError), arg);
                return;
            }
            MessageBox.Show(this, e.Message, "Read error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            DialogResult = DialogResult.Abort;
        }
        void _GH615M_ProgressChanged(string message, int percent)
        {
            try
            {
                if (InvokeRequired)
                {
                    var arg = new object[2];
                    arg[0] = message;
                    arg[1] = percent;
                    BeginInvoke(new ProgressDelegate(_GH615M_ProgressChanged), arg);
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
        void _GH615M_ReadCompleted()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ReadCompletedDelegate(_GH615M_ReadCompleted));
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _gsGH615MReader.Cancel();
            DialogResult = DialogResult.Cancel;
        }
    }
}