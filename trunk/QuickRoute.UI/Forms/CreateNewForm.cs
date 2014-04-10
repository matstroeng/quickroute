using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;
using System.IO;
using QuickRoute.BusinessEntities.Importers;
using QuickRoute.Controls;
using QuickRoute.Resources;
using QuickRoute.UI.Classes;

namespace QuickRoute.UI.Forms
{
  public partial class CreateNewForm : Form
  {
    private readonly List<FileFormat> supportedMapImageFileFormats;
    private readonly List<RouteFileFormat> supportedRouteFileFormats;
    private readonly List<GPSDevice> supportedGPSDevices;
    private Image transformedImage;
    private byte[] originalMapBytes;
    private string imageFileName;
    private double imageRotationInDegrees = 0;
    private double imageScale = 1;
    private Rectangle? imageCropRectangle = null;
    private SourceType? imageSourceType;
    private bool loadingImageNow;
    private DownloadProgressChangedEventArgs loadingImageProgressChangedEventArgs;
    private DateTime lastLabelUpdate;
    private FileLoader imageLoader;

    public CreateNewForm()
      : this(null)
    {

    }

    public CreateNewForm(string initialRouteFileName)
    {
      InitializeComponent();

      mapImageFileName.Items.AddRange(Util.ApplicationSettings.RecentMapImageFileNames.ToArray());
      routeFileName.Items.AddRange(Util.ApplicationSettings.RecentRouteFileNames.ToArray());

      supportedMapImageFileFormats = SupportedImportFormatManager.GetSupportedMapFileFormats();
      mapImageFileFormatComboBox.DataSource = supportedMapImageFileFormats;

      supportedRouteFileFormats = SupportedImportFormatManager.GetSupportedRouteFileFormats();
      routeFileFormatComboBox.DataSource = supportedRouteFileFormats;

      supportedGPSDevices = SupportedImportFormatManager.GetSupportedGPSDevices();

      bool gpsDevicesFound = SearchForGPSDevices();

      if (initialRouteFileName != null)
      {
        routeFileName.Text = initialRouteFileName;
        routeFromFile.Checked = true;
      }
      else
      {
        if (!gpsDevicesFound) routeFromFile.Checked = true;
      }

      InitialTransformation = new Transformation();

      UpdateUI();

      UpdateImageSizeLabel();
    }

    #region Public properties

    public Map Map { get; private set; }

    public ImportResult ImportResult { get; private set; }

    public Transformation InitialTransformation { get; private set; }

    public SessionPerson Person
    {
      get
      {
        return persons.SelectedItem as SessionPerson ?? new SessionPerson() { Name = persons.Text };
      }
      set
      {
        persons.SelectedItem = value;
      }
    }

    public string FileNameProposal
    {
      get
      {
        if (!string.IsNullOrEmpty(mapImageFileName.Text))
        {
          return Path.GetFileNameWithoutExtension(mapImageFileName.Text) + ".qrt";
          //Uri uri = new Uri(mapImageUrl.Text);
          //return Path.GetFileNameWithoutExtension(uri.Segments[uri.Segments.Length - 1]) + ".qrt";
        }
        return Strings.Untitled + ".qrt";
      }
    }

    #endregion

    #region Event handlers

    private void ok_Click(object sender, EventArgs e)
    {
      if (Import() == DialogResult.OK)
      {
        DialogResult = DialogResult.OK;
        Close();
      }
    }

    private void cancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

