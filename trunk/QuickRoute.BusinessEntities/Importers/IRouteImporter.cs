using System;
using System.Windows.Forms;
using QuickRoute.BusinessEntities.Importers;

namespace QuickRoute.BusinessEntities.Importers
{
  public interface IRouteImporter
  {
    ImportResult ImportResult { get; set; }
    DialogResult ShowPreImportDialogs();
    void Import();
    event EventHandler<EventArgs> BeginWork;
    event EventHandler<EventArgs> EndWork;
    event EventHandler<WorkProgressEventArgs> WorkProgress;
  }
}