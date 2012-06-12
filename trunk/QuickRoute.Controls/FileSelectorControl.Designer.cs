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
      resources.ApplyResources(this.files, "files");
      this.files.AllowDrop = true;
      this.files.FormattingEnabled = true;
      this.files.Name = "files";
      this.files.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
      this.files.DragDrop += new System.Windows.Forms.DragEventHandler(this.files_DragDrop);
      this.files.DragEnter += new System.Windows.Forms.DragEventHandler(this.files_DragEnter);
      this.files.KeyDown += new System.Windows.Forms.KeyEventHandler(this.files_KeyDown);
      // 
      // flowLayoutPanel1
      // 
      resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
      this.flowLayoutPanel1.Controls.Add(this.addUrlButton);
      this.flowLayoutPanel1.Controls.Add(this.removeButton);
      this.flowLayoutPanel1.Controls.Add(this.addFileButton);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      // 
      // addFileButton
      // 
      resources.ApplyResources(this.addFileButton, "addFileButton");
      this.addFileButton.Name = "addFileButton";
      this.addFileButton.UseVisualStyleBackColor = true;
      this.addFileButton.Click += new System.EventHandler(this.addFileButton_Click);
      // 
      // addUrlButton
      // 
      resources.ApplyResources(this.addUrlButton, "addUrlButton");
      this.addUrlButton.Name = "addUrlButton";
      this.addUrlButton.UseVisualStyleBackColor = true;
      this.addUrlButton.Click += new System.EventHandler(this.AddUrlButton_Click);
      // 
      // removeButton
      // 
      resources.ApplyResources(this.removeButton, "removeButton");
      this.removeButton.Name = "removeButton";
      this.removeButton.UseVisualStyleBackColor = true;
      this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
      // 
      // FileSelectorControl
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.flowLayoutPanel1);
      this.Controls.Add(this.files);
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
