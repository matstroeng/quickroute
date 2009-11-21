using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QuickRoute.UI.Forms
{
  public partial class SplashScreen : Form
  {
    public SplashScreen()
    {
      InitializeComponent();
    }

    private void SplashScreen_Load(object sender, EventArgs e)
    {
      Main m = new Main();
      m.Show();
      this.Close();
    }
  }
}
