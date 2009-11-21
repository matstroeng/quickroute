using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuickRoute.BusinessEntities.Exporters;
using QuickRoute.Resources;

namespace QuickRoute.UI.Forms
{
  public partial class KmlPropertySelectorDialog : Form
  {
    private KmlProperties properties;
    private bool updatingUINow;

    public KmlPropertySelectorDialog() : this(new KmlProperties())
    {
    }

    public KmlPropertySelectorDialog(KmlProperties properties)
    {
      InitializeComponent();
      Properties = properties;
    }

    public KmlProperties Properties
    {
      get
      {
        return GetPropertiesFromControls();
      }
      set
      {
        if (value == null) return;
        updatingUINow = true;
        properties = value.Clone() as KmlProperties;
        SetPropertiesToControls(properties);
        updatingUINow = false;
        UpdateUI();
      }
    }

    public string DialogTitle
    {
      get
      {
        return Text;
      }
      set
      {
        Text = value;
      }
    }

    private void SetPropertiesToControls(KmlProperties value)
    {
      includeMap.Checked = (value.MapType != KmlExportMapType.None);
      includeRoute.Checked = (value.RouteType != KmlExportRouteType.None || value.MapType == KmlExportMapType.MapAndRoute);
      if (value.MapType == KmlExportMapType.MapAndRoute) routeLineStyle.SelectedIndex = 0;
      else 
      {
        switch (value.RouteType)
        {
          case KmlExportRouteType.Monochrome:
            routeLineStyle.SelectedIndex = 1;
            break;
          case KmlExportRouteType.ColorCoded:
            routeLineStyle.SelectedIndex = 2;
            break;
          default:
            routeLineStyle.SelectedIndex = 0;
            break;
        }
      }
      adaptRouteToMapImage.Checked = (value.RouteAdaptionStyle != KmlRouteAdaptationStyle.NoAdaption);
      adaptReplayToMapImage.Checked = (value.RouteAdaptionStyle != KmlRouteAdaptationStyle.NoAdaption);

      includeReplay.Checked = (value.ReplayType != KmlExportReplayType.None);
      switch (value.ReplayType)
      {
        case KmlExportReplayType.Monochrome:
          replayMarkerStyle.SelectedIndex = 0;
          break;
        case KmlExportReplayType.ColorCoded:
          replayMarkerStyle.SelectedIndex = 1;
          break;
        default:
          replayMarkerStyle.SelectedIndex = 0;
          break;
      }
      replayTimeInterval.Text = value.ReplayTimeInterval.TotalSeconds.ToString();
      replayTailVisible.Checked = value.HasReplayTails;
      if (value.HasReplayTails)
      {
        replayTailDuration.Text = (value.ReplayTails[0].EndVisible.HasValue
                                     ? value.ReplayTails[0].EndVisible.Value.TotalSeconds.ToString()
                                     : Strings.Infinite);
      }
      else
      {
        replayTailDuration.Text = "";
      }
    }

