using System;
using System.Collections.Generic;
using System.Text;
using QuickRoute.BusinessEntities;

namespace QuickRoute.BusinessEntities.Importers
{
  public class ImportResult
  {
    public bool Succeeded { get; set; }
    public Route Route { get; set; }
    public LapCollection Laps { get; set; }
    public string ErrorMessage { get; set; }
    public ImportError Error { get; set; }
    public Exception Exception { get; set; }
  }
}
