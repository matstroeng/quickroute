using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuickRoute.BusinessEntities.ExternalLapDataSources;

namespace QuickRoute.UI.Forms
{
  public partial class AddLapsFromExternalDataSource : Form
  {
    private bool updatingUiNow;
    private IExternalLapDataSource dataSource;

    public AddLapsFromExternalDataSource(DateTime startDate, DateTime endDate, int selectedDataSourceIndex)
    {
      InitializeComponent();
      this.startDate.Value = startDate;
      this.endDate.Value = endDate;
      PopulateDataSources();
      if (dataSources.Items.Count > 0) dataSources.SelectedIndex = Math.Min(selectedDataSourceIndex, dataSources.Items.Count-1);
    }

    public int SelectedDataSourceIndex
    {
      get { return dataSources.SelectedIndex; }
    }

    private void PopulateDataSources()
    {
      var formattedDataSources = FormattedDataSource.ToFormattedDataSources(ExternalLapDataSourceFactory.GetExternalLapDataSources());
      dataSources.DataSource = formattedDataSources;
    }

    private void PopulateEvents()
    {
      Cursor = Cursors.WaitCursor;
      var formattedEvents = FormattedEvent.ToFormattedEvents(dataSource.GetEvents(startDate.Value, endDate.Value, null /* country */));
      Cursor = Cursors.Default;
      updatingUiNow = true;
      Array.Reverse(formattedEvents);
      events.DataSource = formattedEvents;
      if (events.Items.Count > 0) events.SelectedIndex = 0;
      PopulateCategories();
      PopulateRunners();
      updatingUiNow = false;
    }

    private void PopulateCategories()
    {
      Cursor = Cursors.WaitCursor;
      categories.DataSource = events.SelectedItem == null ? new Category[0] : dataSource.GetCategories(((FormattedEvent)events.SelectedItem).Event.DatabaseId);

      Cursor = Cursors.Default;
      categories.DisplayMember = "Name";
    }

    private void PopulateRunners()
    {
      Cursor = Cursors.WaitCursor;
      runners.DataSource = events.SelectedItem == null ? new FormattedRunner[0] : FormattedRunner.ToFormattedRunners(dataSource.GetRunnersAndSplits(((FormattedEvent)events.SelectedItem).Event.DatabaseId, categories.SelectedIndex));
      Cursor = Cursors.Default;
    }

    private class FormattedEvent
    {
      public Event Event { get; set; }

      public override string ToString()
      {
        return Event.StartDate.ToShortDateString() + " " + Event.Name;
      }

      public static FormattedEvent[] ToFormattedEvents(IEnumerable<Event> events)
      {
        List<FormattedEvent> formattedEvents = new List<FormattedEvent>();
        foreach (Event e in events)
        {
          formattedEvents.Add(new FormattedEvent { Event = e });
        }
        return formattedEvents.ToArray();
      }
    }

    private class FormattedDataSource
    {
      public string Name { get; set; }
      public IExternalLapDataSource DataSource { get; set; }

      public override string ToString()
      {
        return Name;
      }

      public static FormattedDataSource[] ToFormattedDataSources(IEnumerable<KeyValuePair<string, IExternalLapDataSource>> dataSources)
      {
        var formattedDataSources = new List<FormattedDataSource>();
        foreach (var d in dataSources)
        {
          formattedDataSources.Add(new FormattedDataSource { Name = d.Key, DataSource = d.Value });
        }
        return formattedDataSources.ToArray();
      }
    }


    private class FormattedRunner
    {
      public Runner Runner { get; set; }

      public override string ToString()
      {
        return Runner.Name + ", " + Runner.Club;
      }

      public static FormattedRunner[] ToFormattedRunners(IEnumerable<Runner> runners)
      {
        var formattedRunners = new List<FormattedRunner>();
        foreach (var e in runners)
        {
          formattedRunners.Add(new FormattedRunner { Runner = e });
        }
        return formattedRunners.ToArray();
      }

    }

    private void events_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (!updatingUiNow) PopulateCategories();
    }

    private void categories_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (!updatingUiNow) PopulateRunners();
    }

    public Runner Runner
    {
      get
      {
        return ((FormattedRunner)runners.SelectedItem).Runner;
      }
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

    private void search_Click(object sender, EventArgs e)
    {
      PopulateEvents();
    }

    private void dataSources_SelectedIndexChanged(object sender, EventArgs e)
    {
      dataSource = ((FormattedDataSource) dataSources.SelectedItem).DataSource;
      PopulateEvents();
    }
  }
}
