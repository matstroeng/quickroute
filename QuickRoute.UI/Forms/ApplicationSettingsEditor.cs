using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;

namespace QuickRoute.UI.Forms
{
  public partial class ApplicationSettingsEditor : Form
  {
    public ApplicationSettingsEditor()
    {
      InitializeComponent();
    }

    public ApplicationSettings ApplicationSettings
    {
      get { return (ApplicationSettings)propertyGrid.SelectedObject; }
      set { propertyGrid.SelectedObject = value; }
    }
  }
}
