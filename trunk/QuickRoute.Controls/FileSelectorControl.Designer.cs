namespace QuickRoute.Controls
{
  partial class FileSelectorControl
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileSelectorControl));
      this.files = new System.Windows.Forms.ListBox();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.addFileButton = new System.Windows.Forms.Button();
      this.addUrlButton = new System.Windows.Forms.Button();
      this.removeButton = new System.Windows.Forms.Button();
      this.flowLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // files
      // 
      this.files.AccessibleDescription = null;
      this.files.AccessibleName = null;
      this.files.AllowDrop = true;
      resources.ApplyResources(this.files, "files");
      this.files.BackgroundImage = null;
      this.files.Font = null;
      this.files.FormattingEnabled = true;
      this.files.Name = "files";
      this.files.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
      this.files.DragDrop += new System.Windows.Forms.DragEventHandler(this.files_DragDrop);
      this.files.DragEnter += new System.Windows.Forms.DragEventHandler(this.files_DragEnter);
      this.files.KeyDown += new System.Windows.Forms.KeyEventHandler(this.files_KeyDown);
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.AccessibleDescription = null;
      this.flowLayoutPanel1.AccessibleName = null;
      resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
      this.flowLayoutPanel1.BackgroundImage = null;
      this.flowLayoutPanel1.Controls.Add(this.addFileButton);
      this.flowLayoutPanel1.Controls.Add(this.addUrlButton);
      this.flowLayoutPanel1.Controls.Add(this.removeButton);
      this.flowLayoutPanel1.Font = null;
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      // 
      // addFileButton
      // 
      this.addFileButton.AccessibleDescription = null;
      this.addFileButton.AccessibleName = null;
      resources.ApplyResources(this.addFileButton, "addFileButton");
      this.addFileButton.BackgroundImage = null;
      this.addFileButton.Font = null;
      this.addFileButton.Name = "addFileButton";
      this.addFileButton.UseVisualStyleBackColor = true;
      this.addFileButton.Click += new System.EventHandler(this.addFileButton_Click);
      // 
      // addUrlButton
      // 
      this.addUrlButton.AccessibleDescription = null;
      this.addUrlButton.AccessibleName = null;
      resources.ApplyResources(this.addUrlButton, "addUrlButton");
      this.addUrlButton.BackgroundImage = null;
      this.addUrlButton.Font = null;
      this.addUrlButton.Name = "addUrlButton";
      this.addUrlButton.UseVisualStyleBackColor = true;
      this.addUrlButton.Click += new System.EventHandler(this.AddUrlButton_Click);
      // 
      // removeButton
      // 
      this.removeButton.AccessibleDescription = null;
      this.removeButton.AccessibleName = null;
      resources.ApplyResources(this.removeButton, "removeButton");
      this.removeButton.BackgroundImage = null;
      this.removeButton.Font = null;
      this.removeButton.Name = "removeButton";
      this.removeButton.UseVisualStyleBackColor = true;
      this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
      // 
      // FileSelectorControl
      // 
      this.AccessibleDescription = null;
      this.AccessibleName = null;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = null;
      this.Controls.Add(this.flowLayoutPanel1);
      this.Controls.Add(this.files);
      this.Font = null;
      this.Name = "FileSelectorControl";
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListBox files;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.Button addFileButton;
    private System.Windows.Forms.Button addUrlButton;
    private System.Windows.Forms.Button removeButton;
  }
}
