namespace QuickRoute.UI.Forms
{
  partial class AddLapsFromWinSplits
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddLapsFromWinSplits));
      this.winSplitsEvents = new System.Windows.Forms.ComboBox();
      this.categories = new System.Windows.Forms.ComboBox();
      this.runners = new System.Windows.Forms.ComboBox();
      this.eventLabel = new System.Windows.Forms.Label();
      this.classLabel = new System.Windows.Forms.Label();
      this.runnerLabel = new System.Windows.Forms.Label();
      this.ok = new System.Windows.Forms.Button();
      this.cancel = new System.Windows.Forms.Button();
      this.startDate = new System.Windows.Forms.DateTimePicker();
      this.endDate = new System.Windows.Forms.DateTimePicker();
      this.startDateLabel = new System.Windows.Forms.Label();
      this.endDateLabel = new System.Windows.Forms.Label();
      this.uiInformation = new System.Windows.Forms.Label();
      this.search = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // winSplitsEvents
      // 
      this.winSplitsEvents.AccessibleDescription = null;
      this.winSplitsEvents.AccessibleName = null;
      resources.ApplyResources(this.winSplitsEvents, "winSplitsEvents");
      this.winSplitsEvents.BackgroundImage = null;
      this.winSplitsEvents.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.winSplitsEvents.Font = null;
      this.winSplitsEvents.FormattingEnabled = true;
      this.winSplitsEvents.Name = "winSplitsEvents";
      this.winSplitsEvents.SelectedIndexChanged += new System.EventHandler(this.winSplitsEvents_SelectedIndexChanged);
      // 
      // categories
      // 
      this.categories.AccessibleDescription = null;
      this.categories.AccessibleName = null;
      resources.ApplyResources(this.categories, "categories");
      this.categories.BackgroundImage = null;
      this.categories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.categories.Font = null;
      this.categories.FormattingEnabled = true;
      this.categories.Name = "categories";
      this.categories.SelectedIndexChanged += new System.EventHandler(this.categories_SelectedIndexChanged);
      // 
      // runners
      // 
      this.runners.AccessibleDescription = null;
      this.runners.AccessibleName = null;
      resources.ApplyResources(this.runners, "runners");
      this.runners.BackgroundImage = null;
      this.runners.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.runners.Font = null;
      this.runners.FormattingEnabled = true;
      this.runners.Name = "runners";
      // 
      // eventLabel
      // 
      this.eventLabel.AccessibleDescription = null;
      this.eventLabel.AccessibleName = null;
      resources.ApplyResources(this.eventLabel, "eventLabel");
      this.eventLabel.Font = null;
      this.eventLabel.Name = "eventLabel";
      // 
      // classLabel
      // 
      this.classLabel.AccessibleDescription = null;
      this.classLabel.AccessibleName = null;
      resources.ApplyResources(this.classLabel, "classLabel");
      this.classLabel.Font = null;
      this.classLabel.Name = "classLabel";
      // 
      // runnerLabel
      // 
      this.runnerLabel.AccessibleDescription = null;
      this.runnerLabel.AccessibleName = null;
      resources.ApplyResources(this.runnerLabel, "runnerLabel");
      this.runnerLabel.Font = null;
      this.runnerLabel.Name = "runnerLabel";
      // 
      // ok
      // 
      this.ok.AccessibleDescription = null;
      this.ok.AccessibleName = null;
      resources.ApplyResources(this.ok, "ok");
      this.ok.BackgroundImage = null;
      this.ok.Font = null;
      this.ok.Name = "ok";
      this.ok.UseVisualStyleBackColor = true;
      this.ok.Click += new System.EventHandler(this.ok_Click);
      // 
      // cancel
      // 
      this.cancel.AccessibleDescription = null;
      this.cancel.AccessibleName = null;
      resources.ApplyResources(this.cancel, "cancel");
      this.cancel.BackgroundImage = null;
      this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancel.Font = null;
      this.cancel.Name = "cancel";
      this.cancel.UseVisualStyleBackColor = true;
      this.cancel.Click += new System.EventHandler(this.cancel_Click);
      // 
      // startDate
      // 
      this.startDate.AccessibleDescription = null;
      this.startDate.AccessibleName = null;
      resources.ApplyResources(this.startDate, "startDate");
      this.startDate.BackgroundImage = null;
      this.startDate.CalendarFont = null;
      this.startDate.CustomFormat = null;
      this.startDate.Font = null;
      this.startDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
      this.startDate.Name = "startDate";
      // 
      // endDate
      // 
      this.endDate.AccessibleDescription = null;
      this.endDate.AccessibleName = null;
      resources.ApplyResources(this.endDate, "endDate");
      this.endDate.BackgroundImage = null;
      this.endDate.CalendarFont = null;
      this.endDate.CustomFormat = null;
      this.endDate.Font = null;
      this.endDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
      this.endDate.Name = "endDate";
      // 
      // startDateLabel
      // 
      this.startDateLabel.AccessibleDescription = null;
      this.startDateLabel.AccessibleName = null;
      resources.ApplyResources(this.startDateLabel, "startDateLabel");
      this.startDateLabel.Font = null;
      this.startDateLabel.Name = "startDateLabel";
      // 
      // endDateLabel
      // 
      this.endDateLabel.AccessibleDescription = null;
      this.endDateLabel.AccessibleName = null;
      resources.ApplyResources(this.endDateLabel, "endDateLabel");
      this.endDateLabel.Font = null;
      this.endDateLabel.Name = "endDateLabel";
      // 
      // uiInformation
      // 
      this.uiInformation.AccessibleDescription = null;
      this.uiInformation.AccessibleName = null;
      resources.ApplyResources(this.uiInformation, "uiInformation");
      this.uiInformation.BackColor = System.Drawing.SystemColors.Info;
      this.uiInformation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.uiInformation.Font = null;
      this.uiInformation.Name = "uiInformation";
      // 
      // search
      // 
      this.search.AccessibleDescription = null;
      this.search.AccessibleName = null;
      resources.ApplyResources(this.search, "search");
      this.search.BackgroundImage = null;
      this.search.Font = null;
      this.search.Name = "search";
      this.search.UseVisualStyleBackColor = true;
      this.search.Click += new System.EventHandler(this.search_Click);
      // 
      // AddLapsFromWinSplits
      // 
      this.AcceptButton = this.ok;
      this.AccessibleDescription = null;
      this.AccessibleName = null;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = null;
      this.CancelButton = this.cancel;
      this.Controls.Add(this.search);
      this.Controls.Add(this.uiInformation);
      this.Controls.Add(this.endDateLabel);
      this.Controls.Add(this.startDateLabel);
      this.Controls.Add(this.endDate);
      this.Controls.Add(this.startDate);
      this.Controls.Add(this.cancel);
      this.Controls.Add(this.ok);
      this.Controls.Add(this.runnerLabel);
      this.Controls.Add(this.classLabel);
      this.Controls.Add(this.eventLabel);
      this.Controls.Add(this.runners);
      this.Controls.Add(this.categories);
      this.Controls.Add(this.winSplitsEvents);
      this.Font = null;
      this.Icon = null;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AddLapsFromWinSplits";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Load += new System.EventHandler(this.AddLapsFromWinSplits_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ComboBox winSplitsEvents;
    private System.Windows.Forms.ComboBox categories;
    private System.Windows.Forms.ComboBox runners;
    private System.Windows.Forms.Label eventLabel;
    private System.Windows.Forms.Label classLabel;
    private System.Windows.Forms.Label runnerLabel;
    private System.Windows.Forms.Button ok;
    private System.Windows.Forms.Button cancel;
    private System.Windows.Forms.DateTimePicker startDate;
    private System.Windows.Forms.DateTimePicker endDate;
    private System.Windows.Forms.Label startDateLabel;
    private System.Windows.Forms.Label endDateLabel;
    private System.Windows.Forms.Label uiInformation;
    private System.Windows.Forms.Button search;
  }
}