namespace QuickRoute.PropertyControls
{
  partial class GradientEditorControl
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.uiInformation = new System.Windows.Forms.Label();
      this.gradientNameTextbox = new System.Windows.Forms.TextBox();
      this.gradientNameLabel = new System.Windows.Forms.Label();
      this.gradientPanel = new System.Windows.Forms.Panel();
      this.SuspendLayout();
      // 
      // uiInformation
      // 
      this.uiInformation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.uiInformation.BackColor = System.Drawing.SystemColors.Info;
      this.uiInformation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.uiInformation.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.uiInformation.Location = new System.Drawing.Point(0, 189);
      this.uiInformation.Name = "uiInformation";
      this.uiInformation.Padding = new System.Windows.Forms.Padding(4);
      this.uiInformation.Size = new System.Drawing.Size(297, 63);
      this.uiInformation.TabIndex = 7;
      this.uiInformation.Text = "Click anywhere in the gradient area to add a color marker. Drag a color marker to" +
          " move its position. Double click a color marker to change its color. Right-click" +
          " a marker to delete it.";
      // 
      // gradientNameTextbox
      // 
      this.gradientNameTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.gradientNameTextbox.Location = new System.Drawing.Point(0, 16);
      this.gradientNameTextbox.Name = "gradientNameTextbox";
      this.gradientNameTextbox.Size = new System.Drawing.Size(297, 20);
      this.gradientNameTextbox.TabIndex = 5;
      this.gradientNameTextbox.TextChanged += new System.EventHandler(gradientNameTextbox_TextChanged);
      this.gradientNameTextbox.Enter += new System.EventHandler(gradientNameTextbox_Enter);
      // 
      // gradientNameLabel
      // 
      this.gradientNameLabel.AutoSize = true;
      this.gradientNameLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.gradientNameLabel.Location = new System.Drawing.Point(-3, 0);
      this.gradientNameLabel.Name = "gradientNameLabel";
      this.gradientNameLabel.Size = new System.Drawing.Size(38, 13);
      this.gradientNameLabel.TabIndex = 4;
      this.gradientNameLabel.Text = "&Name:";
      // 
      // gradientPanel
      // 
      this.gradientPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.gradientPanel.Location = new System.Drawing.Point(0, 59);
      this.gradientPanel.Name = "gradientPanel";
      this.gradientPanel.Size = new System.Drawing.Size(297, 68);
      this.gradientPanel.TabIndex = 6;
      this.gradientPanel.Resize += new System.EventHandler(gradientPanel_Resize);
      this.gradientPanel.Paint += new System.Windows.Forms.PaintEventHandler(gradientPanel_Paint);
      this.gradientPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(gradientPanel_MouseMove);
      this.gradientPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(gradientPanel_MouseDown);
      this.gradientPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(gradientPanel_MouseUp);
      this.gradientPanel.MouseLeave += new System.EventHandler(gradientPanel_MouseLeave);
      this.gradientPanel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(gradientPanel_MouseDoubleClick);
      // 
      // GradientEditorControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.uiInformation);
      this.Controls.Add(this.gradientNameTextbox);
      this.Controls.Add(this.gradientNameLabel);
      this.Controls.Add(this.gradientPanel);
      this.Name = "GradientEditorControl";
      this.Size = new System.Drawing.Size(297, 252);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label uiInformation;
    private System.Windows.Forms.TextBox gradientNameTextbox;
    private System.Windows.Forms.Label gradientNameLabel;
    private System.Windows.Forms.Panel gradientPanel;

  }
}
