using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.Controls.Forms
{
  public partial class LapTimeForm : Form
  {
    private DateTime initialTime;

    public LapTimeForm()
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

    public Session Session { get; set; }

    public DateTime Time
    {
      get 
      {
        var timeString = timeTextbox.Text;
        var timeSeparator = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator;
        timeString = timeString.Replace(".", timeSeparator);
        timeString = timeString.Replace(",", timeSeparator);
        timeString = timeString.Replace(":", timeSeparator);

        TimeSpan ts;
        if (TimeSpan.TryParse(timeString, out ts))
        {
          var dt = initialTime.Date.AddSeconds(ts.TotalSeconds);
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

    private string FormatTime(DateTime time, bool includeDate)
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