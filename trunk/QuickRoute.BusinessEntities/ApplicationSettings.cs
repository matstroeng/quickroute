using System;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using QuickRoute.BusinessEntities.Exporters;
using QuickRoute.BusinessEntities.GlobalizedProperties;
using QuickRoute.BusinessEntities.RouteProperties;

namespace QuickRoute.BusinessEntities
{
  [Serializable]
  public class ApplicationSettings : GlobalizedObject
  {
    private DocumentSettings defaultDocumentSettings = new DocumentSettings();
    [Obsolete]
    private string initialFolder;
    private Size windowSize;
    private Point windowLocation;
    private FormWindowState windowState;
    private int histogramHeight = 120;
    private List<string> recentDocumentFileNames = new List<string>();
    private CultureInfo uiCulture;
    private bool autoAdjustColorRangeInterval = true;
    private List<PublishMapSettingsItem> publishMapSettings = new List<PublishMapSettingsItem>();
    private List<string> recentMapImageFileNames = new List<string>();
    private List<string> recentRouteFileNames = new List<string>();
    private KmlProperties exportKmlProperties = new KmlProperties();
    private KmlMultipleFileExporterProperties exportKmlMultipleFileProperties = new KmlMultipleFileExporterProperties();
    private bool rightPanelVisible = true;
    private bool bottomPanelVisible = true;
    private bool panelSettingsUpdated = true; // flag for handling the rightPanelVisible and bottomPanelVisible when upgrading
    private double exportImagePercentualImageSize = 1;
    private double exportImageQuality = 0.8;
    private List<SessionPerson> recentPersons = new List<SessionPerson>();
    private List<string> dontRemindAboutVersions = new List<string>();
    private Dictionary<FileDialogType, string> initialFolders = new Dictionary<FileDialogType, string>();
    private ExportRouteDataSettings exportRouteDataSettings = new ExportRouteDataSettings();
    private SelectableRoutePropertyTypeCollection lapPropertyTypes = new SelectableRoutePropertyTypeCollection();
    private SelectableRoutePropertyTypeCollection momentaneousInfoPropertyTypes = new SelectableRoutePropertyTypeCollection();

    public DocumentSettings DefaultDocumentSettings
    {
      get { return defaultDocumentSettings; }
      set { defaultDocumentSettings = value; }
    }

    public Size WindowSize
    {
      get { return windowSize; }
      set { windowSize = value; }
    }

    public Point WindowLocation
    {
      get { return windowLocation; }
      set { windowLocation = value; }
    }

    public FormWindowState WindowState
    {
      get { return windowState; }
      set { windowState = value; }
    }

    public int HistogramHeight
    {
      get { return histogramHeight; }
      set { histogramHeight = value; }
    }

    public CultureInfo UiCulture
    {
      get { return uiCulture; }
      set { uiCulture = value; }
    }

    public bool AutoAdjustColorRangeInterval
    {
      get { return autoAdjustColorRangeInterval; }
      set { autoAdjustColorRangeInterval = value; }
    }

    public List<PublishMapSettingsItem> PublishMapSettings
    {
      get { return publishMapSettings; }
      set { publishMapSettings = value; }
    }

    public List<string> RecentDocumentFileNames
    {
      get { return recentDocumentFileNames ?? new List<string>(); }
    }

    public List<string> RecentMapImageFileNames
    {
      get { return recentMapImageFileNames ?? new List<string>(); }
    }

    public List<string> RecentRouteFileNames
    {
      get { return recentRouteFileNames ?? new List<string>(); }
    }

    public KmlProperties ExportKmlProperties
    {
      get { EnsureNotNull(ref exportKmlProperties); return exportKmlProperties; }
      set { exportKmlProperties = value; }
    }

    public KmlMultipleFileExporterProperties ExportKmlMultipleFileProperties
    {
      get { EnsureNotNull(ref exportKmlMultipleFileProperties); return exportKmlMultipleFileProperties; }
      set { exportKmlMultipleFileProperties = value; }
    }

    public bool RightPanelVisible
    {
      get
      {
        if (!panelSettingsUpdated) UpdatePanelSettings();
        return rightPanelVisible;
      }
      set { rightPanelVisible = value; }
    }

    public bool BottomPanelVisible
    {
      get
      {
        if (!panelSettingsUpdated) UpdatePanelSettings();
        return bottomPanelVisible;
      }
      set { bottomPanelVisible = value; }
    }

