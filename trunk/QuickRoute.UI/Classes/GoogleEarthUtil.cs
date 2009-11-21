using System;
using System.IO;
using System.Runtime.InteropServices;
using QuickRoute.Common;

namespace QuickRoute.UI.Classes
{
  public static class GoogleEarthUtil
  {
    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    public static void OpenInGoogleEarth(Stream stream)
    {
      // create file from stream
      var reader = new BinaryReader(stream);
      stream.Position = 0;
      var data = reader.ReadBytes((int)stream.Length);
      reader.Close();
      var fileName = CommonUtil.GetTempFileName("kmz");
      var fileStream = File.Create(fileName);
      fileStream.Write(data, 0, data.Length);
      fileStream.Close();
      fileStream.Dispose();
      
      var googleEarthApplication = new EARTHLib.ApplicationGEClass();
      googleEarthApplication.OpenKmlFile(fileName, 1);
      BringWindowToForeground((IntPtr)googleEarthApplication.GetMainHwnd());
    }

    private static void BringWindowToForeground(IntPtr hWnd)
    {
      SetForegroundWindow(hWnd);
    }

  }


}
