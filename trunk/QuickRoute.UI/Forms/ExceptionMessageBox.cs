using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QuickRoute.UI.Forms
{
  public partial class ExceptionMessageBox : Form
  {
    private int heightWhenStackTraceVisible = 420;
    private const int heightWhenStackTraceHidden = 145;

    public ExceptionMessageBox(string title, Exception exception)
    {
      InitializeComponent();
      Text = title;
      errorMessage.Text = exception.Message;
      stackTrace.Text = exception.Message + Environment.NewLine + 
        "----------------------------------------------------------------------------------------------------" + 
        Environment.NewLine;
      while (exception != null)
      {
        stackTrace.Text += exception.StackTrace + Environment.NewLine + Environment.NewLine;
        exception = exception.InnerException; 
      }
    }

    private void showStackTrace_Click(object sender, EventArgs e)
    {
      stackTrace.Visible = true;
      showStackTrace.Visible = false;
      hideStackTrace.Visible = true;
      MaximumSize = new Size(2048, 2048);
      Height = heightWhenStackTraceVisible;
      stackTrace.Height = Math.Max(0, ok.Top - errorMessage.Bottom - 16);
    }

    private void ok_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
      Close();
    }

    private void hideStackTrace_Click(object sender, EventArgs e)
    {
      stackTrace.Visible = false;
      hideStackTrace.Visible = false;
      showStackTrace.Visible = true;
      MaximumSize = new Size(2048, heightWhenStackTraceHidden);
      Height = heightWhenStackTraceHidden;
    }

    private void ExceptionMessageBox_Resize(object sender, EventArgs e)
    {
      if (stackTrace.Visible) heightWhenStackTraceVisible = Height;
    }
  }
}
