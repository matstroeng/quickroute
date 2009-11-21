namespace QuickRoute.UI.Forms
{
  partial class SelectRoutePropertyTypesForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectRoutePropertyTypesForm));
      this.ok = new System.Windows.Forms.Button();
      this.cancel = new System.Windows.Forms.Button();
      this.routePropertyTypeCheckboxList = new System.Windows.Forms.CheckedListBox();
      this.moveUp = new System.Windows.Forms.Button();
      this.moveDown = new System.Windows.Forms.Button();
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
      this.routePropertyTypeCheckboxList.SelectedIndexChanged += new System.EventHandler(this.routePropertyTypeCheckboxList_SelectedIndexChanged);
      this.routePropertyTypeCheckboxList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.routePropertyTypes_KeyDown);
      // 
      // moveUp
      // 
      this.moveUp.AccessibleDescription = null;
      this.moveUp.AccessibleName = null;
      resources.ApplyResources(this.moveUp, "moveUp");
      this.moveUp.BackgroundImage = null;
      this.moveUp.Font = null;
      this.moveUp.Name = "moveUp";
      this.moveUp.UseVisualStyleBackColor = true;
      this.moveUp.Click += new System.EventHandler(this.moveUp_Click);
      // 
      // moveDown
      // 
      this.moveDown.AccessibleDescription = null;
      this.moveDown.AccessibleName = null;
      resources.ApplyResources(this.moveDown, "moveDown");
      this.moveDown.BackgroundImage = null;
      this.moveDown.Font = null;
      this.moveDown.Name = "moveDown";
      this.moveDown.UseVisualStyleBackColor = true;
      this.moveDown.Click += new System.EventHandler(this.moveDown_Click);
      // 
      // SelectRoutePropertyTypesForm
      // 
      this.AcceptButton = this.ok;
      this.AccessibleDescription = null;
      this.AccessibleName = null;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = null;
      this.CancelButton = this.cancel;
      this.Controls.Add(this.moveDown);
      this.Controls.Add(this.moveUp);
      this.Controls.Add(this.routePropertyTypeCheckboxList);
      this.Controls.Add(this.cancel);
      this.Controls.Add(this.ok);
      this.Font = null;
      this.Icon = null;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "SelectRoutePropertyTypesForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button ok;
    private System.Windows.Forms.Button cancel;
    private System.Windows.Forms.CheckedListBox routePropertyTypeCheckboxList;
    private System.Windows.Forms.Button moveUp;
    private System.Windows.Forms.Button moveDown;
  }
}