    private void UpdatePanelSettings()
    {
      panelSettingsUpdated = true;
      rightPanelVisible = true;
      bottomPanelVisible = true;
    }

    public double ExportImageQuality
    {
      get { return exportImageQuality; }
      set { exportImageQuality = value; }
    }

    public double ExportImagePercentualImageSize
    {
      get { return exportImagePercentualImageSize; }
      set { exportImagePercentualImageSize = value; }
    }

    public List<SessionPerson> RecentPersons
    {
      get { EnsureNotNull(ref recentPersons); return recentPersons; }
      set { recentPersons = value; }
    }

    public List<string> DontRemindAboutVersions
    {
      get { EnsureNotNull(ref dontRemindAboutVersions); return dontRemindAboutVersions; }
      set { dontRemindAboutVersions = value; }
    }

    public Dictionary<FileDialogType, string> InitialFolders
    {
      get { EnsureNotNull(ref initialFolders); return initialFolders; }
      set { initialFolders = value; }
    }

    public ExportRouteDataSettings ExportRouteDataSettings
    {
      get { EnsureNotNull(ref exportRouteDataSettings); return exportRouteDataSettings; }
      set { exportRouteDataSettings = value; }
    }

    public static SelectableRoutePropertyTypeCollection AvailableLapPropertyTypes
    {
      get
      {
          return new SelectableRoutePropertyTypeCollection()
                 {
                   new SelectableRoutePropertyType(typeof (LapNumber), true),
                   new SelectableRoutePropertyType(typeof (RouteProperties.ElapsedTime), true),
                   new SelectableRoutePropertyType(typeof (StraightLineDistance), true),
                   new SelectableRoutePropertyType(typeof (RouteDistance), true),
                   new SelectableRoutePropertyType(typeof (AverageStraightLinePace), true),
                   new SelectableRoutePropertyType(typeof (AverageRoutePace), true),
                   new SelectableRoutePropertyType(typeof (RouteToStraightLine), true),
                   new SelectableRoutePropertyType(typeof (AverageHeartRate), true),
                   new SelectableRoutePropertyType(typeof (Ascent), true),
                   new SelectableRoutePropertyType(typeof (Descent), true),
                   new SelectableRoutePropertyType(typeof (ElapsedTimeFromStart), false),
                   new SelectableRoutePropertyType(typeof (Time), false),
                   new SelectableRoutePropertyType(typeof (CircleTimeAtStartOfSpan), false),
                   new SelectableRoutePropertyType(typeof (CircleTimeAtEndOfSpan), false),
                   new SelectableRoutePropertyType(typeof (StraightLineDistanceFromStart), false),
                   new SelectableRoutePropertyType(typeof (RouteDistanceFromStart), false),
                   new SelectableRoutePropertyType(typeof (RouteToStraightLineFromStart), false),
                   new SelectableRoutePropertyType(typeof (AverageStraightLinePace), false),
                   new SelectableRoutePropertyType(typeof (AverageStraightLinePaceFromStart), false),
                   new SelectableRoutePropertyType(typeof (AverageRoutePace), false),
                   new SelectableRoutePropertyType(typeof (AverageRoutePaceFromStart), false),
                   new SelectableRoutePropertyType(typeof (Pace), false),
                   new SelectableRoutePropertyType(typeof (AverageStraightLineSpeed), false),
                   new SelectableRoutePropertyType(typeof (AverageStraightLineSpeedFromStart), false),
                   new SelectableRoutePropertyType(typeof (AverageRouteSpeed), false),
                   new SelectableRoutePropertyType(typeof (AverageRouteSpeedFromStart), false),
                   new SelectableRoutePropertyType(typeof (Speed), false),
                   new SelectableRoutePropertyType(typeof (AverageHeartRateFromStart), false),
                   new SelectableRoutePropertyType(typeof (MinHeartRate), false),
                   new SelectableRoutePropertyType(typeof (MinHeartRateFromStart), false),
                   new SelectableRoutePropertyType(typeof (MaxHeartRate), false),
                   new SelectableRoutePropertyType(typeof (MaxHeartRateFromStart), false),
                   new SelectableRoutePropertyType(typeof (RouteProperties.HeartRate), false),
                   new SelectableRoutePropertyType(typeof (AltitudeDifference), false),
                   new SelectableRoutePropertyType(typeof (AltitudeDifferenceFromStart), false),
                   new SelectableRoutePropertyType(typeof (AscentFromStart), false),
                   new SelectableRoutePropertyType(typeof (DescentFromStart), false),
                   new SelectableRoutePropertyType(typeof (Inclination), false),
                   new SelectableRoutePropertyType(typeof (AverageInclination), false),
                   new SelectableRoutePropertyType(typeof (AverageInclinationFromStart), false),
                   new SelectableRoutePropertyType(typeof (RouteProperties.Altitude), false),
                   new SelectableRoutePropertyType(typeof (AverageDirectionDeviationToNextLap), false),
                   new SelectableRoutePropertyType(typeof (AverageDirectionDeviationToNextLapFromStart), false),
                   new SelectableRoutePropertyType(typeof (DirectionDeviationToNextLap), false),
                   new SelectableRoutePropertyType(typeof (Direction), false),
                   new SelectableRoutePropertyType(typeof (Location), false),
                 };
      }
    }

