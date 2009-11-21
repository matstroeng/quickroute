using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using QuickRoute.BusinessEntities.Exporters;
using QuickRoute.Resources;

namespace QuickRoute.UI.Forms
{
  public partial class OpenMultipleFilesInGoogleEarthDialog : Form
  {
    private readonly KmlMultipleFileExporterProperties originalMultipleFileProperties;
    private bool updatingUINow;
    
    public OpenMultipleFilesInGoogleEarthDialog()
    {
      InitializeComponent();
    }

    public OpenMultipleFilesInGoogleEarthDialog(IEnumerable<string> initialFileNames, KmlMultipleFileExporterProperties multipleFileProperties) : this()
    {
      updatingUINow = true;
      fileSelector.FileDialogTitle = Strings.AddFiles;
      fileSelector.FileDialogFilter = Strings.FileFilter_AllQuickRouteFiles + "|" + Strings.FileFilter_QuickRouteFiles + "|" + Strings.FileFilter_QuickRoute10Files + "|" + Strings.FileFilter_JpegFilesExportedFromQuickRoute;
      fileSelector.FileDialogFilterIndex = 1;
      fileSelector.AddFiles(initialFileNames);
      originalMultipleFileProperties = (KmlMultipleFileExporterProperties)multipleFileProperties.Clone();
      SetPropertiesToControls(originalMultipleFileProperties);
      updatingUINow = false;
      UpdateUI();
    }

    public List<string> FileNames
    {
      get
      {
        return fileSelector.FileNames;
      }
    }

    public KmlMultipleFileExporterProperties MultipleFileProperties
    {
      get { return GetPropertiesFromControls(); }
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

    private void replayTailsVisibleLabel_Click(object sender, EventArgs e)
    {
      replayTailsVisible.Checked = !replayTailsVisible.Checked;
    }

    private void massStartLabel_Click(object sender, EventArgs e)
    {
      massStart.Checked = !massStart.Checked;
    }

    private void massStart_CheckedChanged(object sender, EventArgs e)
    {
      UpdateUI();
    }

    private void replayTailsVisible_CheckedChanged(object sender, EventArgs e)
    {
      UpdateUI();
    }

    private void replayTailDuration_Leave(object sender, EventArgs e)
    {
      UpdateUI();
    }

    private void replayTimeInterval_Leave(object sender, EventArgs e)
    {
      UpdateUI();
    }

    private void UpdateUI()
    {
      if (updatingUINow) return;
      massStartLabel.Enabled = includeReplay.Checked;
      massStart.Enabled = includeReplay.Checked;
      replayTailsVisibleLabel.Enabled = includeReplay.Checked;
      replayTailsVisible.Enabled = includeReplay.Checked;
      replayTimeIntervalLabel.Enabled = includeReplay.Checked;
      replayTimeInterval.Enabled = includeReplay.Checked;
      replayTailDurationLabel.Enabled = includeReplay.Checked && replayTailsVisible.Checked;
      replayTailDuration.Enabled = includeReplay.Checked && replayTailsVisible.Checked;
      replayRestartAfterEachLap.Enabled = includeReplay.Checked && massStart.Checked;
      replayRestartAfterEachLapLabel.Enabled = includeReplay.Checked && massStart.Checked;

      ok.Enabled = (fileSelector.FileNames.Count > 0);
      var p = GetPropertiesFromControls();
      if(p.HasReplayTails) 
      {
        replayTailDuration.Text = (p.ReplayTails[0].EndVisible.HasValue
                                        ? p.ReplayTails[0].EndVisible.Value.TotalSeconds.ToString()
                                        : Strings.Infinite);
      }

      // validate and adjust values
      if (p.ReplayTimeInterval.TotalSeconds <= 0) p.ReplayTimeInterval = originalMultipleFileProperties.ReplayTimeInterval;
      if (p.RouteLineWidth <= 0 || p.RouteLineWidth > 100) p.RouteLineWidth = originalMultipleFileProperties.RouteLineWidth;
      SetPropertiesToControls(p);
    }

    private KmlMultipleFileExporterProperties GetPropertiesFromControls()
    {
      var p = new KmlMultipleFileExporterProperties();

      // include routes
      p.IncludeRoutes = includeRoutes.Checked;

      // include replay
      p.IncludeReplay = includeReplay.Checked;

      // mass start
      p.MassStart = massStart.Checked;
      
      // tails
      var tails = new List<KmlReplayTail>();
      if (replayTailsVisible.Checked)
      {
        var tail = new KmlReplayTail() { StartVisible = new TimeSpan(0) };
        double tailDurationSeconds = 0;
        if (double.TryParse(replayTailDuration.Text, out tailDurationSeconds))
        {
          tailDurationSeconds = Math.Max(0.1, Math.Min(3600, tailDurationSeconds));
          tail.EndVisible = new TimeSpan((long)(tailDurationSeconds * TimeSpan.TicksPerSecond));
        }
        else
        {
          tail.EndVisible = null;
        }
        tails.Add(tail);
      }
      p.ReplayTails = tails;

      // replay time interval
      double timeInterval;
      if (double.TryParse(replayTimeInterval.Text, out timeInterval))
      {
        timeInterval = Math.Max(0.1, Math.Min(3600, timeInterval));
        p.ReplayTimeInterval = new TimeSpan((long)(timeInterval * TimeSpan.TicksPerSecond));
      }
      else
      {
        p.ReplayTimeInterval = originalMultipleFileProperties.ReplayTimeInterval;
      }

      // restart after each lap
      p.ReplayRestartAfterEachLap = replayRestartAfterEachLap.Checked;


      // opacity
      var opacity = (originalMultipleFileProperties.Colors.Count > 0 ? (double)originalMultipleFileProperties.Colors[0].A / 255 : 1.0);
      for (var i = 0; i < originalMultipleFileProperties.Colors.Count; i++)
      {
        p.Colors[i] = Color.FromArgb((int)(255 * opacity), p.Colors[i]);
      }

      // route line width
      p.RouteLineWidth = originalMultipleFileProperties.RouteLineWidth;

      return p;
    }

    private void SetPropertiesToControls(KmlMultipleFileExporterProperties p)
    {
      updatingUINow = true;
      includeRoutes.Checked = p.IncludeRoutes;
      includeReplay.Checked = p.IncludeReplay;
      massStart.Checked = p.MassStart;
      replayTailsVisible.Checked = p.HasReplayTails;
      if (p.HasReplayTails)
      {
        replayTailDuration.Text = (p.ReplayTails[0].EndVisible.HasValue
                                        ? p.ReplayTails[0].EndVisible.Value.TotalSeconds.ToString()
                                        : Strings.Infinite);
      }
      replayTimeInterval.Text = p.ReplayTimeInterval.TotalSeconds.ToString();
      replayRestartAfterEachLap.Checked = p.ReplayRestartAfterEachLap;
      updatingUINow = false;
    }

    private void fileSelector_FilesChanged(object sender, EventArgs e)
    {
      UpdateUI();
    }

    private void includeReplay_CheckedChanged(object sender, EventArgs e)
    {
      if (!includeRoutes.Checked && !includeReplay.Checked) includeReplay.Checked = true;
      UpdateUI();
    }

    private void includeRoutes_CheckedChanged(object sender, EventArgs e)
    {
      if (!includeRoutes.Checked && !includeReplay.Checked) includeRoutes.Checked = true;
      UpdateUI();
    }

    private void replayRestartAfterEachLapLabel_Click(object sender, EventArgs e)
    {
      replayRestartAfterEachLap.Checked = !replayRestartAfterEachLap.Checked;
    }
   
  }
}