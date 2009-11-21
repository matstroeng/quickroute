using QuickRoute.Controls;

namespace QuickRoute.UI.Forms
{
  partial class OpenMultipleFilesInGoogleEarthDialog
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenMultipleFilesInGoogleEarthDialog));
      this.ok = new System.Windows.Forms.Button();
      this.cancel = new System.Windows.Forms.Button();
      this.filesLabel = new System.Windows.Forms.Label();
      this.replayTailsVisible = new System.Windows.Forms.CheckBox();
      this.replayTailDuration = new System.Windows.Forms.ComboBox();
      this.massStart = new System.Windows.Forms.CheckBox();
      this.replayTimeIntervalLabel = new System.Windows.Forms.Label();
      this.replayTimeInterval = new System.Windows.Forms.ComboBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.replayRestartAfterEachLap = new System.Windows.Forms.CheckBox();
      this.replayTailDurationLabel = new System.Windows.Forms.Label();
      this.replayTailsVisibleLabel = new System.Windows.Forms.Label();
      this.replayRestartAfterEachLapLabel = new System.Windows.Forms.Label();
      this.massStartLabel = new System.Windows.Forms.Label();
      this.uiInformation = new System.Windows.Forms.Label();
      this.includeRoutes = new System.Windows.Forms.CheckBox();
      this.includeReplay = new System.Windows.Forms.CheckBox();
      this.replayGroupBox = new System.Windows.Forms.GroupBox();
      this.routesGroupBox = new System.Windows.Forms.GroupBox();
      this.fileSelector = new QuickRoute.Controls.FileSelectorControl();
      this.tableLayoutPanel1.SuspendLayout();
      this.replayGroupBox.SuspendLayout();
      this.routesGroupBox.SuspendLayout();
      this.SuspendLayout();
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
      // filesLabel
      // 
      this.filesLabel.AccessibleDescription = null;
      this.filesLabel.AccessibleName = null;
      resources.ApplyResources(this.filesLabel, "filesLabel");
      this.filesLabel.Font = null;
      this.filesLabel.Name = "filesLabel";
      // 
      // replayTailsVisible
      // 
      this.replayTailsVisible.AccessibleDescription = null;
      this.replayTailsVisible.AccessibleName = null;
      resources.ApplyResources(this.replayTailsVisible, "replayTailsVisible");
      this.replayTailsVisible.BackgroundImage = null;
      this.replayTailsVisible.Font = null;
      this.replayTailsVisible.Name = "replayTailsVisible";
      this.replayTailsVisible.UseVisualStyleBackColor = true;
      this.replayTailsVisible.CheckedChanged += new System.EventHandler(this.replayTailsVisible_CheckedChanged);
      // 
      // replayTailDuration
      // 
      this.replayTailDuration.AccessibleDescription = null;
      this.replayTailDuration.AccessibleName = null;
      resources.ApplyResources(this.replayTailDuration, "replayTailDuration");
      this.replayTailDuration.BackgroundImage = null;
      this.replayTailDuration.Font = null;
      this.replayTailDuration.FormattingEnabled = true;
      this.replayTailDuration.Items.AddRange(new object[] {
            resources.GetString("replayTailDuration.Items"),
            resources.GetString("replayTailDuration.Items1"),
            resources.GetString("replayTailDuration.Items2"),
            resources.GetString("replayTailDuration.Items3"),
            resources.GetString("replayTailDuration.Items4"),
            resources.GetString("replayTailDuration.Items5"),
            resources.GetString("replayTailDuration.Items6"),
            resources.GetString("replayTailDuration.Items7")});
      this.replayTailDuration.Name = "replayTailDuration";
      this.replayTailDuration.Leave += new System.EventHandler(this.replayTailDuration_Leave);
      // 
      // massStart
      // 
      this.massStart.AccessibleDescription = null;
      this.massStart.AccessibleName = null;
      resources.ApplyResources(this.massStart, "massStart");
      this.massStart.BackgroundImage = null;
      this.massStart.Font = null;
      this.massStart.Name = "massStart";
      this.massStart.UseVisualStyleBackColor = true;
      this.massStart.CheckedChanged += new System.EventHandler(this.massStart_CheckedChanged);
      // 
      // replayTimeIntervalLabel
      // 
      this.replayTimeIntervalLabel.AccessibleDescription = null;
      this.replayTimeIntervalLabel.AccessibleName = null;
      resources.ApplyResources(this.replayTimeIntervalLabel, "replayTimeIntervalLabel");
      this.replayTimeIntervalLabel.Font = null;
      this.replayTimeIntervalLabel.Name = "replayTimeIntervalLabel";
      // 
      // replayTimeInterval
      // 
      this.replayTimeInterval.AccessibleDescription = null;
      this.replayTimeInterval.AccessibleName = null;
      resources.ApplyResources(this.replayTimeInterval, "replayTimeInterval");
      this.replayTimeInterval.BackgroundImage = null;
      this.replayTimeInterval.DropDownWidth = 99;
      this.replayTimeInterval.Font = null;
      this.replayTimeInterval.FormattingEnabled = true;
      this.replayTimeInterval.Items.AddRange(new object[] {
            resources.GetString("replayTimeInterval.Items"),
            resources.GetString("replayTimeInterval.Items1"),
            resources.GetString("replayTimeInterval.Items2"),
            resources.GetString("replayTimeInterval.Items3"),
            resources.GetString("replayTimeInterval.Items4"),
            resources.GetString("replayTimeInterval.Items5"),
            resources.GetString("replayTimeInterval.Items6")});
      this.replayTimeInterval.Name = "replayTimeInterval";
      this.replayTimeInterval.Leave += new System.EventHandler(this.replayTimeInterval_Leave);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.AccessibleDescription = null;
      this.tableLayoutPanel1.AccessibleName = null;
      resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
      this.tableLayoutPanel1.BackgroundImage = null;
      this.tableLayoutPanel1.Controls.Add(this.replayRestartAfterEachLap, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.replayTailsVisible, 1, 3);
      this.tableLayoutPanel1.Controls.Add(this.replayTailDurationLabel, 0, 4);
      this.tableLayoutPanel1.Controls.Add(this.replayTailDuration, 1, 4);
      this.tableLayoutPanel1.Controls.Add(this.replayTailsVisibleLabel, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.replayRestartAfterEachLapLabel, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.massStartLabel, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.replayTimeIntervalLabel, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.replayTimeInterval, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.massStart, 1, 1);
      this.tableLayoutPanel1.Font = null;
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      // 
      // replayRestartAfterEachLap
      // 
      this.replayRestartAfterEachLap.AccessibleDescription = null;
      this.replayRestartAfterEachLap.AccessibleName = null;
      resources.ApplyResources(this.replayRestartAfterEachLap, "replayRestartAfterEachLap");
      this.replayRestartAfterEachLap.BackgroundImage = null;
      this.replayRestartAfterEachLap.Font = null;
      this.replayRestartAfterEachLap.Name = "replayRestartAfterEachLap";
      this.replayRestartAfterEachLap.UseVisualStyleBackColor = true;
      // 
      // replayTailDurationLabel
      // 
      this.replayTailDurationLabel.AccessibleDescription = null;
      this.replayTailDurationLabel.AccessibleName = null;
      resources.ApplyResources(this.replayTailDurationLabel, "replayTailDurationLabel");
      this.replayTailDurationLabel.Font = null;
      this.replayTailDurationLabel.Name = "replayTailDurationLabel";
      // 
      // replayTailsVisibleLabel
      // 
      this.replayTailsVisibleLabel.AccessibleDescription = null;
      this.replayTailsVisibleLabel.AccessibleName = null;
      resources.ApplyResources(this.replayTailsVisibleLabel, "replayTailsVisibleLabel");
      this.replayTailsVisibleLabel.Font = null;
      this.replayTailsVisibleLabel.Name = "replayTailsVisibleLabel";
      this.replayTailsVisibleLabel.Click += new System.EventHandler(this.replayTailsVisibleLabel_Click);
      // 
      // replayRestartAfterEachLapLabel
      // 
      this.replayRestartAfterEachLapLabel.AccessibleDescription = null;
      this.replayRestartAfterEachLapLabel.AccessibleName = null;
      resources.ApplyResources(this.replayRestartAfterEachLapLabel, "replayRestartAfterEachLapLabel");
      this.replayRestartAfterEachLapLabel.Font = null;
      this.replayRestartAfterEachLapLabel.Name = "replayRestartAfterEachLapLabel";
      this.replayRestartAfterEachLapLabel.Click += new System.EventHandler(this.replayRestartAfterEachLapLabel_Click);
      // 
      // massStartLabel
      // 
      this.massStartLabel.AccessibleDescription = null;
      this.massStartLabel.AccessibleName = null;
      resources.ApplyResources(this.massStartLabel, "massStartLabel");
      this.massStartLabel.Font = null;
      this.massStartLabel.Name = "massStartLabel";
      this.massStartLabel.Click += new System.EventHandler(this.massStartLabel_Click);
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
      // includeRoutes
      // 
      this.includeRoutes.AccessibleDescription = null;
      this.includeRoutes.AccessibleName = null;
      resources.ApplyResources(this.includeRoutes, "includeRoutes");
      this.includeRoutes.BackgroundImage = null;
      this.includeRoutes.Font = null;
      this.includeRoutes.Name = "includeRoutes";
      this.includeRoutes.UseVisualStyleBackColor = true;
      this.includeRoutes.CheckedChanged += new System.EventHandler(this.includeRoutes_CheckedChanged);
      // 
      // includeReplay
      // 
      this.includeReplay.AccessibleDescription = null;
      this.includeReplay.AccessibleName = null;
      resources.ApplyResources(this.includeReplay, "includeReplay");
      this.includeReplay.BackgroundImage = null;
      this.includeReplay.Font = null;
      this.includeReplay.Name = "includeReplay";
      this.includeReplay.UseVisualStyleBackColor = true;
      this.includeReplay.CheckedChanged += new System.EventHandler(this.includeReplay_CheckedChanged);
      // 
      // replayGroupBox
      // 
      this.replayGroupBox.AccessibleDescription = null;
      this.replayGroupBox.AccessibleName = null;
      resources.ApplyResources(this.replayGroupBox, "replayGroupBox");
      this.replayGroupBox.BackgroundImage = null;
      this.replayGroupBox.Controls.Add(this.tableLayoutPanel1);
      this.replayGroupBox.Controls.Add(this.includeReplay);
      this.replayGroupBox.Font = null;
      this.replayGroupBox.Name = "replayGroupBox";
      this.replayGroupBox.TabStop = false;
      // 
      // routesGroupBox
      // 
      this.routesGroupBox.AccessibleDescription = null;
      this.routesGroupBox.AccessibleName = null;
      resources.ApplyResources(this.routesGroupBox, "routesGroupBox");
      this.routesGroupBox.BackgroundImage = null;
      this.routesGroupBox.Controls.Add(this.includeRoutes);
      this.routesGroupBox.Font = null;
      this.routesGroupBox.Name = "routesGroupBox";
      this.routesGroupBox.TabStop = false;
      // 
      // fileSelector
      // 
      this.fileSelector.AccessibleDescription = null;
      this.fileSelector.AccessibleName = null;
      this.fileSelector.AllowUrlAdding = true;
      resources.ApplyResources(this.fileSelector, "fileSelector");
      this.fileSelector.BackgroundImage = null;
      this.fileSelector.FileDialogFilter = null;
      this.fileSelector.FileDialogFilterIndex = 0;
      this.fileSelector.FileDialogTitle = null;
      this.fileSelector.Font = null;
      this.fileSelector.Name = "fileSelector";
      this.fileSelector.FilesChanged += new System.EventHandler<System.EventArgs>(this.fileSelector_FilesChanged);
      // 
      // OpenMultipleFilesInGoogleEarthDialog
      // 
      this.AcceptButton = this.ok;
      this.AccessibleDescription = null;
      this.AccessibleName = null;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = null;
      this.CancelButton = this.cancel;
      this.Controls.Add(this.routesGroupBox);
      this.Controls.Add(this.replayGroupBox);
      this.Controls.Add(this.fileSelector);
      this.Controls.Add(this.uiInformation);
      this.Controls.Add(this.filesLabel);
      this.Controls.Add(this.cancel);
      this.Controls.Add(this.ok);
      this.Font = null;
      this.Icon = null;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "OpenMultipleFilesInGoogleEarthDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.replayGroupBox.ResumeLayout(false);
      this.replayGroupBox.PerformLayout();
      this.routesGroupBox.ResumeLayout(false);
      this.routesGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button ok;
    private System.Windows.Forms.Button cancel;
    private System.Windows.Forms.Label filesLabel;
    private System.Windows.Forms.CheckBox replayTailsVisible;
    private System.Windows.Forms.ComboBox replayTailDuration;
    private System.Windows.Forms.CheckBox massStart;
    private System.Windows.Forms.Label replayTimeIntervalLabel;
    private System.Windows.Forms.ComboBox replayTimeInterval;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label massStartLabel;
    private System.Windows.Forms.Label replayTailsVisibleLabel;
    private System.Windows.Forms.Label replayTailDurationLabel;
    private System.Windows.Forms.Label uiInformation;
    private System.Windows.Forms.CheckBox includeRoutes;
    private System.Windows.Forms.CheckBox includeReplay;
    private System.Windows.Forms.GroupBox replayGroupBox;
    private FileSelectorControl fileSelector;
    private System.Windows.Forms.GroupBox routesGroupBox;
    private System.Windows.Forms.CheckBox replayRestartAfterEachLap;
    private System.Windows.Forms.Label replayRestartAfterEachLapLabel;
  }
}