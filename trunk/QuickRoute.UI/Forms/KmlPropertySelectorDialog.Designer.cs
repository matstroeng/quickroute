namespace QuickRoute.UI.Forms
{
  partial class KmlPropertySelectorDialog
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KmlPropertySelectorDialog));
      this.ok = new System.Windows.Forms.Button();
      this.cancel = new System.Windows.Forms.Button();
      this.replayFrame = new System.Windows.Forms.GroupBox();
      this.adaptReplayToMapImage = new System.Windows.Forms.CheckBox();
      this.replayTailLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
      this.replayTailVisible = new System.Windows.Forms.CheckBox();
      this.replayTailDuration = new System.Windows.Forms.ComboBox();
      this.replayTimeIntervalLabel = new System.Windows.Forms.Label();
      this.replayTimeInterval = new System.Windows.Forms.ComboBox();
      this.replayMarkerStyleLabel = new System.Windows.Forms.Label();
      this.replayMarkerStyle = new System.Windows.Forms.ComboBox();
      this.includeReplay = new System.Windows.Forms.CheckBox();
      this.routeLineStyle = new System.Windows.Forms.ComboBox();
      this.includeRoute = new System.Windows.Forms.CheckBox();
      this.mapFrame = new System.Windows.Forms.GroupBox();
      this.includeMap = new System.Windows.Forms.CheckBox();
      this.routeLineStyleLabel = new System.Windows.Forms.Label();
      this.routeFrame = new System.Windows.Forms.GroupBox();
      this.adaptRouteToMapImage = new System.Windows.Forms.CheckBox();
      this.replayFrame.SuspendLayout();
      this.replayTailLayoutPanel.SuspendLayout();
      this.mapFrame.SuspendLayout();
      this.routeFrame.SuspendLayout();
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
      // replayFrame
      // 
      this.replayFrame.AccessibleDescription = null;
      this.replayFrame.AccessibleName = null;
      resources.ApplyResources(this.replayFrame, "replayFrame");
      this.replayFrame.BackgroundImage = null;
      this.replayFrame.Controls.Add(this.adaptReplayToMapImage);
      this.replayFrame.Controls.Add(this.replayTailLayoutPanel);
      this.replayFrame.Controls.Add(this.replayTimeIntervalLabel);
      this.replayFrame.Controls.Add(this.replayTimeInterval);
      this.replayFrame.Controls.Add(this.replayMarkerStyleLabel);
      this.replayFrame.Controls.Add(this.replayMarkerStyle);
      this.replayFrame.Controls.Add(this.includeReplay);
      this.replayFrame.Font = null;
      this.replayFrame.Name = "replayFrame";
      this.replayFrame.TabStop = false;
      // 
      // adaptReplayToMapImage
      // 
      this.adaptReplayToMapImage.AccessibleDescription = null;
      this.adaptReplayToMapImage.AccessibleName = null;
      resources.ApplyResources(this.adaptReplayToMapImage, "adaptReplayToMapImage");
      this.adaptReplayToMapImage.BackgroundImage = null;
      this.adaptReplayToMapImage.Font = null;
      this.adaptReplayToMapImage.Name = "adaptReplayToMapImage";
      this.adaptReplayToMapImage.UseVisualStyleBackColor = true;
      this.adaptReplayToMapImage.CheckedChanged += new System.EventHandler(this.adaptReplayToMapImage_CheckedChanged);
      // 
      // replayTailLayoutPanel
      // 
      this.replayTailLayoutPanel.AccessibleDescription = null;
      this.replayTailLayoutPanel.AccessibleName = null;
      resources.ApplyResources(this.replayTailLayoutPanel, "replayTailLayoutPanel");
      this.replayTailLayoutPanel.BackgroundImage = null;
      this.replayTailLayoutPanel.Controls.Add(this.replayTailVisible);
      this.replayTailLayoutPanel.Controls.Add(this.replayTailDuration);
      this.replayTailLayoutPanel.Font = null;
      this.replayTailLayoutPanel.Name = "replayTailLayoutPanel";
      // 
      // replayTailVisible
      // 
      this.replayTailVisible.AccessibleDescription = null;
      this.replayTailVisible.AccessibleName = null;
      resources.ApplyResources(this.replayTailVisible, "replayTailVisible");
      this.replayTailVisible.BackgroundImage = null;
      this.replayTailVisible.Font = null;
      this.replayTailVisible.Name = "replayTailVisible";
      this.replayTailVisible.UseVisualStyleBackColor = true;
      this.replayTailVisible.CheckedChanged += new System.EventHandler(this.replayTailVisible_CheckedChanged);
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
      // replayMarkerStyleLabel
      // 
      this.replayMarkerStyleLabel.AccessibleDescription = null;
      this.replayMarkerStyleLabel.AccessibleName = null;
      resources.ApplyResources(this.replayMarkerStyleLabel, "replayMarkerStyleLabel");
      this.replayMarkerStyleLabel.Font = null;
      this.replayMarkerStyleLabel.Name = "replayMarkerStyleLabel";
      // 
      // replayMarkerStyle
      // 
      this.replayMarkerStyle.AccessibleDescription = null;
      this.replayMarkerStyle.AccessibleName = null;
      resources.ApplyResources(this.replayMarkerStyle, "replayMarkerStyle");
      this.replayMarkerStyle.BackgroundImage = null;
      this.replayMarkerStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.replayMarkerStyle.Font = null;
      this.replayMarkerStyle.FormattingEnabled = true;
      this.replayMarkerStyle.Items.AddRange(new object[] {
            resources.GetString("replayMarkerStyle.Items"),
            resources.GetString("replayMarkerStyle.Items1")});
      this.replayMarkerStyle.Name = "replayMarkerStyle";
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
      // routeLineStyle
      // 
      this.routeLineStyle.AccessibleDescription = null;
      this.routeLineStyle.AccessibleName = null;
      resources.ApplyResources(this.routeLineStyle, "routeLineStyle");
      this.routeLineStyle.BackgroundImage = null;
      this.routeLineStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.routeLineStyle.Font = null;
      this.routeLineStyle.FormattingEnabled = true;
      this.routeLineStyle.Items.AddRange(new object[] {
            resources.GetString("routeLineStyle.Items"),
            resources.GetString("routeLineStyle.Items1"),
            resources.GetString("routeLineStyle.Items2")});
      this.routeLineStyle.Name = "routeLineStyle";
      this.routeLineStyle.SelectedIndexChanged += new System.EventHandler(this.routeLineStyle_SelectedIndexChanged);
      // 
      // includeRoute
      // 
      this.includeRoute.AccessibleDescription = null;
      this.includeRoute.AccessibleName = null;
      resources.ApplyResources(this.includeRoute, "includeRoute");
      this.includeRoute.BackgroundImage = null;
      this.includeRoute.Font = null;
      this.includeRoute.Name = "includeRoute";
      this.includeRoute.UseVisualStyleBackColor = true;
      this.includeRoute.CheckedChanged += new System.EventHandler(this.includeRoute_CheckedChanged);
      // 
      // mapFrame
      // 
      this.mapFrame.AccessibleDescription = null;
      this.mapFrame.AccessibleName = null;
      resources.ApplyResources(this.mapFrame, "mapFrame");
      this.mapFrame.BackgroundImage = null;
      this.mapFrame.Controls.Add(this.includeMap);
      this.mapFrame.Font = null;
      this.mapFrame.Name = "mapFrame";
      this.mapFrame.TabStop = false;
      // 
      // includeMap
      // 
      this.includeMap.AccessibleDescription = null;
      this.includeMap.AccessibleName = null;
      resources.ApplyResources(this.includeMap, "includeMap");
      this.includeMap.BackgroundImage = null;
      this.includeMap.Font = null;
      this.includeMap.Name = "includeMap";
      this.includeMap.UseVisualStyleBackColor = true;
      this.includeMap.CheckedChanged += new System.EventHandler(this.includeMap_CheckedChanged);
      // 
      // routeLineStyleLabel
      // 
      this.routeLineStyleLabel.AccessibleDescription = null;
      this.routeLineStyleLabel.AccessibleName = null;
      resources.ApplyResources(this.routeLineStyleLabel, "routeLineStyleLabel");
      this.routeLineStyleLabel.Font = null;
      this.routeLineStyleLabel.Name = "routeLineStyleLabel";
      // 
      // routeFrame
      // 
      this.routeFrame.AccessibleDescription = null;
      this.routeFrame.AccessibleName = null;
      resources.ApplyResources(this.routeFrame, "routeFrame");
      this.routeFrame.BackgroundImage = null;
      this.routeFrame.Controls.Add(this.adaptRouteToMapImage);
      this.routeFrame.Controls.Add(this.routeLineStyleLabel);
      this.routeFrame.Controls.Add(this.routeLineStyle);
      this.routeFrame.Controls.Add(this.includeRoute);
      this.routeFrame.Font = null;
      this.routeFrame.Name = "routeFrame";
      this.routeFrame.TabStop = false;
      // 
      // adaptRouteToMapImage
      // 
      this.adaptRouteToMapImage.AccessibleDescription = null;
      this.adaptRouteToMapImage.AccessibleName = null;
      resources.ApplyResources(this.adaptRouteToMapImage, "adaptRouteToMapImage");
      this.adaptRouteToMapImage.BackgroundImage = null;
      this.adaptRouteToMapImage.Font = null;
      this.adaptRouteToMapImage.Name = "adaptRouteToMapImage";
      this.adaptRouteToMapImage.UseVisualStyleBackColor = true;
      this.adaptRouteToMapImage.CheckedChanged += new System.EventHandler(this.adaptRouteToMapImage_CheckedChanged);
      // 
      // KmlPropertySelectorDialog
      // 
      this.AcceptButton = this.ok;
      this.AccessibleDescription = null;
      this.AccessibleName = null;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = null;
      this.CancelButton = this.cancel;
      this.Controls.Add(this.replayFrame);
      this.Controls.Add(this.mapFrame);
      this.Controls.Add(this.routeFrame);
      this.Controls.Add(this.cancel);
      this.Controls.Add(this.ok);
      this.Font = null;
      this.Icon = null;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "KmlPropertySelectorDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.replayFrame.ResumeLayout(false);
      this.replayFrame.PerformLayout();
      this.replayTailLayoutPanel.ResumeLayout(false);
      this.replayTailLayoutPanel.PerformLayout();
      this.mapFrame.ResumeLayout(false);
      this.mapFrame.PerformLayout();
      this.routeFrame.ResumeLayout(false);
      this.routeFrame.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button ok;
    private System.Windows.Forms.Button cancel;
    private System.Windows.Forms.GroupBox replayFrame;
    private System.Windows.Forms.FlowLayoutPanel replayTailLayoutPanel;
    private System.Windows.Forms.CheckBox replayTailVisible;
    private System.Windows.Forms.ComboBox replayTailDuration;
    private System.Windows.Forms.Label replayTimeIntervalLabel;
    private System.Windows.Forms.ComboBox replayTimeInterval;
    private System.Windows.Forms.Label replayMarkerStyleLabel;
    private System.Windows.Forms.ComboBox replayMarkerStyle;
    private System.Windows.Forms.CheckBox includeReplay;
    private System.Windows.Forms.ComboBox routeLineStyle;
    private System.Windows.Forms.CheckBox includeRoute;
    private System.Windows.Forms.GroupBox mapFrame;
    private System.Windows.Forms.CheckBox includeMap;
    private System.Windows.Forms.Label routeLineStyleLabel;
    private System.Windows.Forms.GroupBox routeFrame;
    private System.Windows.Forms.CheckBox adaptRouteToMapImage;
    private System.Windows.Forms.CheckBox adaptReplayToMapImage;
  }
}