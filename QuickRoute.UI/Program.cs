using System;
using System.Windows.Forms;
//using Microsoft.SqlServer.MessageBox;
using QuickRoute.BusinessEntities;
using QuickRoute.Common;
using QuickRoute.Resources;
using QuickRoute.UI.Forms;


namespace QuickRoute.UI
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// Do not enter any metods to this class!
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Main());
    }
  }
}