    private KmlProperties GetPropertiesFromControls()
    {
      var controlProperties = new KmlProperties
                                {
                                  MapType = (includeMap.Checked ? KmlExportMapType.Map : KmlExportMapType.None)
                                };
      if (includeRoute.Checked && routeLineStyle.SelectedIndex == 0) controlProperties.MapType = KmlExportMapType.MapAndRoute;

      if (!includeRoute.Checked)
      {
        controlProperties.RouteType = KmlExportRouteType.None;
      }
      else
      {
        switch (routeLineStyle.SelectedIndex)
        {
          case 1:
            controlProperties.RouteType = KmlExportRouteType.Monochrome;
            break;
          case 2:
            controlProperties.RouteType = KmlExportRouteType.ColorCoded;
            break;
          default:
            controlProperties.RouteType = KmlExportRouteType.None;
            break;
        }
      }
      controlProperties.RouteAdaptionStyle = includeMap.Checked && adaptRouteToMapImage.Checked
                                               ? KmlRouteAdaptationStyle.AdaptToSessionMapImage
                                               : KmlRouteAdaptationStyle.NoAdaption;
      
      if (!includeReplay.Checked)
      {
        controlProperties.ReplayType = KmlExportReplayType.None;
      }
      else
      {
        switch (replayMarkerStyle.SelectedIndex)
        {
          case 0:
            controlProperties.ReplayType = KmlExportReplayType.Monochrome;
            break;
          case 1:
            controlProperties.ReplayType = KmlExportReplayType.ColorCoded;
            break;
          default:
            controlProperties.ReplayType = KmlExportReplayType.None;
            break;
        }
        double timeInterval = 1;
        double.TryParse(replayTimeInterval.Text, out timeInterval);
        timeInterval = Math.Max(0.1, Math.Min(3600, timeInterval));
        controlProperties.ReplayTimeInterval = new TimeSpan((long)(timeInterval * TimeSpan.TicksPerSecond));

        var tails = new List<KmlReplayTail>();
        if (replayTailVisible.Checked)
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
            tail.EndVisible = null; // infinite
          }
          tails.Add(tail);
        }
        controlProperties.ReplayTails = tails;
      }

      return controlProperties;
    }

    private void UpdateUI()
    {
      if (updatingUINow) return;
      routeLineStyle.Enabled = includeRoute.Checked;
      routeLineStyleLabel.Enabled = includeRoute.Checked;
      replayMarkerStyle.Enabled = includeReplay.Checked;
      replayMarkerStyleLabel.Enabled = includeReplay.Checked;
      replayTimeInterval.Enabled = includeReplay.Checked;
      replayTimeIntervalLabel.Enabled = includeReplay.Checked;
      replayTailVisible.Enabled = includeReplay.Checked;
      replayTailDuration.Enabled = includeReplay.Checked && replayTailVisible.Checked;

      var p = GetPropertiesFromControls();
      adaptRouteToMapImage.Enabled = includeMap.Checked && includeRoute.Checked && p.RouteType != KmlExportRouteType.None;
      adaptReplayToMapImage.Enabled = includeMap.Checked && includeReplay.Checked;
      replayTailDuration.Text = (p.HasReplayTails
                                   ? (p.ReplayTails[0].EndVisible.HasValue
                                        ? p.ReplayTails[0].EndVisible.Value.TotalSeconds.ToString()
                                        : Strings.Infinite)
                                   : "");
      if (p.MapType == KmlExportMapType.MapAndRoute) includeMap.Checked = true;
      if (p.ReplayTimeInterval.TotalSeconds <= 0) replayTimeInterval.Text = "1";
      if (replayTailVisible.Checked && replayTailDuration.Text == "") replayTailDuration.Text = "60";
    }

    private void includeRoute_CheckedChanged(object sender, EventArgs e)
    {
      UpdateUI();
    }

    private void includeReplay_CheckedChanged(object sender, EventArgs e)
    {
      UpdateUI();
    }

    private void includeMap_CheckedChanged(object sender, EventArgs e)
    {
      if (!includeMap.Checked && includeRoute.Checked && routeLineStyle.SelectedIndex == 0)
        routeLineStyle.SelectedIndex = 1;
      UpdateUI();
    }

    private void routeLineStyle_SelectedIndexChanged(object sender, EventArgs e)
    {
      UpdateUI();
    }

    private void replayTimeInterval_Leave(object sender, EventArgs e)
    {
      UpdateUI();
    }

    private void replayTailVisible_CheckedChanged(object sender, EventArgs e)
    {
      UpdateUI();
    }

    private void replayTailDuration_Leave(object sender, EventArgs e)
    {
      UpdateUI();
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

    private void adaptRouteToMapImage_CheckedChanged(object sender, EventArgs e)
    {
      adaptReplayToMapImage.Checked = adaptRouteToMapImage.Checked;
    }

    private void adaptReplayToMapImage_CheckedChanged(object sender, EventArgs e)
    {
      adaptRouteToMapImage.Checked = adaptReplayToMapImage.Checked;
    }

  }
}