    public static SelectableRoutePropertyTypeCollection AvailableMomentaneousInfoPropertyTypes
    {
      get
      {
        return new SelectableRoutePropertyTypeCollection()
                 {
                   new SelectableRoutePropertyType(typeof (LapNumber), true),
                   new SelectableRoutePropertyType(typeof (Time), true),
                   new SelectableRoutePropertyType(typeof (RouteProperties.ElapsedTime), true),
                   new SelectableRoutePropertyType(typeof (ElapsedTimeFromStart), true),
                   new SelectableRoutePropertyType(typeof (CircleTimeBackward), true),
                   new SelectableRoutePropertyType(typeof (CircleTimeForward), true),
                   new SelectableRoutePropertyType(typeof (StraightLineDistance), true),
                   new SelectableRoutePropertyType(typeof (RouteDistance), true),
                   new SelectableRoutePropertyType(typeof (RouteDistanceFromStart), true),
                   new SelectableRoutePropertyType(typeof (Pace), true),
                   new SelectableRoutePropertyType(typeof (Speed), true),
                   new SelectableRoutePropertyType(typeof (RouteProperties.HeartRate), true),
                   new SelectableRoutePropertyType(typeof (RouteProperties.Altitude), true),
                   new SelectableRoutePropertyType(typeof (Inclination), true),
                   new SelectableRoutePropertyType(typeof (AscentFromStart), true),
                   new SelectableRoutePropertyType(typeof (DescentFromStart), true),
                   new SelectableRoutePropertyType(typeof (DirectionDeviationToNextLap), true),
                   new SelectableRoutePropertyType(typeof (Location), true),
                   new SelectableRoutePropertyType(typeof (StraightLineDistanceFromStart), false),
                   new SelectableRoutePropertyType(typeof (RouteToStraightLine), false),
                   new SelectableRoutePropertyType(typeof (RouteToStraightLineFromStart), false),
                   new SelectableRoutePropertyType(typeof (AverageStraightLinePace), false),
                   new SelectableRoutePropertyType(typeof (AverageStraightLinePaceFromStart), false),
                   new SelectableRoutePropertyType(typeof (AverageRoutePace), false),
                   new SelectableRoutePropertyType(typeof (AverageRoutePaceFromStart), false),
                   new SelectableRoutePropertyType(typeof (AverageStraightLineSpeed), false),
                   new SelectableRoutePropertyType(typeof (AverageStraightLineSpeedFromStart), false),
                   new SelectableRoutePropertyType(typeof (AverageRouteSpeed), false),
                   new SelectableRoutePropertyType(typeof (AverageRouteSpeedFromStart), false),
                   new SelectableRoutePropertyType(typeof (AverageHeartRate), false),
                   new SelectableRoutePropertyType(typeof (AverageHeartRateFromStart), false),
                   new SelectableRoutePropertyType(typeof (MinHeartRate), false),
                   new SelectableRoutePropertyType(typeof (MinHeartRateFromStart), false),
                   new SelectableRoutePropertyType(typeof (MaxHeartRate), false),
                   new SelectableRoutePropertyType(typeof (MaxHeartRateFromStart), false),
                   new SelectableRoutePropertyType(typeof (AltitudeDifference), false),
                   new SelectableRoutePropertyType(typeof (AltitudeDifferenceFromStart), false),
                   new SelectableRoutePropertyType(typeof (Ascent), false),
                   new SelectableRoutePropertyType(typeof (Descent), false),
                   new SelectableRoutePropertyType(typeof (AverageInclination), false),
                   new SelectableRoutePropertyType(typeof (AverageInclinationFromStart), false),
                   new SelectableRoutePropertyType(typeof (AverageDirectionDeviationToNextLap), false),
                   new SelectableRoutePropertyType(typeof (AverageDirectionDeviationToNextLapFromStart), false),
                   new SelectableRoutePropertyType(typeof (Direction), false),
                 };
      }
    }

