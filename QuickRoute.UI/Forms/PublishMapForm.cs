using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;
using QuickRoute.BusinessEntities.Exporters;
using QuickRoute.Common;
using QuickRoute.Resources;
using QuickRoute.UI.Classes;

namespace QuickRoute.UI.Forms
{
  public partial class PublishMapForm : Form
  {
    private List<MapInfo> allMaps = new List<MapInfo>();
    private readonly IMapPublisher mapPublisher = new Publishers.DOMAPublisher.DOMAPublisher();
    private readonly Document document;
    private readonly ColorRangeProperties colorRangeProperties;
    private readonly WaypointAttribute colorCodingAttribute;
    private readonly WaypointAttribute? secondaryColorCodingAttribute;
    private string usernameOnEnter;
    private string webServiceURLOnEnter;

    public PublishMapForm(Document document, WaypointAttribute colorCodingAttribute, WaypointAttribute? secondaryColorCodingAttribute, ColorRangeProperties colorRangeProperties)
    {
      InitializeComponent();

      this.document = document;
      this.colorCodingAttribute = colorCodingAttribute;
      this.secondaryColorCodingAttribute = secondaryColorCodingAttribute;
      this.colorRangeProperties = colorRangeProperties;
      foreach (var setting in Util.ApplicationSettings.PublishMapSettings)
      {
        if (!webServiceURL.Items.Contains(setting.WebServiceURL))
        {
          webServiceURL.Items.Add(setting.WebServiceURL);
        }
      }
      if (webServiceURL.Items.Count > 0) webServiceURL.SelectedIndex = 0;

      var defaultTime = DateTime.Now.ToLocalTime().Date;
      if (document.Sessions.Count > 0 && document.Sessions[0].Route.FirstWaypoint != null)
      {
        defaultTime = document.Sessions[0].Route.FirstWaypoint.Time.ToLocalTime();
      }

      date.Value = defaultTime;
      SetControlEnabledState(false);
      imageFormat.SelectedIndex = 0;
    }

    #region Event handlers

    private void ok_Click(object sender, EventArgs e)
    {
      PublishMap();
    }

    private void webServiceURL_SelectedIndexChanged(object sender, EventArgs e)
    {
      PopulateUsernameComboBox();
      if (username.Items.Count > 0)
      {
        username.SelectedIndex = 0;
      }
      else
      {
        SetControlEnabledState(false);
      }
    }

    private void username_SelectedIndexChanged(object sender, EventArgs e)
    {
      foreach (var setting in Util.ApplicationSettings.PublishMapSettings)
      {
        if (webServiceURL.Text == setting.WebServiceURL && username.Text == setting.Username)
        {
          password.Text = setting.Password;
          SavePassword.Checked = (setting.Password != null);
          break;
        }
      }
      SetControlEnabledState(false);
    }

    private void cancel_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void connectButton_Click(object sender, EventArgs e)
    {
      Connect();
    }

    private void map_SelectedIndexChanged(object sender, EventArgs e)
    {
      var selectedMap = map.SelectedItem as MapInfo;
      if (!(selectedMap != null && selectedMap.ID != 0))
      {
        var defaultTime = DateTime.Now.ToLocalTime().Date;
        if (document.Sessions.Count > 0 && document.Sessions[0].Route.FirstWaypoint != null)
        {
          defaultTime = document.Sessions[0].Route.FirstWaypoint.Time.ToLocalTime();
        }
        selectedMap = new MapInfo
                        {
                          Date = defaultTime
                        };

      }
      SetControlValues(selectedMap);
    }

    private void uiInformation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      System.Diagnostics.Process.Start(Strings.DomaWebsiteUrl);
    }

    private void control_Enter(object sender, EventArgs e)
    {
      if (sender is ComboBox)
      {
        (sender as ComboBox).SelectAll();
      }
      if (sender is TextBox)
      {
        (sender as TextBox).SelectAll();
      }
    }

    private void webServiceURL_Enter(object sender, EventArgs e)
    {
      webServiceURLOnEnter = webServiceURL.Text;
    }

    private void webServiceURL_Leave(object sender, EventArgs e)
    {
      if (!string.IsNullOrEmpty(webServiceURL.Text) && !webServiceURL.Text.Contains("://")) webServiceURL.Text = "http://" + webServiceURL.Text;
      if (webServiceURL.Text != webServiceURLOnEnter) SetControlEnabledState(false);
    }

    private void username_Enter(object sender, EventArgs e)
    {
      usernameOnEnter = username.Text;
    }

