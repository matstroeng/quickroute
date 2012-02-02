using System.Collections.Generic;
using QuickRoute.BusinessEntities.ExternalLapDataSources.CzechSplits;
using QuickRoute.BusinessEntities.ExternalLapDataSources.WinSplits;

namespace QuickRoute.BusinessEntities.ExternalLapDataSources
{
  public static class ExternalLapDataSourceFactory
  {
    public static IEnumerable<KeyValuePair<string, IExternalLapDataSource>> GetExternalLapDataSources()
    {
      return new[]
               {
                 new KeyValuePair<string, IExternalLapDataSource>("WinSplits", new WinSplitsLapDataSource()), 
                 new KeyValuePair<string, IExternalLapDataSource>("CzechSplits", new CzechSplitsLapDataSource()) 
               };
    }
  }
}
