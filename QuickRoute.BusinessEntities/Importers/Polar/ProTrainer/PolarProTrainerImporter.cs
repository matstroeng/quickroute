using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using QuickRoute.BusinessEntities;
using QuickRoute.BusinessEntities.Importers.GPX;

namespace QuickRoute.BusinessEntities.Importers.Polar.ProTrainer
{
  public class PolarProTrainerImporter : IGPSDeviceImporter
  {
    private List<string> paths = new List<string>();
    private PolarSession sessionToImport;

    /// <summary>
    /// The paths that Program Files\Polar\Polar ProTrainer\ path. Shall end with a backslash.
    /// </summary>
    public List<string> Paths
    {
      get { return paths; }
      set { paths = value; }
    }

    private static List<PolarPerson> GetPersons(IEnumerable<DirectoryInfo> personDirectories)
    {
      var persons = new List<PolarPerson>();
      // for each person
      foreach (DirectoryInfo di in personDirectories)
      {
        var yearDirectories = new List<DirectoryInfo>(di.GetDirectories());
        var person = new PolarPerson(di.Name, new List<PolarSession>());
        // remove all directories with non-numeric names
        yearDirectories.RemoveAll(item =>
        {
          int dummy; return !int.TryParse(item.Name, out dummy);
        });
        // for each year directory
        foreach (var ydi in yearDirectories)
        {
          // get all gpx files
          var gpxFiles = ydi.GetFiles("*.gpx");
          foreach (var gpxFile in gpxFiles)
          {
            var session = CreatePersonSessionFromFile(gpxFile.FullName);
            if (session != null) person.Sessions.Add(session);
          }
        }
        person.Sessions.Sort();
        person.Sessions.Reverse();
        persons.Add(person);
      }
      return persons;
    }

    private static PolarSession CreatePersonSessionFromFile(string fileName)
    {
      var reader = new XmlTextReader(fileName);
      PolarSession session = null;
      try
      {
        var doc = new XPathDocument(reader);
        var nav = doc.CreateNavigator();
        var nsManager = new XmlNamespaceManager(nav.NameTable);
        nsManager.AddNamespace("gpx10", "http://www.topografix.com/GPX/1/0");
        nsManager.AddNamespace("gpx11", "http://www.topografix.com/GPX/1/1");
        XPathNavigator timeNode;
        timeNode = nav.SelectSingleNode("/gpx10:gpx/gpx10:time", nsManager);
        if (timeNode != null)
        {
          session = new PolarSession(timeNode.ValueAsDateTime, fileName);
        }
        timeNode = nav.SelectSingleNode("/gpx11:gpx/gpx11:time", nsManager);
        if (timeNode != null)
        {
          session = new PolarSession(timeNode.ValueAsDateTime, fileName);
        }
      }
      catch (Exception)
      {

      }
      reader.Close();
      return session;
    }

    #region IRouteImporter Members
    public ImportResult ImportResult { get; set; }

    public DialogResult ShowPreImportDialogs()
    {
      if (BeginWork != null) BeginWork(this, new EventArgs());

      List<PolarPerson> persons = new List<PolarPerson>();

      foreach (var path in paths)
      {
        var baseDir = new DirectoryInfo(path);
        if (baseDir.Exists)
        {
          // get all persons
          var personDirectories = new List<DirectoryInfo>(baseDir.GetDirectories());
          // exclude logo, samples, themes and usb directories
          personDirectories.RemoveAll(
            di => (new List<string> { "logo", "samples", "themes", "usb" }).Contains(di.Name.ToLower()));
          persons.AddRange(GetPersons(personDirectories));
        }
      }
      if (EndWork != null) EndWork(this, new EventArgs());

      using (var dlg = new PersonSessionSelector(persons))
      {
        DialogResult result = dlg.ShowDialog();
        if (result == DialogResult.OK)
        {
          sessionToImport = dlg.SelectedSession;
        }
        return result;
      }
    }

    public void Import()
    {
      ImportResult = new ImportResult();
      // import GPX
      GPX.GPXImporter GPXImporter = new GPXImporter();
      GPXImporter.FileName = sessionToImport.FileName;
      GPXImporter.Import();

      ImportResult = GPXImporter.ImportResult;
    }

    public event EventHandler<EventArgs> BeginWork;
    public event EventHandler<EventArgs> EndWork;
    public event EventHandler<WorkProgressEventArgs> WorkProgress;

    #endregion

    #region IGPSDeviceImporter Members

    public bool IsConnected
    {
      get
      {
        foreach (var path in paths)
        {
          if (Directory.Exists(path)) return true;
        }
        return false;
      }
    }

    public bool CachedDataExists
    {
      get { return IsConnected; }
    }

    public string DeviceName
    {
      get
      {
        return "Polar ProTrainer";
      }
    }

    public void Refresh()
    {
      // do nothing
    }

    #endregion
  }

  public class PolarPerson
  {
    public PolarPerson(string name, List<PolarSession> sessions)
    {
      Name = name;
      Sessions = sessions;
    }
    public string Name { get; set; }
    public List<PolarSession> Sessions { get; set; }

    public override string ToString()
    {
      return string.Format("{0} ({1})", Name, Sessions.Count);
    }
  }

  public class PolarSession : IComparable<PolarSession>
  {
    public PolarSession(DateTime time, string fileName)
    {
      Time = time;
      FileName = fileName;
    }

    public string FileName { get; set; }
    public DateTime Time { get; set; }

    public int CompareTo(PolarSession other)
    {
      return Time.CompareTo(other.Time);
    }

    public override string ToString()
    {
      return Time.ToString("yyyy-MM-dd HH:mm:ss");
    }
  }
}
