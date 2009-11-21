namespace QuickRoute.UI.Forms
{
  partial class AddSessionsForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddSessionsForm));
      this.ok = new System.Windows.Forms.Button();
      this.cancel = new System.Windows.Forms.Button();
      this.addRouteGroup = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.routeFromGpsDevice = new System.Windows.Forms.RadioButton();
      this.routeFromFile = new System.Windows.Forms.RadioButton();
      this.routeFileFormatLabel = new System.Windows.Forms.Label();
      this.routeGpsDevice = new System.Windows.Forms.ComboBox();
      this.routeFileName = new System.Windows.Forms.TextBox();
      this.routeFileFormatComboBox = new System.Windows.Forms.ComboBox();
      this.routeFileNameBrowse = new System.Windows.Forms.Button();
      this.addSessionButton = new System.Windows.Forms.Button();
      this.sessionGrid = new System.Windows.Forms.DataGridView();
      this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.addRouteGroup.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.sessionGrid)).BeginInit();
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
      // addRouteGroup
      // 
      this.addRouteGroup.AccessibleDescription = null;
      this.addRouteGroup.AccessibleName = null;
      resources.ApplyResources(this.addRouteGroup, "addRouteGroup");
      this.addRouteGroup.BackgroundImage = null;
      this.addRouteGroup.Controls.Add(this.tableLayoutPanel1);
      this.addRouteGroup.Font = null;
      this.addRouteGroup.Name = "addRouteGroup";
      this.addRouteGroup.TabStop = false;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.AccessibleDescription = null;
      this.tableLayoutPanel1.AccessibleName = null;
      resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
      this.tableLayoutPanel1.BackgroundImage = null;
      this.tableLayoutPanel1.Controls.Add(this.routeFromGpsDevice, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.routeFromFile, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.routeFileFormatLabel, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.routeGpsDevice, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.routeFileName, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.routeFileFormatComboBox, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this.routeFileNameBrowse, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this.addSessionButton, 1, 3);
      this.tableLayoutPanel1.Font = null;
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      // 
      // routeFromGpsDevice
      // 
      this.routeFromGpsDevice.AccessibleDescription = null;
      this.routeFromGpsDevice.AccessibleName = null;
      resources.ApplyResources(this.routeFromGpsDevice, "routeFromGpsDevice");
      this.routeFromGpsDevice.BackgroundImage = null;
      this.routeFromGpsDevice.Checked = true;
      this.routeFromGpsDevice.Font = null;
      this.routeFromGpsDevice.Name = "routeFromGpsDevice";
      this.routeFromGpsDevice.TabStop = true;
      this.routeFromGpsDevice.UseVisualStyleBackColor = true;
      this.routeFromGpsDevice.CheckedChanged += new System.EventHandler(this.routeFromGpsDevice_CheckedChanged);
      // 
      // routeFromFile
      // 
      this.routeFromFile.AccessibleDescription = null;
      this.routeFromFile.AccessibleName = null;
      resources.ApplyResources(this.routeFromFile, "routeFromFile");
      this.routeFromFile.BackgroundImage = null;
      this.routeFromFile.Font = null;
      this.routeFromFile.Name = "routeFromFile";
      this.routeFromFile.UseVisualStyleBackColor = true;
      this.routeFromFile.CheckedChanged += new System.EventHandler(this.routeFromFile_CheckedChanged);
      // 
      // routeFileFormatLabel
      // 
      this.routeFileFormatLabel.AccessibleDescription = null;
      this.routeFileFormatLabel.AccessibleName = null;
      resources.ApplyResources(this.routeFileFormatLabel, "routeFileFormatLabel");
      this.routeFileFormatLabel.Font = null;
      this.routeFileFormatLabel.Name = "routeFileFormatLabel";
      // 
      // routeGpsDevice
      // 
      this.routeGpsDevice.AccessibleDescription = null;
      this.routeGpsDevice.AccessibleName = null;
      resources.ApplyResources(this.routeGpsDevice, "routeGpsDevice");
      this.routeGpsDevice.BackgroundImage = null;
      this.routeGpsDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.routeGpsDevice.Font = null;
      this.routeGpsDevice.FormattingEnabled = true;
      this.routeGpsDevice.Name = "routeGpsDevice";
      // 
      // routeFileName
      // 
      this.routeFileName.AccessibleDescription = null;
      this.routeFileName.AccessibleName = null;
      resources.ApplyResources(this.routeFileName, "routeFileName");
      this.routeFileName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
      this.routeFileName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
      this.routeFileName.BackgroundImage = null;
      this.routeFileName.Font = null;
      this.routeFileName.Name = "routeFileName";
      this.routeFileName.TextChanged += new System.EventHandler(this.routeFileName_TextChanged);
      // 
      // routeFileFormatComboBox
      // 
      this.routeFileFormatComboBox.AccessibleDescription = null;
      this.routeFileFormatComboBox.AccessibleName = null;
      resources.ApplyResources(this.routeFileFormatComboBox, "routeFileFormatComboBox");
      this.routeFileFormatComboBox.BackgroundImage = null;
      this.routeFileFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.routeFileFormatComboBox.Font = null;
      this.routeFileFormatComboBox.FormattingEnabled = true;
      this.routeFileFormatComboBox.Name = "routeFileFormatComboBox";
      // 
      // routeFileNameBrowse
      // 
      this.routeFileNameBrowse.AccessibleDescription = null;
      this.routeFileNameBrowse.AccessibleName = null;
      resources.ApplyResources(this.routeFileNameBrowse, "routeFileNameBrowse");
      this.routeFileNameBrowse.BackgroundImage = null;
      this.routeFileNameBrowse.Font = null;
      this.routeFileNameBrowse.Name = "routeFileNameBrowse";
      this.routeFileNameBrowse.UseVisualStyleBackColor = true;
      this.routeFileNameBrowse.Click += new System.EventHandler(this.routeFileNameBrowse_Click);
      // 
      // addSessionButton
      // 
      this.addSessionButton.AccessibleDescription = null;
      this.addSessionButton.AccessibleName = null;
      resources.ApplyResources(this.addSessionButton, "addSessionButton");
      this.addSessionButton.BackgroundImage = null;
      this.addSessionButton.Font = null;
      this.addSessionButton.Name = "addSessionButton";
      this.addSessionButton.UseVisualStyleBackColor = true;
      this.addSessionButton.Click += new System.EventHandler(this.addSessionButton_Click);
      // 
      // sessionGrid
      // 
      this.sessionGrid.AccessibleDescription = null;
      this.sessionGrid.AccessibleName = null;
      this.sessionGrid.AllowUserToAddRows = false;
      this.sessionGrid.AllowUserToDeleteRows = false;
      resources.ApplyResources(this.sessionGrid, "sessionGrid");
      this.sessionGrid.BackgroundImage = null;
      this.sessionGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.sessionGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameColumn});
      this.sessionGrid.Font = null;
      this.sessionGrid.Name = "sessionGrid";
      this.sessionGrid.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.sessionGrid_CellFormatting);
      // 
      // NameColumn
      // 
      this.NameColumn.DataPropertyName = "Name";
      resources.ApplyResources(this.NameColumn, "NameColumn");
      this.NameColumn.Name = "NameColumn";
      // 
      // AddSessionsForm
      // 
      this.AcceptButton = this.ok;
      this.AccessibleDescription = null;
      this.AccessibleName = null;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = null;
      this.CancelButton = this.cancel;
      this.Controls.Add(this.sessionGrid);
      this.Controls.Add(this.cancel);
      this.Controls.Add(this.addRouteGroup);
      this.Controls.Add(this.ok);
      this.Font = null;
      this.Icon = null;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AddSessionsForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.addRouteGroup.ResumeLayout(false);
      this.addRouteGroup.PerformLayout();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.sessionGrid)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button ok;
    private System.Windows.Forms.Button cancel;
    private System.Windows.Forms.GroupBox addRouteGroup;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.RadioButton routeFromGpsDevice;
    private System.Windows.Forms.RadioButton routeFromFile;
    private System.Windows.Forms.TextBox routeFileName;
    private System.Windows.Forms.Button routeFileNameBrowse;
    private System.Windows.Forms.ComboBox routeFileFormatComboBox;
    private System.Windows.Forms.Label routeFileFormatLabel;
    private System.Windows.Forms.ComboBox routeGpsDevice;
    private System.Windows.Forms.Button addSessionButton;
    private System.Windows.Forms.DataGridView sessionGrid;
    private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
  }
}