using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace QuickRoute.BusinessEntities.Importers
{
  public partial class SessionSelector : Form
  {
    private List<object> sessions; 

    public SessionSelector()
    {
      InitializeComponent();
    }

    public List<object> Sessions
    {
      get { return sessions; }
      set 
      { 
        sessions = value;
        sessionsComboBox.Items.Clear();
        sessionsComboBox.DataSource = sessions;
        int maxWidth = 0;
        foreach(object s in sessions)
        {
          maxWidth = Math.Max(maxWidth, TextRenderer.MeasureText(s.ToString(), sessionsComboBox.Font).Width);
        }
        maxWidth += 32;
        if (maxWidth > sessionsComboBox.Width) Width += maxWidth - sessionsComboBox.Width;
      }
    }

    public object SelectedSession
    {
      get { return sessionsComboBox.SelectedValue; }
      set { sessionsComboBox.SelectedValue = value; }
    }

    private void ok_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
      Close();
    }

    private void cancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

  }
}