using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QuickRoute.UI.Forms
{
  public partial class NewVersionDialog : Form
  {
    public NewVersionDialog()
    {
      InitializeComponent();
    }

    public string Message
    {
      get
      {
        return message.Text;
      }
      set
      {
        message.Text = value;
      }
    }

    public string DownloadUrl { get; set; }

    public bool DontRemindMe
    {
      get
      {
        return dontRemindMe.Checked;
      }
      set
      {
        dontRemindMe.Checked = value;
      }
    }

    private void goToDownloadPage_Click(object sender, EventArgs e)
    {
      System.Diagnostics.Process.Start(DownloadUrl);
      Close();
    }

    private void close_Click(object sender, EventArgs e)
    {
      Close();
    }
  }
}
