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
      // filesLabel
      // 
      resources.ApplyResources(this.filesLabel, "filesLabel");
      this.filesLabel.Name = "filesLabel";
      // 
      // replayTailsVisible
      // 
      resources.ApplyResources(this.replayTailsVisible, "replayTailsVisible");
      this.replayTailsVisible.Name = "replayTailsVisible";
      this.replayTailsVisible.UseVisualStyleBackColor = true;
      this.replayTailsVisible.CheckedChanged += new System.EventHandler(this.replayTailsVisible_CheckedChanged);
      // 
      // replayTailDuration
      // 
      resources.ApplyResources(this.replayTailDuration, "replayTailDuration");
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
      resources.ApplyResources(this.massStart, "massStart");
      this.massStart.Name = "massStart";
      this.massStart.UseVisualStyleBackColor = true;
      this.massStart.CheckedChanged += new System.EventHandler(this.massStart_CheckedChanged);
      // 
      // replayTimeIntervalLabel
      // 
      resources.ApplyResources(this.replayTimeIntervalLabel, "replayTimeIntervalLabel");
      this.replayTimeIntervalLabel.Name = "replayTimeIntervalLabel";
      // 
      // replayTimeInterval
      // 
      resources.ApplyResources(this.replayTimeInterval, "replayTimeInterval");
      this.replayTimeInterval.DropDownWidth = 99;
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
      resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
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
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      // 
      // replayRestartAfterEachLap
      // 
      resources.ApplyResources(this.replayRestartAfterEachLap, "replayRestartAfterEachLap");
      this.replayRestartAfterEachLap.Name = "replayRestartAfterEachLap";
      this.replayRestartAfterEachLap.UseVisualStyleBackColor = true;
      // 
      // replayTailDurationLabel
      // 
      resources.ApplyResources(this.replayTailDurationLabel, "replayTailDurationLabel");
      this.replayTailDurationLabel.Name = "replayTailDurationLabel";
      // 
      // replayTailsVisibleLabel
      // 
      resources.ApplyResources(this.replayTailsVisibleLabel, "replayTailsVisibleLabel");
      this.replayTailsVisibleLabel.Name = "replayTailsVisibleLabel";
      this.replayTailsVisibleLabel.Click += new System.EventHandler(this.replayTailsVisibleLabel_Click);
      // 
      // replayRestartAfterEachLapLabel
      // 
      resources.ApplyResources(this.replayRestartAfterEachLapLabel, "replayRestartAfterEachLapLabel");
      this.replayRestartAfterEachLapLabel.Name = "replayRestartAfterEachLapLabel";
      this.replayRestartAfterEachLapLabel.Click += new System.EventHandler(this.replayRestartAfterEachLapLabel_Click);
      // 
      // massStartLabel
      // 
      resources.ApplyResources(this.massStartLabel, "massStartLabel");
      this.massStartLabel.Name = "massStartLabel";
      this.massStartLabel.Click += new System.EventHandler(this.massStartLabel_Click);
      // 
      // uiInformation
      // 
      resources.ApplyResources(this.uiInformation, "uiInformation");
      this.uiInformation.BackColor = System.Drawing.SystemColors.Info;
      this.uiInformation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.uiInformation.Name = "uiInformation";
      // 
      // includeRoutes
      // 
      resources.ApplyResources(this.includeRoutes, "includeRoutes");
      this.includeRoutes.Name = "includeRoutes";
      this.includeRoutes.UseVisualStyleBackColor = true;
      this.includeRoutes.CheckedChanged += new System.EventHandler(this.includeRoutes_CheckedChanged);
      // 
      // includeReplay
      // 
      resources.ApplyResources(this.includeReplay, "includeReplay");
      this.includeReplay.Name = "includeReplay";
      this.includeReplay.UseVisualStyleBackColor = true;
      this.includeReplay.CheckedChanged += new System.EventHandler(this.includeReplay_CheckedChanged);
      // 
      // replayGroupBox
      // 
      resources.ApplyResources(this.replayGroupBox, "replayGroupBox");
      this.replayGroupBox.Controls.Add(this.tableLayoutPanel1);
      this.replayGroupBox.Controls.Add(this.includeReplay);
      this.replayGroupBox.Name = "replayGroupBox";
      this.replayGroupBox.TabStop = false;
      // 
      // routesGroupBox
      // 
      resources.ApplyResources(this.routesGroupBox, "routesGroupBox");
      this.routesGroupBox.Controls.Add(this.includeRoutes);
      this.routesGroupBox.Name = "routesGroupBox";
      this.routesGroupBox.TabStop = false;
      // 
      // fileSelector
      // 
      resources.ApplyResources(this.fileSelector, "fileSelector");
      this.fileSelector.AllowUrlAdding = true;
      this.fileSelector.FileDialogFilter = null;
      this.fileSelector.FileDialogFilterIndex = 0;
      this.fileSelector.FileDialogTitle = null;
      this.fileSelector.Name = "fileSelector";
      this.fileSelector.FilesChanged += new System.EventHandler<System.EventArgs>(this.fileSelector_FilesChanged);
      // 
      // OpenMultipleFilesInGoogleEarthDialog
      // 
      this.AcceptButton = this.ok;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancel;
      this.Controls.Add(this.routesGroupBox);
      this.Controls.Add(this.replayGroupBox);
      this.Controls.Add(this.fileSelector);
      this.Controls.Add(this.uiInformation);
      this.Controls.Add(this.filesLabel);
      this.Controls.Add(this.cancel);
      this.Controls.Add(this.ok);
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