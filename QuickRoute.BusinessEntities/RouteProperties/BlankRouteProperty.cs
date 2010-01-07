using System;
using System.Collections.Generic;
using System.Text;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class BlankRouteProperty : RouteProperty
  {
    public BlankRouteProperty() : base(null, null, null)
    {
    }

    protected override void Calculate()
    {
      value = 0;
    }

    public override string MaxWidthString
    {
      get { return ToString(); }
    }

    public override bool ContainsValue
    {
      get { return false; }
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      return "";
    }

    public override string ToString()
    {
      return ValueToString(Value);
    } 
  }
}
