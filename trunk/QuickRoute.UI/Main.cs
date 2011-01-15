using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO.Ports;
using System.Net;
using System.Text;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;
using QuickRoute.BusinessEntities.Actions;
using QuickRoute.BusinessEntities.Exporters;
using QuickRoute.BusinessEntities.Numeric;
using QuickRoute.BusinessEntities.RouteProperties;
using QuickRoute.Common;
using QuickRoute.Controls;
using QuickRoute.UI.Classes;
using QuickRoute.UI.Forms;
using System.IO;
using QuickRoute.Resources;

namespace QuickRoute.UI
{
  public partial class Main : Form
  {
    private string untitledDocumentFileNameProposal;
    private bool documentChanged;
    ColorfulHistogram lapHistogram;
    private List<LapInfo> lapInfoList;
    private Dictionary<Type, Rectangle> momentaneousInfoLabelRectangles;
    private readonly Padding momentaneousInfoLabelPadding = new Padding(6, 3, 6, 3);
    private Bitmap momentaneousInfoPanelBackBuffer;
    private Graphics momentaneousInfoPanelBackBufferGraphics;
    private RoutePropertyCacheManager cacheManager = new RoutePropertyCacheManager();

    private bool updatingUINow;
    private readonly bool startingUpNow;
    private bool updatingZoomNow;
    private bool openingDocumentNow;

    private readonly Stack<IAction> undoStack = new Stack<IAction>();
    private readonly Stack<IAction> redoStack = new Stack<IAction>();

    private int workLevel;
    private Cursor previousCursor;
    private int lapSortOrderColumnIndex;
    private SortOrder lapSortOrder;
    private int lapIndexToDelete;

    private MainFormState formState;

    public Main()
    {
      startingUpNow = true;
      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
      Application.ThreadException += Application_ThreadException;

      var log4NetLogFileName = ConfigurationManager.AppSettings.Get("log4netLogFileName") ??
                               CommonUtil.GetApplicationDataPath() + "QuickRoute.log";
      LogUtil.Configure(log4NetLogFileName);
      Util.EnsureApplicationDataFolderExists();
      Util.UpdateApplicationSettingsToCurrentVersion();
      CommonUtil.ClearTempFolder();

      if (ApplicationSettings.UiCulture == null)
      {
        Util.SelectUICulture(false);
      }
      if (ApplicationSettings.UiCulture != null)
      {
        System.Threading.Thread.CurrentThread.CurrentUICulture = ApplicationSettings.UiCulture;
      }

      InitializeComponent();

      Util.CheckForNewVersion();

      // Display the splash screen
      //SplashScreen SplashScreen = new SplashScreen();
      //SplashScreen.Show();

      toolStripZoom.SelectedText = "100%";
      gradientAlphaAdjustment.TrackBarControl.Minimum = -10;
      gradientAlphaAdjustment.TrackBarControl.Maximum = 10;

      // add color coded waypoint attributes to combobox
      List<WaypointAttributeString> cca = new List<WaypointAttributeString>
                                            {
                                              new WaypointAttributeString(WaypointAttribute.Pace),
                                              new WaypointAttributeString(WaypointAttribute.Speed),
                                              new WaypointAttributeString(WaypointAttribute.HeartRate),
                                              new WaypointAttributeString(WaypointAttribute.Altitude),
                                              new WaypointAttributeString(WaypointAttribute.DirectionDeviationToNextLap),
                                              new WaypointAttributeString(WaypointAttribute.MapReadingDuration)
                                            };
      colorCodingAttributes.ComboBox.DataSource = cca;
      colorCodingAttributes.SelectedIndex = 0;

      routeLineWidth.NumericUpDownControl.DecimalPlaces = 1;
      routeLineMaskWidth.NumericUpDownControl.DecimalPlaces = 1;

      UpdateUI();
      UpdateRecentDocumentsListInMenu();
      CreateMomentaneousInfoPanelBackBuffer();
      //SplashScreen.Close();

      //check for /openInGoogleEarth flag
      if (Environment.GetCommandLineArgs().Length == 3 && Environment.GetCommandLineArgs()[1].ToLower() == "/openingoogleearth")
      {
        OpenInGoogleEarthFromCommandLine(Environment.GetCommandLineArgs()[2]);
      }

      startingUpNow = false;
    }

    ~Main()
    {
      if (momentaneousInfoPanelBackBuffer != null) momentaneousInfoPanelBackBuffer.Dispose();
      if (momentaneousInfoPanelBackBufferGraphics != null) momentaneousInfoPanelBackBufferGraphics.Dispose();
      CommonUtil.ClearTempFolder();
    }

    #region Public properties

    public ApplicationSettings ApplicationSettings
    {
      get
      {
        return Util.ApplicationSettings;
      }
      set
      {
        Util.ApplicationSettings = value;
      }
    }

    #endregion

    #region User interface commands

    private void NewDocument()
    {
      NewDocument(null);
    }

    private void NewDocument(string routeFileName)
    {
      if (documentChanged)
      {
        // save any unsaved document
        if (QuerySaveDocument() == DialogResult.Cancel) return;
      }

      using (var cnf = new CreateNewForm(routeFileName))
      {
        if (ApplicationSettings.RecentPersons.Count == 0)
          ApplicationSettings.AddRecentPerson(new SessionPerson() { Name = Environment.UserName });
        cnf.SetPersons(ApplicationSettings.RecentPersons);
        if (cnf.ShowDialog() == DialogResult.OK)
        {
          BeginWork();
          try
          {
            openingDocumentNow = true;
            ApplicationSettings.AddRecentPerson(cnf.Person);
            canvas.PreventRedraw = true;
            canvas.Document = new Document(cnf.Map, cnf.ImportResult.Route, cnf.ImportResult.Laps, cnf.InitialTransformation.TransformationMatrix, cnf.InitialTransformation.ProjectionOrigin,
                                           ApplicationSettings.DefaultDocumentSettings.Copy());
            canvas.CurrentSession = canvas.Document.Sessions[0];
            canvas.CurrentSession.SessionInfo.Person = cnf.Person;
            canvas.SelectedSessions = new SessionCollection { canvas.CurrentSession };
            InitColorRangeIntervals();
            untitledDocumentFileNameProposal = cnf.FileNameProposal;
            documentChanged = true;
            updatingUINow = true;
            PopulateSessionList();
            updatingUINow = false;
            ResetActionStacks();
            lapSortOrderColumnIndex = 0;
            lapSortOrder = SortOrder.Ascending;
            CalculateLapInfo();
            updatingUINow = true;
            laps.Rows[laps.Rows.Count - 1].Selected = true;
            updatingUINow = false;
            CalculateMomentaneousInfoLabelRectangles();
            CreateLapHistogram();
            DrawLapHistogram();
            CreateLineGraph();
            DrawLineGraph();
            canvas.PreventRedraw = false;
            canvas.Init();
            canvas.CurrentMouseTool = Canvas.MouseTool.AdjustRoute;
            openingDocumentNow = false;
          }
          catch (Exception ex)
          {
            canvas.Document = null;
            Util.ShowExceptionMessageBox(ex, Strings.Error_NewDocument);
            documentChanged = false;
          }
          UpdateUI();
          EndWork();
        }
      }
    }

    private void InitColorRangeIntervals()
    {
      var was = new[]
                {
                  WaypointAttribute.Pace, 
                  WaypointAttribute.Speed, 
                  WaypointAttribute.HeartRate,
                  WaypointAttribute.Altitude,
                  WaypointAttribute.DirectionDeviationToNextLap, 
                  WaypointAttribute.MapReadingDuration 
                };
      Route r = canvas.CurrentSession.Route;
      foreach (WaypointAttribute wa in was)
      {
        if (ApplicationSettings.AutoAdjustColorRangeInterval)
        {

          List<double?> values = r.GetOrderedValues(wa, GetAutoAdjustColorRangeIntervalPercentages(wa), new TimeSpan(0, 0, 1));
          if (values.Count > 1 && values[0].HasValue && values[1].HasValue)
          {
            double span = values[1].Value - values[0].Value;
            values[0] -= 0.1 * span;
            values[0] = Math.Max(0, values[0].Value);
            values[1] += 0.1 * span;
            if (wa == WaypointAttribute.Pace) values[1] = Math.Min(30 * 60, values[1].Value);
            if (wa == WaypointAttribute.DirectionDeviationToNextLap) values[0] = 0;
            if (wa == WaypointAttribute.MapReadingDuration) values[0] = 0;
            UpdateColorRangeInterval(wa, values[0].Value, values[1].Value, true);
          }
        }
        else
        {
          ColorRange cr =
            ApplicationSettings.DefaultDocumentSettings.DefaultSessionSettings.RouteLineSettingsCollection[wa].ColorRange;
          UpdateColorRangeInterval(wa, cr.StartValue, cr.EndValue, true);
        }
      }
    }

    private static List<double> GetAutoAdjustColorRangeIntervalPercentages(WaypointAttribute attribute)
    {
      // use different start and end percentages for different waypoint attributes
      switch (attribute)
      {
        case WaypointAttribute.Altitude:
          return new List<double>(new double[] { 0, 1 });
        case WaypointAttribute.HeartRate:
          return new List<double>(new double[] { 0.1, 1 });
        case WaypointAttribute.DirectionDeviationToNextLap:
          return new List<double>(new double[] { 0, 0.9 });
        case WaypointAttribute.MapReadingDuration:
          return new List<double>(new double[] { 0, 1.1 });
        default:
          return new List<double>(new double[] { 0.1, 0.9 });
      }
    }

