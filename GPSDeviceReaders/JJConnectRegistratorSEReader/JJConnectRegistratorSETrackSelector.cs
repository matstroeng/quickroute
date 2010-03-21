#region Namespace Inclusions

using System;
using System.Windows.Forms;

#endregion

namespace QuickRoute.GPSDeviceReaders.JJConnectRegistratorSEReader
{
    public partial class JJConnectRegistratorSETrackSelector : Form
    {

        #region Local Variables

        private JJConnectRegistratorSEReader _deviceReader;

        #endregion

        #region Constructor

        public JJConnectRegistratorSETrackSelector()
        {
            // Build the form
            InitializeComponent();

            // Enable/disable controls based on the current state
            SetGUIState(true);
        }

        #endregion

        #region Local Methods

        private void SetGUIState(bool enabled)
        {
            if (enabled)
            {
                btnScan.Enabled = true;
                if (_deviceReader != null && _deviceReader.IsConnected)
                {
                    cbSelectedTrack.Enabled = true;
                    btnOk.Enabled = true;
                }
            }
            else
            {
                btnOk.Enabled = false;
                btnScan.Enabled = false;
                cbSelectedTrack.Enabled = false;
            }
        }

        #endregion

        #region Local Properties

        public JJConnectRegistratorSEReader DeviceReader
        {
            get { return _deviceReader; }
            set { _deviceReader = value;
                cbSelectedTrack.DataSource = _deviceReader.GetTracks(); }
        }

        public IRegSETrack SelectedTrack
        {
            get { return cbSelectedTrack.SelectedItem as IRegSETrack; }
            set { cbSelectedTrack.SelectedItem = value; }
        }

        #endregion

        private void btnScan_Click(object sender, EventArgs e)
        {
            using (var progressIndicator = new ProgressIndicator(_deviceReader))
            {
                cbSelectedTrack.DataSource = null;
                cbSelectedTrack.Items.Clear();
                cbSelectedTrack.Enabled = false;
                _deviceReader.StartRescanPortThread();
                progressIndicator.ShowDialog();
                cbSelectedTrack.DataSource = _deviceReader.GetTracksInfo();
                cbSelectedTrack.Enabled = (cbSelectedTrack.Items.Count > 0);
                btnOk.Enabled = (cbSelectedTrack.Items.Count > 0);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}