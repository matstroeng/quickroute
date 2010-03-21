using System;
using System.Windows.Forms;
using QuickRoute.BusinessEntities.Importers;

namespace QuickRoute.UI.Forms
{
    public partial class RefreshDevicesProgressIndicator : Form
    {
        public RefreshDevicesProgressIndicator()
        {
            InitializeComponent();
            SupportedImportFormatManager.RefreshProgressChanged += RefreshProgressChanged;
            SupportedImportFormatManager.RefreshCompleted += RefreshCompleted;
            status.Text = "";
        }

        void RefreshProgressChanged(string message, int percent)
        {
            try
            {
                if (InvokeRequired)
                {
                    var arg = new object[2];
                    arg[0] = message;
                    arg[1] = percent;
                    BeginInvoke(new RefreshProgressDelegate(RefreshProgressChanged), arg);
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
        void RefreshCompleted()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new RefreshCompletedDelegate(RefreshCompleted));
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // TODO: implement refresh cancel
            DialogResult = DialogResult.Cancel;
        }
    }
}