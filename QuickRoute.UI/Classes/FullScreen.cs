using System.Drawing;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;

namespace QuickRoute.UI.Classes
{
  /// <summary>
  /// Class used to preserve / restore / maximize state of the form.
  /// See http://www.codeproject.com/KB/cs/FullScreenDotNetApp.aspx.
  /// </summary>
  public class MainFormState
  {
    private FormWindowState winState;
    private FormBorderStyle brdStyle;
    private Rectangle bounds;
    private ApplicationSettings settings;

    private bool isFullScreen;

    public void EnterFullScreenMode(Main form)
    {
      if (!isFullScreen)
      {
        isFullScreen = true;
        Save(form);
        form.WindowState = FormWindowState.Maximized;
        form.FormBorderStyle = FormBorderStyle.None;
        form.ApplicationSettings.RightPanelVisible = false;
        form.ApplicationSettings.BottomPanelVisible = false;
        form.UpdateUI();
        WinApi.SetWinFullScreen(form.Handle);
      }
    }

    private void Save(Main form)
    {
      winState = form.WindowState;
      brdStyle = form.FormBorderStyle;
      bounds = form.Bounds;
      settings = form.ApplicationSettings.Copy();
    }

    public void ExitFullScreenMode(Main form)
    {
      if(isFullScreen)
      {
        form.WindowState = winState;
        form.FormBorderStyle = brdStyle;
        form.Bounds = bounds;
        form.ApplicationSettings = settings;
        form.UpdateUI();
        isFullScreen = false;
      }
    }
  }
}
