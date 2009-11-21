namespace QuickRoute.UI.Forms
{
  partial class CreateNewForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateNewForm));
      this.ok = new System.Windows.Forms.Button();
      this.cancel = new System.Windows.Forms.Button();
      this.mapImageGroup = new System.Windows.Forms.GroupBox();
      this.mapImageLayoutTable = new System.Windows.Forms.TableLayoutPanel();
      this.mapImageFileName = new System.Windows.Forms.ComboBox();
      this.mapImageFileNameBrowse = new System.Windows.Forms.Button();
      this.mapImageFromFile = new System.Windows.Forms.RadioButton();
      this.mapImageBlank = new System.Windows.Forms.RadioButton();
      this.mapImageFileFormatLabel = new System.Windows.Forms.Label();
      this.mapImageFileFormatComboBox = new System.Windows.Forms.ComboBox();
      this.routeGroup = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.routeFromGpsDevice = new System.Windows.Forms.RadioButton();
      this.routeFromFile = new System.Windows.Forms.RadioButton();
      this.routeFileFormatLabel = new System.Windows.Forms.Label();
      this.routeGpsDevice = new System.Windows.Forms.ComboBox();
      this.routeFileFormatComboBox = new System.Windows.Forms.ComboBox();
      this.routeFileNameBrowse = new System.Windows.Forms.Button();
      this.routeFileName = new System.Windows.Forms.ComboBox();
      this.mapImageUrl = new System.Windows.Forms.TextBox();
      this.mapImageFromUrl = new System.Windows.Forms.RadioButton();
      this.personGroup = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.persons = new System.Windows.Forms.ComboBox();
      this.personNameLabel = new System.Windows.Forms.Label();
      this.mapImageGroup.SuspendLayout();
      this.mapImageLayoutTable.SuspendLayout();
      this.routeGroup.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this.personGroup.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
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
      // mapImageGroup
      // 
      resources.ApplyResources(this.mapImageGroup, "mapImageGroup");
      this.mapImageGroup.Controls.Add(this.mapImageLayoutTable);
      this.mapImageGroup.Name = "mapImageGroup";
      this.mapImageGroup.TabStop = false;
      // 
      // mapImageLayoutTable
      // 
      resources.ApplyResources(this.mapImageLayoutTable, "mapImageLayoutTable");
      this.mapImageLayoutTable.Controls.Add(this.mapImageFileName, 1, 0);
      this.mapImageLayoutTable.Controls.Add(this.mapImageFileNameBrowse, 2, 0);
      this.mapImageLayoutTable.Controls.Add(this.mapImageFromFile, 0, 0);
      this.mapImageLayoutTable.Controls.Add(this.mapImageBlank, 0, 2);
      this.mapImageLayoutTable.Controls.Add(this.mapImageFileFormatLabel, 0, 1);
      this.mapImageLayoutTable.Controls.Add(this.mapImageFileFormatComboBox, 1, 1);
      this.mapImageLayoutTable.Name = "mapImageLayoutTable";
      // 
      // mapImageFileName
      // 
      resources.ApplyResources(this.mapImageFileName, "mapImageFileName");
      this.mapImageFileName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
      this.mapImageFileName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
      this.mapImageFileName.FormattingEnabled = true;
      this.mapImageFileName.Name = "mapImageFileName";
      this.mapImageFileName.TextChanged += new System.EventHandler(this.mapImageFileName_TextChanged);
      // 
      // mapImageFileNameBrowse
      // 
      resources.ApplyResources(this.mapImageFileNameBrowse, "mapImageFileNameBrowse");
      this.mapImageFileNameBrowse.Name = "mapImageFileNameBrowse";
      this.mapImageFileNameBrowse.UseVisualStyleBackColor = true;
      this.mapImageFileNameBrowse.Click += new System.EventHandler(this.mapImageFileNameBrowse_Click);
      // 
      // mapImageFromFile
      // 
      resources.ApplyResources(this.mapImageFromFile, "mapImageFromFile");
      this.mapImageFromFile.Checked = true;
      this.mapImageFromFile.Name = "mapImageFromFile";
      this.mapImageFromFile.TabStop = true;
      this.mapImageFromFile.UseVisualStyleBackColor = true;
      this.mapImageFromFile.CheckedChanged += new System.EventHandler(this.mapImageFromFile_CheckedChanged);
      // 
      // mapImageBlank
      // 
      resources.ApplyResources(this.mapImageBlank, "mapImageBlank");
      this.mapImageBlank.Name = "mapImageBlank";
      this.mapImageBlank.UseVisualStyleBackColor = true;
      // 
      // mapImageFileFormatLabel
      // 
      resources.ApplyResources(this.mapImageFileFormatLabel, "mapImageFileFormatLabel");
      this.mapImageFileFormatLabel.Name = "mapImageFileFormatLabel";
      // 
      // mapImageFileFormatComboBox
      // 
      resources.ApplyResources(this.mapImageFileFormatComboBox, "mapImageFileFormatComboBox");
      this.mapImageFileFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.mapImageFileFormatComboBox.FormattingEnabled = true;
      this.mapImageFileFormatComboBox.Name = "mapImageFileFormatComboBox";
      // 
      // routeGroup
      // 
      resources.ApplyResources(this.routeGroup, "routeGroup");
      this.routeGroup.Controls.Add(this.tableLayoutPanel1);
      this.routeGroup.Name = "routeGroup";
      this.routeGroup.TabStop = false;
      // 
      // tableLayoutPanel1
      // 
      resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
      this.tableLayoutPanel1.Controls.Add(this.routeFromGpsDevice, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.routeFromFile, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.routeFileFormatLabel, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.routeGpsDevice, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.routeFileFormatComboBox, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this.routeFileNameBrowse, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this.routeFileName, 1, 1);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      // 
      // routeFromGpsDevice
      // 
      resources.ApplyResources(this.routeFromGpsDevice, "routeFromGpsDevice");
      this.routeFromGpsDevice.Checked = true;
      this.routeFromGpsDevice.Name = "routeFromGpsDevice";
      this.routeFromGpsDevice.TabStop = true;
      this.routeFromGpsDevice.UseVisualStyleBackColor = true;
      this.routeFromGpsDevice.CheckedChanged += new System.EventHandler(this.routeFromGpsDevice_CheckedChanged);
      // 
      // routeFromFile
      // 
      resources.ApplyResources(this.routeFromFile, "routeFromFile");
      this.routeFromFile.Name = "routeFromFile";
      this.routeFromFile.UseVisualStyleBackColor = true;
      this.routeFromFile.CheckedChanged += new System.EventHandler(this.routeFromFile_CheckedChanged);
      // 
      // routeFileFormatLabel
      // 
      resources.ApplyResources(this.routeFileFormatLabel, "routeFileFormatLabel");
      this.routeFileFormatLabel.Name = "routeFileFormatLabel";
      // 
      // routeGpsDevice
      // 
      resources.ApplyResources(this.routeGpsDevice, "routeGpsDevice");
      this.routeGpsDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.routeGpsDevice.FormattingEnabled = true;
      this.routeGpsDevice.Name = "routeGpsDevice";
      // 
      // routeFileFormatComboBox
      // 
      resources.ApplyResources(this.routeFileFormatComboBox, "routeFileFormatComboBox");
      this.routeFileFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.routeFileFormatComboBox.FormattingEnabled = true;
      this.routeFileFormatComboBox.Name = "routeFileFormatComboBox";
      // 
      // routeFileNameBrowse
      // 
      resources.ApplyResources(this.routeFileNameBrowse, "routeFileNameBrowse");
      this.routeFileNameBrowse.Name = "routeFileNameBrowse";
      this.routeFileNameBrowse.UseVisualStyleBackColor = true;
      this.routeFileNameBrowse.Click += new System.EventHandler(this.routeFileNameBrowse_Click);
      // 
      // routeFileName
      // 
      resources.ApplyResources(this.routeFileName, "routeFileName");
      this.routeFileName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
      this.routeFileName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
      this.routeFileName.FormattingEnabled = true;
      this.routeFileName.Name = "routeFileName";
      this.routeFileName.TextChanged += new System.EventHandler(this.routeFileName_TextChanged);
      // 
      // mapImageUrl
      // 
      resources.ApplyResources(this.mapImageUrl, "mapImageUrl");
      this.mapImageUrl.Name = "mapImageUrl";
      // 
      // mapImageFromUrl
      // 
      resources.ApplyResources(this.mapImageFromUrl, "mapImageFromUrl");
      this.mapImageFromUrl.Name = "mapImageFromUrl";
      this.mapImageFromUrl.UseVisualStyleBackColor = true;
      this.mapImageFromUrl.ClientSizeChanged += new System.EventHandler(this.mapImageFromUrl_CheckedChanged);
      // 
      // personGroup
      // 
      resources.ApplyResources(this.personGroup, "personGroup");
      this.personGroup.Controls.Add(this.tableLayoutPanel2);
      this.personGroup.Name = "personGroup";
      this.personGroup.TabStop = false;
      // 
      // tableLayoutPanel2
      // 
      resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
      this.tableLayoutPanel2.Controls.Add(this.persons, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.personNameLabel, 0, 0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      // 
      // persons
      // 
      resources.ApplyResources(this.persons, "persons");
      this.persons.FormattingEnabled = true;
      this.persons.Name = "persons";
      // 
      // personNameLabel
      // 
      resources.ApplyResources(this.personNameLabel, "personNameLabel");
      this.personNameLabel.Name = "personNameLabel";
      // 
      // CreateNewForm
      // 
      this.AcceptButton = this.ok;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancel;
      this.Controls.Add(this.personGroup);
      this.Controls.Add(this.mapImageUrl);
      this.Controls.Add(this.mapImageFromUrl);
      this.Controls.Add(this.mapImageGroup);
      this.Controls.Add(this.cancel);
      this.Controls.Add(this.routeGroup);
      this.Controls.Add(this.ok);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "CreateNewForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.mapImageGroup.ResumeLayout(false);
      this.mapImageLayoutTable.ResumeLayout(false);
      this.mapImageLayoutTable.PerformLayout();
      this.routeGroup.ResumeLayout(false);
      this.routeGroup.PerformLayout();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.personGroup.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button ok;
    private System.Windows.Forms.Button cancel;
    private System.Windows.Forms.GroupBox mapImageGroup;
    private System.Windows.Forms.TableLayoutPanel mapImageLayoutTable;
    private System.Windows.Forms.RadioButton mapImageFromFile;
    private System.Windows.Forms.GroupBox routeGroup;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.RadioButton routeFromGpsDevice;
    private System.Windows.Forms.RadioButton routeFromFile;
    private System.Windows.Forms.Button routeFileNameBrowse;
    private System.Windows.Forms.ComboBox routeFileFormatComboBox;
    private System.Windows.Forms.Label routeFileFormatLabel;
    private System.Windows.Forms.ComboBox routeGpsDevice;
    private System.Windows.Forms.Button mapImageFileNameBrowse;
    private System.Windows.Forms.TextBox mapImageUrl;
    private System.Windows.Forms.RadioButton mapImageFromUrl;
    private System.Windows.Forms.RadioButton mapImageBlank;
    private System.Windows.Forms.Label mapImageFileFormatLabel;
    private System.Windows.Forms.ComboBox mapImageFileFormatComboBox;
    private System.Windows.Forms.ComboBox mapImageFileName;
    private System.Windows.Forms.ComboBox routeFileName;
    private System.Windows.Forms.GroupBox personGroup;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Label personNameLabel;
    private System.Windows.Forms.ComboBox persons;
  }
}