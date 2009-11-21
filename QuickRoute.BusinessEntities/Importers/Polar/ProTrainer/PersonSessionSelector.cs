using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuickRoute.BusinessEntities.Importers.Polar.ProTrainer;

namespace QuickRoute.BusinessEntities.Importers.Polar.ProTrainer
{
  public partial class PersonSessionSelector : Form
  {
    public List<PolarPerson> Persons { get; set; }

    public PersonSessionSelector(List<PolarPerson> persons)
    {
      InitializeComponent();
      Persons = persons;
      personsComboBox.DataSource = Persons;
    }

    public PolarSession SelectedSession
    {
      get { return sessionsComboBox.SelectedValue as PolarSession; }
      set { personsComboBox.SelectedValue = value; }
    }

    private void ok_Click(object sender, EventArgs e)
    {
      DialogResult = (SelectedSession != null ? DialogResult.OK : DialogResult.Cancel);
      Close();
    }

    private void cancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

    private void personsComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      PolarPerson p = personsComboBox.SelectedItem as PolarPerson;
      if (p!= null)  sessionsComboBox.DataSource = p.Sessions;
    }

  }
}