    public SelectableRoutePropertyTypeCollection LapPropertyTypes
    {
      get
      {
        if (lapPropertyTypes == null || lapPropertyTypes.Count == 0)
        {
          lapPropertyTypes = AvailableLapPropertyTypes;
        }
        return lapPropertyTypes;
      }
      set { lapPropertyTypes = value; }
    }

    public SelectableRoutePropertyTypeCollection MomentaneousInfoPropertyTypes
    {
      get
      {
        if (momentaneousInfoPropertyTypes == null || momentaneousInfoPropertyTypes.Count == 0)
        {
          momentaneousInfoPropertyTypes = AvailableMomentaneousInfoPropertyTypes;
        }
        else if (momentaneousInfoPropertyTypes.Count < AvailableMomentaneousInfoPropertyTypes.Count)
        {
          // make sure new property types are propagated to the settings
          foreach (var mipt in AvailableMomentaneousInfoPropertyTypes)
          {
            if (!momentaneousInfoPropertyTypes.ContainsRoutePropertyType(mipt.RoutePropertyType))
            {
              momentaneousInfoPropertyTypes.Add(mipt);
            }
          }
        }
        return momentaneousInfoPropertyTypes;
      }
      set { momentaneousInfoPropertyTypes = value; }
    }

    public void AddRecentDocumentFileName(string fileName)
    {
      AddRecentFileName(fileName, ref recentDocumentFileNames, 0);
    }

    public void AddRecentDocumentFileName(string fileName, int maxNoOfItems)
    {
      AddRecentFileName(fileName, ref recentDocumentFileNames, maxNoOfItems);
    }

    public void AddRecentMapImageFileName(string fileName)
    {
      AddRecentFileName(fileName, ref recentMapImageFileNames, 0);
    }

    public void AddRecentMapImageFileName(string fileName, int maxNoOfItems)
    {
      AddRecentFileName(fileName, ref recentMapImageFileNames, maxNoOfItems);
    }

    public void AddRecentRouteFileName(string fileName)
    {
      AddRecentFileName(fileName, ref recentRouteFileNames, 0);
    }

    public void AddRecentRouteFileName(string fileName, int maxNoOfItems)
    {
      AddRecentFileName(fileName, ref recentRouteFileNames, maxNoOfItems);
    }

    public void AddRecentPerson(SessionPerson person)
    {
      if (person.Name != "" || person.Club != "") AddRecentPerson(person, ref recentPersons, 0);
    }

    private static void AddRecentFileName(string fileName, ref List<string> fileNames, int maxNoOfItems)
    {
      if (fileNames == null) fileNames = new List<string>();
      fileNames.RemoveAll(s => s.ToLower() == fileName.ToLower());
      fileNames.Insert(0, fileName);
      if (maxNoOfItems > 0 && fileNames.Count > maxNoOfItems)
      {
        fileNames.RemoveRange(maxNoOfItems, fileNames.Count - maxNoOfItems);
      }
    }

    private static void AddRecentPerson(SessionPerson person, ref List<SessionPerson> persons, int maxNoOfItems)
    {
      if (persons == null) persons = new List<SessionPerson>();
      persons.RemoveAll(p => p.Name.ToLower() == person.Name.ToLower() && p.Club.ToLower() == person.Club.ToLower());
      persons.Insert(0, person);
      if (maxNoOfItems > 0 && persons.Count > maxNoOfItems)
      {
        persons.RemoveRange(maxNoOfItems, persons.Count - maxNoOfItems);
      }
    }

    private static void EnsureNotNull<T>(ref T item) where T : class, new()
    {
      if (item == null) item = new T();
    }

    public ApplicationSettings Copy()
    {
      var ms = new MemoryStream();
      var bf = new BinaryFormatter();
      bf.Serialize(ms, this);
      ms.Flush();
      ms.Seek(0, SeekOrigin.Begin);
      return bf.Deserialize(ms) as ApplicationSettings;
    }

    [Serializable]
    public enum FileDialogType
    {
      OpenDocument,
      SaveDocument,
      ImportMapImage,
      ImportRoute,
      ExportImage,
      ExportFile
    }

  }

  [Serializable]
  public class PublishMapSettingsItem
  {
    public string WebServiceURL { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
  }
}
