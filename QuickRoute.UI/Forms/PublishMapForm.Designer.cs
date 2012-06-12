namespace QuickRoute.UI.Forms
{
  partial class PublishMapForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PublishMapForm));
      this.cancel = new System.Windows.Forms.Button();
      this.ok = new System.Windows.Forms.Button();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.date = new System.Windows.Forms.DateTimePicker();
      this.commentLabel = new System.Windows.Forms.Label();
      this.resultListUrlLabel = new System.Windows.Forms.Label();
      this.usernameLabel = new System.Windows.Forms.Label();
      this.webServiceLabel = new System.Windows.Forms.Label();
      this.webServiceURL = new System.Windows.Forms.ComboBox();
      this.username = new System.Windows.Forms.TextBox();
      this.passwordLabel = new System.Windows.Forms.Label();
      this.password = new System.Windows.Forms.TextBox();
      this.mapLabel = new System.Windows.Forms.Label();
      this.map = new System.Windows.Forms.ComboBox();
      this.dateLabel = new System.Windows.Forms.Label();
      this.nameLabel = new System.Windows.Forms.Label();
      this.mapNameLabel = new System.Windows.Forms.Label();
      this.organiserLabel = new System.Windows.Forms.Label();
      this.countryLabel = new System.Windows.Forms.Label();
      this.disciplineLabel = new System.Windows.Forms.Label();
      this.relayLegLabel = new System.Windows.Forms.Label();
      this.name = new System.Windows.Forms.TextBox();
      this.mapName = new System.Windows.Forms.TextBox();
      this.organiser = new System.Windows.Forms.TextBox();
      this.country = new System.Windows.Forms.TextBox();
      this.type = new System.Windows.Forms.TextBox();
      this.relayLeg = new System.Windows.Forms.TextBox();
      this.resultListUrl = new System.Windows.Forms.TextBox();
      this.comment = new System.Windows.Forms.TextBox();
      this.imageFormat = new System.Windows.Forms.ComboBox();
      this.imageFormatLabel = new System.Windows.Forms.Label();
      this.category = new System.Windows.Forms.ComboBox();
      this.categoryLabel = new System.Windows.Forms.Label();
      this.panel1 = new System.Windows.Forms.Panel();
      this.ConnectButton = new System.Windows.Forms.Button();
      this.SavePassword = new System.Windows.Forms.CheckBox();
      this.uiInformation = new System.Windows.Forms.LinkLabel();
      this.tableLayoutPanel1.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // cancel
      // 
      resources.ApplyResources(this.cancel, "cancel");
      this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancel.Name = "cancel";
      this.cancel.UseVisualStyleBackColor = true;
      this.cancel.Click += new System.EventHandler(this.cancel_Click);
      // 
      // ok
      // 
      resources.ApplyResources(this.ok, "ok");
      this.ok.Name = "ok";
      this.ok.UseVisualStyleBackColor = true;
      this.ok.Click += new System.EventHandler(this.ok_Click);
      // 
      // tableLayoutPanel1
      // 
      resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
      this.tableLayoutPanel1.Controls.Add(this.date, 1, 6);
      this.tableLayoutPanel1.Controls.Add(this.commentLabel, 0, 15);
      this.tableLayoutPanel1.Controls.Add(this.resultListUrlLabel, 0, 14);
      this.tableLayoutPanel1.Controls.Add(this.usernameLabel, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.webServiceLabel, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.webServiceURL, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.username, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.passwordLabel, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.password, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this.mapLabel, 0, 5);
      this.tableLayoutPanel1.Controls.Add(this.map, 1, 5);
      this.tableLayoutPanel1.Controls.Add(this.dateLabel, 0, 6);
      this.tableLayoutPanel1.Controls.Add(this.nameLabel, 0, 8);
      this.tableLayoutPanel1.Controls.Add(this.mapNameLabel, 0, 9);
      this.tableLayoutPanel1.Controls.Add(this.organiserLabel, 0, 10);
      this.tableLayoutPanel1.Controls.Add(this.countryLabel, 0, 11);
      this.tableLayoutPanel1.Controls.Add(this.disciplineLabel, 0, 12);
      this.tableLayoutPanel1.Controls.Add(this.relayLegLabel, 0, 13);
      this.tableLayoutPanel1.Controls.Add(this.name, 1, 8);
      this.tableLayoutPanel1.Controls.Add(this.mapName, 1, 9);
      this.tableLayoutPanel1.Controls.Add(this.organiser, 1, 10);
      this.tableLayoutPanel1.Controls.Add(this.country, 1, 11);
      this.tableLayoutPanel1.Controls.Add(this.type, 1, 12);
      this.tableLayoutPanel1.Controls.Add(this.relayLeg, 1, 13);
      this.tableLayoutPanel1.Controls.Add(this.resultListUrl, 1, 14);
      this.tableLayoutPanel1.Controls.Add(this.comment, 1, 15);
      this.tableLayoutPanel1.Controls.Add(this.imageFormat, 1, 16);
      this.tableLayoutPanel1.Controls.Add(this.imageFormatLabel, 0, 16);
      this.tableLayoutPanel1.Controls.Add(this.category, 1, 7);
      this.tableLayoutPanel1.Controls.Add(this.categoryLabel, 0, 7);
      this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 3);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      // 
      // date
      // 
      resources.ApplyResources(this.date, "date");
      this.date.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
      this.date.Name = "date";
      // 
      // commentLabel
      // 
      resources.ApplyResources(this.commentLabel, "commentLabel");
      this.commentLabel.Name = "commentLabel";
      // 
      // resultListUrlLabel
      // 
      resources.ApplyResources(this.resultListUrlLabel, "resultListUrlLabel");
      this.resultListUrlLabel.Name = "resultListUrlLabel";
      // 
      // usernameLabel
      // 
      resources.ApplyResources(this.usernameLabel, "usernameLabel");
      this.usernameLabel.Name = "usernameLabel";
      // 
      // webServiceLabel
      // 
      resources.ApplyResources(this.webServiceLabel, "webServiceLabel");
      this.webServiceLabel.Name = "webServiceLabel";
      // 
      // webServiceURL
      // 
      resources.ApplyResources(this.webServiceURL, "webServiceURL");
      this.webServiceURL.FormattingEnabled = true;
      this.webServiceURL.Name = "webServiceURL";
      this.webServiceURL.SelectedIndexChanged += new System.EventHandler(this.webServiceURL_SelectedIndexChanged);
      this.webServiceURL.Enter += new System.EventHandler(this.control_Enter);
      this.webServiceURL.Leave += new System.EventHandler(this.webServiceURL_Leave);
      // 
      // username
      // 
      resources.ApplyResources(this.username, "username");
      this.username.Name = "username";
      this.username.Enter += new System.EventHandler(this.control_Enter);
      // 
      // passwordLabel
      // 
      resources.ApplyResources(this.passwordLabel, "passwordLabel");
      this.passwordLabel.Name = "passwordLabel";
      // 
      // password
      // 
      resources.ApplyResources(this.password, "password");
      this.password.Name = "password";
      this.password.UseSystemPasswordChar = true;
      this.password.Enter += new System.EventHandler(this.control_Enter);
      // 
      // mapLabel
      // 
      resources.ApplyResources(this.mapLabel, "mapLabel");
      this.mapLabel.Name = "mapLabel";
      // 
      // map
      // 
      resources.ApplyResources(this.map, "map");
      this.map.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.map.FormattingEnabled = true;
      this.map.Name = "map";
      this.map.SelectedIndexChanged += new System.EventHandler(this.map_SelectedIndexChanged);
      this.map.Enter += new System.EventHandler(this.control_Enter);
      // 
      // dateLabel
      // 
      resources.ApplyResources(this.dateLabel, "dateLabel");
      this.dateLabel.Name = "dateLabel";
      // 
      // nameLabel
      // 
      resources.ApplyResources(this.nameLabel, "nameLabel");
      this.nameLabel.Name = "nameLabel";
      // 
      // mapNameLabel
      // 
      resources.ApplyResources(this.mapNameLabel, "mapNameLabel");
      this.mapNameLabel.Name = "mapNameLabel";
      // 
      // organiserLabel
      // 
      resources.ApplyResources(this.organiserLabel, "organiserLabel");
      this.organiserLabel.Name = "organiserLabel";
      // 
      // countryLabel
      // 
      resources.ApplyResources(this.countryLabel, "countryLabel");
      this.countryLabel.Name = "countryLabel";
      // 
      // disciplineLabel
      // 
      resources.ApplyResources(this.disciplineLabel, "disciplineLabel");
      this.disciplineLabel.Name = "disciplineLabel";
      // 
      // relayLegLabel
      // 
      resources.ApplyResources(this.relayLegLabel, "relayLegLabel");
      this.relayLegLabel.Name = "relayLegLabel";
      // 
      // name
      // 
      resources.ApplyResources(this.name, "name");
      this.name.Name = "name";
      this.name.Enter += new System.EventHandler(this.control_Enter);
      // 
      // mapName
      // 
      resources.ApplyResources(this.mapName, "mapName");
      this.mapName.Name = "mapName";
      this.mapName.Enter += new System.EventHandler(this.control_Enter);
      // 
      // organiser
      // 
      resources.ApplyResources(this.organiser, "organiser");
      this.organiser.Name = "organiser";
      this.organiser.Enter += new System.EventHandler(this.control_Enter);
      // 
      // country
      // 
      resources.ApplyResources(this.country, "country");
      this.country.Name = "country";
      this.country.Enter += new System.EventHandler(this.control_Enter);
      // 
      // type
      // 
      resources.ApplyResources(this.type, "type");
      this.type.Name = "type";
      this.type.Enter += new System.EventHandler(this.control_Enter);
      // 
      // relayLeg
      // 
      resources.ApplyResources(this.relayLeg, "relayLeg");
      this.relayLeg.Name = "relayLeg";
      this.relayLeg.Enter += new System.EventHandler(this.control_Enter);
      // 
      // resultListUrl
      // 
      resources.ApplyResources(this.resultListUrl, "resultListUrl");
      this.resultListUrl.Name = "resultListUrl";
      this.resultListUrl.Enter += new System.EventHandler(this.control_Enter);
      // 
      // comment
      // 
      resources.ApplyResources(this.comment, "comment");
      this.comment.Name = "comment";
      this.comment.Enter += new System.EventHandler(this.control_Enter);
      // 
      // imageFormat
      // 
      resources.ApplyResources(this.imageFormat, "imageFormat");
      this.imageFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.imageFormat.FormattingEnabled = true;
      this.imageFormat.Items.AddRange(new object[] {
            resources.GetString("imageFormat.Items"),
            resources.GetString("imageFormat.Items1")});
      this.imageFormat.Name = "imageFormat";
      this.imageFormat.Enter += new System.EventHandler(this.control_Enter);
      // 
      // imageFormatLabel
      // 
      resources.ApplyResources(this.imageFormatLabel, "imageFormatLabel");
      this.imageFormatLabel.Name = "imageFormatLabel";
      // 
      // category
      // 
      resources.ApplyResources(this.category, "category");
      this.category.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.category.FormattingEnabled = true;
      this.category.Name = "category";
      // 
      // categoryLabel
      // 
      resources.ApplyResources(this.categoryLabel, "categoryLabel");
      this.categoryLabel.Name = "categoryLabel";
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.ConnectButton);
      this.panel1.Controls.Add(this.SavePassword);
      resources.ApplyResources(this.panel1, "panel1");
      this.panel1.Name = "panel1";
      // 
      // ConnectButton
      // 
      resources.ApplyResources(this.ConnectButton, "ConnectButton");
      this.ConnectButton.Name = "ConnectButton";
      this.ConnectButton.UseVisualStyleBackColor = true;
      this.ConnectButton.Click += new System.EventHandler(this.connectButton_Click);
      // 
      // SavePassword
      // 
      resources.ApplyResources(this.SavePassword, "SavePassword");
      this.SavePassword.Name = "SavePassword";
      this.SavePassword.UseVisualStyleBackColor = true;
      // 
      // uiInformation
      // 
      resources.ApplyResources(this.uiInformation, "uiInformation");
      this.uiInformation.BackColor = System.Drawing.SystemColors.Info;
      this.uiInformation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.uiInformation.Name = "uiInformation";
      this.uiInformation.TabStop = true;
      this.uiInformation.UseCompatibleTextRendering = true;
      this.uiInformation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.uiInformation_LinkClicked);
      // 
      // PublishMapForm
      // 
      this.AcceptButton = this.ok;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancel;
      this.Controls.Add(this.uiInformation);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Controls.Add(this.ok);
      this.Controls.Add(this.cancel);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "PublishMapForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button cancel;
    private System.Windows.Forms.Button ok;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label usernameLabel;
    private System.Windows.Forms.Label webServiceLabel;
    private System.Windows.Forms.ComboBox webServiceURL;
    private System.Windows.Forms.TextBox username;
    private System.Windows.Forms.Label passwordLabel;
    private System.Windows.Forms.TextBox password;
    private System.Windows.Forms.Label mapLabel;
    private System.Windows.Forms.ComboBox map;
    private System.Windows.Forms.Label dateLabel;
    private System.Windows.Forms.Label nameLabel;
    private System.Windows.Forms.Label mapNameLabel;
    private System.Windows.Forms.Label organiserLabel;
    private System.Windows.Forms.Label countryLabel;
    private System.Windows.Forms.Label disciplineLabel;
    private System.Windows.Forms.Label commentLabel;
    private System.Windows.Forms.Label resultListUrlLabel;
    private System.Windows.Forms.Label relayLegLabel;
    private System.Windows.Forms.TextBox name;
    private System.Windows.Forms.TextBox mapName;
    private System.Windows.Forms.TextBox organiser;
    private System.Windows.Forms.TextBox country;
    private System.Windows.Forms.TextBox type;
    private System.Windows.Forms.TextBox relayLeg;
    private System.Windows.Forms.TextBox resultListUrl;
    private System.Windows.Forms.TextBox comment;
    private System.Windows.Forms.LinkLabel uiInformation;
    private System.Windows.Forms.DateTimePicker date;
    private System.Windows.Forms.ComboBox imageFormat;
    private System.Windows.Forms.Label imageFormatLabel;
    private System.Windows.Forms.ComboBox category;
    private System.Windows.Forms.Label categoryLabel;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button ConnectButton;
    private System.Windows.Forms.CheckBox SavePassword;
  }
}