    private void OpenDocumentShowDialog()
    {
      if (documentChanged)
      {
        // save any unsaved document
        if (QuerySaveDocument() == DialogResult.Cancel) return;
      }

      using (var ofd = new OpenFileDialog
                         {
                           Title = Strings.OpenQuickRouteFileTitle,
                           Filter = (Strings.FileFilter_AllQuickRouteFiles + "|" +
                                     Strings.FileFilter_QuickRouteFiles + "|" +
                                     Strings.FileFilter_QuickRoute10Files + "|" +
                                     Strings.FileFilter_JpegFilesExportedFromQuickRoute + "|" +
                                     Strings.FileFilter_AllFiles),
                           FilterIndex = 1,
                           AddExtension = true,
                           CheckFileExists = true
                         })
      {
        if (ApplicationSettings.InitialFolders.ContainsKey(ApplicationSettings.FileDialogType.OpenDocument))
        {
          ofd.InitialDirectory = ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.OpenDocument];
        }

        if (ofd.ShowDialog() == DialogResult.OK)
        {
          QuickRouteFileFormat fileFormat;
          switch (ofd.FilterIndex)
          {
            case 2:
              fileFormat = QuickRouteFileFormat.Qrt;
              break;
            case 3:
              fileFormat = QuickRouteFileFormat.Xml;
              break;
            case 4:
              fileFormat = QuickRouteFileFormat.Jpeg;
              break;
            default:
              fileFormat = Document.GetFileFormat(ofd.FileName);
              break;
          }
          OpenDocumentWithFileName(ofd.FileName, fileFormat, false);
          ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.OpenDocument] =
            Path.GetDirectoryName(ofd.FileName);
        }
      }
    }

    private bool OpenDocumentWithFileName(string fileName, bool querySaveDocument)
    {
      return OpenDocumentWithFileName(fileName, QuickRouteFileFormat.Unknown, querySaveDocument);
    }

    private bool OpenDocumentWithFileName(string fileName, QuickRouteFileFormat fileFormat, bool querySaveDocument)
    {
      var result = false;
      if (documentChanged && querySaveDocument)
      {
        // save any unsaved document
        if (QuerySaveDocument() == DialogResult.Cancel) return result;
      }

      try
      {
        BeginWork();
        openingDocumentNow = true;
        canvas.PreventRedraw = true;

        fileName = CommonUtil.GetDownloadedFileName(fileName);

        Document document = null;
        switch (fileFormat)
        {
          case QuickRouteFileFormat.Qrt:
            document = Document.OpenFromQrt(fileName);
            break;
          case QuickRouteFileFormat.Xml:
            document = Document.OpenFromXml(fileName, ApplicationSettings.DefaultDocumentSettings.Copy());
            break;
          case QuickRouteFileFormat.Jpeg:
            document = Document.OpenFromJpeg(fileName, ApplicationSettings.DefaultDocumentSettings.Copy());
            break;
          case QuickRouteFileFormat.Unknown:
            document = Document.Open(fileName, ApplicationSettings.DefaultDocumentSettings.Copy());
            break;
        }
        if (document == null)
        {
          // unrecognized file
          openingDocumentNow = false;
          canvas.PreventRedraw = false;
          EndWork();
          MessageBox.Show(string.Format(Strings.UnrecognizedFileFormat, fileName), Strings.Error_OpenDocument, MessageBoxButtons.OK, MessageBoxIcon.Error);
          return result;
        }
        canvas.Document = document;
        canvas.CurrentSession = canvas.Document.Sessions[0];
        canvas.SelectedSessions = new SessionCollection();
        canvas.SelectedSessions.Add(canvas.CurrentSession);
        //if (ApplicationSettings.AutoAdjustColorRangeInterval) PerformColorRangeIntervalAutoAdjustment();
        updatingUINow = true;
        PopulateSessionList();
        updatingUINow = false;
        ResetActionStacks();
        lapSortOrderColumnIndex = 0;
        lapSortOrder = SortOrder.Ascending;
        CalculateLapInfo();
        updatingUINow = true;
        laps.Rows[laps.Rows.Count - 1].Selected = true;
        updatingUINow = false;
        CalculateMomentaneousInfoLabelRectangles();
        CreateLapHistogram();
        DrawLapHistogram();
        CreateLineGraph();
        DrawLineGraph();
        canvas.PreventRedraw = false;
        canvas.Init();
        canvas.CurrentMouseTool = Canvas.MouseTool.Pointer;
        if (canvas.Document.FileFormat == QuickRouteFileFormat.Qrt || canvas.Document.FileFormat == QuickRouteFileFormat.Jpeg)
        {
          Util.InsertIntoRecentDocumentsList(fileName);
          UpdateRecentDocumentsListInMenu();
        }
        if (canvas.Document.FileFormat != QuickRouteFileFormat.Qrt) untitledDocumentFileNameProposal = canvas.Document.FileName + ".qrt";
        result = true;
      }
      catch (Exception ex)
      {
        canvas.Document = null;
        Util.ShowExceptionMessageBox(ex, Strings.Error_OpenDocument);
      }
      documentChanged = false;
      UpdateUI();
      openingDocumentNow = false;
      EndWork();
      return result;
    }

    private void PopulateSessionList()
    {
      sessions.Items.Clear();
      foreach (var s in canvas.Document.Sessions)
      {
        string text = (s.SessionInfo == null || s.SessionInfo.Person == null ? "" : s.SessionInfo.Person.Name);
        if (text == "") text = (sessions.Items.Count + 1).ToString();
        sessions.Items.Add(text, canvas.SelectedSessions.Contains(s));
      }
      if (sessions.Items.Count > 0) sessions.SelectedIndex = 0;
    }

    /// <summary>
    /// Saves the document using the current filename, or if an untitled file, queries for the filename in a dialog
    /// </summary>
    private void SaveDocument()
    {
      if (canvas.Document.FileName == null || canvas.Document.FileFormat != QuickRouteFileFormat.Qrt)
      {
        SaveDocumentShowDialog();
      }
      else
      {
        SaveDocumentWithFileName(canvas.Document.FileName);
      }
    }

    private void SaveDocumentShowDialog()
    {
      using (var sfd = new SaveFileDialog
                  {
                    Title = Strings.SaveAsTitle,
                    FileName =
                      (canvas.Document.FileName != null && canvas.Document.FileFormat == QuickRouteFileFormat.Qrt
                         ? Path.GetFileName(canvas.Document.FileName)
                         : untitledDocumentFileNameProposal),
                    Filter = Strings.FileFilter_QuickRouteFiles,
                    FilterIndex = 1,
                    AddExtension = true,
                    OverwritePrompt = true
                  })
      {
        if (canvas.Document.FileName != null)
        {
          sfd.InitialDirectory = Path.GetDirectoryName(canvas.Document.FileName);
        }
        else if (ApplicationSettings.InitialFolders.ContainsKey(ApplicationSettings.FileDialogType.SaveDocument))
        {
          sfd.InitialDirectory = ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.SaveDocument];
        }

        if (sfd.ShowDialog() == DialogResult.OK)
        {
          SaveDocumentWithFileName(sfd.FileName);
          ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.SaveDocument] =
            Path.GetDirectoryName(sfd.FileName);
          UpdateUI();
        }
      }
    }

    private void SaveDocumentWithFileName(string fileName)
    {
      BeginWork();
      try
      {
        canvas.Document.Save(fileName);
        documentChanged = false;
        Util.InsertIntoRecentDocumentsList(fileName);
        UpdateRecentDocumentsListInMenu();
        UpdateUI();
      }
      catch (Exception ex)
      {
        Util.ShowExceptionMessageBox(ex, Strings.Error_SaveDocument);
      }
      EndWork();
    }

    private DialogResult QuerySaveDocument()
    {
      DialogResult result = MessageBox.Show(
       String.Format(Strings.PromptSaveChanges, (canvas.Document.FileName == null ? Strings.Untitled : Path.GetFileName(canvas.Document.FileName))),
       Strings.QuickRoute,
       MessageBoxButtons.YesNoCancel,
       MessageBoxIcon.Exclamation);

      if (result == DialogResult.Yes) SaveDocument();
      return result;
    }

    private void ExportImage()
    {
      using (var sfd = new SaveFileDialog
                         {
                           Title = Strings.ExportImageTitle,
                           Filter = (Strings.FileFilter_JpegFiles + "|" +
                                     Strings.FileFilter_PngFiles + "|" +
                                     Strings.FileFilter_TiffFiles),
                           FileName =
                             (canvas.Document != null && canvas.Document.FileName != null
                                ? Path.GetFileNameWithoutExtension(canvas.Document.FileName)
                                : Path.GetFileNameWithoutExtension(untitledDocumentFileNameProposal)),
                           FilterIndex = 1,
                           AddExtension = true,
                           OverwritePrompt = true
                         })
      {
        if (ApplicationSettings.InitialFolders.ContainsKey(ApplicationSettings.FileDialogType.ExportFile))
        {
          sfd.InitialDirectory = ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.ExportFile];
        }

        if (sfd.ShowDialog() == DialogResult.OK)
        {
          try
          {
            // TODO: change to saved PercentualImageSize and Quality when dialog is working
            IMapImageFileExporterDialog selector = null;
            switch (sfd.FilterIndex)
            {
              case 1: // jpeg
                selector = new JpegPropertySelector
                             {
                               SizeCalculator = canvas.Document.CalculateImageForExportSize,
                               PercentualImageSize = 1,
                               //ApplicationSettings.ExportImagePercentualImageSize,
                               Quality = 0.8 //ApplicationSettings.ExportImageQuality,
                             };
                break;

              case 2: // png
                selector = new PngPropertySelector
                             {
                               SizeCalculator = canvas.Document.CalculateImageForExportSize,
                               PercentualImageSize = 1,
                               //ApplicationSettings.ExportImagePercentualImageSize
                             };
                break;

              default: // tiff
                selector = new TiffPropertySelector
                             {
                               SizeCalculator = canvas.Document.CalculateImageForExportSize,
                               PercentualImageSize = 1,
                               //ApplicationSettings.ExportImagePercentualImageSize
                             };
                break;
            }
            if (selector.ShowPropertyDialog() == DialogResult.OK)
            {
              BeginWork();
              using (var fs = new FileStream(sfd.FileName, FileMode.Create))
              {
                var imageExporterProperties = new ImageExporterProperties()
                                                {
                                                  EncodingInfo = selector.EncodingInfo,
                                                  RouteDrawingMode = Document.RouteDrawingMode.Extended,
                                                  ColorCodingAttribute = SelectedColorCodingAttribute,
                                                  PercentualSize = selector.PercentualImageSize,
                                                  ColorRangeProperties = GetCurrentColorRangeProperties()
                                                };
                var imageExporter = new ImageExporter(canvas.Document, canvas.SelectedSessions, fs)
                                      {
                                        Properties = imageExporterProperties
                                      };
                imageExporter.Export();
                fs.Close();
              }
              ApplicationSettings.ExportImagePercentualImageSize = selector.PercentualImageSize;
              if (selector is JpegPropertySelector)
                ApplicationSettings.ExportImageQuality = ((JpegPropertySelector)selector).Quality; // ugly!
              ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.ExportImage] =
                Path.GetDirectoryName(sfd.FileName);
              EndWork();
            }
          }
          catch (Exception ex)
          {
            Util.ShowExceptionMessageBox(ex, Strings.Error_ExportImage);
          }
        }
      }
    }

    private void ExportGpx()
    {
      using (var sfd = new SaveFileDialog
      {
        Title = Strings.ExportGPXTitle,
        Filter = Strings.FileFilter_GpxFiles,
        FileName =
          ((canvas.Document.FileName != null
              ? Path.GetFileNameWithoutExtension(canvas.Document.FileName)
              : Path.GetFileNameWithoutExtension(untitledDocumentFileNameProposal))),
        FilterIndex = 1,
        AddExtension = true,
        OverwritePrompt = true
      })
      {
        if (ApplicationSettings.InitialFolders.ContainsKey(ApplicationSettings.FileDialogType.ExportFile))
        {
          sfd.InitialDirectory = ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.ExportFile];
        }

        if (sfd.ShowDialog() == DialogResult.OK)
        {
          try
          {
            BeginWork();
            using (var stream = new FileStream(sfd.FileName, FileMode.Create))
            {
              var gpxExporter = new GpxExporter(canvas.CurrentSession, stream);
              gpxExporter.Export();
              stream.Close();
            }
            ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.ExportImage] =
              Path.GetDirectoryName(sfd.FileName);
            EndWork();
          }
          catch (Exception ex)
          {
            EndWork();
            Util.ShowExceptionMessageBox(ex, Strings.Error_ExportRoute);
          }
        }
      }
    }

    private void ExportKmz()
    {
      using (var kmlPropertySelector = new KmlPropertySelectorDialog(ApplicationSettings.ExportKmlProperties))
      {

        var result = kmlPropertySelector.ShowDialog();
        if (result == DialogResult.OK)
        {

          var sfd = new SaveFileDialog
                      {
                        Title = Strings.ExportKMZTitle,
                        Filter = Strings.FileFilter_KmzFiles,
                        FileName =
                          ((canvas.Document.FileName != null
                              ? Path.GetFileNameWithoutExtension(canvas.Document.FileName)
                              : Path.GetFileNameWithoutExtension(untitledDocumentFileNameProposal))),
                        FilterIndex = 1,
                        AddExtension = true,
                        OverwritePrompt = true
                      };
          if (ApplicationSettings.InitialFolders.ContainsKey(ApplicationSettings.FileDialogType.ExportFile))
          {
            sfd.InitialDirectory = ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.ExportFile];
          }

          if (sfd.ShowDialog() == DialogResult.OK)
          {
            try
            {
              BeginWork();
              ApplicationSettings.ExportKmlProperties = kmlPropertySelector.Properties;
              using (var stream = new FileStream(sfd.FileName, FileMode.Create))
              {
                CreateKmz(canvas.Document, stream, kmlPropertySelector.Properties, SelectedColorCodingAttribute,
                          GetCurrentColorRangeProperties());
                stream.Close();
              }
              ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.ExportFile] =
                Path.GetDirectoryName(sfd.FileName);
              EndWork();
            }
            catch (Exception ex)
            {
              EndWork();
              Util.ShowExceptionMessageBox(ex, Strings.Error_ExportRoute);
            }
          }
          sfd.Dispose();
        }
      }
    }

    private void ExportMultipleDocumentsToKmz()
    {
      // 1. select which documents to export in some list. it should be possible to include documents from different folders
      // 2. use kml export property dialog (or rather embedded user control)
      // 3. use some color range property selector user control (color coding attribute, gradient, intervals, etc)

      //var files = Directory.GetFiles(@"c:\Orientering\GPS\Träning", "*.qrt");
      //CreateKmz(files, stream, kmlPropertySelector.Properties);
    }

    private void ExportRouteData()
    {
      using (var exportRouteDialog = new ExportRouteDataDialog(ApplicationSettings.ExportRouteDataSettings))
      {
        if (exportRouteDialog.ShowDialog() == DialogResult.OK)
        {
          var sfd = new SaveFileDialog
          {
            Title = Strings.ExportRouteDataTitle,
            Filter = Strings.FileFilter_RouteDataFiles,
            FileName =
              ((canvas.Document.FileName != null
                  ? Path.GetFileNameWithoutExtension(canvas.Document.FileName)
                  : Path.GetFileNameWithoutExtension(untitledDocumentFileNameProposal))),
            FilterIndex = 1,
            AddExtension = true,
            OverwritePrompt = true
          };
          if (ApplicationSettings.InitialFolders.ContainsKey(ApplicationSettings.FileDialogType.ExportFile))
          {
            sfd.InitialDirectory = ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.ExportFile];
          }

          if (sfd.ShowDialog() == DialogResult.OK)
          {
            try
            {
              BeginWork();
              using (var stream = new FileStream(sfd.FileName, FileMode.Create))
              {
                var exporter = new RouteDataExporter(canvas.CurrentSession, stream) { Settings = exportRouteDialog.Settings };
                exporter.Export();
                stream.Close();
              }
              ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.ExportFile] =
                Path.GetDirectoryName(sfd.FileName);
              EndWork();
            }
            catch (Exception ex)
            {
              EndWork();
              Util.ShowExceptionMessageBox(ex, Strings.Error_ExportRoute);
            }
          }
          sfd.Dispose();
          ApplicationSettings.ExportRouteDataSettings = exportRouteDialog.Settings;
        }
      }
    }

    private ColorRangeProperties GetCurrentColorRangeProperties()
    {
      // create scale creator that starts at the start value slider and ends at the end value slider
      var t = colorRangeIntervalSlider.SliderControl.ScaleCreator.GetType();
      var sc = (ScaleCreatorBase)Activator.CreateInstance(t, colorRangeIntervalSlider.SliderControl.ColorRange.StartValue, colorRangeIntervalSlider.SliderControl.ColorRange.EndValue, 10, false);
      var wa = SelectedColorCodingAttribute;
      return new ColorRangeProperties
               {
                 NumericConverter = Util.GetNumericConverterFromWaypointAttribute(wa),
                 ScaleCreator = sc,
                 ScaleCaption = new WaypointAttributeString(wa).Name,
                 ScaleUnit = new WaypointAttributeString(wa).Unit
               };
    }

    private void OpenInGoogleEarth(string documentFileName, WaypointAttribute? colorCodingAttribute, ColorRangeProperties colorRangeProperties)
    {
      var document = Document.Open(documentFileName);
      try
      {
        OpenInGoogleEarth(document, colorCodingAttribute, colorRangeProperties);
      }
      catch (Exception)
      {
        MessageBox.Show(Strings.Error_InvalidQuickRouteFile, Strings.OpenInGoogleEarth, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
      }
    }


    private void OpenInGoogleEarth(Document document, WaypointAttribute? colorCodingAttribute, ColorRangeProperties colorRangeProperties)
    {
      if (document == null) throw new ArgumentNullException("document");
      // set default values
      if (!colorCodingAttribute.HasValue) colorCodingAttribute = SelectedColorCodingAttribute;
      if (colorRangeProperties == null) colorRangeProperties = GetCurrentColorRangeProperties();

      using (var kmlPropertySelector = new KmlPropertySelectorDialog(ApplicationSettings.ExportKmlProperties)
                                  {
                                    DialogTitle = Strings.OpenInGoogleEarth
                                  })
      {
        var result = kmlPropertySelector.ShowDialog();
        if (result == DialogResult.OK)
        {
          BeginWork();
          try
          {
            ApplicationSettings.ExportKmlProperties = kmlPropertySelector.Properties;
            using (var stream = new MemoryStream())
            {
              CreateKmz(document, stream, kmlPropertySelector.Properties, colorCodingAttribute.Value,
                        colorRangeProperties);
              GoogleEarthUtil.OpenInGoogleEarth(stream);
              stream.Close();
            }
            EndWork();
          }
          catch (Exception ex)
          {
            EndWork();
            Util.ShowExceptionMessageBox(Strings.Error_GoogleEarthNotInstalledMessage, ex,
                                         Strings.Error_GoogleEarthNotInstalledTitle);
          }
        }
      }
    }

    private void OpenMultipleFilesInGoogleEarth()
    {
      var initialFileNames = new List<string>();
      if (canvas.Document != null && canvas.Document.FileName != null)
      {
        initialFileNames.Add(canvas.Document.FileName);
      }

      using (var dialog = new OpenMultipleFilesInGoogleEarthDialog(initialFileNames, ApplicationSettings.ExportKmlMultipleFileProperties))
      {

        if (dialog.ShowDialog() == DialogResult.OK)
        {
          BeginWork();
          ApplicationSettings.ExportKmlMultipleFileProperties = dialog.MultipleFileProperties;
          using (var stream = new MemoryStream())
          {
            var downloadedFiles = DownloadFilesToTempDirectory(dialog.FileNames);
            var localFileNames = new List<string>();
            foreach (var df in downloadedFiles)
            {
              localFileNames.Add(df.LocalFileName);
            }
            var exporter = new KmlMultipleFilesExporter(localFileNames, dialog.MultipleFileProperties);
            if (exporter.Document == null)
            {
              MessageBox.Show(Strings.MultipleFileExporterNoValidFiles, Strings.OpenMultipleFilesInGoogleEarth,
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
              exporter.Export(stream);
              var dialogResult = DialogResult.OK;
              if (exporter.InvalidFileNames.Count > 0)
              {
                var invalidFileNames = new List<string>();
                foreach (var ifn in exporter.InvalidFileNames)
                {
                  foreach (var df in downloadedFiles)
                  {
                    if (df.LocalFileName == ifn) invalidFileNames.Add(df.FileName);
                  }
                }

                dialogResult =
                  MessageBox.Show(
                    string.Format(Strings.MultipleFileExporterSomeInvalidFiles,
                                  string.Join("\n", invalidFileNames.ToArray())), Strings.OpenMultipleFilesInGoogleEarth,
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
              }
              if (dialogResult == DialogResult.OK) GoogleEarthUtil.OpenInGoogleEarth(stream);
            }
            // delete temp files
            foreach (var downloadedFile in downloadedFiles)
            {
              if (downloadedFile.IsDownloaded) File.Delete(downloadedFile.LocalFileName);
            }
          }
          EndWork();
        }
      }
    }

    private List<DownloadedFile> DownloadFilesToTempDirectory(IEnumerable<string> fileNames)
    {
      var downloadedFileNames = new List<DownloadedFile>();
      foreach (var fileName in fileNames)
      {
        if (fileName.StartsWith("http"))
        {
          var client = new WebClient();
          var downloadedFileName = CommonUtil.GetTempFileName() + Path.GetExtension(fileName);
          try
          {
            client.DownloadFile(fileName, downloadedFileName);
            downloadedFileNames.Add(new DownloadedFile() { LocalFileName = downloadedFileName, Url = fileName });
          }
          catch (Exception ex)
          {
            // todo: better error message, and maybe some sort of download meter
            Util.ShowExceptionMessageBox(ex, Strings.QuickRoute);
          }
        }
        else
        {
          downloadedFileNames.Add(new DownloadedFile() { LocalFileName = fileName });
        }
      }
      return downloadedFileNames;
    }

    private class DownloadedFile
    {
      public string LocalFileName { get; set; }
      public string Url { get; set; }
      public bool IsDownloaded
      {
        get
        {
          return Url != null;
        }
      }

      public string FileName
      {
        get { return IsDownloaded ? Url : LocalFileName; }
      }
    }

    private void CreateKmz(Document document, Stream stream, KmlProperties kmlProperties, WaypointAttribute colorCodingAttribute, ColorRangeProperties colorRangeProperties)
    {
      var imageExporterProperties = new ImageExporterProperties()
      {
        ColorCodingAttribute = colorCodingAttribute,
        ColorRangeProperties = colorRangeProperties
      };

      var imageExporter = new ImageExporter(document)
      {
        Properties = imageExporterProperties
      };

      var kmlExporter = new KmlExporter(document, imageExporter, stream)
      {
        KmlProperties = kmlProperties
      };
      kmlExporter.ExportKmz(CommonUtil.GetTempFileName() + @"\");
    }

    private void CreateKmz(IEnumerable<string> documentFileNames, Stream stream, KmlProperties kmlProperties)
    {
      var imageExporterProperties = new ImageExporterProperties()
      {
        ColorCodingAttribute = SelectedColorCodingAttribute,
        ColorRangeProperties = GetCurrentColorRangeProperties(),
        SessionSettings = new SessionSettings()
      };

      var kmlExporter = new KmlExporter(
        documentFileNames,
        imageExporterProperties,
        stream)
      {
        KmlProperties = kmlProperties
      };
      kmlExporter.ExportKmz(CommonUtil.GetTempFileName() + @"\");
    }

    private void Undo()
    {
      if (undoStack.Count > 0)
      {
        IAction action = undoStack.Pop();
        redoStack.Push(action);
        action.Undo();
        UpdateAfterUndoRedo(action);
      }
    }

    private void Redo()
    {
      if (redoStack.Count > 0)
      {
        IAction action = redoStack.Pop();
        undoStack.Push(action);
        action.Execute();
        UpdateAfterUndoRedo(action);
      }
    }

    private void ChangeStartTime()
    {
      using (var dlg = new TimeDialog
                  {
                    InitialTime = canvas.CurrentSession.Route.FirstWaypoint.Time
                  })
      {
        if (dlg.ShowDialog() == DialogResult.OK)
        {
          var timeOffset = dlg.Time - canvas.CurrentSession.Route.FirstWaypoint.Time;
          var action = new AddTimeOffsetToSessionAction(canvas.CurrentSession, timeOffset);
          action.Execute();
          HandleAction(action);
        }
      }
    }

    #endregion

    #region Public methods

    public void UpdateUI()
    {
      if (updatingUINow) return;
      updatingUINow = true;

      var documentOpened = (canvas.Document != null);

      // window caption
      if (documentOpened)
      {
        Text = (canvas.Document.FileName == null ? Strings.Untitled : Path.GetFileName(canvas.Document.FileName)) + " - " + Strings.QuickRoute;
      }
      else
      {
        Text = Strings.QuickRoute;
      }

      // menu
      menuFileSave.Enabled = documentOpened;
      menuFileSaveAs.Enabled = documentOpened;
      menuFileImportSessions.Enabled = documentOpened;
      menuFileExport.Enabled = documentOpened;
      menuEdit.Visible = documentOpened;
      menuEditUndo.Enabled = (undoStack.Count > 0);
      menuEditRedo.Enabled = (redoStack.Count > 0);
      menuEditSeparator1.Enabled = documentOpened;
      menuEditChangeStartTime.Enabled = documentOpened;
      menuToolsAddLapsFromWinSplits.Enabled = documentOpened;
      menuToolsPublishMap.Enabled = documentOpened;
      menuToolsOpenInGoogleEarth.Enabled = documentOpened;
      menuViewRightPanelVisible.Enabled = documentOpened;
      menuViewBottomPanelVisible.Enabled = documentOpened;

      // toolstrip
      toolStripSave.Enabled = documentOpened;
      toolStripUndo.Enabled = (undoStack.Count > 0);
      toolStripRedo.Enabled = (redoStack.Count > 0);
      toolStripZoom.Enabled = documentOpened;
      toolStripZoom.Text = string.Format("{0:P0}", canvas.Zoom);
      toolStripToolPointer.Visible = documentOpened;
      toolStripToolAdjustRoute.Visible = documentOpened;
      toolStripToolZoomIn.Visible = documentOpened;
      toolStripToolZoomOut.Visible = documentOpened;
      toolStripToolCut.Visible = documentOpened;
      toolStripToolLap.Visible = documentOpened;
      toolStripToolPointer.Checked = (canvas.CurrentMouseTool == Canvas.MouseTool.Pointer);
      toolStripToolAdjustRoute.Checked = (canvas.CurrentMouseTool == Canvas.MouseTool.AdjustRoute);
      toolStripToolZoomIn.Checked = (canvas.CurrentMouseTool == Canvas.MouseTool.ZoomIn);
      toolStripToolZoomOut.Checked = (canvas.CurrentMouseTool == Canvas.MouseTool.ZoomOut);
      toolStripToolCut.Checked = (canvas.CurrentMouseTool == Canvas.MouseTool.Cut);
      toolStripToolLap.Checked = (canvas.CurrentMouseTool == Canvas.MouseTool.Lap);
      tstSeparator4.Visible = documentOpened;
      toolStripOpenInGoogleEarth.Visible = documentOpened;
      toolStripPublishMap.Visible = documentOpened;
      tstToolsSeparator.Visible = documentOpened;

      // view settings
      toolStripFullScreen.Checked = (formState != null);
      menuViewFullScreen.Checked = (formState != null);
      toolStripRightPanelVisible.Visible = documentOpened;
      toolStripBottomPanelVisible.Visible = documentOpened;
      toolStripRightPanelVisible.Checked = ApplicationSettings.RightPanelVisible;
      toolStripBottomPanelVisible.Checked = ApplicationSettings.BottomPanelVisible;
      menuViewRightPanelVisible.Checked = ApplicationSettings.RightPanelVisible;
      menuViewBottomPanelVisible.Checked = ApplicationSettings.BottomPanelVisible;

      toolStripAutoAdjustColorRangeInterval.Checked = ApplicationSettings.AutoAdjustColorRangeInterval;

      // route appearance toolstrip
      routeAppearanceToolstrip.Visible = documentOpened;
      if (documentOpened && canvas.CurrentSession != null)
      {
        RouteLineSettings rls = canvas.CurrentSession.Settings.RouteLineSettingsCollection[SelectedColorCodingAttribute];
        ColorRangeIntervalSlider slider = colorRangeIntervalSlider.SliderControl;
        ColorRangeIntervalSliderSettings sliderSettings = canvas.Document.Settings.ColorRangeIntervalSliderSettings[SelectedColorCodingAttribute];

        slider.PreventRedraw = true;
        switch (SelectedColorCodingAttribute)
        {
          case WaypointAttribute.Pace:
            slider.ScaleCreator = new TimeScaleCreator(sliderSettings.MinValue, sliderSettings.MaxValue, 20, false);
            slider.NumericConverter = new TimeConverter(TimeConverter.TimeConverterType.ElapsedTime);
            break;
          case WaypointAttribute.Speed:
            slider.ScaleCreator = new DoubleScaleCreator(sliderSettings.MinValue, sliderSettings.MaxValue, 20, false);
            slider.NumericConverter = new NumericConverter();
            break;
          case WaypointAttribute.HeartRate:
          case WaypointAttribute.Altitude:
            slider.ScaleCreator = new DoubleScaleCreator(sliderSettings.MinValue, sliderSettings.MaxValue, 20, false);
            slider.NumericConverter = new IntConverter();
            break;
          case WaypointAttribute.DirectionDeviationToNextLap:
            slider.ScaleCreator = new DoubleScaleCreator(sliderSettings.MinValue, sliderSettings.MaxValue, 20, false);
            slider.NumericConverter = new NumericConverter { NoOfDecimals = 0 };
            break;
          case WaypointAttribute.MapReadingDuration:
            slider.ScaleCreator = new TimeScaleCreator(sliderSettings.MinValue, sliderSettings.MaxValue, 5, false);
            slider.NumericConverter = new TimeConverter(TimeConverter.TimeConverterType.ElapsedTime);
            break;
        }
        slider.ColorRange = rls.ColorRange;
        slider.MinValue = sliderSettings.MinValue;
        slider.MaxValue = sliderSettings.MaxValue;
        slider.AlphaAdjustment = rls.AlphaAdjustment;
        slider.PreventRedraw = false;

        SelectedColorCodingAttribute = canvas.ColorCodingAttribute;
        colorRangeStartValue.Text = FormatColorRangeValue(rls.ColorRange.StartValue);
        colorRangeEndValue.Text = FormatColorRangeValue(rls.ColorRange.EndValue);

        routeLineMaskWidth.NumericUpDownControl.Value = (decimal)rls.MaskWidth;
        routeLineWidth.NumericUpDownControl.Value = (decimal)(canvas.CurrentMouseTool == Canvas.MouseTool.AdjustRoute ? rls.MonochromeWidth : rls.Width);
        
        routeLineMaskVisible.Checked = rls.MaskVisible;
        routeLineMaskColorButton.Image = CreateRouteLineMaskColorImage(canvas.CurrentMouseTool == Canvas.MouseTool.AdjustRoute ? rls.MonochromeColor : rls.MaskColor);
        gradientAlphaAdjustment.TrackBarControl.Value = (int)(rls.AlphaAdjustment * 10);

        canvas.RouteLineSettings = rls;
        slider.Refresh();

        // smoothing interval length
        var tc = new TimeConverter(TimeConverter.TimeConverterType.ElapsedTime);
        smoothingIntervalLength.Text = tc.ToString(canvas.Document.Settings.DefaultSessionSettings.SmoothingIntervals[SelectedColorCodingAttribute].Length);

        // smoothing interval tooltip
        smoothingIntervalLength.ToolTipText = string.Format(Strings.SmoothingIntervalForX, colorCodingAttributes.Text.ToLower());

        // circle time radius
        circleTimeRadius.Text = canvas.Document.Settings.DefaultSessionSettings.CircleTimeRadius.ToString();

        // lap histogram toolstrip
        lapHistogramBinWidth.Text = slider.NumericConverter.ToString(canvas.Document.Settings.LapHistogramSettings[SelectedColorCodingAttribute].BinWidth);
      }

      // canvas
      canvas.Visible = documentOpened;

      // right panel
      rightPanel.Visible = (documentOpened && ApplicationSettings.RightPanelVisible);
      rightSplitter.Visible = (documentOpened && ApplicationSettings.RightPanelVisible);
      rightSplitter.Enabled = (documentOpened && ApplicationSettings.RightPanelVisible);
      sessionPanel.Visible = (documentOpened && canvas.Document.Sessions.Count > 1);
      rightPanelTopSplitter.Visible = (documentOpened && canvas.Document.Sessions.Count > 1);

      // dynamic help
      dynamicHelpLabel.Visible = documentOpened;
      UpdateDynamicHelp();

      // bottom panel
      bottomPanel.Visible = (documentOpened && ApplicationSettings.BottomPanelVisible);
      bottomSplitter.Visible = (documentOpened && ApplicationSettings.BottomPanelVisible);
      bottomSplitter.Enabled = (documentOpened && ApplicationSettings.BottomPanelVisible);
      switch (canvas.CurrentMouseTool)
      {
        case Canvas.MouseTool.Lap:
          lineGraph.Cursor = Canvas.GetCursor(Canvas.MouseTool.Lap);
          break;
        case Canvas.MouseTool.Cut:
          lineGraph.Cursor = Canvas.GetCursor(Canvas.MouseTool.Cut);
          break;
        default:
          lineGraph.Cursor = Cursors.Default;
          break;
      }

      // momentaneous info
      momentaneousInfoPanel.Visible = documentOpened;

      updatingUINow = false;
    }

    #endregion

    #region Private methods

    private void UpdateDynamicHelp()
    {
      switch (canvas.CurrentMouseTool)
      {
        case Canvas.MouseTool.AdjustRoute:
          dynamicHelpLabel.Text = Strings.DynamicHelp_MouseTool_AdjustRoute;
          break;
        case Canvas.MouseTool.Cut:
          dynamicHelpLabel.Text = Strings.DynamicHelp_MouseTool_Cut;
          break;
        case Canvas.MouseTool.ZoomIn:
          dynamicHelpLabel.Text = Strings.DynamicHelp_MouseTool_ZoomIn;
          break;
        case Canvas.MouseTool.ZoomOut:
          dynamicHelpLabel.Text = Strings.DynamicHelp_MouseTool_ZoomOut;
          break;
        case Canvas.MouseTool.Lap:
          dynamicHelpLabel.Text = Strings.DynamicHelp_MouseTool_Lap;
          break;
        default:
          dynamicHelpLabel.Text = Strings.DynamicHelp_MouseTool_Pointer;
          break;
      }
    }

    private void AddActionToUndoStack(IAction action)
    {
      undoStack.Push(action);
      redoStack.Clear();
    }

    private Bitmap CreateRouteLineMaskColorImage(Color maskColor)
    {
      Bitmap bmp = new Bitmap(routeLineMaskColorButton.Size.Width, routeLineMaskColorButton.Size.Height);
      Graphics g = Graphics.FromImage(bmp);
      Rectangle r = new Rectangle(new Point(0, 0), bmp.Size);
      Gradient.FillCheckerboardRectangle(g, r, 8);
      Brush b = new SolidBrush(maskColor);
      g.FillRectangle(b, r);
      g.DrawRectangle(Pens.Gray, new Rectangle(0, 0, bmp.Width - 1, bmp.Height - 1));
      b.Dispose();
      g.Dispose();
      return bmp;
    }

    private void CalculateLapInfo()
    {
      Session s = canvas.CurrentSession;
      if (s == null) return;
      Route r = s.Route;
      ParameterizedLocation previousLapPL = ParameterizedLocation.Start;

      lapInfoList = new List<LapInfo>();
      for (int i = 0; i < s.Laps.Count; i++)
      {
        Lap lap = s.Laps[i];
        ParameterizedLocation lapPL = r.GetParameterizedLocationFromTime(lap.Time, previousLapPL, ParameterizedLocation.Direction.Forward);
        if (lap.LapType != LapType.Start && i > 0)
        {
          var li = new LapInfo { LapType = lap.LapType, Index = i };

          var locations = new RouteLocations(previousLapPL, lapPL);

          foreach (var lpType in ApplicationSettings.LapPropertyTypes)
          {
            if (lpType.Selected)
            {
              RetrieveExternalPropertyDelegate dlg = new ExternalRoutePropertyRetriever(s.Settings).RetrieveExternalProperty;
              var lp = Activator.CreateInstance(lpType.RoutePropertyType, s, locations, dlg) as RouteProperty;
              li.AddProperty(lp);
            }
          }

          if (s.Laps.Count > 2)
          {
            lapInfoList.Add(li);
          }
        }
        previousLapPL = lapPL;
      }

      // total row
      var totalInfo = new LapInfo() { LapType = LapType.Stop, Index = -1 };
      foreach (var lpType in ApplicationSettings.LapPropertyTypes)
      {
        if (lpType.Selected)
        {
          // create route span property object
          var routeSpanProperty =
            Activator.CreateInstance(lpType.RoutePropertyType, s, new RouteLocations(ParameterizedLocation.Start), null) as
            RouteSpanProperty;
          if (routeSpanProperty != null)
          {
            // get the route from start property type for this object
            Type routeFromStartPropertyType = routeSpanProperty.GetRouteFromStartPropertyType();
            // create an instance of that type
            RetrieveExternalPropertyDelegate dlg = new ExternalRoutePropertyRetriever(s.Settings).RetrieveExternalProperty;
            var routeFromStartProperty =
              Activator.CreateInstance(routeFromStartPropertyType, s, new RouteLocations(r.LastPL), dlg) as
              RouteFromStartProperty;
            if (routeFromStartProperty == null)
            {
              // no matching route from start property, add blank column
              totalInfo.AddProperty(new BlankRouteProperty());
            }
            else
            {
              totalInfo.AddProperty(routeFromStartProperty);
            }
          }
          else
          {
            RetrieveExternalPropertyDelegate dlg = new ExternalRoutePropertyRetriever(s.Settings).RetrieveExternalProperty;
            var routeFromStartProperty =
              Activator.CreateInstance(lpType.RoutePropertyType, s, new RouteLocations(r.LastPL), dlg) as
              RouteFromStartProperty;
            if (routeFromStartProperty == null)
            {
              // no matching route from start property, add blank column
              totalInfo.AddProperty(new BlankRouteProperty());
            }
            else
            {
              totalInfo.AddProperty(routeFromStartProperty);
            }
          }
        }
      }
      lapInfoList.Add(totalInfo);

      // set number of rows and columns in grid
      updatingUINow = true;
      laps.RowCount = 0;
      laps.ColumnCount = 0;
      laps.RowCount = lapInfoList.Count;
      laps.ColumnCount = (lapInfoList.Count == 0 ? 0 : lapInfoList[0].GetProperties().Count);
      updatingUINow = false;
      SetLapGridHeaders();
      SortLapGrid();
      laps.Invalidate();

      laps.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
      if (laps.Rows.Count > 0) laps.Rows[laps.Rows.Count - 1].Selected = true;
      int width = 0;
      foreach (DataGridViewColumn c in laps.Columns)
      {
        width += c.Width;
      }

      updatingUINow = true;
      rightPanel.Width = width + laps.Margin.Horizontal;
      updatingUINow = false;
    }

    private void SetLapGridHeaders()
    {
      if (lapInfoList.Count > 0)
      {
        for (var i = 0; i < laps.Columns.Count; i++)
        {
          var propertyType = lapInfoList[0].GetProperties()[i].GetType();
          var tooltipTemplateText = Strings.ResourceManager.GetString("RoutePropertyName_" + propertyType.Name);
          if (tooltipTemplateText == null) tooltipTemplateText = Strings.ResourceManager.GetString("RoutePropertyName_" + propertyType.GetType().Name.Replace("FromStart", ""));
          if (tooltipTemplateText != null) laps.Columns[i].HeaderText = tooltipTemplateText;
        }
      }
    }

    private void SortLapGrid()
    {
      if (lapInfoList.Count > 1)
      {
        // preserve selected lap
        var selectedLapNumber = -1;
        if (laps.SelectedRows.Count > 0) selectedLapNumber = lapInfoList[laps.SelectedRows[0].Index].Index;

        // don't sort last item (the total row, having index -1)
        lapInfoList.Sort(
          (lap1, lap2) => (lap1.Index == -1 ? 1 :
                           lap2.Index == -1 ? -1 : lap1.CompareProperty(lap2, lapSortOrderColumnIndex)));
        if (lapSortOrder == SortOrder.Descending) lapInfoList.Reverse(0, lapInfoList.Count - 1);

        // re-select selected lap
        for (var i = 0; i < lapInfoList.Count; i++)
        {
          if (lapInfoList[i].Index == selectedLapNumber)
          {
            updatingUINow = true;
            laps.Rows[i].Selected = true;
            updatingUINow = false;
            return;
          }
        }
      }

    }

    private double? ParseColorRangeValue(string text)
    {
      return Util.GetNumericConverterFromWaypointAttribute(SelectedColorCodingAttribute).ToNumeric(text);
    }

    private string FormatColorRangeValue(double? value)
    {
      return Util.GetNumericConverterFromWaypointAttribute(SelectedColorCodingAttribute).ToString(value);
    }

    private void ShowGradientEditor()
    {
      using (var ge = new GradientEditor())
      {
        string gradientPath = CommonUtil.GetApplicationDataPath() + "Gradients\\";
        // get all unique gradients
        var gradients = new List<Gradient>();
        // from default route line settings
        foreach (RouteLineSettings rls in SessionSettings.CreateDefaultRouteLineSettingsCollection().Values)
        {
          if (!gradients.Contains(rls.ColorRange.Gradient)) gradients.Add(rls.ColorRange.Gradient);
        }
        // from gradients in this document
        foreach (RouteLineSettings rls in canvas.CurrentSession.Settings.RouteLineSettingsCollection.Values)
        {
          if (!gradients.Contains(rls.ColorRange.Gradient)) gradients.Add(rls.ColorRange.Gradient);
        }
        // from gradients saved on disk
        foreach (Gradient g in Util.GetGradientsInFolder(gradientPath))
        {
          if (!gradients.Contains(g)) gradients.Add(g);
        }

        ge.Gradients = gradients;

        ge.CurrentGradient = colorRangeIntervalSlider.SliderControl.ColorRange.Gradient;

        if (ge.ShowDialog() == DialogResult.OK)
        {
          foreach (string fileName in Directory.GetFiles(gradientPath))
          {
            File.Delete(fileName);
          }
          Util.SaveGradientsToFolder(ge.Gradients, gradientPath);
          if (ge.CurrentGradient != null)
          {
            BeginWork();
            canvas.CurrentSession.Settings.RouteLineSettingsCollection[SelectedColorCodingAttribute].ColorRange.Gradient
              = ge.CurrentGradient;
            canvas.CurrentSession.Settings.RouteLineSettingsCollection[SelectedColorCodingAttribute].MonochromeColor =
              ge.CurrentGradient.GetColor(1);
            UpdateUI();
            canvas.DrawMap(Canvas.MapDrawingFlags.Markers | Canvas.MapDrawingFlags.Route);
            CreateLapHistogram();
            DrawLapHistogram();
            CreateLineGraph();
            DrawLineGraph();
            EndWork();
          }
        }
      }
    }

    private void UpdateZoom()
    {
      if (startingUpNow || updatingZoomNow) return;
      updatingZoomNow = true;
      string text = toolStripZoom.Text.Replace("%", "");
      double zoom;
      if (double.TryParse(text, out zoom))
      {
        zoom /= 100.0;
      }
      else
      {
        zoom = 1.0;
      }
      canvas.Zoom = zoom;
      toolStripZoom.Text = string.Format("{0:P0}", canvas.Zoom);
      updatingZoomNow = false;
    }

    private void ResetMomentaneousInfo()
    {
      UpdateMomentaneousInfo(null);
    }

    private void CreateLapHistogram()
    {
      if (laps.SelectedRows.Count == 0)
      {
        lapHistogram = null;
        return;
      }

      lapHistogram = new ColorfulHistogram();

      LapInfo li = lapInfoList[laps.SelectedRows[0].Index];
      DateTime startTime;
      DateTime endTime;
      if (li.Index == -1)
      {
        startTime = canvas.CurrentSession.Route.FirstWaypoint.Time;
        endTime = canvas.CurrentSession.Route.LastWaypoint.Time;
      }
      else
      {
        Lap lap = canvas.CurrentSession.Laps[li.Index];
        startTime = canvas.CurrentSession.Laps[li.Index - 1].Time;
        endTime = lap.Time;
      }

      Route r = canvas.CurrentSession.Route;
      List<double?> list = r.GetMomentaneousValueList(
        SelectedColorCodingAttribute,
        r.GetParameterizedLocationFromTime(startTime),
        r.GetParameterizedLocationFromTime(endTime),
        new TimeSpan(0, 0, 1));

      lapHistogram.HistogramData.Clear();
      foreach (double? value in list)
      {
        if (value.HasValue)
        {
          lapHistogram.HistogramData.Add(value.Value);
        }
      }

      RouteLineSettings rls = canvas.CurrentSession.Settings.RouteLineSettingsCollection[SelectedColorCodingAttribute];
      lapHistogram.StartValue = canvas.Document.Settings.ColorRangeIntervalSliderSettings[SelectedColorCodingAttribute].MinValue;
      lapHistogram.EndValue = canvas.Document.Settings.ColorRangeIntervalSliderSettings[SelectedColorCodingAttribute].MaxValue;
      lapHistogram.BinWidth = canvas.Document.Settings.LapHistogramSettings[SelectedColorCodingAttribute].BinWidth;
      lapHistogram.ColorRange = rls.ColorRange;
      lapHistogram.XAxisCaption =
        new WaypointAttributeString(SelectedColorCodingAttribute).Name +
        " (" + new WaypointAttributeString(SelectedColorCodingAttribute).Unit + ")";
      lapHistogram.IncludeOutOfRangeValues = true;
      lapHistogram.XAxisNumericConverter = Util.GetNumericConverterFromWaypointAttribute(SelectedColorCodingAttribute);
      Type t = colorRangeIntervalSlider.SliderControl.ScaleCreator.GetType();
      lapHistogram.XAxisScaleCreator = (ScaleCreatorBase)Activator.CreateInstance(t, lapHistogram.StartValue, lapHistogram.EndValue, 10, false);
    }

    private void ValidateAndFormatLapHistogramBinWidthValue()
    {
      BeginWork();
      double? binWidth = ParseColorRangeValue(lapHistogramBinWidth.Text);
      if (binWidth.HasValue)
      {
        canvas.Document.Settings.LapHistogramSettings[SelectedColorCodingAttribute].BinWidth = binWidth.Value;
      }
      lapHistogramBinWidth.Text = FormatColorRangeValue(canvas.Document.Settings.LapHistogramSettings[SelectedColorCodingAttribute].BinWidth);
      CreateLapHistogram();
      DrawLapHistogram();
      EndWork();
    }

    private void DrawLapHistogram()
    {
      if (lapHistogram != null)
      {
        RectangleF drawingRectangle = new RectangleF(
          (lapHistogramPanel.Padding.Left),
          (lapHistogramPanel.Padding.Top),
          (lapHistogramPanel.Width - lapHistogramPanel.Padding.Horizontal),
          (lapHistogramPanel.Height - lapHistogramPanel.Padding.Vertical));
        lapHistogram.Draw(lapHistogramPanel.CreateGraphics(), drawingRectangle);
      }
    }

    private void ExportLapHistogramImage()
    {
      using (var sfd = new SaveFileDialog
                  {
                    Title = Strings.ExportImageTitle,
                    Filter = (Strings.FileFilter_JpegFiles + "|" +
                              Strings.FileFilter_PngFiles + "|" +
                              Strings.FileFilter_TiffFiles),
                    FileName =
                      ((canvas.Document.FileName != null
                          ? Path.GetFileNameWithoutExtension(canvas.Document.FileName)
                          : Path.GetFileNameWithoutExtension(untitledDocumentFileNameProposal)) +
                       ", " + Strings.Histogram),
                    FilterIndex = 1,
                    AddExtension = true,
                    OverwritePrompt = true
                  })
      {
        if (ApplicationSettings.InitialFolders.ContainsKey(ApplicationSettings.FileDialogType.ExportImage))
        {
          sfd.InitialDirectory = ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.ExportImage];
        }

        if (sfd.ShowDialog() == DialogResult.OK)
        {
          BeginWork();
          System.Drawing.Imaging.ImageFormat format = null;

          try
          {
            switch (sfd.FilterIndex)
            {
              case 1:
                format = System.Drawing.Imaging.ImageFormat.Jpeg;
                break;
              case 2:
                format = System.Drawing.Imaging.ImageFormat.Png;
                break;
              case 3:
                format = System.Drawing.Imaging.ImageFormat.Tiff;
                break;
            }

            var bmp = new Bitmap(lapHistogramPanel.Width - lapHistogramPanel.Padding.Horizontal,
                                 lapHistogramPanel.Height - lapHistogramPanel.Padding.Vertical);
            var g = Graphics.FromImage(bmp);
            lapHistogram.Draw(g, new RectangleF(0, 0, bmp.Width, bmp.Height));
            bmp.Save(sfd.FileName, format);
            g.Dispose();
            ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.ExportImage] =
              Path.GetDirectoryName(sfd.FileName);
          }
          catch (Exception ex)
          {
            Util.ShowExceptionMessageBox(ex, Strings.Error_ExportImage);
          }
          EndWork();
        }
      }
    }

    private void CreateLineGraph()
    {
      if (canvas.Document == null) return;
      ColorRangeIntervalSliderSettings criss = canvas.Document.Settings.ColorRangeIntervalSliderSettings[SelectedColorCodingAttribute];
      lineGraph.Graph.YAxisAttribute = SelectedColorCodingAttribute;
      lineGraph.Graph.Session = canvas.CurrentSession;
      lineGraph.Graph.YAxisMinValue = criss.MinValue;
      lineGraph.Graph.YAxisMaxValue = criss.MaxValue;
      Type t = colorRangeIntervalSlider.SliderControl.ScaleCreator.GetType();
      lineGraph.Graph.YAxisScaleCreator = (ScaleCreatorBase)Activator.CreateInstance(t, criss.MinValue, criss.MaxValue, 10, false);
      lineGraph.Graph.YAxisNumericConverter = Util.GetNumericConverterFromWaypointAttribute(SelectedColorCodingAttribute);
      lineGraph.Graph.XAxisAttribute = DomainAttribute.TimeOfDay;

      var li = lapInfoList[laps.SelectedRows[0].Index];
      if (li.Index == -1)
      {
        lineGraph.Graph.StartPL = canvas.CurrentSession.Route.GetParameterizedLocationFromTime(canvas.CurrentSession.Route.FirstWaypoint.Time);
        lineGraph.Graph.EndPL = canvas.CurrentSession.Route.GetParameterizedLocationFromTime(canvas.CurrentSession.Route.LastWaypoint.Time);
      }
      else
      {
        Lap lap = canvas.CurrentSession.Laps[li.Index];
        lineGraph.Graph.StartPL = canvas.CurrentSession.Route.GetParameterizedLocationFromTime(canvas.CurrentSession.Laps[li.Index - 1].Time);
        lineGraph.Graph.EndPL = canvas.CurrentSession.Route.GetParameterizedLocationFromTime(lap.Time);
      }

      switch (lineGraph.Graph.YAxisAttribute)
      {
        case WaypointAttribute.Pace:
          lineGraph.Graph.YAxisCaption = Strings.Pace + " (" + Strings.Unit_Pace + ")";
          break;
        case WaypointAttribute.Speed:
          lineGraph.Graph.YAxisCaption = Strings.Speed + " (" + Strings.Unit_Speed + ")";
          break;
        case WaypointAttribute.HeartRate:
          lineGraph.Graph.YAxisCaption = Strings.HeartRate + " (" + Strings.Unit_HeartRate + ")";
          break;
        case WaypointAttribute.Altitude:
          lineGraph.Graph.YAxisCaption = Strings.Altitude + " (" + Strings.Unit_Altitude + ")";
          break;
        case WaypointAttribute.DirectionDeviationToNextLap:
          lineGraph.Graph.YAxisCaption = Strings.Direction + " (" + Strings.Unit_Direction + ")";
          break;
        case WaypointAttribute.MapReadingDuration:
          lineGraph.Graph.YAxisCaption = Strings.MapReadingDuration;
          break;
      }
      switch (lineGraph.Graph.XAxisAttribute)
      {
        case DomainAttribute.TimeOfDay:
          lineGraph.Graph.XAxisCaption = Strings.Time;
          break;
        case DomainAttribute.ElapsedTime:
          lineGraph.Graph.XAxisCaption = Strings.ElapsedTime;
          break;
        case DomainAttribute.Distance:
          lineGraph.Graph.XAxisCaption = Strings.Distance + " (" + Strings.Unit_Distance + ")";
          break;
      }
      lineGraph.Graph.Calculate();
      lineGraph.Create();
    }

    private void DrawLineGraph()
    {
      lineGraph.Draw();
    }

    private void ValidateAndFormatSmoothingIntervalLengthValue()
    {
      BeginWork();
      TimeConverter tc = new TimeConverter(TimeConverter.TimeConverterType.ElapsedTime);
      double? intervalLength = tc.ToNumeric(smoothingIntervalLength.Text);
      if (intervalLength.HasValue)
      {
        canvas.Document.Settings.DefaultSessionSettings.SmoothingIntervals[SelectedColorCodingAttribute] = new Interval(-intervalLength.Value / 2, intervalLength.Value / 2);

        // make sure speed and pace are always the same
        if (SelectedColorCodingAttribute == WaypointAttribute.Speed)
          canvas.Document.Settings.DefaultSessionSettings.SmoothingIntervals[WaypointAttribute.Pace] = new Interval(canvas.Document.Settings.DefaultSessionSettings.SmoothingIntervals[SelectedColorCodingAttribute]);
        if (SelectedColorCodingAttribute == WaypointAttribute.Pace)
          canvas.Document.Settings.DefaultSessionSettings.SmoothingIntervals[WaypointAttribute.Speed] = new Interval(canvas.Document.Settings.DefaultSessionSettings.SmoothingIntervals[SelectedColorCodingAttribute]);

        foreach (Session s in canvas.Document.Sessions)
        {
          s.Route.SmoothingIntervals = canvas.Document.Settings.DefaultSessionSettings.SmoothingIntervals;
        }
        canvas.DrawMap(Canvas.MapDrawingFlags.Route | Canvas.MapDrawingFlags.Markers);
        CalculateLapInfo();
        CreateLapHistogram();
        DrawLapHistogram();
        CreateLineGraph();
        DrawLineGraph();
      }
      smoothingIntervalLength.Text = tc.ToString(canvas.Document.Settings.DefaultSessionSettings.SmoothingIntervals[SelectedColorCodingAttribute].Length);
      EndWork();
    }

    private void ValidateAndFormatCircleTimeRadiusValue()
    {
      BeginWork();
      double value;
      if(double.TryParse(circleTimeRadius.Text, out value))
      {
        canvas.Document.Settings.DefaultSessionSettings.CircleTimeRadius = value;
        CalculateLapInfo();
      }
      circleTimeRadius.Text = canvas.Document.Settings.DefaultSessionSettings.CircleTimeRadius.ToString();
      EndWork();
    }

    private void ResetActionStacks()
    {
      undoStack.Clear();
      redoStack.Clear();
    }

    private void UpdateRecentDocumentsListInMenu()
    {
      List<string> fileNames = ApplicationSettings.RecentDocumentFileNames;

      // remove old filenames
      for (int i = menuFile.DropDown.Items.Count - 1; i >= 0; i--)
      {
        ToolStripMenuItem tsmi = menuFile.DropDown.Items[i] as ToolStripMenuItem;
        if (tsmi != null)
        {
          string tagString = tsmi.Tag as string;
          if (tagString != null && tagString.StartsWith("RecentFile_"))
          {
            tsmi.Click -= RecentDocumentClicked;
            menuFile.DropDown.Items.RemoveAt(i);
          }
        }
      }

      // insert filenames into the menu
      menuFileRecentDocumentsSeparator.Visible = (fileNames.Count > 0);
      if (fileNames.Count > 0)
      {
        int startPosition = menuFile.DropDown.Items.IndexOf(menuFileRecentDocumentsSeparator);
        int count = 0;
        foreach (string fn in fileNames)
        {
          count++;
          string numberString = (count == 10 ? "1&0" : "&" + count);
          ToolStripMenuItem tsmi = new ToolStripMenuItem(numberString + " " + Util.PathShortener(fn, 50).Replace("&", "&&"));
          tsmi.Click += RecentDocumentClicked;
          tsmi.Tag = "RecentFile_" + fn;
          menuFile.DropDown.Items.Insert(startPosition + count, tsmi);
        }
      }
    }

    private void RecentDocumentClicked(object sender, EventArgs e)
    {
      var tsmi = sender as ToolStripMenuItem;
      if (tsmi == null) return;
      var tagString = tsmi.Tag as string;
      if (tagString == null) return;
      var fileName = tagString.Substring(11);
      OpenDocumentWithFileName(fileName, true);
    }

    private void UpdateAfterUndoRedo(IAction action)
    {
      updatingUINow = true;
      UpdateAfterAction(action);
      canvas.DrawMap(Canvas.MapDrawingFlags.Route | Canvas.MapDrawingFlags.Markers);
      updatingUINow = false;
      UpdateUI();
    }

    private void UpdateAfterAction(IAction action)
    {
      bool isLapAction =
        (action is AddLapAction ||
         action is EditLapAction ||
         action is DeleteLapAction ||
         action is CutRouteAction ||
         action is TrimRouteAndAddLapsAction ||
         action is AddTimeOffsetToSessionAction);

      if (isLapAction)
      {
        // initializes session in order to recalculate waypoint attributes
        canvas.CurrentSession.Initialize();
        CalculateLapInfo();
        AlphaAdjustSelectedLap();
        CreateLapHistogram();
        DrawLapHistogram();
        CreateLineGraph();
        DrawLineGraph();
      }
      if (action is CutRouteAction)
      {
        //if (ApplicationSettings.AutoAdjustColorRangeInterval) PerformColorRangeIntervalAutoAdjustment();
      }
    }

    private void HandleMouseHover(ParameterizedLocation pl, bool showMarker)
    {
      if (showMarker && pl != null)
      {
        UpdateMomentaneousInfo(pl);
      }
      else
      {
        ResetMomentaneousInfo();
      }
    }

    private void CalculateMomentaneousInfoLabelRectangles()
    {
      momentaneousInfoLabelRectangles = new Dictionary<Type, Rectangle>();
      if (canvas.Document == null) return;

      // calculate size and locations of labels
      Route r = canvas.CurrentSession.Route;
      var noOfRows = 0;
      var xPos = momentaneousInfoPanel.Padding.Left;
      var rowHeight = 0;
      foreach (var selectableRoutePropertyType in ApplicationSettings.MomentaneousInfoPropertyTypes)
      {
        if (selectableRoutePropertyType.Selected)
        {
          RetrieveExternalPropertyDelegate dlg = new ExternalRoutePropertyRetriever(canvas.CurrentSession.Settings).RetrieveExternalProperty;
          var property = Activator.CreateInstance(selectableRoutePropertyType.RoutePropertyType, canvas.CurrentSession, new RouteLocations(r.FirstPL), dlg) as RouteProperty;
          if (property != null && property.ContainsValue)
          {
            var templateString = Strings.ResourceManager.GetString("RoutePropertyNameAndValue_" +
                                                                   property.GetType().Name)
                                 ??
                                 property.GetType().Name + ": {0}";
            var maxWidthString = string.Format(templateString, property.MaxWidthString);
            var label = new Rectangle()
                          {
                            Size = TextRenderer.MeasureText(maxWidthString, momentaneousInfoPanel.Font)
                          };

            if (rowHeight == 0) rowHeight = label.Size.Height + momentaneousInfoLabelPadding.Vertical;
            if (xPos + label.Size.Width + momentaneousInfoLabelPadding.Horizontal + momentaneousInfoPanel.Padding.Right >
                momentaneousInfoPanel.Width)
            {
              // new row needed
              noOfRows++;
              xPos = momentaneousInfoPanel.Padding.Left;
            }
            label.Location = new Point(xPos + momentaneousInfoLabelPadding.Left,
                                       noOfRows * rowHeight + momentaneousInfoLabelPadding.Top +
                                       momentaneousInfoPanel.Padding.Top);
            xPos += label.Size.Width + momentaneousInfoLabelPadding.Horizontal;
            momentaneousInfoLabelRectangles.Add(selectableRoutePropertyType.RoutePropertyType, label);
          }
        }
      }
      momentaneousInfoPanel.Visible = true;
      momentaneousInfoPanel.Height = Math.Max((noOfRows + 1) * rowHeight + momentaneousInfoPanel.Padding.Vertical, 1);
      UpdateMomentaneousInfo(null);
    }

    private void UpdateMomentaneousInfo(ParameterizedLocation pl)
    {
      // TODO: backbuffering?
      // TODO: toolstrip back color? At least some sort of top line that separates it from above

      var g = momentaneousInfoPanelBackBufferGraphics;

      g.Clear(momentaneousInfoPanel.BackColor);
      g.TextRenderingHint = TextRenderingHint.AntiAlias;
      var lightPen = new Pen(SystemColors.ControlLight, 1F);
      var darkPen = new Pen(SystemColors.ControlDark, 1F);

      g.DrawLine(darkPen, 0, 0, momentaneousInfoPanelBackBuffer.Width - 1, 0);

      if (pl != null)
      {
        Route r = canvas.CurrentSession.Route;
        var lapStartPL = r.GetLapStartParameterizedLocation(pl);
        foreach (var selectableRoutePropertyType in ApplicationSettings.MomentaneousInfoPropertyTypes)
        {
          if (selectableRoutePropertyType.Selected)
          {
            RetrieveExternalPropertyDelegate dlg = new ExternalRoutePropertyRetriever(canvas.CurrentSession.Settings).RetrieveExternalProperty;
            var property =
              Activator.CreateInstance(selectableRoutePropertyType.RoutePropertyType, canvas.CurrentSession, new RouteLocations(lapStartPL, pl), dlg) as
              RouteProperty;
            if (property != null )
            {
              var templateString = Strings.ResourceManager.GetString("RoutePropertyNameAndValue_" +
                                                                     property.GetType().Name)
                                   ??
                                   property.GetType().Name + ": {0}";
              var text = property.Value != null
                           ? string.Format(templateString, property)
                           : string.Format(templateString, "-");
              if (momentaneousInfoLabelRectangles.ContainsKey(selectableRoutePropertyType.RoutePropertyType)) // this should not be needed, but people seem to have problem with "The given key was not present in the dictionary. (mscorlib)"
              {
                var rect = momentaneousInfoLabelRectangles[selectableRoutePropertyType.RoutePropertyType];

                g.DrawString(text, momentaneousInfoPanel.Font, Brushes.Black, rect.Location);
                // draw separator
                g.DrawLine(lightPen,
                           rect.Right + momentaneousInfoLabelPadding.Right, rect.Top,
                           rect.Right + momentaneousInfoLabelPadding.Right, rect.Bottom);
                g.DrawLine(darkPen,
                           rect.Right + momentaneousInfoLabelPadding.Right + 1, rect.Top,
                           rect.Right + momentaneousInfoLabelPadding.Right + 1, rect.Bottom);
              }
            }
          }
        }
      }
      lightPen.Dispose();
      darkPen.Dispose();
      CopyMomentaneousInfoBackBufferToScreen();
    }

    private void CopyMomentaneousInfoBackBufferToScreen()
    {
      // copy back buffer to screen
      momentaneousInfoPanel.CreateGraphics().DrawImage(momentaneousInfoPanelBackBuffer, 0, 0);
    }


    private void PerformFullRedraw()
    {
      if (!openingDocumentNow)
      {
        UpdateUI();
        canvas.DrawMap(Canvas.MapDrawingFlags.Route | Canvas.MapDrawingFlags.Markers);
        CreateLapHistogram();
        DrawLapHistogram();
        CreateLineGraph();
        DrawLineGraph();
      }
    }

    private void AlphaAdjustSelectedLap()
    {
      Lap lap = null;
      if (laps.SelectedRows.Count > 0)
      {
        if (laps.SelectedRows[0].Index < laps.Rows.Count - 1) lap = canvas.CurrentSession.Laps[lapInfoList[laps.SelectedRows[0].Index].Index];
        RouteLineSettings rls = canvas.CurrentSession.Settings.RouteLineSettingsCollection[SelectedColorCodingAttribute];
        canvas.AlphaAdjustLap(
          canvas.CurrentSession,
          lap,
          GraphicsUtil.CombineAlphaAdjustments(rls.AlphaAdjustment, 0.5),
          GraphicsUtil.CombineAlphaAdjustments(rls.AlphaAdjustment, -0.5),
          rls.AlphaAdjustment);
        if (lap != null)
        {
          Lap lastLap = canvas.CurrentSession.Laps[lapInfoList[laps.SelectedRows[0].Index].Index - 1];
          canvas.EnsureLegIsVisible(
            canvas.CurrentSession.AdjustedRoute.GetBoundingRectangle(
              canvas.CurrentSession.Route.GetParameterizedLocationFromTime(lastLap.Time),
              canvas.CurrentSession.Route.GetParameterizedLocationFromTime(lap.Time)
              ),
            true
            );
        }
      }
    }

    private void PerformColorRangeIntervalAutoAdjustment()
    {
      Route r = canvas.CurrentSession.Route;
      var samplingInterval = new TimeSpan(0, 0, 1);
      List<double?> values = r.GetOrderedValues(SelectedColorCodingAttribute, GetAutoAdjustColorRangeIntervalPercentages(SelectedColorCodingAttribute), samplingInterval);
      if (values[0].HasValue && values[1].HasValue)
      {
        double span = values[1].Value - values[0].Value;
        values[0] -= 0.1 * span;
        values[1] += 0.1 * span;
        if (SelectedColorCodingAttribute == WaypointAttribute.Pace) values[1] = Math.Min(30 * 60, values[1].Value);
        if (SelectedColorCodingAttribute == WaypointAttribute.DirectionDeviationToNextLap) values[0] = 0;
        if (SelectedColorCodingAttribute == WaypointAttribute.MapReadingDuration) values[0] = 0;

        BeginWork();
        UpdateColorRangeInterval(SelectedColorCodingAttribute, values[0].Value, values[1].Value, true);
        PerformFullRedraw();
        EndWork();
      }
    }

    private void BeginWork()
    {
      workLevel++;
      if (workLevel == 1)
      {
        previousCursor = Cursor;
        Cursor = Cursors.WaitCursor;
      }
    }

    private void EndWork()
    {
      workLevel--;
      if (workLevel == 0)
      {
        Cursor = previousCursor;
      }
    }

    private void AddLapsFromWinSplits()
    {
      using (var form = new AddLapsFromWinSplits())
      {
        form.ShowDialog();
        if (form.DialogResult == DialogResult.OK)
        {
          var action = new TrimRouteAndAddLapsAction(
            form.Runner.StartTime.HasValue ? new DateTime?(form.Runner.StartTime.Value.ToUniversalTime()) : null,
            form.Runner.FinishTime.HasValue ? new DateTime?(form.Runner.FinishTime.Value.ToUniversalTime()) : null,
            new List<Lap>(),
            canvas.CurrentSession);
          // add laps for each punch
          if (canvas.CurrentSession.Route.FirstWaypoint != null && canvas.CurrentSession.Route.LastWaypoint != null)
          {
            foreach (DateTime dt in form.Runner.Splits)
            {
              var utc = dt.ToUniversalTime();
              action.Laps.Add(new Lap(utc, LapType.Lap));
            }
          }
          action.Execute();
          canvas.CurrentSession.CreateAdjustedRoute();
          HandleAction(action);

          canvas.DrawMap(Canvas.MapDrawingFlags.Route | Canvas.MapDrawingFlags.Markers);
        }
      }
    }

    private string GetLapsInfoAsText()
    {
      var sb = new StringBuilder();
      foreach (DataGridViewRow row in laps.Rows)
      {
        foreach (DataGridViewCell cell in row.Cells)
        {
          sb.Append(cell.FormattedValue.ToString()).Append("\t");
        }
        sb.Append("\n");
      }
      return sb.ToString();
    }

    private void PublishMap()
    {
      using (var pmForm = new PublishMapForm(canvas.Document, SelectedColorCodingAttribute, GetCurrentColorRangeProperties()))
      {
        pmForm.ShowDialog();
      }
    }

    private void ToggleFullScreen(bool visible)
    {
      if (updatingUINow) return;
      if (visible)
      {
        formState = new MainFormState();
        formState.EnterFullScreenMode(this);
      }
      else
      {
        formState.ExitFullScreenMode(this);
        formState = null;
      }
      UpdateUI();
    }

    private void ToggleRightPanel(bool visible)
    {
      if (updatingUINow) return;
      ApplicationSettings.RightPanelVisible = visible;
      UpdateUI();
    }

    private void ToggleBottomPanel(bool visible)
    {
      if (updatingUINow) return;
      ApplicationSettings.BottomPanelVisible = visible;
      UpdateUI();
    }

    private void UpdateColorRangeInterval(WaypointAttribute attribute, double minValue, double maxValue, bool setSlidersToMinAndMax)
    {
      // prevent too high paces
      if (attribute == WaypointAttribute.Pace && minValue > 30 * 60) minValue = 30 * 60;
      if (attribute == WaypointAttribute.Pace && maxValue > 30 * 60) maxValue = 30 * 60;

      ColorRangeIntervalSliderSettings sliderSettings = canvas.Document.Settings.ColorRangeIntervalSliderSettings[attribute];
      RouteLineSettings rls = canvas.CurrentSession.Settings.RouteLineSettingsCollection[attribute];
      updatingUINow = true;
      sliderSettings.MinValue = minValue;
      sliderSettings.MaxValue = maxValue;
      if (setSlidersToMinAndMax)
      {
        rls.ColorRange.StartValue = sliderSettings.MinValue;
        rls.ColorRange.EndValue = sliderSettings.MaxValue;
      }
      else
      {
        if (rls.ColorRange.StartValue < sliderSettings.MinValue) rls.ColorRange.StartValue = sliderSettings.MinValue;
        if (rls.ColorRange.StartValue > sliderSettings.MaxValue) rls.ColorRange.StartValue = sliderSettings.MaxValue;
        if (rls.ColorRange.EndValue < sliderSettings.MinValue) rls.ColorRange.EndValue = sliderSettings.MinValue;
        if (rls.ColorRange.EndValue > sliderSettings.MaxValue) rls.ColorRange.EndValue = sliderSettings.MaxValue;
      }
      updatingUINow = false;
    }

    private void HandleAction(IAction action)
    {
      AddActionToUndoStack(action);
      documentChanged = true;
      UpdateAfterAction(action);
      UpdateUI();
    }

    private void OpenInGoogleEarthFromCommandLine(string fileName)
    {
      var wa = WaypointAttribute.Pace;
      var sc = new TimeScaleCreator(210, 1200, 10, false);
      var crp = new ColorRangeProperties
      {
        NumericConverter = Util.GetNumericConverterFromWaypointAttribute(wa),
        ScaleCreator = sc,
        ScaleCaption = new WaypointAttributeString(wa).Name,
        ScaleUnit = new WaypointAttributeString(wa).Unit
      };

      OpenInGoogleEarth(fileName, wa, crp);
      Util.SaveSettings(ApplicationSettings);

      var process = Process.GetCurrentProcess();
      process.Kill();
    }

    private void HandleDragDrop(DragEventArgs e)
    {
      var manager = new DragDropManager();
      var allowedFileExtensions = FileFormatManager.GetQuickRouteFileExtensions();
      var fileNames = manager.GetDroppedFileNames(e, allowedFileExtensions);
      if (fileNames.Count > 0)
      {
        OpenDocumentWithFileName(fileNames[0], true);
      }
      else
      {
        MessageBox.Show(Strings.Error_InvalidQuickRouteFile, Strings.QuickRoute, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
      }
    }

    #endregion

    #region Private properties

    private WaypointAttribute SelectedColorCodingAttribute
    {
      get
      {
        return ((WaypointAttributeString)colorCodingAttributes.SelectedItem).WaypointAttribute;
      }
      set
      {
        colorCodingAttributes.SelectedItem = new WaypointAttributeString(value);
      }
    }

    #endregion

    #region Event handlers

    private void Main_Load(object sender, EventArgs e)
    {
      WindowState = FormWindowState.Maximized;
      if (Environment.GetCommandLineArgs().Length == 2)
      {
        var success = OpenDocumentWithFileName(Environment.GetCommandLineArgs()[1], false);
        if (!success)
        {
          NewDocument(Environment.GetCommandLineArgs()[1]);
        }
      }

    }

    private void canvas_DocumentChanged(object sender, EventArgs e)
    {
      UpdateUI();
      documentChanged = true;
    }

    private void canvas_RouteMouseHover(object sender, Canvas.RouteMouseHoverEventArgs e)
    {
      HandleMouseHover(e.ParameterizedLocation, e.IsClose && e.ParameterizedLocation != null);
      if (e.IsClose)
      {
        double xValue = 0;
        switch (lineGraph.Graph.XAxisAttribute)
        {
          case DomainAttribute.TimeOfDay:
            xValue = (double)canvas.CurrentSession.Route.GetTimeFromParameterizedLocation(e.ParameterizedLocation).ToLocalTime().Ticks / TimeSpan.TicksPerSecond;
            break;
          case DomainAttribute.ElapsedTime:
            xValue = (double)(canvas.CurrentSession.Route.GetTimeFromParameterizedLocation(e.ParameterizedLocation).Ticks - canvas.CurrentSession.Route.FirstWaypoint.Time.Ticks) / TimeSpan.TicksPerSecond;
            break;
          case DomainAttribute.Distance:
            xValue = canvas.CurrentSession.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.Distance, e.ParameterizedLocation).Value;
            break;
        }
        lineGraph.HoverXValue = xValue;
      }
      else
      {
        lineGraph.HoverXValue = null;
      }
    }

    private void canvas_BeforeZoomChanged(object sender, EventArgs e)
    {
      BeginWork();
      UpdateUI();
    }

    private void canvas_AfterZoomChanged(object sender, EventArgs e)
    {
      UpdateUI();
      toolStripZoom.SelectAll();
      EndWork();
    }

    private void canvas_MouseLeave(object sender, EventArgs e)
    {
      ResetMomentaneousInfo();
    }

    private void canvas_CurrentSessionChanged(object sender, System.EventArgs e)
    {
      if (!openingDocumentNow)
      {
        UpdateUI();
        CalculateLapInfo();
        AlphaAdjustSelectedLap();
        CalculateMomentaneousInfoLabelRectangles();
        CreateLapHistogram();
        DrawLapHistogram();
        CreateLineGraph();
        DrawLineGraph();
      }
    }

    private void Main_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (documentChanged)
      {
        e.Cancel = (QuerySaveDocument() == DialogResult.Cancel);
      }
      if (!e.Cancel)
      {
        Util.SaveSettings(ApplicationSettings);
      }
    }

    private void Main_KeyDown(object sender, KeyEventArgs e)
    {
    }

    private void laps_SelectionChanged(object sender, EventArgs e)
    {
      if (updatingUINow) return;

      BeginWork();
      CreateLapHistogram();
      DrawLapHistogram();

      CreateLineGraph();
      DrawLineGraph();

      AlphaAdjustSelectedLap();
      EndWork();
    }

    private void laps_CellMouseEnter(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
    {
      if (e.RowIndex == -1) laps.Columns[e.ColumnIndex].HeaderCell.Style.BackColor = SystemColors.ControlLightLight;
    }

    private void laps_CellMouseLeave(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
    {
      if (e.RowIndex == -1) laps.Columns[e.ColumnIndex].HeaderCell.Style.BackColor = SystemColors.Control;
    }

    private void canvas_ActionPerformed(object sender, Canvas.ActionEventArgs e)
    {
      HandleAction(e.Action);
    }

    private void menuHelpAbout_Click(object sender, EventArgs e)
    {
      using (var ab = new AboutBox())
      {
        ab.ShowDialog();
      }
    }

    private void laps_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
    {
      if (updatingUINow) return;

      if (e.RowIndex == lapInfoList.Count - 1 && e.ColumnIndex == 0)
      {
        // the "Total" caption
        e.Value = Strings.Total;
      }
      else if (lapInfoList[e.RowIndex][e.ColumnIndex].GetType() == typeof(BlankRouteProperty))
      {
        e.Value = "-";
      }
      else
      {
        e.Value = lapInfoList[e.RowIndex][e.ColumnIndex];
      }
    }

    private void laps_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
    {
      if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
      var obj = laps.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
      var tooltipTemplateText = Strings.ResourceManager.GetString("RoutePropertyNameAndValue_" + obj.GetType().Name);
      if (tooltipTemplateText == null) tooltipTemplateText = Strings.ResourceManager.GetString("RoutePropertyNameAndValue_" + obj.GetType().Name.Replace("FromStart", ""));
      if (tooltipTemplateText != null) e.ToolTipText = string.Format(tooltipTemplateText, obj);
    }

    private void laps_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
      if (e.RowIndex == laps.Rows.Count - 1) e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
    }

    private void lapHistogramPanel_Paint(object sender, PaintEventArgs e)
    {
      DrawLapHistogram();
    }

    private void lapHistogramBinWidth_Leave(object sender, EventArgs e)
    {
      ValidateAndFormatLapHistogramBinWidthValue();
    }

    private void laps_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      lapSortOrder = (e.ColumnIndex == lapSortOrderColumnIndex && lapSortOrder == SortOrder.Ascending
                         ? SortOrder.Descending
                         : SortOrder.Ascending);
      lapSortOrderColumnIndex = e.ColumnIndex;
      SortLapGrid();
      laps.Invalidate();
    }

    private void Main_Resize(object sender, EventArgs e)
    {
      CalculateMomentaneousInfoLabelRectangles();
      // ugly trick to place dynamic help label correctly when minimizing/maximizing the form - .NET seems to be buggy
      dynamicHelpLabel.Height = dynamicHelpLabel.Height + 1;
      dynamicHelpLabel.Height = dynamicHelpLabel.Height - 1;
    }

    private void lapHistogramPanel_Resize(object sender, EventArgs e)
    {
      DrawLapHistogram();
    }

    private void lineGraph_GraphMouseHover(object sender, Canvas.RouteMouseHoverEventArgs e)
    {
      // draw/erase marker on canvas
      canvas.DrawActiveHandle(e.ParameterizedLocation);
      HandleMouseHover(e.ParameterizedLocation, e.IsClose && e.ParameterizedLocation != null);
    }

    private void lineGraph_GraphMouseDown(object sender, Canvas.RouteMouseHoverEventArgs e)
    {
      if (e.ParameterizedLocation != null)
      {
        switch (canvas.CurrentMouseTool)
        {
          case Canvas.MouseTool.Lap:
            bool showLapTimeForm = ((ModifierKeys & Keys.Shift) == Keys.Shift);
            canvas.AddLap(e.ParameterizedLocation, showLapTimeForm);
            break;
          case Canvas.MouseTool.Cut:
            canvas.Cut(e.ParameterizedLocation);
            break;
        }
      }
    }

    private void lineGraph_MouseLeave(object sender, EventArgs e)
    {
      lineGraph.HoverXValue = null;
      ResetMomentaneousInfo();
    }

    private void laps_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        var menu = new ContextMenu();
        menu.MenuItems.Add(new MenuItem(Strings.SelectLapInfoColumns, laps_ContextMenuSelectLapInfoColumns_ItemClick));
        menu.MenuItems.Add(new MenuItem(Strings.CopyToClipboard, laps_ContextMenuCopyToClipboard_ItemClick));
        menu.MenuItems.Add(new MenuItem(Strings.Print, laps_ContextMenuPrint_ItemClick));
        var rowIndex = laps.HitTest(e.X, e.Y).RowIndex;
        if(rowIndex > -1 && rowIndex < laps.Rows.Count -1)
        {
          lapIndexToDelete = lapInfoList[rowIndex].Index;
          menu.MenuItems.Add(new MenuItem(string.Format(Strings.DeleteLapX, lapInfoList[rowIndex].Index), laps_ContextMenuDelete_ItemClick));
        }
        menu.Show(laps, e.Location);
      }
    }

    private void laps_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyData == Keys.Delete && laps.SelectedRows.Count == 1)
      {
        var rowIndex = laps.SelectedRows[0].Index;
        if (rowIndex > -1 && rowIndex < laps.Rows.Count - 1) canvas.DeleteLap(canvas.CurrentSession.Laps[lapInfoList[rowIndex].Index]);
      }
    }

    private void laps_ContextMenuSelectLapInfoColumns_ItemClick(object sender, EventArgs e)
    {
      // need to handle LapNumber separately as it should not be visible
      var routePropertyTypes = new SelectableRoutePropertyTypeCollection();
      routePropertyTypes.AddRange(ApplicationSettings.LapPropertyTypes);
      routePropertyTypes.RemoveAt(0);
      using (var form = new SelectRoutePropertyTypesForm(routePropertyTypes))
      {
        form.Text = Strings.SelectLapInfoColumns;
        if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
          // need to handle LapNumber separately as it always should be included
          ApplicationSettings.LapPropertyTypes = new SelectableRoutePropertyTypeCollection() { new SelectableRoutePropertyType(typeof(LapNumber), true) };
          ApplicationSettings.LapPropertyTypes.AddRange(form.RoutePropertyTypes);
          CalculateLapInfo();
        }
      }
    }

    private void laps_ContextMenuCopyToClipboard_ItemClick(object sender, EventArgs e)
    {
      string text = GetLapsInfoAsText();
      Clipboard.SetText(text);
    }

    private void laps_ContextMenuPrint_ItemClick(object sender, EventArgs e)
    {
      var printer = new LapGridViewPrinter(laps);
      printer.Print(canvas.Document.FileName);
    }

    private void laps_ContextMenuDelete_ItemClick(object sender, EventArgs e)
    {
      canvas.DeleteLap(canvas.CurrentSession.Laps[lapIndexToDelete]);
    }

    private void sessions_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (updatingUINow) return;
      canvas.CurrentSession = canvas.Document.Sessions[sessions.SelectedIndex];
      canvas.DrawMap(Canvas.MapDrawingFlags.Route | Canvas.MapDrawingFlags.Markers);
    }

    private void sessions_ItemCheck(object sender, ItemCheckEventArgs e)
    {
      if (updatingUINow) return;
      if (e.NewValue == CheckState.Unchecked)
      {
        canvas.SelectedSessions.Remove(canvas.Document.Sessions[e.Index]);
      }
      else
      {
        canvas.SelectedSessions.Add(canvas.Document.Sessions[e.Index]);
      }
      canvas.DrawMap(Canvas.MapDrawingFlags.Route | Canvas.MapDrawingFlags.Markers);
    }

    private void bottomPanel_Paint(object sender, PaintEventArgs e)
    {
      Pen p = new Pen(Color.FromArgb(128, Color.Black));
      if (canvas.Document != null) e.Graphics.DrawLine(p, e.ClipRectangle.Left, 0, e.ClipRectangle.Right, 0);
      p.Dispose();
    }

    private void canvas_DragEnter(object sender, DragEventArgs e)
    {
      e.Effect = (e.AllowedEffect & DragDropEffects.Move) != DragDropEffects.Move ? e.AllowedEffect : DragDropEffects.Move;
    }

    private void canvas_DragDrop(object sender, DragEventArgs e)
    {
      HandleDragDrop(e);
    }

    private void toolStripContainer1_ContentPanel_DragEnter(object sender, DragEventArgs e)
    {
      e.Effect = (e.AllowedEffect & DragDropEffects.Move) != DragDropEffects.Move ? e.AllowedEffect : DragDropEffects.Move;
    }

    private void toolStripContainer1_ContentPanel_DragDrop(object sender, DragEventArgs e)
    {
      HandleDragDrop(e);
    }

    private void momentaneousInfoPanel_Paint(object sender, PaintEventArgs e)
    {
      CopyMomentaneousInfoBackBufferToScreen();
    }

    private void momentaneousInfoPanel_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        var menu = new ContextMenu();
        menu.MenuItems.Add(new MenuItem(Strings.SelectMomentaneousInfoProperties, momentaneousInfoPanel_ContextMenuSelectProperties_ItemClick));
        menu.Show(momentaneousInfoPanel, e.Location);
      }
    }

    private void momentaneousInfoPanel_Resize(object sender, EventArgs e)
    {
      CreateMomentaneousInfoPanelBackBuffer();
    }

    private void CreateMomentaneousInfoPanelBackBuffer()
    {
      momentaneousInfoPanelBackBuffer = new Bitmap(Math.Max(1, momentaneousInfoPanel.Width), Math.Max(1,momentaneousInfoPanel.Height), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
      momentaneousInfoPanelBackBufferGraphics = Graphics.FromImage(momentaneousInfoPanelBackBuffer);
      momentaneousInfoPanelBackBufferGraphics.SmoothingMode = SmoothingMode.AntiAlias;
    }

    private void momentaneousInfoPanel_ContextMenuSelectProperties_ItemClick(object sender, EventArgs e)
    {
      using (var form = new SelectRoutePropertyTypesForm(ApplicationSettings.MomentaneousInfoPropertyTypes))
      {
        form.Text = Strings.SelectMomentaneousInfoProperties;
        if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
          ApplicationSettings.MomentaneousInfoPropertyTypes = form.RoutePropertyTypes;
          CalculateMomentaneousInfoLabelRectangles();
        }
      }
    }

    #endregion

    #region Menustrip event handlers

    private void menuFileNew_Click(object sender, EventArgs e)
    {
      NewDocument();
    }

    private void menuFileOpen_Click(object sender, EventArgs e)
    {
      OpenDocumentShowDialog();
    }

    private void menuFileSave_Click(object sender, EventArgs e)
    {
      SaveDocument();
    }

    private void menuFileSaveAs_Click(object sender, EventArgs e)
    {
      SaveDocumentShowDialog();
    }

    private void menuFileExportImage_Click(object sender, EventArgs e)
    {
      ExportImage();
    }

    private void menuFileExportGPX_Click(object sender, EventArgs e)
    {
      ExportGpx();
    }

    private void menuFileExportKMZ_Click(object sender, EventArgs e)
    {
      ExportKmz();
    }

    private void menuFileExportRouteData_Click(object sender, EventArgs e)
    {
      ExportRouteData();
    }

    private void menuFileExit_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void menuEditUndo_Click(object sender, EventArgs e)
    {
      Undo();
    }

    private void menuEditRedo_Click(object sender, EventArgs e)
    {
      Redo();
    }

    private void menuEditChangeStartTime_Click(object sender, EventArgs e)
    {
      ChangeStartTime();
    }

    private void menuViewFullScreen_CheckedChanged(object sender, EventArgs e)
    {
      ToggleFullScreen(menuViewFullScreen.Checked);
    }

    private void menuViewRightPanelVisible_CheckedChanged(object sender, EventArgs e)
    {
      ToggleRightPanel(menuViewRightPanelVisible.Checked);
    }

    private void menuViewBottomPanelVisible_CheckedChanged(object sender, EventArgs e)
    {
      ToggleBottomPanel(menuViewBottomPanelVisible.Checked);
    }

    private void menuToolsAddLapsFromWinSplits_Click(object sender, EventArgs e)
    {
      AddLapsFromWinSplits();
    }

    private void menuToolsPublishMap_Click(object sender, EventArgs e)
    {
      PublishMap();
    }

    private void menuToolsOpenInGoogleEarth_Click(object sender, EventArgs e)
    {
      OpenInGoogleEarth(canvas.Document, null, null);
    }

    private void menuToolsOpenMultipleFilesInGoogleEarth_Click(object sender, EventArgs e)
    {
      OpenMultipleFilesInGoogleEarth();
    }

    private void menuHelpHelp_Click(object sender, EventArgs e)
    {
      Util.ShowHelp();
    }

    private void menuSettingsLanguage_Click(object sender, EventArgs e)
    {
      Util.SelectUICulture(true);
    }

    private void menuFileImportSessions_Click(object sender, EventArgs e)
    {
      using (var isf = new AddSessionsForm(canvas.Document))
      {
        if (isf.ShowDialog() == DialogResult.OK)
        {
          canvas.Document.Sessions = isf.Sessions;
          canvas.SelectedSessions = canvas.Document.Sessions;
          UpdateUI();
          updatingUINow = true;
          PopulateSessionList();
          updatingUINow = false;
          canvas.DrawMap(Canvas.MapDrawingFlags.Route | Canvas.MapDrawingFlags.Markers);
        }
      }
    }

    #endregion

    #region Toolstrip event handlers

    private void toolStripNew_Click(object sender, EventArgs e)
    {
      NewDocument();
    }

    private void toolStripOpen_Click(object sender, EventArgs e)
    {
      OpenDocumentShowDialog();
    }

    private void toolStripSave_Click(object sender, EventArgs e)
    {
      SaveDocument();
    }

    private void toolStripUndo_Click(object sender, EventArgs e)
    {
      Undo();
    }

    private void toolStripRedo_Click(object sender, EventArgs e)
    {
      Redo();
    }

    private void toolStripToolPointer_Click(object sender, EventArgs e)
    {
      canvas.CurrentMouseTool = Canvas.MouseTool.Pointer;
      UpdateUI();
    }

    private void toolStripToolAdjustRoute_Click(object sender, EventArgs e)
    {
      canvas.CurrentMouseTool = Canvas.MouseTool.AdjustRoute;
      UpdateUI();
    }

    private void toolStripToolZoomIn_Click(object sender, EventArgs e)
    {
      canvas.CurrentMouseTool = Canvas.MouseTool.ZoomIn;
      UpdateUI();
    }

    private void toolStripToolZoomOut_Click(object sender, EventArgs e)
    {
      canvas.CurrentMouseTool = Canvas.MouseTool.ZoomOut;
      UpdateUI();
    }

    private void toolStripToolCut_Click(object sender, EventArgs e)
    {
      canvas.CurrentMouseTool = Canvas.MouseTool.Cut;
      UpdateUI();
    }

    private void toolStripToolLap_Click(object sender, EventArgs e)
    {
      canvas.CurrentMouseTool = Canvas.MouseTool.Lap;
      UpdateUI();
    }

    private void toolStripHelp_Click(object sender, EventArgs e)
    {
      Util.ShowHelp();
    }

    private void toolStripZoom_SelectedIndexChanged(object sender, EventArgs e)
    {
      UpdateZoom();
    }

    private void toolStripZoom_Leave(object sender, EventArgs e)
    {
      UpdateZoom();
    }

    private void toolStripZoom_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (e.KeyChar == 13)
      {
        UpdateZoom();
        e.Handled = true;
        toolStripZoom.SelectAll();
      }
    }

    private void toolStripOpenInGoogleEarth_Click(object sender, EventArgs e)
    {
      OpenInGoogleEarth(canvas.Document, null, null);
    }

    private void toolStripPublishMap_Click(object sender, EventArgs e)
    {
      PublishMap();
    }

    private void toolStripFullScreen_CheckedChanged(object sender, EventArgs e)
    {
      if (!updatingUINow) ToggleFullScreen(toolStripFullScreen.Checked);
    }

    private void toolStripRightPanelVisible_CheckedChanged(object sender, EventArgs e)
    {
      if (!updatingUINow) ToggleRightPanel(toolStripRightPanelVisible.Checked);
    }

    private void toolStripBottomPanelVisible_CheckedChanged(object sender, EventArgs e)
    {
      if (!updatingUINow) ToggleBottomPanel(toolStripBottomPanelVisible.Checked);
    }

    private void colorRangeIntervalSlider_ColorRangeClicked(object sender, MouseEventArgs e)
    {
      ShowGradientEditor();
    }

    private void routeLineMaskVisible_CheckedChanged(object sender, EventArgs e)
    {
      RouteLineSettings routeLineSettings = canvas.CurrentSession.Settings.RouteLineSettingsCollection[SelectedColorCodingAttribute];
      routeLineSettings.MaskVisible = routeLineMaskVisible.Checked;
      if (!updatingUINow) canvas.DrawMap(Canvas.MapDrawingFlags.Markers | Canvas.MapDrawingFlags.Route);
    }

    private void gradientAlphaAdjustment_ValueChanged(object sender, EventArgs e)
    {
      if (!updatingUINow)
      {
        RouteLineSettings routeLineSettings = canvas.CurrentSession.Settings.RouteLineSettingsCollection[SelectedColorCodingAttribute];
        routeLineSettings.AlphaAdjustment = (double)gradientAlphaAdjustment.TrackBarControl.Value / 10;
        colorRangeIntervalSlider.SliderControl.AlphaAdjustment = routeLineSettings.AlphaAdjustment;
        colorRangeIntervalSlider.SliderControl.Refresh();
      }
    }

    private void gradientAlphaAdjustment_MouseUp(object sender, MouseEventArgs e)
    {
      if (updatingUINow) return;
      RouteLineSettings routeLineSettings = canvas.CurrentSession.Settings.RouteLineSettingsCollection[SelectedColorCodingAttribute];
      Lap lap = null;
      if (laps.SelectedRows[0].Index < laps.Rows.Count - 1) lap = canvas.CurrentSession.Laps[lapInfoList[laps.SelectedRows[0].Index].Index];
      canvas.AlphaAdjustLap(
        canvas.CurrentSession,
        lap,
        GraphicsUtil.CombineAlphaAdjustments(routeLineSettings.AlphaAdjustment, 0.5),
        GraphicsUtil.CombineAlphaAdjustments(routeLineSettings.AlphaAdjustment, -0.5),
        routeLineSettings.AlphaAdjustment);
      canvas.DrawMap(Canvas.MapDrawingFlags.Route | Canvas.MapDrawingFlags.Markers);
    }

    private void gradientAlphaAdjustment_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right) gradientAlphaAdjustment.TrackBarControl.Value = 0;
    }

    private void routeLineMaskColorButton_Click(object sender, EventArgs e)
    {
      using (var cc = new ColorChooser())
      {
        RouteLineSettings rls = canvas.CurrentSession.Settings.RouteLineSettingsCollection[SelectedColorCodingAttribute];
        cc.Color = canvas.CurrentMouseTool == Canvas.MouseTool.AdjustRoute ? rls.MonochromeColor : rls.MaskColor;
        if (cc.ShowDialog() == DialogResult.OK)
        {
          if (canvas.CurrentMouseTool == Canvas.MouseTool.AdjustRoute)
          {
            rls.MonochromeColor = cc.Color;
          }
          else
          {
            rls.MaskColor = cc.Color;
          }
          UpdateUI();
          canvas.DrawMap(Canvas.MapDrawingFlags.Markers | Canvas.MapDrawingFlags.Route);
        }
      }
    }

    private void colorRangeIntervalButton_Click(object sender, EventArgs e)
    {
      ColorRangeIntervalSliderSettings sliderSettings = canvas.Document.Settings.ColorRangeIntervalSliderSettings[SelectedColorCodingAttribute];
      using (var crie = new ColorRangeIntervalEditor(Util.GetNumericConverterFromWaypointAttribute(SelectedColorCodingAttribute), sliderSettings.MinValue, sliderSettings.MaxValue))
      {
        if (crie.ShowDialog() == DialogResult.OK)
        {
          BeginWork();
          UpdateColorRangeInterval(SelectedColorCodingAttribute, crie.IntervalStart, crie.IntervalEnd, false);
          PerformFullRedraw();
          EndWork();
        }
      }
    }

    private void colorRangeIntervalSlider_ColorRangeEndValueChanged(object sender, EventArgs e)
    {
      if (updatingUINow) return;
      BeginWork();
      colorRangeIntervalSlider_ColorRangeEndValueChanging(sender, e);
      canvas.DrawMap(Canvas.MapDrawingFlags.Route | Canvas.MapDrawingFlags.Markers);
      CreateLapHistogram();
      DrawLapHistogram();
      CreateLineGraph();
      DrawLineGraph();
      EndWork();
    }

    private void colorRangeIntervalSlider_ColorRangeStartValueChanged(object sender, EventArgs e)
    {
      if (updatingUINow) return;
      BeginWork();
      colorRangeIntervalSlider_ColorRangeStartValueChanging(sender, e);
      canvas.DrawMap(Canvas.MapDrawingFlags.Route | Canvas.MapDrawingFlags.Markers);
      CreateLapHistogram();
      DrawLapHistogram();
      CreateLineGraph();
      DrawLineGraph();
      EndWork();
    }

    private void colorRangeIntervalSlider_ColorRangeEndValueChanging(object sender, EventArgs e)
    {
      if (!updatingUINow)
      {
        updatingUINow = true;
        double value = colorRangeIntervalSlider.SliderControl.ColorRange.EndValue;
        colorRangeEndValue.Text = FormatColorRangeValue(value);
        canvas.CurrentSession.Settings.RouteLineSettingsCollection[SelectedColorCodingAttribute].ColorRange.EndValue = value;
        colorRangeEndValue.SelectAll();
        updatingUINow = false;
      }
    }

    private void colorRangeIntervalSlider_ColorRangeStartValueChanging(object sender, EventArgs e)
    {
      if (!updatingUINow)
      {
        updatingUINow = true;
        double value = colorRangeIntervalSlider.SliderControl.ColorRange.StartValue;
        colorRangeStartValue.Text = FormatColorRangeValue(value);
        canvas.CurrentSession.Settings.RouteLineSettingsCollection[SelectedColorCodingAttribute].ColorRange.StartValue = value;
        colorRangeStartValue.SelectAll();
        updatingUINow = false;
      }
    }

    private void colorRangeStartValue_Leave(object sender, EventArgs e)
    {
      double? startValue = ParseColorRangeValue(colorRangeStartValue.Text);
      RouteLineSettings rls = canvas.CurrentSession.Settings.RouteLineSettingsCollection[SelectedColorCodingAttribute];
      if (startValue.HasValue)
      {
        if (startValue.Value < colorRangeIntervalSlider.SliderControl.MinValue)
        {
          colorRangeIntervalSlider.SliderControl.MinValue = startValue.Value;
          UpdateColorRangeInterval(SelectedColorCodingAttribute, colorRangeIntervalSlider.SliderControl.MinValue, colorRangeIntervalSlider.SliderControl.MaxValue, false);
          UpdateUI();
        }
        rls.ColorRange.StartValue =
          Math.Max(Math.Min(startValue.Value, colorRangeIntervalSlider.SliderControl.MaxValue), colorRangeIntervalSlider.SliderControl.MinValue);
      }

      colorRangeStartValue.Text = FormatColorRangeValue(rls.ColorRange.StartValue);
      colorRangeIntervalSlider.SliderControl.ColorRange.StartValue = rls.ColorRange.StartValue;
    }

    private void colorRangeStartValue_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        colorRangeStartValue_Leave(sender, e);
        colorRangeStartValue.SelectAll();
        e.SuppressKeyPress = true;
      }
    }

    private void colorRangeEndValue_Leave(object sender, EventArgs e)
    {
      double? endValue = ParseColorRangeValue(colorRangeEndValue.Text);
      RouteLineSettings rls = canvas.CurrentSession.Settings.RouteLineSettingsCollection[SelectedColorCodingAttribute];
      if (endValue.HasValue)
      {
        if (endValue.Value > colorRangeIntervalSlider.SliderControl.MaxValue)
        {
          colorRangeIntervalSlider.SliderControl.MaxValue = endValue.Value;
          UpdateColorRangeInterval(SelectedColorCodingAttribute, colorRangeIntervalSlider.SliderControl.MinValue, colorRangeIntervalSlider.SliderControl.MaxValue, false);
          UpdateUI();
        }
        rls.ColorRange.EndValue =
          Math.Max(Math.Min(endValue.Value, colorRangeIntervalSlider.SliderControl.MaxValue), colorRangeIntervalSlider.SliderControl.MinValue);
      }

      colorRangeEndValue.Text = FormatColorRangeValue(rls.ColorRange.EndValue);
      colorRangeIntervalSlider.SliderControl.ColorRange.EndValue = rls.ColorRange.EndValue;
    }

    private void colorRangeEndValue_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        colorRangeEndValue_Leave(sender, e);
        colorRangeEndValue.SelectAll();
        e.SuppressKeyPress = true;
      }
    }

    private void colorCodingAttributes_SelectedIndexChanged(object sender, EventArgs e)
    {
      BeginWork();
      canvas.PreventRedraw = true;
      canvas.ColorCodingAttribute = SelectedColorCodingAttribute;
      canvas.PreventRedraw = false;
      if (!updatingUINow)
      {
        UpdateUI();
      }
      CreateLapHistogram();
      DrawLapHistogram();
      CreateLineGraph();
      DrawLineGraph();
      canvas.DrawMap(Canvas.MapDrawingFlags.Markers | Canvas.MapDrawingFlags.Route);
      EndWork();
    }

    private void routeLineMaskWidth_ValueChanged(object sender, EventArgs e)
    {
      if (!updatingUINow)
      {
        RouteLineSettings routeLineSettings = canvas.CurrentSession.Settings.RouteLineSettingsCollection[SelectedColorCodingAttribute];
        routeLineSettings.MaskWidth = (double)routeLineMaskWidth.NumericUpDownControl.Value;
        canvas.DrawMap(Canvas.MapDrawingFlags.Markers | Canvas.MapDrawingFlags.Route);
      }
    }

    private void routeLineWidth_ValueChanged(object sender, EventArgs e)
    {
      if (!updatingUINow)
      {
        RouteLineSettings routeLineSettings = canvas.CurrentSession.Settings.RouteLineSettingsCollection[SelectedColorCodingAttribute];
        if (canvas.CurrentMouseTool == Canvas.MouseTool.AdjustRoute)
        {
          routeLineSettings.MonochromeWidth = (double)routeLineWidth.NumericUpDownControl.Value;
        }
        else
        {
          routeLineSettings.Width = (double)routeLineWidth.NumericUpDownControl.Value;
        }
        canvas.DrawMap(Canvas.MapDrawingFlags.Markers | Canvas.MapDrawingFlags.Route);
      }
    }

    private void routeLineWidth_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        e.SuppressKeyPress = true;
      }
    }

    private void routeLineMaskWidth_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        e.SuppressKeyPress = true;
      }
    }

    private void toolStripDonate_Click(object sender, EventArgs e)
    {
      System.Diagnostics.Process.Start(Strings.DonationUrl);
    }

    private void toolStripApplicationSettings_Click(object sender, EventArgs e)
    {
      using (var settingsEditor = new ApplicationSettingsEditor())
      {
        settingsEditor.ApplicationSettings = ApplicationSettings;
        if (settingsEditor.ShowDialog() == DialogResult.OK)
        {

        }
      }
    }

    private void smoothingIntervalLength_Leave(object sender, EventArgs e)
    {
      ValidateAndFormatSmoothingIntervalLengthValue();
    }

    private void smoothingIntervalLength_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        ValidateAndFormatSmoothingIntervalLengthValue();
        smoothingIntervalLength.SelectAll();
        e.SuppressKeyPress = true;
      }
    }

    private void circleTimeRadius_Leave(object sender, EventArgs e)
    {
      ValidateAndFormatCircleTimeRadiusValue();
    }

    private void circleTimeRadius_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        ValidateAndFormatCircleTimeRadiusValue();
        circleTimeRadius.SelectAll();
        e.SuppressKeyPress = true;
      }
    }
    
    private void exportLapHistogramImage_Click(object sender, EventArgs e)
    {
      ExportLapHistogramImage();
    }

    private void lapHistogramBinWidth_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        ValidateAndFormatLapHistogramBinWidthValue();
        lapHistogramBinWidth.SelectAll();
        e.SuppressKeyPress = true;
      }
    }

    private void toolStripAutoAdjustColorRangeInterval_CheckedChanged(object sender, EventArgs e)
    {
      if (updatingUINow) return;
      ApplicationSettings.AutoAdjustColorRangeInterval = toolStripAutoAdjustColorRangeInterval.Checked;
      if (ApplicationSettings.AutoAdjustColorRangeInterval) PerformColorRangeIntervalAutoAdjustment();
    }

    private void toolStrip_DragOver(object sender, DragEventArgs e)
    {
      e.Effect = toolStripOpenInGoogleEarth.Bounds.Contains(e.X, e.Y) ? DragDropEffects.Move : DragDropEffects.None;
    }

    #endregion

    #region Private enums

    #endregion

    #region Handling of unexpected exceptions

    public void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      var ex = e.ExceptionObject as Exception;
      if (ex != null)
      {
        LogUtil.LogError(ex.Message + " " + ex.StackTrace);
        ShowUnhandledExceptionMessageBox(ex);
      }
      Environment.Exit(0);
    }

    public void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
    {
      LogUtil.LogError(e.Exception.Message + " " + e.Exception.StackTrace);
      ShowUnhandledExceptionMessageBox(e.Exception);
      Environment.Exit(0);
    }

    private void ShowUnhandledExceptionMessageBox(Exception ex)
    {
      var topLevelException = new ApplicationException(Strings.UnexpectedExceptionMessage, ex);
      Util.ShowExceptionMessageBox(topLevelException, Strings.QuickRoute);
    }

    #endregion

    private void routeAppearanceToolstrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {

    }
  }
}