    private void mapImageFileNameBrowse_Click(object sender, EventArgs e)
    {
      using (var ofd = new OpenFileDialog { CheckFileExists = true })
      {
        var filterItems = new string[supportedMapImageFileFormats.Count];
        for (int i = 0; i < supportedMapImageFileFormats.Count; i++)
        {
          filterItems[i] = supportedMapImageFileFormats[i].FileFilter;
        }
        ofd.Filter = string.Join("|", filterItems);
        ofd.FilterIndex = mapImageFileFormatComboBox.SelectedIndex + 1;
        ofd.Title = Strings.SelectMapImageFile;
        if (Util.ApplicationSettings.InitialFolders.ContainsKey(ApplicationSettings.FileDialogType.ImportMapImage))
        {
          ofd.InitialDirectory =
            Util.ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.ImportMapImage];
        }

        if (ofd.ShowDialog() == DialogResult.OK)
        {
          mapImageFileName.Text = ofd.FileName;
          mapImageFileFormatComboBox.SelectedIndex = ofd.FilterIndex - 1;
          Util.ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.ImportMapImage] =
            Path.GetDirectoryName(ofd.FileName);
          LoadImage();
        }
      }
    }

    private void routeFileNameBrowse_Click(object sender, EventArgs e)
    {
      var filterItems = new string[supportedRouteFileFormats.Count];
      for (var i = 0; i < supportedRouteFileFormats.Count; i++)
      {
        filterItems[i] = supportedRouteFileFormats[i].FileFilter;
      }
      using (var ofd = new OpenFileDialog
                         {
                           CheckFileExists = true,
                           Filter = string.Join("|", filterItems),
                           FilterIndex = (routeFileFormatComboBox.SelectedIndex + 1),
                           Title = Strings.SelectRouteFile
                         })
      {
        if (Util.ApplicationSettings.InitialFolders.ContainsKey(ApplicationSettings.FileDialogType.ImportRoute))
        {
          ofd.InitialDirectory = Util.ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.ImportRoute];
        }

        if (ofd.ShowDialog() == DialogResult.OK)
        {
          routeFileName.Text = ofd.FileName;
          routeFileFormatComboBox.SelectedIndex = ofd.FilterIndex - 1;
          Util.ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.ImportRoute] =
            Path.GetDirectoryName(ofd.FileName);
        }
      }
    }

    private void routeFromFile_CheckedChanged(object sender, EventArgs e)
    {
      UpdateUI();
    }

    private void routeFromGpsDevice_CheckedChanged(object sender, EventArgs e)
    {
      if (routeFromGpsDevice.Checked) SearchForGPSDevices();
      UpdateUI();
    }

    private void routeFileName_TextChanged(object sender, EventArgs e)
    {
      UpdateRouteFileFormat();
    }

    private void mapImageFileName_TextChanged(object sender, EventArgs e)
    {
        UpdateMapImageFileFormat();
    }

    private void mapImageFileName_SelectedValueChanged(object sender, EventArgs e)
    {
      LoadImage();
    }

    private void mapImageFileName_Leave(object sender, EventArgs e)
    {
      UpdateMapImageFileFormat();
      LoadImage();
    }

    private void refreshButton_Click(object sender, EventArgs e)
    {
      using (var progressIndicator = new RefreshDevicesProgressIndicator())
      {
        routeGpsDevice.DataSource = null;
        routeGpsDevice.Items.Clear();
        SupportedImportFormatManager.StartRefreshGPSDevices();
        progressIndicator.ShowDialog();
        SearchForGPSDevices();
      }
    }

    private void imageSizeLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      try
      {
        using (var image = GetImageFromBytes(originalMapBytes, SelectedImageFileFormat))
        {
          using (var imageManipulatorForm = new ImageManipulator(image, imageScale, imageRotationInDegrees, imageCropRectangle))
          {
            var result = imageManipulatorForm.ShowDialog();
            if (result == DialogResult.OK)
            {
              transformedImage = imageManipulatorForm.ManipulatedImage;
              imageScale = imageManipulatorForm.ImageScale;
              imageRotationInDegrees = imageManipulatorForm.ImageRotationInDegrees;
              imageCropRectangle = imageManipulatorForm.ImageCropRectangle;
              UpdateImageSizeLabel();
            }
          }
        }
      }
      catch (Exception)
      {
      }
    }

    private void mapImageFileName_DragEnter(object sender, DragEventArgs e)
    {
      e.Effect = (e.AllowedEffect & DragDropEffects.Move) != DragDropEffects.Move ? e.AllowedEffect : DragDropEffects.Move;
    }

    private void mapImageFileName_DragDrop(object sender, DragEventArgs e)
    {
      var fileNames = new DragDropManager().GetDroppedFileNames(e);
      if (fileNames.Count > 0)
      {
        mapImageFileName.Text = fileNames[0];
        LoadImage();
      }
    }

    protected void routeImporter_EndWork(object sender, EventArgs e)
    {
      Cursor = Cursors.Default;
    }

    protected void routeImporter_WorkProgress(object sender, WorkProgressEventArgs e)
    {
    }

    protected void routeImporter_BeginWork(object sender, EventArgs e)
    {
      Cursor = Cursors.WaitCursor;
    }

    #endregion

    private void LoadImage()
    {
      if (mapImageFileName.Text == imageFileName) return;

      imageFileName = mapImageFileName.Text;
      originalMapBytes = null;
      transformedImage = null;
      if (imageLoader != null)
      {
        // cancel previous load
        imageLoader.Dispose();
      }
      UpdateImageSizeLabel();

      imageLoader = new FileLoader(imageFileName);
      imageLoader.Loaded += ImageLoaded;
      imageLoader.Error += ImageLoadingError;
      imageLoader.ProgressChanged += ImageLoadingProgressChanged;
      loadingImageNow = true;
      loadingImageProgressChangedEventArgs = null;
      imageLoader.Load();
    }

    private void ImageLoadingError(object sender, ErrorEventArgs e)
    {
      originalMapBytes = null;
      transformedImage = null;
      loadingImageNow = false;
      imageSourceType = e.SourceType;
      imageLoader.Dispose();
      UpdateImageSizeLabel();
    }

    private void ImageLoadingProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
      if ((DateTime.Now - lastLabelUpdate).TotalMilliseconds > 200)
      {
        loadingImageProgressChangedEventArgs = e;
        UpdateImageSizeLabel();
        lastLabelUpdate = DateTime.Now;
      }
    }

    private void ImageLoaded(object sender, LoadedEventArgs e)
    {
      loadingImageNow = false;
      originalMapBytes = e.Data;
      transformedImage = GetImageFromBytes(e.Data, SelectedImageFileFormat);
      imageScale = 1;
      imageRotationInDegrees = 0;
      imageCropRectangle = null;
      imageSourceType = e.SourceType;
      UpdateImageSizeLabel();
      imageLoader.Dispose();
    }

    public void SetPersons(List<SessionPerson> personList)
    {
      persons.Items.Clear();
      if (personList == null || personList.Count == 0) return;
      persons.Items.AddRange(personList.ToArray());
      persons.SelectedItem = personList[0];
    }

    private DialogResult Import()
    {
      // validate file names
      if (imageSourceType != SourceType.Url && mapImageFileName.Text != "" && !File.Exists(mapImageFileName.Text))
      {
        MessageBox.Show(Strings.MapImageFileDoesNotExist, Strings.InvalidMapImage, MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
        return DialogResult.Cancel;
      }
      if (routeFromFile.Checked && !File.Exists(routeFileName.Text))
      {
        MessageBox.Show(Strings.RouteFileDoesNotExist, Strings.InvalidRoute, MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
        return DialogResult.Cancel;
      }

      IRouteImporter routeImporter = null;
      if (routeFromFile.Checked)
      {
        IRouteFileImporter routeFileImporter = ((RouteFileFormat)routeFileFormatComboBox.SelectedItem).Importer;
        routeFileImporter.FileName = routeFileName.Text;
        routeImporter = routeFileImporter;
      }
      else if (routeFromGpsDevice.Checked)
      {
        GPSDevice gpsDevice = routeGpsDevice.SelectedItem as GPSDevice;
        if (gpsDevice == null)
        {
          MessageBox.Show(Strings.NoGPSDevicesConnectedMessageBox, Strings.InvalidRoute, MessageBoxButtons.OK,
                          MessageBoxIcon.Error);
          return DialogResult.Cancel;
        }
        routeImporter = gpsDevice.Importer;
      }

      routeImporter.BeginWork += routeImporter_BeginWork;
      routeImporter.WorkProgress += routeImporter_WorkProgress;
      routeImporter.EndWork += routeImporter_EndWork;

      DialogResult result;
      try
      {
        result = routeImporter.ShowPreImportDialogs();
      }
      catch (Exception ex)
      {
        Cursor = Cursors.Default;
        Util.ShowExceptionMessageBox(ex, Strings.InvalidRoute);
        return DialogResult.Cancel;
      }
      if (result == DialogResult.OK)
      {
        try
        {
          routeImporter.Import();
        }
        catch (Exception ex)
        {
          routeImporter.ImportResult.Succeeded = false;
          routeImporter.ImportResult.Error = ImportError.Unknown;
          routeImporter.ImportResult.ErrorMessage = ex.Message;
          routeImporter.ImportResult.Exception = ex;
        }

        if (!routeImporter.ImportResult.Succeeded)
        {
          // an error occured, show relevant error info and cancel creation of new document.
          switch (routeImporter.ImportResult.Error)
          {
            case ImportError.NoWaypoints:
              routeImporter.ImportResult.ErrorMessage = Strings.RouteImportError_NoWaypoints;
              break;

            case ImportError.NoWaypointTimes:
              routeImporter.ImportResult.ErrorMessage = Strings.RouteImportError_NoWaypointTimes;
              break;

          }
          Cursor = Cursors.Default;
          if (routeImporter.ImportResult.Exception != null)
          {
            Util.ShowExceptionMessageBox(routeImporter.ImportResult.Exception, Strings.InvalidRoute);
          }
          else
          {
            MessageBox.Show(routeImporter.ImportResult.ErrorMessage, Strings.InvalidRoute, MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
          }
          return DialogResult.Cancel;
        }

        try
        {
          if (string.IsNullOrEmpty(mapImageFileName.Text))
          {
            var blankMap = new Bitmap(1500, 1500);
            using (Graphics g = Graphics.FromImage(blankMap))
            {
              g.Clear(Color.White);
            }
            Map = new Map(blankMap);
          }
          else if (!ImageHasBeenTransformed)
          {
            CreateMapAndSetInitialTransformations();
          }
          else
          {
            // don't care about transformations embedded in jpg, qrt or kmz files since the original file has been transformed
            Map = new Map(new Bitmap(transformedImage));
          }
        }
        catch (Exception ex)
        {
          Cursor = Cursors.Default;
          Util.ShowExceptionMessageBox(ex, Strings.InvalidMapImage);
          return DialogResult.Cancel;
        }
        ImportResult = routeImporter.ImportResult;

        if (!string.IsNullOrEmpty(mapImageFileName.Text)) Util.ApplicationSettings.AddRecentMapImageFileName(mapImageFileName.Text);
        if (routeFromFile.Checked) Util.ApplicationSettings.AddRecentRouteFileName(routeFileName.Text);
      }
      return result;
    }

    private void CreateMapAndSetInitialTransformations()
    {
      switch (SelectedImageFileFormat)
      {
        case ImageFileFormat.ImageFile:
          using (var ms = new MemoryStream(originalMapBytes))
          {
            Map = new Map(ms);
            ms.Position = 0;
            var ed = QuickRouteJpegExtensionData.FromStream(ms);
            // is it a QuickRoute image? if yes, use embedded transformation matrix
            if (ed != null && ed.Sessions != null && ed.Sessions.Count > 0)
            {
              InitialTransformation = ed.Sessions.CalculateAverageTransformation();
            }
            break;
          }

        case ImageFileFormat.QuickRouteFile:
          using (var ms = new MemoryStream(originalMapBytes))
          {
            var d = Document.OpenFromQrt(ms);
            if (d != null)
            {
              Map = d.Map;
              InitialTransformation = d.Sessions.CalculateAverageTransformation();
            }
          }
          break;

        case ImageFileFormat.KmzFile:
          using (var ms = new MemoryStream(originalMapBytes))
          {
            var kmz = new KmzDocument(ms);
            if (kmz.ImageStream != null)
            {
              Map = new Map(kmz.ImageStream);
              InitialTransformation = kmz.Transformation;
            }
            break;
          }
      }
    }

    private void UpdateUI()
    {
      routeFileName.Enabled = routeFromFile.Checked;
      routeFileNameBrowse.Enabled = routeFromFile.Checked;
      routeFileFormatComboBox.Enabled = routeFromFile.Checked;

      routeGpsDevice.Enabled = routeFromGpsDevice.Checked;
      refreshButton.Enabled = routeFromGpsDevice.Checked;
    }

    private bool SearchForGPSDevices()
    {
      routeGpsDevice.DataSource = null;
      routeGpsDevice.Items.Clear();
      var devices = SupportedImportFormatManager.SearchForGPSDevices();
      bool devicesFound = (devices.Count > 0);
      if (!devicesFound)
      {
        routeGpsDevice.Items.Add(Strings.NoGPSDevicesConnected);
      }
      else
      {
        routeGpsDevice.DataSource = devices;
      }
      routeGpsDevice.SelectedIndex = 0;
      return devicesFound;
    }

    private void UpdateRouteFileFormat()
    {
      bool found = false;
      foreach (RouteFileFormat ff in supportedRouteFileFormats)
      {
        foreach (string e in ff.Extensions)
        {
          if (routeFileName.Text.EndsWith(e))
          {
            routeFileFormatComboBox.SelectedItem = ff;
            found = true;
            break;
          }
        }
        if (found) break;
      }
    }

    private void UpdateMapImageFileFormat()
    {
      bool found = false;
      foreach (FileFormat ff in supportedMapImageFileFormats)
      {
        foreach (string e in ff.Extensions)
        {
          if (mapImageFileName.Text.EndsWith(e, StringComparison.InvariantCultureIgnoreCase))
          {
            mapImageFileFormatComboBox.SelectedItem = ff;
            found = true;
            break;
          }
        }
        if (found) break;
      }

    }

    private void UpdateImageSizeLabel()
    {
      if (loadingImageNow && loadingImageProgressChangedEventArgs != null)
      {
        imageSizeLabel.Text = string.Format(
          "{0}: {1:P0}",
          Strings.DownloadingImage,
          (double)loadingImageProgressChangedEventArgs.ProgressPercentage / 100);
        imageSizeLabel.LinkArea = new LinkArea();
        Application.DoEvents();
        return;
      }

      if (transformedImage == null)
      {
        // no image loaded yet, so no image size info to show
        imageSizeLabel.Text = "";
        return;
      }

      var text = string.Format("{0}: {1} x {2}", Strings.ImageSize, transformedImage.Width, transformedImage.Height);
      if (imageScale != 1 || imageRotationInDegrees != 0)
      {
        text += string.Format(" ({0}{1}{2})",
                                       imageScale == 1 ? null : imageScale.ToString("P0"),
                                       imageScale == 1 || imageRotationInDegrees == 0 ? null : ", ",
                                       imageRotationInDegrees == 0 ? null : imageRotationInDegrees + "°");
      }
      text += ". " + Strings.Change;

      imageSizeLabel.Text = text;
      // need to assign a new linkarea first, otherwise the text is clipped by two letters for some strange reason, bug in .NET?
      imageSizeLabel.LinkArea = new LinkArea();
      imageSizeLabel.LinkArea = new LinkArea(text.Length - Strings.Change.Length, Strings.Change.Length);
    }

    private static Image GetImageFromBytes(byte[] bytes, ImageFileFormat fileFormat)
    {
      try
      {
        switch (fileFormat)
        {
          case ImageFileFormat.ImageFile:
            using (var ms = new MemoryStream(bytes))
            {
              return Image.FromStream(ms);
            }

          case ImageFileFormat.QuickRouteFile:
            using (var ms = new MemoryStream(bytes))
            {
              var d = Document.OpenFromQrt(ms);
              return d.Map.Image;
            }

          case ImageFileFormat.KmzFile:
            using (var ms = new MemoryStream(bytes))
            {
              var kmz = new KmzDocument(ms);
              if (kmz.ImageStream != null)
              {
                return Image.FromStream(kmz.ImageStream);
              }
            }
            break;
        }
      }
      catch 
      { 
      }

      return null;
    }

    private ImageFileFormat SelectedImageFileFormat
    {
      get
      {
        switch (mapImageFileFormatComboBox.SelectedIndex)
        {
          case 0: return ImageFileFormat.ImageFile;
          case 1: return ImageFileFormat.QuickRouteFile;
          case 2: return ImageFileFormat.KmzFile;
        }
        return default(ImageFileFormat);
      }
    }

    private bool ImageHasBeenTransformed
    {
      get { return imageScale != 1 || imageRotationInDegrees != 0 || imageCropRectangle != null; }
    }

  }

  public enum ImageFileFormat
  {
    ImageFile,
    QuickRouteFile,
    KmzFile
  }

  public class FileLoader : IDisposable
  {
    private readonly string fileNameOrUrl;
    private WebClient webClient;

    public FileLoader(string fileNameOrUrl)
    {
      this.fileNameOrUrl = fileNameOrUrl;
    }

    public void Load()
    {
      if (File.Exists(fileNameOrUrl))
      {
        LoadFromDisk(fileNameOrUrl);
      }
      else if (Uri.IsWellFormedUriString(fileNameOrUrl, UriKind.Absolute))
      {
        LoadFromUrl(fileNameOrUrl);
      }
      else
      {
        if (Error != null) Error(this, new ErrorEventArgs() { Exception = new Exception("Not a file or url."), SourceType = SourceType.Unknown });
      }
    }

    private void LoadFromDisk(string fileName)
    {
      try
      {
        var data = File.ReadAllBytes(fileName);
        if (Loaded != null) Loaded(this, new LoadedEventArgs() { Data = data, SourceType = SourceType.Disk });
      }
      catch (Exception ex)
      {
        if (Error != null) Error(this, new ErrorEventArgs() { Exception = ex, SourceType = SourceType.Disk });
      }
    }

    private void LoadFromUrl(string url)
    {
      try
      {
        webClient = new WebClient();
        webClient.DownloadDataCompleted += DownloadDataCompleted;
        webClient.DownloadProgressChanged += DownloadProgressChanged;
        webClient.DownloadDataAsync(new Uri(url));
      }
      catch (Exception ex)
      {
        if (Error != null) Error(this, new ErrorEventArgs() { Exception = ex, SourceType = SourceType.Url });
        webClient.Dispose();
      }
    }

    private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
      if (ProgressChanged != null) ProgressChanged(this, e);
    }

    private void DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
    {
      try
      {
        if (!e.Cancelled)
        {
          if (Loaded != null) Loaded(this, new LoadedEventArgs() { Data = e.Result, SourceType = SourceType.Url });
        }
      }
      catch (Exception ex)
      {
        if (Error != null) Error(this, new ErrorEventArgs() { Exception = ex, SourceType = SourceType.Url });
      }
      finally
      {
        webClient.Dispose();
      }
    }

    public event EventHandler<LoadedEventArgs> Loaded;
    public event EventHandler<DownloadProgressChangedEventArgs> ProgressChanged;
    public event EventHandler<ErrorEventArgs> Error;

    public void Dispose()
    {
      if (webClient != null)
      {
        webClient.CancelAsync();
        webClient.Dispose();
      }
    }
  }

  public class LoadedEventArgs : EventArgs
  {
    public byte[] Data { get; set; }
    public SourceType SourceType { get; set; }
  }

  public class ErrorEventArgs : EventArgs
  {
    public Exception Exception { get; set; }
    public SourceType SourceType { get; set; }
  }

  public enum SourceType
  {
    Disk,
    Url,
    Unknown
  }

}