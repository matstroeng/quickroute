using System;
using System.Windows.Forms;

namespace QuickRoute.UI.Forms
{
  public partial class TimeDialog : Form
  {
    private DateTime initialTime;

    public TimeDialog()
    {
      InitializeComponent();
    }

    public DateTime InitialTime
    {
      get { return initialTime; }
      set 
      {
        initialTime = value;
        timeTextbox.Text = FormatTime(initialTime.ToLocalTime(), false);
      }
    }

    public DateTime Time
    {
      get 
      {
        string timeString = timeTextbox.Text;
        string timeSeparator = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator;
        timeString = timeString.Replace(".", timeSeparator);
        timeString = timeString.Replace(",", timeSeparator);
        timeString = timeString.Replace(":", timeSeparator);

        TimeSpan ts;
        if (TimeSpan.TryParse(timeString, out ts))
        {
          DateTime dt = initialTime.Date.AddSeconds(ts.TotalSeconds);
          dt = new DateTime(dt.Ticks, DateTimeKind.Local).ToUniversalTime();
          return dt;
        }
        
        DateTime t;
        if (DateTime.TryParse(timeString, out t))
        {
          return t.ToUniversalTime();
        }
        return initialTime.ToUniversalTime();
      }
    }

    private static string FormatTime(DateTime time, bool includeDate)
    {
      var firstPart = includeDate ? time.ToString("G") : time.ToLongTimeString();
      return firstPart + time.ToString(".fffffff").TrimEnd("0".ToCharArray()).TrimEnd(".".ToCharArray());
    }

    private void OK_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
      Close();
    }

    private void Cancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

    private void TimeTextbox_Leave(object sender, EventArgs e)
    {
      timeTextbox.Text = FormatTime(Time.ToLocalTime(), Time.Date != initialTime.Date);
    }

    private void TimeTextbox_Enter(object sender, EventArgs e)
    {
      timeTextbox.SelectAll();
    }
  
  }
}