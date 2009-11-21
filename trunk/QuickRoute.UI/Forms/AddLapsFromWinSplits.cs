using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuickRoute.UI.WinSplitsWebServices;

namespace QuickRoute.UI.Forms
{
  public partial class AddLapsFromWinSplits : Form
  {
    private bool updatingUiNow;

    public AddLapsFromWinSplits()
    {
      InitializeComponent();
    }

    private void AddLapsFromWinSplits_Load(object sender, EventArgs e)
    {
      startDate.Value = DateTime.Now.AddDays(-7).Date;
      endDate.Value = DateTime.Now.Date;

      PopulateEvents();
    }

    private void PopulateCategories()
    {
      WebServices s = new WebServices();
      Cursor = Cursors.WaitCursor;
      categories.DataSource = s.GetCategories(((FormattedEvent)winSplitsEvents.SelectedItem).Event.DatabaseId);
      Cursor = Cursors.Default;
      categories.DisplayMember = "Name";
    }

    private void PopulateRunners()
    {
      WebServices s = new WebServices();
      Cursor = Cursors.WaitCursor;
      runners.DataSource = FormattedRunner.ToFormattedRunners(s.GetRunnersAndSplits(((FormattedEvent)winSplitsEvents.SelectedItem).Event.DatabaseId, (short)categories.SelectedIndex));
      Cursor = Cursors.Default;
    }

    private void PopulateEvents()
    {
      WebServices s = new WebServices();
      Cursor = Cursors.WaitCursor;
      FormattedEvent[] events = FormattedEvent.ToFormattedEvents(s.GetEvents(startDate.Value, endDate.Value));
      Cursor = Cursors.Default;
      updatingUiNow = true;
      Array.Reverse(events);
      winSplitsEvents.DataSource = events;
      if(winSplitsEvents.Items.Count > 0) winSplitsEvents.SelectedIndex = 0;
      PopulateCategories();
      PopulateRunners();
      updatingUiNow = false;
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

    private class FormattedRunner
    {
      public Runner Runner { get; set; }

      public override string ToString()
      {
        return Runner.Name + ", " + Runner.Club;
      }

      public static FormattedRunner[] ToFormattedRunners(IEnumerable<Runner> Runners)
      {
        List<FormattedRunner> formattedRunners = new List<FormattedRunner>();
        foreach (Runner e in Runners)
        {
          formattedRunners.Add(new FormattedRunner { Runner = e });
        }
        return formattedRunners.ToArray();
      }

    }


    private void winSplitsEvents_SelectedIndexChanged(object sender, EventArgs e)
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
  }
}
