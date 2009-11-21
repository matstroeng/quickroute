using System;
using System.Windows.Forms;
using QuickRoute.Resources;

namespace QuickRoute.UI.Forms
{
  public partial class AboutBox : Form
  {
    public AboutBox()
    {
      InitializeComponent();
      var v = new Version(Application.ProductVersion);
      versionLabel.Text = string.Format(versionLabel.Text, v.Major + "." + v.Minor + (v.Build > 0 ? "." + v.Build : ""));
    }

    private void closeButton_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void websiteLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      System.Diagnostics.Process.Start(Strings.WebsiteUrl);
    }
  }
}