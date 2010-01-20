using System.Drawing;
using QuickRoute.Controls;

namespace QuickRoute.UI
{
  partial class Main
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      QuickRoute.BusinessEntities.LineGraph lineGraph1 = new QuickRoute.BusinessEntities.LineGraph();
      this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
      this.canvas = new QuickRoute.Controls.Canvas();
      this.dynamicHelpLabel = new QuickRoute.Controls.ScrollableLabel();
      this.rightSplitter = new System.Windows.Forms.Splitter();
      this.rightPanel = new System.Windows.Forms.Panel();
      this.lapGridPanel = new System.Windows.Forms.Panel();
      this.laps = new System.Windows.Forms.DataGridView();
      this.lapsLabel = new System.Windows.Forms.Label();
      this.rightPanelTopSplitter = new System.Windows.Forms.Splitter();
      this.sessionPanel = new System.Windows.Forms.Panel();
      this.sessions = new System.Windows.Forms.CheckedListBox();
      this.rightPanelBottomSplitter = new System.Windows.Forms.Splitter();
      this.lapHistogramContainerPanel = new System.Windows.Forms.Panel();
      this.lapHistogramPanel = new System.Windows.Forms.Panel();
      this.lapHistogramToolstrip = new System.Windows.Forms.ToolStrip();
      this.exportLapHistogramImage = new System.Windows.Forms.ToolStripButton();
      this.lapHistogramToolstripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.lapHistogramBinWidthLabel = new System.Windows.Forms.ToolStripLabel();
      this.lapHistogramBinWidth = new System.Windows.Forms.ToolStripTextBox();
      this.bottomSplitter = new System.Windows.Forms.Splitter();
      this.bottomPanel = new System.Windows.Forms.Panel();
      this.lineGraph = new QuickRoute.Controls.LineGraphControl();
      this.momentaneousInfoPanel = new System.Windows.Forms.Panel();
      this.routeAppearanceToolstrip = new System.Windows.Forms.ToolStrip();
      this.colorCodingAttributes = new System.Windows.Forms.ToolStripComboBox();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.colorRangeStartValue = new System.Windows.Forms.ToolStripTextBox();
      this.colorRangeIntervalSlider = new QuickRoute.Controls.ToolstripColorRangeIntervalSlider();
      this.colorRangeEndValue = new System.Windows.Forms.ToolStripTextBox();
      this.colorRangeIntervalButton = new System.Windows.Forms.ToolStripButton();
      this.toolStripAutoAdjustColorRangeInterval = new System.Windows.Forms.ToolStripButton();
      this.gradientAlphaAdjustment = new QuickRoute.Controls.ToolstripTrackBar();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.routeLineWidthLabel = new System.Windows.Forms.ToolStripLabel();
      this.routeLineWidth = new QuickRoute.Controls.ToolstripNumericUpDown();
      this.routeLineMaskVisible = new System.Windows.Forms.ToolStripButton();
      this.routeLineMaskWidth = new QuickRoute.Controls.ToolstripNumericUpDown();
      this.routeLineMaskColorButton = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
      this.smoothingIntervalLengthLabel = new System.Windows.Forms.ToolStripLabel();
      this.smoothingIntervalLength = new System.Windows.Forms.ToolStripTextBox();
      this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
      this.circleTimeRadiusLabel = new System.Windows.Forms.ToolStripLabel();
      this.circleTimeRadius = new System.Windows.Forms.ToolStripTextBox();
      this.toolStrip = new System.Windows.Forms.ToolStrip();
      this.toolStripNew = new System.Windows.Forms.ToolStripButton();
      this.toolStripOpen = new System.Windows.Forms.ToolStripButton();
      this.toolStripSave = new System.Windows.Forms.ToolStripButton();
      this.tstSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripUndo = new System.Windows.Forms.ToolStripButton();
      this.toolStripRedo = new System.Windows.Forms.ToolStripButton();
      this.tstSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripZoom = new System.Windows.Forms.ToolStripComboBox();
      this.tstSeparator3 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripToolPointer = new System.Windows.Forms.ToolStripButton();
      this.toolStripToolAdjustRoute = new System.Windows.Forms.ToolStripButton();
      this.toolStripToolLap = new System.Windows.Forms.ToolStripButton();
      this.toolStripToolCut = new System.Windows.Forms.ToolStripButton();
      this.toolStripToolZoomIn = new System.Windows.Forms.ToolStripButton();
      this.toolStripToolZoomOut = new System.Windows.Forms.ToolStripButton();
      this.tstSeparator4 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripOpenInGoogleEarth = new System.Windows.Forms.ToolStripButton();
      this.toolStripPublishMap = new System.Windows.Forms.ToolStripButton();
      this.tstToolsSeparator = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripFullScreen = new System.Windows.Forms.ToolStripButton();
      this.toolStripRightPanelVisible = new System.Windows.Forms.ToolStripButton();
      this.toolStripBottomPanelVisible = new System.Windows.Forms.ToolStripButton();
      this.tstViewSeparator = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripHelp = new System.Windows.Forms.ToolStripButton();
      this.toolStripApplicationSettings = new System.Windows.Forms.ToolStripButton();
      this.tstSeparator5 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripDonate = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
      this.menuStrip = new System.Windows.Forms.MenuStrip();
      this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
      this.menuFileNew = new System.Windows.Forms.ToolStripMenuItem();
      this.menuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
      this.menuFileSave = new System.Windows.Forms.ToolStripMenuItem();
      this.menuFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
      this.menuFileExport = new System.Windows.Forms.ToolStripMenuItem();
      this.menuFileExportImage = new System.Windows.Forms.ToolStripMenuItem();
      this.menuFileExportGPX = new System.Windows.Forms.ToolStripMenuItem();
      this.menuFileExportKMZ = new System.Windows.Forms.ToolStripMenuItem();
      this.menuFileExportRouteData = new System.Windows.Forms.ToolStripMenuItem();
      this.menuFileImportSessions = new System.Windows.Forms.ToolStripMenuItem();
      this.menuFileRecentDocumentsSeparator = new System.Windows.Forms.ToolStripSeparator();
      this.menuFileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.menuFileExit = new System.Windows.Forms.ToolStripMenuItem();
      this.menuEdit = new System.Windows.Forms.ToolStripMenuItem();
      this.menuEditUndo = new System.Windows.Forms.ToolStripMenuItem();
      this.menuEditRedo = new System.Windows.Forms.ToolStripMenuItem();
      this.menuEditSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.menuEditChangeStartTime = new System.Windows.Forms.ToolStripMenuItem();
      this.menuView = new System.Windows.Forms.ToolStripMenuItem();
      this.menuViewFullScreen = new System.Windows.Forms.ToolStripMenuItem();
      this.menuViewRightPanelVisible = new System.Windows.Forms.ToolStripMenuItem();
      this.menuViewBottomPanelVisible = new System.Windows.Forms.ToolStripMenuItem();
      this.menuTools = new System.Windows.Forms.ToolStripMenuItem();
      this.menuToolsOpenInGoogleEarth = new System.Windows.Forms.ToolStripMenuItem();
      this.menuToolsOpenMultipleFilesInGoogleEarth = new System.Windows.Forms.ToolStripMenuItem();
      this.menuToolsPublishMap = new System.Windows.Forms.ToolStripMenuItem();
      this.menuToolsAddLapsFromWinSplits = new System.Windows.Forms.ToolStripMenuItem();
      this.menuSettings = new System.Windows.Forms.ToolStripMenuItem();
      this.menuSettingsLanguage = new System.Windows.Forms.ToolStripMenuItem();
      this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
      this.menuHelpHelp = new System.Windows.Forms.ToolStripMenuItem();
      this.menuHelpSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.menuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripContainer1.ContentPanel.SuspendLayout();
      this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
      this.toolStripContainer1.SuspendLayout();
      this.rightPanel.SuspendLayout();
      this.lapGridPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.laps)).BeginInit();
      this.sessionPanel.SuspendLayout();
      this.lapHistogramContainerPanel.SuspendLayout();
      this.lapHistogramToolstrip.SuspendLayout();
      this.bottomPanel.SuspendLayout();
      this.routeAppearanceToolstrip.SuspendLayout();
      this.toolStrip.SuspendLayout();
      this.menuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // toolStripContainer1
      // 
      // 
      // toolStripContainer1.ContentPanel
      // 
      this.toolStripContainer1.ContentPanel.AllowDrop = true;
      this.toolStripContainer1.ContentPanel.Controls.Add(this.canvas);
      this.toolStripContainer1.ContentPanel.Controls.Add(this.dynamicHelpLabel);
      this.toolStripContainer1.ContentPanel.Controls.Add(this.rightSplitter);
      this.toolStripContainer1.ContentPanel.Controls.Add(this.rightPanel);
      this.toolStripContainer1.ContentPanel.Controls.Add(this.bottomSplitter);
      this.toolStripContainer1.ContentPanel.Controls.Add(this.bottomPanel);
      this.toolStripContainer1.ContentPanel.Controls.Add(this.momentaneousInfoPanel);
      resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
      this.toolStripContainer1.ContentPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.toolStripContainer1_ContentPanel_DragDrop);
      this.toolStripContainer1.ContentPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.toolStripContainer1_ContentPanel_DragEnter);
      resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
      this.toolStripContainer1.Name = "toolStripContainer1";
      // 
      // toolStripContainer1.TopToolStripPanel
      // 
      this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.routeAppearanceToolstrip);
      this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip);
      // 
      // canvas
      // 
      this.canvas.AllowDrop = true;
      this.canvas.BackColor = System.Drawing.SystemColors.AppWorkspace;
      this.canvas.ColorCodingAttribute = QuickRoute.BusinessEntities.WaypointAttribute.Pace;
      this.canvas.CurrentMouseTool = QuickRoute.Controls.Canvas.MouseTool.AdjustRoute;
      this.canvas.CurrentSession = null;
      resources.ApplyResources(this.canvas, "canvas");
      this.canvas.Document = null;
      this.canvas.MaxZoom = 2;
      this.canvas.MinZoom = 0.25;
      this.canvas.Name = "canvas";
      this.canvas.PreventRedraw = false;
      this.canvas.RouteDrawingMode = QuickRoute.BusinessEntities.Document.RouteDrawingMode.Extended;
      this.canvas.RouteLineSettings = null;
      this.canvas.SelectedSessions = null;
      this.canvas.SessionsToDraw = QuickRoute.Controls.Canvas.SessionDrawingMode.Selected;
      this.canvas.Zoom = 1;
      this.canvas.DocumentChanged += new System.EventHandler<System.EventArgs>(this.canvas_DocumentChanged);
      this.canvas.CurrentSessionChanged += new System.EventHandler<System.EventArgs>(this.canvas_CurrentSessionChanged);
      this.canvas.DragDrop += new System.Windows.Forms.DragEventHandler(this.canvas_DragDrop);
      this.canvas.DragEnter += new System.Windows.Forms.DragEventHandler(this.canvas_DragEnter);
      this.canvas.BeforeZoomChanged += new System.EventHandler<System.EventArgs>(this.canvas_BeforeZoomChanged);
      this.canvas.AfterZoomChanged += new System.EventHandler<System.EventArgs>(this.canvas_AfterZoomChanged);
      this.canvas.ActionPerformed += new System.EventHandler<QuickRoute.Controls.Canvas.ActionEventArgs>(this.canvas_ActionPerformed);
      this.canvas.RouteMouseHover += new System.EventHandler<QuickRoute.Controls.Canvas.RouteMouseHoverEventArgs>(this.canvas_RouteMouseHover);
      this.canvas.MouseLeave += new System.EventHandler(this.canvas_MouseLeave);
      // 
      // dynamicHelpLabel
      // 
      resources.ApplyResources(this.dynamicHelpLabel, "dynamicHelpLabel");
      this.dynamicHelpLabel.MaxHeight = 36;
      this.dynamicHelpLabel.MinHeight = 36;
      this.dynamicHelpLabel.Name = "dynamicHelpLabel";
      // 
      // rightSplitter
      // 
      resources.ApplyResources(this.rightSplitter, "rightSplitter");
      this.rightSplitter.Name = "rightSplitter";
      this.rightSplitter.TabStop = false;
      // 
      // rightPanel
      // 
      this.rightPanel.Controls.Add(this.lapGridPanel);
      this.rightPanel.Controls.Add(this.rightPanelTopSplitter);
      this.rightPanel.Controls.Add(this.sessionPanel);
      this.rightPanel.Controls.Add(this.rightPanelBottomSplitter);
      this.rightPanel.Controls.Add(this.lapHistogramContainerPanel);
      resources.ApplyResources(this.rightPanel, "rightPanel");
      this.rightPanel.Name = "rightPanel";
      // 
      // lapGridPanel
      // 
      this.lapGridPanel.Controls.Add(this.laps);
      this.lapGridPanel.Controls.Add(this.lapsLabel);
      resources.ApplyResources(this.lapGridPanel, "lapGridPanel");
      this.lapGridPanel.Name = "lapGridPanel";
      // 
      // laps
      // 
      this.laps.AllowUserToAddRows = false;
      this.laps.AllowUserToDeleteRows = false;
      this.laps.AllowUserToResizeColumns = false;
      this.laps.AllowUserToResizeRows = false;
      this.laps.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
      this.laps.BackgroundColor = System.Drawing.SystemColors.Control;
      this.laps.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.laps.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
      resources.ApplyResources(this.laps, "laps");
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
      dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Calibri", 10F);
      dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.laps.DefaultCellStyle = dataGridViewCellStyle1;
      this.laps.EnableHeadersVisualStyles = false;
      this.laps.GridColor = System.Drawing.SystemColors.ControlLight;
      this.laps.MultiSelect = false;
      this.laps.Name = "laps";
      this.laps.ReadOnly = true;
      this.laps.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
      this.laps.RowHeadersVisible = false;
      this.laps.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.laps.VirtualMode = true;
      this.laps.MouseDown += new System.Windows.Forms.MouseEventHandler(this.laps_MouseDown);
      this.laps.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.laps_CellMouseLeave);
      this.laps.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.laps_ColumnHeaderMouseClick);
      this.laps.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.laps_CellValueNeeded);
      this.laps.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.laps_CellFormatting);
      this.laps.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.laps_CellMouseEnter);
      this.laps.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.laps_CellToolTipTextNeeded);
      this.laps.KeyDown += new System.Windows.Forms.KeyEventHandler(this.laps_KeyDown);
      this.laps.SelectionChanged += new System.EventHandler(this.laps_SelectionChanged);
      // 
      // lapsLabel
      // 
      resources.ApplyResources(this.lapsLabel, "lapsLabel");
      this.lapsLabel.Name = "lapsLabel";
      // 
      // rightPanelTopSplitter
      // 
      this.rightPanelTopSplitter.BackColor = System.Drawing.SystemColors.Control;
      this.rightPanelTopSplitter.Cursor = System.Windows.Forms.Cursors.HSplit;
      resources.ApplyResources(this.rightPanelTopSplitter, "rightPanelTopSplitter");
      this.rightPanelTopSplitter.Name = "rightPanelTopSplitter";
      this.rightPanelTopSplitter.TabStop = false;
      // 
      // sessionPanel
      // 
      this.sessionPanel.Controls.Add(this.sessions);
      resources.ApplyResources(this.sessionPanel, "sessionPanel");
      this.sessionPanel.Name = "sessionPanel";
      // 
      // sessions
      // 
      resources.ApplyResources(this.sessions, "sessions");
      this.sessions.FormattingEnabled = true;
      this.sessions.MultiColumn = true;
      this.sessions.Name = "sessions";
      this.sessions.SelectedIndexChanged += new System.EventHandler(this.sessions_SelectedIndexChanged);
      this.sessions.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.sessions_ItemCheck);
      // 
      // rightPanelBottomSplitter
      // 
      this.rightPanelBottomSplitter.BackColor = System.Drawing.SystemColors.Control;
      resources.ApplyResources(this.rightPanelBottomSplitter, "rightPanelBottomSplitter");
      this.rightPanelBottomSplitter.Name = "rightPanelBottomSplitter";
      this.rightPanelBottomSplitter.TabStop = false;
      // 
      // lapHistogramContainerPanel
      // 
      this.lapHistogramContainerPanel.Controls.Add(this.lapHistogramPanel);
      this.lapHistogramContainerPanel.Controls.Add(this.lapHistogramToolstrip);
      resources.ApplyResources(this.lapHistogramContainerPanel, "lapHistogramContainerPanel");
      this.lapHistogramContainerPanel.Name = "lapHistogramContainerPanel";
      // 
      // lapHistogramPanel
      // 
      resources.ApplyResources(this.lapHistogramPanel, "lapHistogramPanel");
      this.lapHistogramPanel.Name = "lapHistogramPanel";
      this.lapHistogramPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.lapHistogramPanel_Paint);
      this.lapHistogramPanel.Resize += new System.EventHandler(this.lapHistogramPanel_Resize);
      // 
      // lapHistogramToolstrip
      // 
      this.lapHistogramToolstrip.AllowMerge = false;
      this.lapHistogramToolstrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.lapHistogramToolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportLapHistogramImage,
            this.lapHistogramToolstripSeparator1,
            this.lapHistogramBinWidthLabel,
            this.lapHistogramBinWidth});
      resources.ApplyResources(this.lapHistogramToolstrip, "lapHistogramToolstrip");
      this.lapHistogramToolstrip.Name = "lapHistogramToolstrip";
      // 
      // exportLapHistogramImage
      // 
      this.exportLapHistogramImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.exportLapHistogramImage, "exportLapHistogramImage");
      this.exportLapHistogramImage.Name = "exportLapHistogramImage";
      this.exportLapHistogramImage.Click += new System.EventHandler(this.exportLapHistogramImage_Click);
      // 
      // lapHistogramToolstripSeparator1
      // 
      this.lapHistogramToolstripSeparator1.Name = "lapHistogramToolstripSeparator1";
      resources.ApplyResources(this.lapHistogramToolstripSeparator1, "lapHistogramToolstripSeparator1");
      // 
      // lapHistogramBinWidthLabel
      // 
      this.lapHistogramBinWidthLabel.Name = "lapHistogramBinWidthLabel";
      resources.ApplyResources(this.lapHistogramBinWidthLabel, "lapHistogramBinWidthLabel");
      // 
      // lapHistogramBinWidth
      // 
      this.lapHistogramBinWidth.Name = "lapHistogramBinWidth";
      resources.ApplyResources(this.lapHistogramBinWidth, "lapHistogramBinWidth");
      this.lapHistogramBinWidth.Leave += new System.EventHandler(this.lapHistogramBinWidth_Leave);
      this.lapHistogramBinWidth.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lapHistogramBinWidth_KeyDown);
      // 
      // bottomSplitter
      // 
      resources.ApplyResources(this.bottomSplitter, "bottomSplitter");
      this.bottomSplitter.Name = "bottomSplitter";
      this.bottomSplitter.TabStop = false;
      // 
      // bottomPanel
      // 
      this.bottomPanel.Controls.Add(this.lineGraph);
      resources.ApplyResources(this.bottomPanel, "bottomPanel");
      this.bottomPanel.Name = "bottomPanel";
      this.bottomPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.bottomPanel_Paint);
      // 
      // lineGraph
      // 
      resources.ApplyResources(this.lineGraph, "lineGraph");
      lineGraph1.EndPL = null;
      lineGraph1.Session = null;
      lineGraph1.StartPL = null;
      lineGraph1.XAxisAttribute = QuickRoute.BusinessEntities.DomainAttribute.TimeOfDay;
      lineGraph1.XAxisCaption = null;
      lineGraph1.XAxisMaxValue = 0;
      lineGraph1.XAxisMinValue = 0;
      lineGraph1.XAxisNumericConverter = null;
      lineGraph1.XAxisScaleCreator = null;
      lineGraph1.YAxisAttribute = QuickRoute.BusinessEntities.WaypointAttribute.Pace;
      lineGraph1.YAxisCaption = null;
      lineGraph1.YAxisMaxValue = 0;
      lineGraph1.YAxisMinValue = 0;
      lineGraph1.YAxisNumericConverter = null;
      lineGraph1.YAxisScaleCreator = null;
      this.lineGraph.Graph = lineGraph1;
      this.lineGraph.HoverXValue = null;
      this.lineGraph.Name = "lineGraph";
      this.lineGraph.MouseLeave += new System.EventHandler(this.lineGraph_MouseLeave);
      this.lineGraph.GraphMouseDown += new System.EventHandler<QuickRoute.Controls.Canvas.RouteMouseHoverEventArgs>(this.lineGraph_GraphMouseDown);
      this.lineGraph.GraphMouseHover += new System.EventHandler<QuickRoute.Controls.Canvas.RouteMouseHoverEventArgs>(this.lineGraph_GraphMouseHover);
      // 
      // momentaneousInfoPanel
      // 
      resources.ApplyResources(this.momentaneousInfoPanel, "momentaneousInfoPanel");
      this.momentaneousInfoPanel.Name = "momentaneousInfoPanel";
      this.momentaneousInfoPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.momentaneousInfoPanel_Paint);
      this.momentaneousInfoPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.momentaneousInfoPanel_MouseClick);
      this.momentaneousInfoPanel.Resize += new System.EventHandler(this.momentaneousInfoPanel_Resize);
      // 
      // routeAppearanceToolstrip
      // 
      resources.ApplyResources(this.routeAppearanceToolstrip, "routeAppearanceToolstrip");
      this.routeAppearanceToolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.colorCodingAttributes,
            this.toolStripSeparator1,
            this.colorRangeStartValue,
            this.colorRangeIntervalSlider,
            this.colorRangeEndValue,
            this.colorRangeIntervalButton,
            this.toolStripAutoAdjustColorRangeInterval,
            this.gradientAlphaAdjustment,
            this.toolStripSeparator2,
            this.routeLineWidthLabel,
            this.routeLineWidth,
            this.routeLineMaskVisible,
            this.routeLineMaskWidth,
            this.routeLineMaskColorButton,
            this.toolStripSeparator9,
            this.smoothingIntervalLengthLabel,
            this.smoothingIntervalLength,
            this.toolStripSeparator4,
            this.circleTimeRadiusLabel,
            this.circleTimeRadius});
      this.routeAppearanceToolstrip.Name = "routeAppearanceToolstrip";
      this.routeAppearanceToolstrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.routeAppearanceToolstrip_ItemClicked);
      // 
      // colorCodingAttributes
      // 
      this.colorCodingAttributes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.colorCodingAttributes.Name = "colorCodingAttributes";
      resources.ApplyResources(this.colorCodingAttributes, "colorCodingAttributes");
      this.colorCodingAttributes.SelectedIndexChanged += new System.EventHandler(this.colorCodingAttributes_SelectedIndexChanged);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
      // 
      // colorRangeStartValue
      // 
      this.colorRangeStartValue.Name = "colorRangeStartValue";
      resources.ApplyResources(this.colorRangeStartValue, "colorRangeStartValue");
      this.colorRangeStartValue.Leave += new System.EventHandler(this.colorRangeStartValue_Leave);
      this.colorRangeStartValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.colorRangeStartValue_KeyDown);
      // 
      // colorRangeIntervalSlider
      // 
      resources.ApplyResources(this.colorRangeIntervalSlider, "colorRangeIntervalSlider");
      this.colorRangeIntervalSlider.BackColor = System.Drawing.SystemColors.Control;
      this.colorRangeIntervalSlider.Name = "colorRangeIntervalSlider";
      this.colorRangeIntervalSlider.ColorRangeStartValueChanged += new System.EventHandler(this.colorRangeIntervalSlider_ColorRangeStartValueChanged);
      this.colorRangeIntervalSlider.ColorRangeEndValueChanged += new System.EventHandler(this.colorRangeIntervalSlider_ColorRangeEndValueChanged);
      this.colorRangeIntervalSlider.ColorRangeClicked += new System.Windows.Forms.MouseEventHandler(this.colorRangeIntervalSlider_ColorRangeClicked);
      this.colorRangeIntervalSlider.ColorRangeStartValueChanging += new System.EventHandler(this.colorRangeIntervalSlider_ColorRangeStartValueChanging);
      this.colorRangeIntervalSlider.ColorRangeEndValueChanging += new System.EventHandler(this.colorRangeIntervalSlider_ColorRangeEndValueChanging);
      // 
      // colorRangeEndValue
      // 
      this.colorRangeEndValue.Name = "colorRangeEndValue";
      resources.ApplyResources(this.colorRangeEndValue, "colorRangeEndValue");
      this.colorRangeEndValue.Leave += new System.EventHandler(this.colorRangeEndValue_Leave);
      this.colorRangeEndValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.colorRangeEndValue_KeyDown);
      // 
      // colorRangeIntervalButton
      // 
      this.colorRangeIntervalButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.colorRangeIntervalButton, "colorRangeIntervalButton");
      this.colorRangeIntervalButton.Name = "colorRangeIntervalButton";
      this.colorRangeIntervalButton.Click += new System.EventHandler(this.colorRangeIntervalButton_Click);
      // 
      // toolStripAutoAdjustColorRangeInterval
      // 
      this.toolStripAutoAdjustColorRangeInterval.CheckOnClick = true;
      this.toolStripAutoAdjustColorRangeInterval.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.toolStripAutoAdjustColorRangeInterval, "toolStripAutoAdjustColorRangeInterval");
      this.toolStripAutoAdjustColorRangeInterval.Name = "toolStripAutoAdjustColorRangeInterval";
      this.toolStripAutoAdjustColorRangeInterval.CheckedChanged += new System.EventHandler(this.toolStripAutoAdjustColorRangeInterval_CheckedChanged);
      // 
      // gradientAlphaAdjustment
      // 
      resources.ApplyResources(this.gradientAlphaAdjustment, "gradientAlphaAdjustment");
      this.gradientAlphaAdjustment.Name = "gradientAlphaAdjustment";
      this.gradientAlphaAdjustment.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gradientAlphaAdjustment_MouseDown);
      this.gradientAlphaAdjustment.ValueChanged += new System.EventHandler(this.gradientAlphaAdjustment_ValueChanged);
      this.gradientAlphaAdjustment.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gradientAlphaAdjustment_MouseUp);
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
      // 
      // routeLineWidthLabel
      // 
      this.routeLineWidthLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.routeLineWidthLabel, "routeLineWidthLabel");
      this.routeLineWidthLabel.Margin = new System.Windows.Forms.Padding(1, 1, 2, 2);
      this.routeLineWidthLabel.Name = "routeLineWidthLabel";
      // 
      // routeLineWidth
      // 
      resources.ApplyResources(this.routeLineWidth, "routeLineWidth");
      this.routeLineWidth.Name = "routeLineWidth";
      this.routeLineWidth.KeyDown += new System.Windows.Forms.KeyEventHandler(this.routeLineWidth_KeyDown);
      this.routeLineWidth.ValueChanged += new System.EventHandler(this.routeLineWidth_ValueChanged);
      // 
      // routeLineMaskVisible
      // 
      this.routeLineMaskVisible.CheckOnClick = true;
      this.routeLineMaskVisible.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.routeLineMaskVisible, "routeLineMaskVisible");
      this.routeLineMaskVisible.Name = "routeLineMaskVisible";
      this.routeLineMaskVisible.CheckedChanged += new System.EventHandler(this.routeLineMaskVisible_CheckedChanged);
      // 
      // routeLineMaskWidth
      // 
      resources.ApplyResources(this.routeLineMaskWidth, "routeLineMaskWidth");
      this.routeLineMaskWidth.Name = "routeLineMaskWidth";
      this.routeLineMaskWidth.KeyDown += new System.Windows.Forms.KeyEventHandler(this.routeLineMaskWidth_KeyDown);
      this.routeLineMaskWidth.ValueChanged += new System.EventHandler(this.routeLineMaskWidth_ValueChanged);
      // 
      // routeLineMaskColorButton
      // 
      this.routeLineMaskColorButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.routeLineMaskColorButton, "routeLineMaskColorButton");
      this.routeLineMaskColorButton.Name = "routeLineMaskColorButton";
      this.routeLineMaskColorButton.Click += new System.EventHandler(this.routeLineMaskColorButton_Click);
      // 
      // toolStripSeparator9
      // 
      this.toolStripSeparator9.Name = "toolStripSeparator9";
      resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
      // 
      // smoothingIntervalLengthLabel
      // 
      this.smoothingIntervalLengthLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.smoothingIntervalLengthLabel, "smoothingIntervalLengthLabel");
      this.smoothingIntervalLengthLabel.Margin = new System.Windows.Forms.Padding(1, 1, 2, 2);
      this.smoothingIntervalLengthLabel.Name = "smoothingIntervalLengthLabel";
      // 
      // smoothingIntervalLength
      // 
      this.smoothingIntervalLength.Name = "smoothingIntervalLength";
      resources.ApplyResources(this.smoothingIntervalLength, "smoothingIntervalLength");
      this.smoothingIntervalLength.Leave += new System.EventHandler(this.smoothingIntervalLength_Leave);
      this.smoothingIntervalLength.KeyDown += new System.Windows.Forms.KeyEventHandler(this.smoothingIntervalLength_KeyDown);
      // 
      // toolStripSeparator4
      // 
      this.toolStripSeparator4.Name = "toolStripSeparator4";
      resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
      // 
      // circleTimeRadiusLabel
      // 
      this.circleTimeRadiusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.circleTimeRadiusLabel, "circleTimeRadiusLabel");
      this.circleTimeRadiusLabel.Margin = new System.Windows.Forms.Padding(1, 1, 2, 2);
      this.circleTimeRadiusLabel.Name = "circleTimeRadiusLabel";
      // 
      // circleTimeRadius
      // 
      this.circleTimeRadius.Name = "circleTimeRadius";
      resources.ApplyResources(this.circleTimeRadius, "circleTimeRadius");
      this.circleTimeRadius.Leave += new System.EventHandler(this.circleTimeRadius_Leave);
      this.circleTimeRadius.KeyDown += new System.Windows.Forms.KeyEventHandler(this.circleTimeRadius_KeyDown);
      // 
      // toolStrip
      // 
      resources.ApplyResources(this.toolStrip, "toolStrip");
      this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripNew,
            this.toolStripOpen,
            this.toolStripSave,
            this.tstSeparator1,
            this.toolStripUndo,
            this.toolStripRedo,
            this.tstSeparator2,
            this.toolStripZoom,
            this.tstSeparator3,
            this.toolStripToolPointer,
            this.toolStripToolAdjustRoute,
            this.toolStripToolLap,
            this.toolStripToolCut,
            this.toolStripToolZoomIn,
            this.toolStripToolZoomOut,
            this.tstSeparator4,
            this.toolStripOpenInGoogleEarth,
            this.toolStripPublishMap,
            this.tstToolsSeparator,
            this.toolStripFullScreen,
            this.toolStripRightPanelVisible,
            this.toolStripBottomPanelVisible,
            this.tstViewSeparator,
            this.toolStripHelp,
            this.toolStripApplicationSettings,
            this.tstSeparator5,
            this.toolStripDonate});
      this.toolStrip.Name = "toolStrip";
      this.toolStrip.DragOver += new System.Windows.Forms.DragEventHandler(this.toolStrip_DragOver);
      // 
      // toolStripNew
      // 
      this.toolStripNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.toolStripNew, "toolStripNew");
      this.toolStripNew.Name = "toolStripNew";
      this.toolStripNew.Click += new System.EventHandler(this.toolStripNew_Click);
      // 
      // toolStripOpen
      // 
      this.toolStripOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.toolStripOpen, "toolStripOpen");
      this.toolStripOpen.Name = "toolStripOpen";
      this.toolStripOpen.Click += new System.EventHandler(this.toolStripOpen_Click);
      // 
      // toolStripSave
      // 
      this.toolStripSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.toolStripSave, "toolStripSave");
      this.toolStripSave.Name = "toolStripSave";
      this.toolStripSave.Click += new System.EventHandler(this.toolStripSave_Click);
      // 
      // tstSeparator1
      // 
      this.tstSeparator1.Name = "tstSeparator1";
      resources.ApplyResources(this.tstSeparator1, "tstSeparator1");
      // 
      // toolStripUndo
      // 
      this.toolStripUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.toolStripUndo, "toolStripUndo");
      this.toolStripUndo.Name = "toolStripUndo";
      this.toolStripUndo.Click += new System.EventHandler(this.toolStripUndo_Click);
      // 
      // toolStripRedo
      // 
      this.toolStripRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.toolStripRedo, "toolStripRedo");
      this.toolStripRedo.Name = "toolStripRedo";
      this.toolStripRedo.Click += new System.EventHandler(this.toolStripRedo_Click);
      // 
      // tstSeparator2
      // 
      this.tstSeparator2.Name = "tstSeparator2";
      resources.ApplyResources(this.tstSeparator2, "tstSeparator2");
      // 
      // toolStripZoom
      // 
      resources.ApplyResources(this.toolStripZoom, "toolStripZoom");
      this.toolStripZoom.Items.AddRange(new object[] {
            resources.GetString("toolStripZoom.Items"),
            resources.GetString("toolStripZoom.Items1"),
            resources.GetString("toolStripZoom.Items2"),
            resources.GetString("toolStripZoom.Items3"),
            resources.GetString("toolStripZoom.Items4"),
            resources.GetString("toolStripZoom.Items5")});
      this.toolStripZoom.Name = "toolStripZoom";
      this.toolStripZoom.SelectedIndexChanged += new System.EventHandler(this.toolStripZoom_SelectedIndexChanged);
      this.toolStripZoom.Leave += new System.EventHandler(this.toolStripZoom_Leave);
      this.toolStripZoom.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.toolStripZoom_KeyPress);
      // 
      // tstSeparator3
      // 
      this.tstSeparator3.Name = "tstSeparator3";
      resources.ApplyResources(this.tstSeparator3, "tstSeparator3");
      // 
      // toolStripToolPointer
      // 
      this.toolStripToolPointer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.toolStripToolPointer, "toolStripToolPointer");
      this.toolStripToolPointer.Name = "toolStripToolPointer";
      this.toolStripToolPointer.Click += new System.EventHandler(this.toolStripToolPointer_Click);
      // 
      // toolStripToolAdjustRoute
      // 
      this.toolStripToolAdjustRoute.Checked = true;
      this.toolStripToolAdjustRoute.CheckState = System.Windows.Forms.CheckState.Checked;
      this.toolStripToolAdjustRoute.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.toolStripToolAdjustRoute, "toolStripToolAdjustRoute");
      this.toolStripToolAdjustRoute.Name = "toolStripToolAdjustRoute";
      this.toolStripToolAdjustRoute.Click += new System.EventHandler(this.toolStripToolAdjustRoute_Click);
      // 
      // toolStripToolLap
      // 
      this.toolStripToolLap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.toolStripToolLap, "toolStripToolLap");
      this.toolStripToolLap.Name = "toolStripToolLap";
      this.toolStripToolLap.Click += new System.EventHandler(this.toolStripToolLap_Click);
      // 
      // toolStripToolCut
      // 
      this.toolStripToolCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.toolStripToolCut, "toolStripToolCut");
      this.toolStripToolCut.Name = "toolStripToolCut";
      this.toolStripToolCut.Click += new System.EventHandler(this.toolStripToolCut_Click);
      // 
      // toolStripToolZoomIn
      // 
      this.toolStripToolZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.toolStripToolZoomIn, "toolStripToolZoomIn");
      this.toolStripToolZoomIn.Name = "toolStripToolZoomIn";
      this.toolStripToolZoomIn.Click += new System.EventHandler(this.toolStripToolZoomIn_Click);
      // 
      // toolStripToolZoomOut
      // 
      this.toolStripToolZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.toolStripToolZoomOut, "toolStripToolZoomOut");
      this.toolStripToolZoomOut.Name = "toolStripToolZoomOut";
      this.toolStripToolZoomOut.Click += new System.EventHandler(this.toolStripToolZoomOut_Click);
      // 
      // tstSeparator4
      // 
      this.tstSeparator4.Name = "tstSeparator4";
      resources.ApplyResources(this.tstSeparator4, "tstSeparator4");
      // 
      // toolStripOpenInGoogleEarth
      // 
      this.toolStripOpenInGoogleEarth.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.toolStripOpenInGoogleEarth, "toolStripOpenInGoogleEarth");
      this.toolStripOpenInGoogleEarth.Name = "toolStripOpenInGoogleEarth";
      this.toolStripOpenInGoogleEarth.Click += new System.EventHandler(this.toolStripOpenInGoogleEarth_Click);
      // 
      // toolStripPublishMap
      // 
      this.toolStripPublishMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.toolStripPublishMap, "toolStripPublishMap");
      this.toolStripPublishMap.Name = "toolStripPublishMap";
      this.toolStripPublishMap.Click += new System.EventHandler(this.toolStripPublishMap_Click);
      // 
      // tstToolsSeparator
      // 
      this.tstToolsSeparator.Name = "tstToolsSeparator";
      resources.ApplyResources(this.tstToolsSeparator, "tstToolsSeparator");
      // 
      // toolStripFullScreen
      // 
      this.toolStripFullScreen.CheckOnClick = true;
      this.toolStripFullScreen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.toolStripFullScreen, "toolStripFullScreen");
      this.toolStripFullScreen.Name = "toolStripFullScreen";
      this.toolStripFullScreen.CheckedChanged += new System.EventHandler(this.toolStripFullScreen_CheckedChanged);
      // 
      // toolStripRightPanelVisible
      // 
      this.toolStripRightPanelVisible.Checked = true;
      this.toolStripRightPanelVisible.CheckOnClick = true;
      this.toolStripRightPanelVisible.CheckState = System.Windows.Forms.CheckState.Checked;
      this.toolStripRightPanelVisible.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.toolStripRightPanelVisible, "toolStripRightPanelVisible");
      this.toolStripRightPanelVisible.Name = "toolStripRightPanelVisible";
      this.toolStripRightPanelVisible.CheckedChanged += new System.EventHandler(this.toolStripRightPanelVisible_CheckedChanged);
      // 
      // toolStripBottomPanelVisible
      // 
      this.toolStripBottomPanelVisible.Checked = true;
      this.toolStripBottomPanelVisible.CheckOnClick = true;
      this.toolStripBottomPanelVisible.CheckState = System.Windows.Forms.CheckState.Checked;
      this.toolStripBottomPanelVisible.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.toolStripBottomPanelVisible, "toolStripBottomPanelVisible");
      this.toolStripBottomPanelVisible.Name = "toolStripBottomPanelVisible";
      this.toolStripBottomPanelVisible.CheckedChanged += new System.EventHandler(this.toolStripBottomPanelVisible_CheckedChanged);
      // 
      // tstViewSeparator
      // 
      this.tstViewSeparator.Name = "tstViewSeparator";
      resources.ApplyResources(this.tstViewSeparator, "tstViewSeparator");
      // 
      // toolStripHelp
      // 
      this.toolStripHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.toolStripHelp, "toolStripHelp");
      this.toolStripHelp.Name = "toolStripHelp";
      this.toolStripHelp.Click += new System.EventHandler(this.toolStripHelp_Click);
      // 
      // toolStripApplicationSettings
      // 
      this.toolStripApplicationSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      resources.ApplyResources(this.toolStripApplicationSettings, "toolStripApplicationSettings");
      this.toolStripApplicationSettings.Name = "toolStripApplicationSettings";
      this.toolStripApplicationSettings.Click += new System.EventHandler(this.toolStripApplicationSettings_Click);
      // 
      // tstSeparator5
      // 
      this.tstSeparator5.Name = "tstSeparator5";
      resources.ApplyResources(this.tstSeparator5, "tstSeparator5");
      // 
      // toolStripDonate
      // 
      resources.ApplyResources(this.toolStripDonate, "toolStripDonate");
      this.toolStripDonate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolStripDonate.Name = "toolStripDonate";
      this.toolStripDonate.Click += new System.EventHandler(this.toolStripDonate_Click);
      // 
      // toolStripSeparator12
      // 
      this.toolStripSeparator12.Name = "toolStripSeparator12";
      resources.ApplyResources(this.toolStripSeparator12, "toolStripSeparator12");
      // 
      // toolStripSeparator11
      // 
      this.toolStripSeparator11.Name = "toolStripSeparator11";
      resources.ApplyResources(this.toolStripSeparator11, "toolStripSeparator11");
      // 
      // menuStrip
      // 
      this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuEdit,
            this.menuView,
            this.menuTools,
            this.menuSettings,
            this.menuHelp});
      resources.ApplyResources(this.menuStrip, "menuStrip");
      this.menuStrip.Name = "menuStrip";
      // 
      // menuFile
      // 
      this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFileNew,
            this.menuFileOpen,
            this.menuFileSave,
            this.menuFileSaveAs,
            this.menuFileExport,
            this.menuFileImportSessions,
            this.menuFileRecentDocumentsSeparator,
            this.menuFileSeparator1,
            this.menuFileExit});
      this.menuFile.Name = "menuFile";
      resources.ApplyResources(this.menuFile, "menuFile");
      // 
      // menuFileNew
      // 
      resources.ApplyResources(this.menuFileNew, "menuFileNew");
      this.menuFileNew.Name = "menuFileNew";
      this.menuFileNew.Click += new System.EventHandler(this.menuFileNew_Click);
      // 
      // menuFileOpen
      // 
      resources.ApplyResources(this.menuFileOpen, "menuFileOpen");
      this.menuFileOpen.Name = "menuFileOpen";
      this.menuFileOpen.Click += new System.EventHandler(this.menuFileOpen_Click);
      // 
      // menuFileSave
      // 
      resources.ApplyResources(this.menuFileSave, "menuFileSave");
      this.menuFileSave.Name = "menuFileSave";
      this.menuFileSave.Click += new System.EventHandler(this.menuFileSave_Click);
      // 
      // menuFileSaveAs
      // 
      this.menuFileSaveAs.Name = "menuFileSaveAs";
      resources.ApplyResources(this.menuFileSaveAs, "menuFileSaveAs");
      this.menuFileSaveAs.Click += new System.EventHandler(this.menuFileSaveAs_Click);
      // 
      // menuFileExport
      // 
      this.menuFileExport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFileExportImage,
            this.menuFileExportGPX,
            this.menuFileExportKMZ,
            this.menuFileExportRouteData});
      this.menuFileExport.Name = "menuFileExport";
      resources.ApplyResources(this.menuFileExport, "menuFileExport");
      // 
      // menuFileExportImage
      // 
      this.menuFileExportImage.Name = "menuFileExportImage";
      resources.ApplyResources(this.menuFileExportImage, "menuFileExportImage");
      this.menuFileExportImage.Click += new System.EventHandler(this.menuFileExportImage_Click);
      // 
      // menuFileExportGPX
      // 
      this.menuFileExportGPX.Name = "menuFileExportGPX";
      resources.ApplyResources(this.menuFileExportGPX, "menuFileExportGPX");
      this.menuFileExportGPX.Click += new System.EventHandler(this.menuFileExportGPX_Click);
      // 
      // menuFileExportKMZ
      // 
      this.menuFileExportKMZ.Name = "menuFileExportKMZ";
      resources.ApplyResources(this.menuFileExportKMZ, "menuFileExportKMZ");
      this.menuFileExportKMZ.Click += new System.EventHandler(this.menuFileExportKMZ_Click);
      // 
      // menuFileExportRouteData
      // 
      this.menuFileExportRouteData.Name = "menuFileExportRouteData";
      resources.ApplyResources(this.menuFileExportRouteData, "menuFileExportRouteData");
      this.menuFileExportRouteData.Click += new System.EventHandler(this.menuFileExportRouteData_Click);
      // 
      // menuFileImportSessions
      // 
      this.menuFileImportSessions.Name = "menuFileImportSessions";
      resources.ApplyResources(this.menuFileImportSessions, "menuFileImportSessions");
      this.menuFileImportSessions.Click += new System.EventHandler(this.menuFileImportSessions_Click);
      // 
      // menuFileRecentDocumentsSeparator
      // 
      this.menuFileRecentDocumentsSeparator.Name = "menuFileRecentDocumentsSeparator";
      resources.ApplyResources(this.menuFileRecentDocumentsSeparator, "menuFileRecentDocumentsSeparator");
      // 
      // menuFileSeparator1
      // 
      this.menuFileSeparator1.Name = "menuFileSeparator1";
      resources.ApplyResources(this.menuFileSeparator1, "menuFileSeparator1");
      // 
      // menuFileExit
      // 
      this.menuFileExit.Name = "menuFileExit";
      resources.ApplyResources(this.menuFileExit, "menuFileExit");
      this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
      // 
      // menuEdit
      // 
      this.menuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuEditUndo,
            this.menuEditRedo,
            this.menuEditSeparator1,
            this.menuEditChangeStartTime});
      this.menuEdit.Name = "menuEdit";
      resources.ApplyResources(this.menuEdit, "menuEdit");
      // 
      // menuEditUndo
      // 
      resources.ApplyResources(this.menuEditUndo, "menuEditUndo");
      this.menuEditUndo.Name = "menuEditUndo";
      this.menuEditUndo.Click += new System.EventHandler(this.menuEditUndo_Click);
      // 
      // menuEditRedo
      // 
      resources.ApplyResources(this.menuEditRedo, "menuEditRedo");
      this.menuEditRedo.Name = "menuEditRedo";
      this.menuEditRedo.Click += new System.EventHandler(this.menuEditRedo_Click);
      // 
      // menuEditSeparator1
      // 
      this.menuEditSeparator1.Name = "menuEditSeparator1";
      resources.ApplyResources(this.menuEditSeparator1, "menuEditSeparator1");
      // 
      // menuEditChangeStartTime
      // 
      this.menuEditChangeStartTime.Name = "menuEditChangeStartTime";
      resources.ApplyResources(this.menuEditChangeStartTime, "menuEditChangeStartTime");
      this.menuEditChangeStartTime.Click += new System.EventHandler(this.menuEditChangeStartTime_Click);
      // 
      // menuView
      // 
      this.menuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuViewFullScreen,
            this.menuViewRightPanelVisible,
            this.menuViewBottomPanelVisible});
      this.menuView.Name = "menuView";
      resources.ApplyResources(this.menuView, "menuView");
      // 
      // menuViewFullScreen
      // 
      this.menuViewFullScreen.CheckOnClick = true;
      resources.ApplyResources(this.menuViewFullScreen, "menuViewFullScreen");
      this.menuViewFullScreen.Name = "menuViewFullScreen";
      this.menuViewFullScreen.CheckedChanged += new System.EventHandler(this.menuViewFullScreen_CheckedChanged);
      // 
      // menuViewRightPanelVisible
      // 
      this.menuViewRightPanelVisible.Checked = true;
      this.menuViewRightPanelVisible.CheckOnClick = true;
      this.menuViewRightPanelVisible.CheckState = System.Windows.Forms.CheckState.Checked;
      resources.ApplyResources(this.menuViewRightPanelVisible, "menuViewRightPanelVisible");
      this.menuViewRightPanelVisible.Name = "menuViewRightPanelVisible";
      this.menuViewRightPanelVisible.CheckedChanged += new System.EventHandler(this.menuViewRightPanelVisible_CheckedChanged);
      // 
      // menuViewBottomPanelVisible
      // 
      this.menuViewBottomPanelVisible.Checked = true;
      this.menuViewBottomPanelVisible.CheckOnClick = true;
      this.menuViewBottomPanelVisible.CheckState = System.Windows.Forms.CheckState.Checked;
      resources.ApplyResources(this.menuViewBottomPanelVisible, "menuViewBottomPanelVisible");
      this.menuViewBottomPanelVisible.Name = "menuViewBottomPanelVisible";
      this.menuViewBottomPanelVisible.CheckedChanged += new System.EventHandler(this.menuViewBottomPanelVisible_CheckedChanged);
      // 
      // menuTools
      // 
      this.menuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolsOpenInGoogleEarth,
            this.menuToolsOpenMultipleFilesInGoogleEarth,
            this.menuToolsPublishMap,
            this.menuToolsAddLapsFromWinSplits});
      this.menuTools.Name = "menuTools";
      resources.ApplyResources(this.menuTools, "menuTools");
      // 
      // menuToolsOpenInGoogleEarth
      // 
      resources.ApplyResources(this.menuToolsOpenInGoogleEarth, "menuToolsOpenInGoogleEarth");
      this.menuToolsOpenInGoogleEarth.Name = "menuToolsOpenInGoogleEarth";
      this.menuToolsOpenInGoogleEarth.Click += new System.EventHandler(this.menuToolsOpenInGoogleEarth_Click);
      // 
      // menuToolsOpenMultipleFilesInGoogleEarth
      // 
      this.menuToolsOpenMultipleFilesInGoogleEarth.Name = "menuToolsOpenMultipleFilesInGoogleEarth";
      resources.ApplyResources(this.menuToolsOpenMultipleFilesInGoogleEarth, "menuToolsOpenMultipleFilesInGoogleEarth");
      this.menuToolsOpenMultipleFilesInGoogleEarth.Click += new System.EventHandler(this.menuToolsOpenMultipleFilesInGoogleEarth_Click);
      // 
      // menuToolsPublishMap
      // 
      resources.ApplyResources(this.menuToolsPublishMap, "menuToolsPublishMap");
      this.menuToolsPublishMap.Name = "menuToolsPublishMap";
      this.menuToolsPublishMap.Click += new System.EventHandler(this.menuToolsPublishMap_Click);
      // 
      // menuToolsAddLapsFromWinSplits
      // 
      this.menuToolsAddLapsFromWinSplits.Name = "menuToolsAddLapsFromWinSplits";
      resources.ApplyResources(this.menuToolsAddLapsFromWinSplits, "menuToolsAddLapsFromWinSplits");
      this.menuToolsAddLapsFromWinSplits.Click += new System.EventHandler(this.menuToolsAddLapsFromWinSplits_Click);
      // 
      // menuSettings
      // 
      this.menuSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSettingsLanguage});
      this.menuSettings.Name = "menuSettings";
      resources.ApplyResources(this.menuSettings, "menuSettings");
      // 
      // menuSettingsLanguage
      // 
      this.menuSettingsLanguage.Name = "menuSettingsLanguage";
      resources.ApplyResources(this.menuSettingsLanguage, "menuSettingsLanguage");
      this.menuSettingsLanguage.Click += new System.EventHandler(this.menuSettingsLanguage_Click);
      // 
      // menuHelp
      // 
      this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelpHelp,
            this.menuHelpSeparator1,
            this.menuHelpAbout});
      this.menuHelp.Name = "menuHelp";
      resources.ApplyResources(this.menuHelp, "menuHelp");
      // 
      // menuHelpHelp
      // 
      resources.ApplyResources(this.menuHelpHelp, "menuHelpHelp");
      this.menuHelpHelp.Name = "menuHelpHelp";
      this.menuHelpHelp.Click += new System.EventHandler(this.menuHelpHelp_Click);
      // 
      // menuHelpSeparator1
      // 
      this.menuHelpSeparator1.Name = "menuHelpSeparator1";
      resources.ApplyResources(this.menuHelpSeparator1, "menuHelpSeparator1");
      // 
      // menuHelpAbout
      // 
      this.menuHelpAbout.Name = "menuHelpAbout";
      resources.ApplyResources(this.menuHelpAbout, "menuHelpAbout");
      this.menuHelpAbout.Click += new System.EventHandler(this.menuHelpAbout_Click);
      // 
      // toolStripSeparator3
      // 
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
      // 
      // Main
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.toolStripContainer1);
      this.Controls.Add(this.menuStrip);
      this.KeyPreview = true;
      this.MainMenuStrip = this.menuStrip;
      this.Name = "Main";
      this.Load += new System.EventHandler(this.Main_Load);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
      this.Resize += new System.EventHandler(this.Main_Resize);
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Main_KeyDown);
      this.toolStripContainer1.ContentPanel.ResumeLayout(false);
      this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
      this.toolStripContainer1.TopToolStripPanel.PerformLayout();
      this.toolStripContainer1.ResumeLayout(false);
      this.toolStripContainer1.PerformLayout();
      this.rightPanel.ResumeLayout(false);
      this.lapGridPanel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.laps)).EndInit();
      this.sessionPanel.ResumeLayout(false);
      this.lapHistogramContainerPanel.ResumeLayout(false);
      this.lapHistogramContainerPanel.PerformLayout();
      this.lapHistogramToolstrip.ResumeLayout(false);
      this.lapHistogramToolstrip.PerformLayout();
      this.bottomPanel.ResumeLayout(false);
      this.routeAppearanceToolstrip.ResumeLayout(false);
      this.routeAppearanceToolstrip.PerformLayout();
      this.toolStrip.ResumeLayout(false);
      this.toolStrip.PerformLayout();
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ToolStrip toolStrip;
    private System.Windows.Forms.ToolStripButton toolStripNew;
    private System.Windows.Forms.ToolStripButton toolStripOpen;
    private System.Windows.Forms.ToolStripButton toolStripSave;
    private System.Windows.Forms.ToolStripSeparator tstSeparator1;
    private System.Windows.Forms.ToolStripButton toolStripHelp;
    private System.Windows.Forms.ToolStripSeparator tstSeparator2;
    private System.Windows.Forms.ToolStripComboBox toolStripZoom;
    private System.Windows.Forms.MenuStrip menuStrip;
    private System.Windows.Forms.ToolStripMenuItem menuFile;
    private System.Windows.Forms.ToolStripMenuItem menuFileNew;
    private System.Windows.Forms.ToolStripMenuItem menuFileOpen;
    private System.Windows.Forms.ToolStripMenuItem menuFileSave;
    private System.Windows.Forms.ToolStripMenuItem menuHelp;
    private System.Windows.Forms.ToolStripMenuItem menuFileExit;
    private System.Windows.Forms.ToolStripSeparator menuFileSeparator1;
    private System.Windows.Forms.ToolStripMenuItem menuHelpHelp;
    private QuickRoute.Controls.Canvas canvas;
    private System.Windows.Forms.ToolStripButton toolStripToolAdjustRoute;
    private System.Windows.Forms.ToolStripButton toolStripToolZoomIn;
    private System.Windows.Forms.ToolStripButton toolStripToolZoomOut;
    private System.Windows.Forms.ToolStripSeparator tstSeparator3;
    private System.Windows.Forms.Panel rightPanel;
    private System.Windows.Forms.Splitter rightSplitter;
    private System.Windows.Forms.Panel bottomPanel;
    private System.Windows.Forms.Splitter bottomSplitter;
    private System.Windows.Forms.ToolStripMenuItem menuFileSaveAs;
    private System.Windows.Forms.ToolStripContainer toolStripContainer1;
    private System.Windows.Forms.ToolStrip routeAppearanceToolstrip;
    private System.Windows.Forms.ToolStripComboBox colorCodingAttributes;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripTextBox colorRangeStartValue;
    private System.Windows.Forms.ToolStripTextBox colorRangeEndValue;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private QuickRoute.Controls.ToolstripColorRangeIntervalSlider colorRangeIntervalSlider;
    private QuickRoute.Controls.ToolstripNumericUpDown routeLineWidth;
    private System.Windows.Forms.ToolStripButton routeLineMaskVisible;
    private QuickRoute.Controls.ToolstripNumericUpDown routeLineMaskWidth;
    private QuickRoute.Controls.ToolstripTrackBar gradientAlphaAdjustment;
    private System.Windows.Forms.ToolStripButton routeLineMaskColorButton;
    private System.Windows.Forms.ToolStripButton colorRangeIntervalButton;
    private System.Windows.Forms.ToolStripButton toolStripToolCut;
    private System.Windows.Forms.ToolStripButton toolStripToolLap;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
    private System.Windows.Forms.ToolStripButton toolStripUndo;
    private System.Windows.Forms.ToolStripButton toolStripRedo;
    private System.Windows.Forms.ToolStripSeparator tstSeparator4;
    private System.Windows.Forms.ToolStripMenuItem menuEdit;
    private System.Windows.Forms.ToolStripMenuItem menuEditUndo;
    private System.Windows.Forms.ToolStripMenuItem menuEditRedo;
    private System.Windows.Forms.ToolStripMenuItem menuHelpAbout;
    private System.Windows.Forms.ToolStripSeparator menuHelpSeparator1;
    private System.Windows.Forms.Panel lapGridPanel;
    private System.Windows.Forms.Label lapsLabel;
    private System.Windows.Forms.Splitter rightPanelTopSplitter;
    private System.Windows.Forms.Splitter rightPanelBottomSplitter;
    private System.Windows.Forms.Panel lapHistogramContainerPanel;
    private System.Windows.Forms.Panel lapHistogramPanel;
    private System.Windows.Forms.ToolStrip lapHistogramToolstrip;
    private System.Windows.Forms.ToolStripButton exportLapHistogramImage;
    private System.Windows.Forms.ToolStripSeparator lapHistogramToolstripSeparator1;
    private System.Windows.Forms.ToolStripLabel lapHistogramBinWidthLabel;
    private System.Windows.Forms.ToolStripTextBox lapHistogramBinWidth;
    private System.Windows.Forms.ToolStripSeparator tstViewSeparator;
    private System.Windows.Forms.ToolStripTextBox smoothingIntervalLength;
    private System.Windows.Forms.ToolStripButton toolStripApplicationSettings;
    private QuickRoute.Controls.ScrollableLabel dynamicHelpLabel;
    private System.Windows.Forms.ToolStripSeparator tstSeparator5;
    private System.Windows.Forms.ToolStripButton toolStripDonate;
    private System.Windows.Forms.ToolStripButton toolStripToolPointer;
    private System.Windows.Forms.ToolStripMenuItem menuSettings;
    private System.Windows.Forms.ToolStripMenuItem menuSettingsLanguage;
    private System.Windows.Forms.ToolStripButton toolStripRightPanelVisible;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
    private QuickRoute.Controls.LineGraphControl lineGraph;
    private System.Windows.Forms.ToolStripSeparator menuFileRecentDocumentsSeparator;
    private System.Windows.Forms.ToolStripButton toolStripAutoAdjustColorRangeInterval;
    private System.Windows.Forms.ToolStripButton toolStripBottomPanelVisible;
    private System.Windows.Forms.ToolStripMenuItem menuTools;
    private System.Windows.Forms.ToolStripMenuItem menuToolsAddLapsFromWinSplits;
    private System.Windows.Forms.Panel sessionPanel;
    private System.Windows.Forms.CheckedListBox sessions;
    private System.Windows.Forms.ToolStripMenuItem menuFileImportSessions;
    private System.Windows.Forms.ToolStripMenuItem menuToolsPublishMap;
    private System.Windows.Forms.ToolStripMenuItem menuFileExport;
    private System.Windows.Forms.ToolStripMenuItem menuFileExportImage;
    private System.Windows.Forms.ToolStripMenuItem menuFileExportGPX;
    private System.Windows.Forms.ToolStripMenuItem menuToolsOpenInGoogleEarth;
    private System.Windows.Forms.ToolStripMenuItem menuFileExportKMZ;
    private System.Windows.Forms.ToolStripMenuItem menuFileExportRouteData;
    private System.Windows.Forms.ToolStripMenuItem menuToolsOpenMultipleFilesInGoogleEarth;
    private System.Windows.Forms.ToolStripButton toolStripOpenInGoogleEarth;
    private System.Windows.Forms.ToolStripSeparator tstToolsSeparator;
    private System.Windows.Forms.ToolStripButton toolStripPublishMap;
    private System.Windows.Forms.ToolStripButton toolStripFullScreen;
    private System.Windows.Forms.ToolStripMenuItem menuView;
    private System.Windows.Forms.ToolStripMenuItem menuViewFullScreen;
    private System.Windows.Forms.ToolStripMenuItem menuViewRightPanelVisible;
    private System.Windows.Forms.ToolStripMenuItem menuViewBottomPanelVisible;
    private System.Windows.Forms.ToolStripSeparator menuEditSeparator1;
    private System.Windows.Forms.ToolStripMenuItem menuEditChangeStartTime;
    private System.Windows.Forms.DataGridView laps;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.Panel momentaneousInfoPanel;
    private System.Windows.Forms.ToolStripLabel smoothingIntervalLengthLabel;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    private System.Windows.Forms.ToolStripLabel circleTimeRadiusLabel;
    private System.Windows.Forms.ToolStripTextBox circleTimeRadius;
    private System.Windows.Forms.ToolStripLabel routeLineWidthLabel;
  }
}