    private void username_Leave(object sender, EventArgs e)
    {
      var found = false;
      foreach (var setting in Util.ApplicationSettings.PublishMapSettings)
      {
        if (webServiceURL.Text == setting.WebServiceURL && username.Text == setting.Username)
        {
          password.Text = setting.Password;
          SavePassword.Checked = (setting.Password != null);
          found = true;
          break;
        }
      }
      if (!found)
      {
        password.Text = "";
        SavePassword.Checked = false;
      }
      if (username.Text != usernameOnEnter) SetControlEnabledState(false);
    }

    #endregion

    #region Logic

    private void PublishMap()
    {
      Cursor = Cursors.WaitCursor;
      MapInfo mi = CreateMapInfoFromControls();
      Cursor = Cursors.Default;
      if (mi == null) return; // user canceled
      Cursor = Cursors.WaitCursor;
      var result = mapPublisher.Publish(mi);
      Cursor = Cursors.Default;
      if (result.Success)
      {
        var message = string.Format(Strings.MapPublish_Success, result.URL);
        MessageBox.Show(message, Strings.MapPublish_SuccessTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        Close();
      }
      else
      {
        MessageBox.Show(result.ErrorMessage, Strings.MapPublish_FailureTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private MapInfo CreateMapInfoFromControls()
    {
      MapInfo mapInfo = new MapInfo();
      var selectedMap = map.SelectedValue as MapInfo;
      if (selectedMap != null) mapInfo.ID = selectedMap.ID;
      mapInfo.Date = date.Value.ToUniversalTime();
      var selectedCategory = category.SelectedItem as Category;
      if (selectedCategory != null) mapInfo.CategoryID = selectedCategory.ID;
      mapInfo.Name = name.Text;
      mapInfo.MapName = mapName.Text;
      mapInfo.Organiser = organiser.Text;
      mapInfo.Country = country.Text;
      mapInfo.Discipline = type.Text;
      mapInfo.RelayLeg = relayLeg.Text;
      mapInfo.ResultListUrl = resultListUrl.Text;
      mapInfo.Comment = comment.Text;
      mapInfo.MapImageFileExtension = imageFormat.Text;

      IMapImageFileExporterDialog selector = null;
      switch (imageFormat.Text)
      {
        case "jpg":
          selector = new JpegPropertySelector
          {
            SizeCalculator = document.CalculateImageForExportSize
          };
          break;
        case "png":
        default:
          selector = new PngPropertySelector
          {
            SizeCalculator = document.CalculateImageForExportSize
          };
          break;
        //case "tiff":
        //  selector = new TiffPropertySelector
        //  {
        //    SizeCalculator = document.CalculateImageForExportSize
        //  };

        //  break;

      }

      if (selector.ShowPropertyDialog() == DialogResult.OK)
      {
        Application.DoEvents();

        // map image
        using (var ms = new MemoryStream())
        {
          var imageExporterProperties = new ImageExporterProperties()
          {
            EncodingInfo = selector.EncodingInfo,
            RouteDrawingMode = Document.RouteDrawingMode.Extended,
            ColorCodingAttribute = colorCodingAttribute,
            SecondaryColorCodingAttribute = secondaryColorCodingAttribute,
            PercentualSize = selector.PercentualImageSize,
            ColorRangeProperties = colorRangeProperties
          };
          var imageExporter = new ImageExporter(document, document.Sessions, ms)
          {
            Properties = imageExporterProperties 
          };
          imageExporter.Export();
          mapInfo.MapImageData = ms.ToArray();
        }

        // blank map image
        using (var ms = new MemoryStream())
        {
          var imageExporterProperties = new ImageExporterProperties()
          {
            EncodingInfo = selector.EncodingInfo,
            RouteDrawingMode = Document.RouteDrawingMode.None,
            PercentualSize = selector.PercentualImageSize
          };
          var blankImageExporter = new ImageExporter(document, document.Sessions, ms)
          {
            Properties = imageExporterProperties
          };
          blankImageExporter.Export();
          mapInfo.BlankMapImageData = ms.ToArray();
        }
        return mapInfo;
      }
      return null;
    }

    private void Connect()
    {
      mapPublisher.Password = password.Text;
      mapPublisher.Username = username.Text;
      mapPublisher.WebServiceUrl = webServiceURL.Text;

      allMaps.Clear();

      ConnectResult connectResult;
      GetAllMapsResult getAllMapsResult = null;
      GetAllCategoriesResult getAllCategoriesResult = null;
      try
      {
        Cursor = Cursors.WaitCursor;
        connectResult = mapPublisher.Connect();
        Cursor = Cursors.Default;
        if (connectResult.Success)
        {
          Cursor = Cursors.WaitCursor;
          SaveConnectionSettings();
          PopulateUsernameComboBox();
          getAllMapsResult = GetAllMaps();
          getAllCategoriesResult = mapPublisher.GetAllCategories();
        }
      }
      catch (Exception ex)
      {
        connectResult = new ConnectResult
        {
          Success = false,
          ErrorMessage = ex.Message,
          Version = null
        };
      }
      Cursor = Cursors.Default;
      map.DataSource = allMaps;

      var success = true;
      if (!connectResult.Success)
      {
        MessageBox.Show(connectResult.ErrorMessage, Strings.Error_MapPublishCommunication, MessageBoxButtons.OK, MessageBoxIcon.Error);
        success = false;
      }
      if (getAllMapsResult != null && !getAllMapsResult.Success)
      {
        MessageBox.Show(getAllMapsResult.ErrorMessage, Strings.Error_MapPublishCommunication, MessageBoxButtons.OK, MessageBoxIcon.Error);
        success = false;
      }
      if (getAllCategoriesResult != null && !getAllCategoriesResult.Success)
      {
        MessageBox.Show(getAllCategoriesResult.ErrorMessage, Strings.Error_MapPublishCommunication, MessageBoxButtons.OK, MessageBoxIcon.Error);
        success = false;
      }

      if (getAllCategoriesResult != null) category.DataSource = getAllCategoriesResult.Categories;

      SetControlEnabledState(success);
      if (map.Items.Count > 0) map.SelectedIndex = 0;
    }

    private GetAllMapsResult GetAllMaps()
    {
      GetAllMapsResult getAllMapsResult = mapPublisher.GetAllMaps();
      if (getAllMapsResult.Success)
      {
        allMaps = getAllMapsResult.Maps;
        var newMap = new MapInfo
                       {
                         ID = 0,
                         Name = string.Format("[{0}]", Strings.NewMap)
                       };
        allMaps.Insert(0, newMap);
      }
      return getAllMapsResult;
    }

    private void SetControlEnabledState(bool enabled)
    {
      mapLabel.Enabled = enabled;
      dateLabel.Enabled = enabled;
      categoryLabel.Enabled = enabled;
      nameLabel.Enabled = enabled;
      mapNameLabel.Enabled = enabled;
      organiserLabel.Enabled = enabled;
      countryLabel.Enabled = enabled;
      disciplineLabel.Enabled = enabled;
      commentLabel.Enabled = enabled;
      resultListUrlLabel.Enabled = enabled;
      relayLegLabel.Enabled = enabled;
      imageFormatLabel.Enabled = enabled;
      map.Enabled = enabled;
      date.Enabled = enabled;
      category.Enabled = enabled;
      name.Enabled = enabled;
      mapName.Enabled = enabled;
      organiser.Enabled = enabled;
      country.Enabled = enabled;
      type.Enabled = enabled;
      relayLeg.Enabled = enabled;
      resultListUrl.Enabled = enabled;
      comment.Enabled = enabled;
      imageFormat.Enabled = enabled;

      ok.Enabled = enabled;

      if(!enabled)
      {
        category.DataSource = null;
        category.Items.Clear();
        SetControlValues(new MapInfo() { Date = date.Value });
      }
    }

    private void SetControlValues(MapInfo mapInfo)
    {
      date.Value = mapInfo.Date.ToLocalTime();
      name.Text = mapInfo.Name;
      mapName.Text = mapInfo.MapName;
      foreach (var c in category.Items)
      {
        if (c is Category && (c as Category).ID == mapInfo.CategoryID) category.SelectedItem = c;
      }
      organiser.Text = mapInfo.Organiser;
      country.Text = mapInfo.Country;
      type.Text = mapInfo.Discipline;
      relayLeg.Text = mapInfo.RelayLeg;
      resultListUrl.Text = mapInfo.ResultListUrl;
      comment.Text = mapInfo.Comment;
    }

    private void SaveConnectionSettings()
    {
      bool found = false;
      foreach (var setting in Util.ApplicationSettings.PublishMapSettings)
      {
        if (webServiceURL.Text == setting.WebServiceURL && setting.Username == username.Text)
        {
          setting.Username = username.Text;
          setting.Password = SavePassword.Checked ? password.Text : null;
          found = true;
          Util.ApplicationSettings.PublishMapSettings.Remove(setting);
          Util.ApplicationSettings.PublishMapSettings.Insert(0, setting);
          break;
        }
      }

      if (!found)
      {
        var setting = new PublishMapSettingsItem
        {
          WebServiceURL = webServiceURL.Text,
          Username = username.Text,
          Password = SavePassword.Checked ? password.Text : null
        };
        Util.ApplicationSettings.PublishMapSettings.Insert(0, setting);
      }
    }

    private void PopulateUsernameComboBox()
    {
      username.Items.Clear();
      foreach (var setting in Util.ApplicationSettings.PublishMapSettings)
      {
        if (webServiceURL.Text == setting.WebServiceURL)
        {
          username.Items.Add(setting.Username);
        }
      }
    }

    #endregion
  }
}
