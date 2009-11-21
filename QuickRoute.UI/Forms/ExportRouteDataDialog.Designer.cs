using System.Windows.Forms;

namespace QuickRoute.UI.Forms
{
  partial class ExportRouteDataDialog
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportRouteDataDialog));
      this.cancel = new System.Windows.Forms.Button();
      this.ok = new System.Windows.Forms.Button();
      this.routePropertyTypeCheckboxList = new System.Windows.Forms.CheckedListBox();
      this.dataToInclude = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.samplingIntervalDropdown = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
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
      // routePropertyTypeCheckboxList
      // 
      this.routePropertyTypeCheckboxList.AccessibleDescription = null;
      this.routePropertyTypeCheckboxList.AccessibleName = null;
      resources.ApplyResources(this.routePropertyTypeCheckboxList, "routePropertyTypeCheckboxList");
      this.routePropertyTypeCheckboxList.BackgroundImage = null;
      this.routePropertyTypeCheckboxList.CheckOnClick = true;
      this.routePropertyTypeCheckboxList.Font = null;
      this.routePropertyTypeCheckboxList.FormattingEnabled = true;
      this.routePropertyTypeCheckboxList.Name = "routePropertyTypeCheckboxList";
      // 
      // dataToInclude
      // 
      this.dataToInclude.AccessibleDescription = null;
      this.dataToInclude.AccessibleName = null;
      resources.ApplyResources(this.dataToInclude, "dataToInclude");
      this.dataToInclude.Font = null;
      this.dataToInclude.Name = "dataToInclude";
      // 
      // label1
      // 
      this.label1.AccessibleDescription = null;
      this.label1.AccessibleName = null;
      resources.ApplyResources(this.label1, "label1");
      this.label1.Font = null;
      this.label1.Name = "label1";
      // 
      // samplingIntervalDropdown
      // 
      this.samplingIntervalDropdown.AccessibleDescription = null;
      this.samplingIntervalDropdown.AccessibleName = null;
      resources.ApplyResources(this.samplingIntervalDropdown, "samplingIntervalDropdown");
      this.samplingIntervalDropdown.BackgroundImage = null;
      this.samplingIntervalDropdown.Font = null;
      this.samplingIntervalDropdown.FormattingEnabled = true;
      this.samplingIntervalDropdown.Items.AddRange(new object[] {
            resources.GetString("samplingIntervalDropdown.Items"),
            resources.GetString("samplingIntervalDropdown.Items1"),
            resources.GetString("samplingIntervalDropdown.Items2"),
            resources.GetString("samplingIntervalDropdown.Items3"),
            resources.GetString("samplingIntervalDropdown.Items4"),
            resources.GetString("samplingIntervalDropdown.Items5"),
            resources.GetString("samplingIntervalDropdown.Items6")});
      this.samplingIntervalDropdown.Name = "samplingIntervalDropdown";
      this.samplingIntervalDropdown.Leave += new System.EventHandler(this.samplingIntervalDropdown_Leave);
      // 
      // ExportRouteDataDialog
      // 
      this.AcceptButton = this.ok;
      this.AccessibleDescription = null;
      this.AccessibleName = null;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = null;
      this.CancelButton = this.cancel;
      this.Controls.Add(this.samplingIntervalDropdown);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.dataToInclude);
      this.Controls.Add(this.routePropertyTypeCheckboxList);
      this.Controls.Add(this.cancel);
      this.Controls.Add(this.ok);
      this.Font = null;
      this.Icon = null;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ExportRouteDataDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button cancel;
    private System.Windows.Forms.Button ok;
    private System.Windows.Forms.CheckedListBox routePropertyTypeCheckboxList;
    private System.Windows.Forms.Label dataToInclude;
    private Label label1;
    private ComboBox samplingIntervalDropdown;

  }
}