namespace QuickRoute.UI.Forms
{
  partial class GradientEditor
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GradientEditor));
      this.ok = new System.Windows.Forms.Button();
      this.cancel = new System.Windows.Forms.Button();
      this.availableGradients = new System.Windows.Forms.GroupBox();
      this.deleteGradient = new System.Windows.Forms.Button();
      this.addGradient = new System.Windows.Forms.Button();
      this.gradientList = new QuickRoute.Controls.GradientListBox();
      this.gradientProperties = new System.Windows.Forms.GroupBox();
      this.uiInformation = new System.Windows.Forms.Label();
      this.gradientNameTextbox = new System.Windows.Forms.TextBox();
      this.gradientNameLabel = new System.Windows.Forms.Label();
      this.gradientPanel = new System.Windows.Forms.Panel();
      this.availableGradients.SuspendLayout();
      this.gradientProperties.SuspendLayout();
      this.SuspendLayout();
      // 
      // ok
      // 
      resources.ApplyResources(this.ok, "ok");
      this.ok.Name = "ok";
      this.ok.UseVisualStyleBackColor = true;
      this.ok.Click += new System.EventHandler(this.OK_Click);
      // 
      // cancel
      // 
      resources.ApplyResources(this.cancel, "cancel");
      this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancel.Name = "cancel";
      this.cancel.UseVisualStyleBackColor = true;
      this.cancel.Click += new System.EventHandler(this.Cancel_Click);
      // 
      // availableGradients
      // 
      resources.ApplyResources(this.availableGradients, "availableGradients");
      this.availableGradients.Controls.Add(this.deleteGradient);
      this.availableGradients.Controls.Add(this.addGradient);
      this.availableGradients.Controls.Add(this.gradientList);
      this.availableGradients.Name = "availableGradients";
      this.availableGradients.TabStop = false;
      // 
      // deleteGradient
      // 
      resources.ApplyResources(this.deleteGradient, "deleteGradient");
      this.deleteGradient.Name = "deleteGradient";
      this.deleteGradient.UseVisualStyleBackColor = true;
      this.deleteGradient.Click += new System.EventHandler(this.DeleteGradient_Click);
      // 
      // addGradient
      // 
      resources.ApplyResources(this.addGradient, "addGradient");
      this.addGradient.Name = "addGradient";
      this.addGradient.UseVisualStyleBackColor = true;
      this.addGradient.Click += new System.EventHandler(this.AddGradient_Click);
      // 
      // gradientList
      // 
      resources.ApplyResources(this.gradientList, "gradientList");
      this.gradientList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this.gradientList.FormattingEnabled = true;
      this.gradientList.Name = "gradientList";
      this.gradientList.NamePadding = 8;
      this.gradientList.NameWidth = 100;
      this.gradientList.SelectedIndexChanged += new System.EventHandler(this.GradientList_SelectedIndexChanged);
      // 
      // gradientProperties
      // 
      resources.ApplyResources(this.gradientProperties, "gradientProperties");
      this.gradientProperties.Controls.Add(this.uiInformation);
      this.gradientProperties.Controls.Add(this.gradientNameTextbox);
      this.gradientProperties.Controls.Add(this.gradientNameLabel);
      this.gradientProperties.Controls.Add(this.gradientPanel);
      this.gradientProperties.Name = "gradientProperties";
      this.gradientProperties.TabStop = false;
      // 
      // uiInformation
      // 
      resources.ApplyResources(this.uiInformation, "uiInformation");
      this.uiInformation.BackColor = System.Drawing.SystemColors.Info;
      this.uiInformation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.uiInformation.Name = "uiInformation";
      // 
      // gradientNameTextbox
      // 
      resources.ApplyResources(this.gradientNameTextbox, "gradientNameTextbox");
      this.gradientNameTextbox.Name = "gradientNameTextbox";
      this.gradientNameTextbox.TextChanged += new System.EventHandler(this.GradientNameTextbox_TextChanged);
      this.gradientNameTextbox.Enter += new System.EventHandler(this.GradientNameTextbox_Enter);
      // 
      // gradientNameLabel
      // 
      resources.ApplyResources(this.gradientNameLabel, "gradientNameLabel");
      this.gradientNameLabel.Name = "gradientNameLabel";
      // 
      // gradientPanel
      // 
      resources.ApplyResources(this.gradientPanel, "gradientPanel");
      this.gradientPanel.Name = "gradientPanel";
      this.gradientPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.GradientPanel_Paint);
      this.gradientPanel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.GradientPanel_MouseDoubleClick);
      this.gradientPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GradientPanel_MouseDown);
      this.gradientPanel.MouseLeave += new System.EventHandler(this.GradientPanel_MouseLeave);
      this.gradientPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GradientPanel_MouseMove);
      this.gradientPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GradientPanel_MouseUp);
      this.gradientPanel.Resize += new System.EventHandler(this.GradientPanel_Resize);
      // 
      // GradientEditor
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.gradientProperties);
      this.Controls.Add(this.availableGradients);
      this.Controls.Add(this.ok);
      this.Controls.Add(this.cancel);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "GradientEditor";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Activated += new System.EventHandler(this.GradientEditor_Activated);
      this.availableGradients.ResumeLayout(false);
      this.gradientProperties.ResumeLayout(false);
      this.gradientProperties.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button ok;
    private System.Windows.Forms.Button cancel;
    private System.Windows.Forms.GroupBox availableGradients;
    private System.Windows.Forms.GroupBox gradientProperties;
    private System.Windows.Forms.TextBox gradientNameTextbox;
    private System.Windows.Forms.Label gradientNameLabel;
    private System.Windows.Forms.Panel gradientPanel;
    private System.Windows.Forms.Button deleteGradient;
    private System.Windows.Forms.Button addGradient;
    private QuickRoute.Controls.GradientListBox gradientList;
    private System.Windows.Forms.Label uiInformation;
  }
}