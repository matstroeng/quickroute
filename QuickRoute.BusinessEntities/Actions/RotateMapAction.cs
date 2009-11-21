using System;
using System.Collections.Generic;
using System.Text;

namespace QuickRoute.BusinessEntities.Actions
{
  public class RotateMapAction : IAction 
  {
    /// <summary>
    /// Angle in radians, counterclockwise direction
    /// </summary>
    public double Angle { get; set; }

    /// <summary>
    /// The document to rotate
    /// </summary>
    public Document Document { get; set; }

    public RotateMapAction(double angle, Document document)
    {
      Angle = angle;
      Document = document;
    }

    public void Execute()
    {

    }

    public void Undo()
    {
      
    }
  }
}
