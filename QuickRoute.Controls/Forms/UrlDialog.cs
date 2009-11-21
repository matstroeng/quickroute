using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QuickRoute.Controls.Forms
{
  public partial class UrlDialog : Form
  {
    public UrlDialog()
    {
      InitializeComponent();
    }

    public List<string> Urls
    {
      get
      {
        var list = new List<string>();
        list.AddRange(urls.Text.Split(new [] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries));
        for(var i = 0; i < list.Count; i++)
        {
          if (!list[i].StartsWith("http") && !list[i].StartsWith("ftp")) list[i] = "http://" + list[i];
        }
        return list;
      }
      set
      {
        urls.Text = string.Join("\r\n", value.ToArray()).Trim(new [] { '\r', '\n'} );
      }
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

    private void urls_Enter(object sender, EventArgs e)
    {
      urls.SelectAll();
    }

    private void UrlDialog_Load(object sender, EventArgs e)
    {
      urls.Focus();
    }
  }
}
