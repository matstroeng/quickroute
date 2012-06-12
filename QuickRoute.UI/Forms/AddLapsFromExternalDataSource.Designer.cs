namespace QuickRoute.UI.Forms
{
  partial class AddLapsFromExternalDataSource
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddLapsFromExternalDataSource));
      this.events = new System.Windows.Forms.ComboBox();
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
      this.dataSourceLabel = new System.Windows.Forms.Label();
      this.dataSources = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // events
      // 
      resources.ApplyResources(this.events, "events");
      this.events.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.events.FormattingEnabled = true;
      this.events.Name = "events";
      this.events.SelectedIndexChanged += new System.EventHandler(this.events_SelectedIndexChanged);
      // 
      // categories
      // 
      resources.ApplyResources(this.categories, "categories");
      this.categories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.categories.FormattingEnabled = true;
      this.categories.Name = "categories";
      this.categories.SelectedIndexChanged += new System.EventHandler(this.categories_SelectedIndexChanged);
      // 
      // runners
      // 
      resources.ApplyResources(this.runners, "runners");
      this.runners.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.runners.FormattingEnabled = true;
      this.runners.Name = "runners";
      // 
      // eventLabel
      // 
      resources.ApplyResources(this.eventLabel, "eventLabel");
      this.eventLabel.Name = "eventLabel";
      // 
      // classLabel
      // 
      resources.ApplyResources(this.classLabel, "classLabel");
      this.classLabel.Name = "classLabel";
      // 
      // runnerLabel
      // 
      resources.ApplyResources(this.runnerLabel, "runnerLabel");
      this.runnerLabel.Name = "runnerLabel";
      // 
      // ok
      // 
      resources.ApplyResources(this.ok, "ok");
      this.ok.Name = "ok";
      this.ok.UseVisualStyleBackColor = true;
      this.ok.Click += new System.EventHandler(this.ok_Click);
      // 
      // cancel
      // 
      resources.ApplyResources(this.cancel, "cancel");
      this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancel.Name = "cancel";
      this.cancel.UseVisualStyleBackColor = true;
      this.cancel.Click += new System.EventHandler(this.cancel_Click);
      // 
      // startDate
      // 
      resources.ApplyResources(this.startDate, "startDate");
      this.startDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
      this.startDate.Name = "startDate";
      // 
      // endDate
      // 
      resources.ApplyResources(this.endDate, "endDate");
      this.endDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
      this.endDate.Name = "endDate";
      // 
      // startDateLabel
      // 
      resources.ApplyResources(this.startDateLabel, "startDateLabel");
      this.startDateLabel.Name = "startDateLabel";
      // 
      // endDateLabel
      // 
      resources.ApplyResources(this.endDateLabel, "endDateLabel");
      this.endDateLabel.Name = "endDateLabel";
      // 
      // uiInformation
      // 
      resources.ApplyResources(this.uiInformation, "uiInformation");
      this.uiInformation.BackColor = System.Drawing.SystemColors.Info;
      this.uiInformation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.uiInformation.Name = "uiInformation";
      // 
      // search
      // 
      resources.ApplyResources(this.search, "search");
      this.search.Name = "search";
      this.search.UseVisualStyleBackColor = true;
      this.search.Click += new System.EventHandler(this.search_Click);
      // 
      // dataSourceLabel
      // 
      resources.ApplyResources(this.dataSourceLabel, "dataSourceLabel");
      this.dataSourceLabel.Name = "dataSourceLabel";
      // 
      // dataSources
      // 
      resources.ApplyResources(this.dataSources, "dataSources");
      this.dataSources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.dataSources.FormattingEnabled = true;
      this.dataSources.Name = "dataSources";
      this.dataSources.SelectedIndexChanged += new System.EventHandler(this.dataSources_SelectedIndexChanged);
      // 
      // AddLapsFromExternalDataSource
      // 
      this.AcceptButton = this.ok;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancel;
      this.Controls.Add(this.dataSourceLabel);
      this.Controls.Add(this.dataSources);
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
      this.Controls.Add(this.events);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AddLapsFromExternalDataSource";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ComboBox events;
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
    private System.Windows.Forms.Label dataSourceLabel;
    private System.Windows.Forms.ComboBox dataSources;
  }
}