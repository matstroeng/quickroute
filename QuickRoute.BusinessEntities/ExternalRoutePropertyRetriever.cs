using System;
using System.Collections.Generic;
using System.Text;

namespace QuickRoute.BusinessEntities
{
  public class ExternalRoutePropertyRetriever
  {
    public SessionSettings SessionSettings { get; private set; }

    public ExternalRoutePropertyRetriever(SessionSettings sessionSettings)
    {
      SessionSettings = sessionSettings;
    }

    public double RetrieveExternalProperty(string name)
    {
      switch(name.ToLower())
      {
        case "circletimeradius":
          return SessionSettings.CircleTimeRadius;
      }
      return 0;
    }
  }
}
