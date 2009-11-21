using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using QuickRoute.BusinessEntities;
using QuickRoute.BusinessEntities.Importers.GPX.GPX11;

namespace QuickRoute.BusinessEntities.Importers.QuickRoute
{
  public class QuickRouteImporter : IRouteFileImporter
  {
    private string fileName;
    private ImportResult importResult;

    public ImportResult ImportResult
    {
      get { return importResult; }
      set { importResult = value; }
    }

    public string FileName
    {
      get { return fileName; }
      set { fileName = value; }
    }

    public DialogResult ShowPreImportDialogs()
    {
      return DialogResult.OK;
    }

    public void Import()
    {
      ImportResult = new ImportResult();
      var document = Document.Open(FileName);
      if (document != null && document.Sessions.Count > 0)
      {
        ImportResult.Succeeded = true;
        ImportResult.Route = document.Sessions[0].Route;
        ImportResult.Laps = document.Sessions[0].Laps;
      }
      else
      {
        ImportResult.Succeeded = false;
      }
    }


    #region IRouteImporter Members

    public event EventHandler<EventArgs> BeginWork;

    public event EventHandler<EventArgs> EndWork;

    public event EventHandler<WorkProgressEventArgs> WorkProgress;

    #endregion
  }

}
