using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;

namespace QuickRoute.Controls
{
  public partial class FileSelectorControl : UserControl
  {
    public string FileDialogTitle { get; set; }
    public string FileDialogFilter { get; set; }
    public int FileDialogFilterIndex { get; set; }

    public event EventHandler<EventArgs> FilesChanged;

    public FileSelectorControl()
    {
      InitializeComponent();
    }

    public FileSelectorControl(IEnumerable<string> fileNames, bool allowUrlAdding)
      : this()
    {
      AddFiles(fileNames);
      AllowUrlAdding = allowUrlAdding;
    }

    public List<string> FileNames
    {
      get
      {
        var fileNames = new List<string>();
        foreach (var fileName in files.Items)
        {
          fileNames.Add(fileName.ToString());
        }
        return fileNames;
      }
    }

    public bool AllowUrlAdding
    {
      get
      {
        return addUrlButton.Visible;
      }
      set
      {
        addUrlButton.Visible = value;
      }
    }



    public void AddFiles(IEnumerable<string> fileNames)
    {
      foreach (var fileName in fileNames)
      {
        files.Items.Add(fileName);
      }
      InvokeFilesChanged();
    }

    private void RemoveFiles(IEnumerable<string> fileNames)
    {
      foreach (var fileName in fileNames)
      {
        files.Items.Remove(fileName);
      }
      InvokeFilesChanged();
    }

    public void AddFile(string fileName)
    {
      AddFiles(new List<string>() { fileName });
    }

    public void RemoveFile(string fileName)
    {
      RemoveFiles(new List<string>() { fileName });
    }

    private void InvokeFilesChanged()
    {
      var eventHandler = FilesChanged;
      if (eventHandler != null) eventHandler(this, EventArgs.Empty);
    }

    private void addFileButton_Click(object sender, EventArgs e)
    {
      using (var ofd = new OpenFileDialog
      {
        AddExtension = true,
        CheckFileExists = true,
        Multiselect = true,
        Title = FileDialogTitle,
        Filter = FileDialogFilter,
        FilterIndex = FileDialogFilterIndex
      })
      {
        if (ofd.ShowDialog() == DialogResult.OK)
        {
          AddFiles(GetDuplicateFreeFiles(ofd.FileNames));
        }
      }
    }

    private void AddUrlButton_Click(object sender, EventArgs e)
    {
      using (var urlDialog = new Forms.UrlDialog())
      {
        if (urlDialog.ShowDialog() == DialogResult.OK)
        {
          AddFiles(GetDuplicateFreeFiles(urlDialog.Urls));
        }
      }
    }

    private void removeButton_Click(object sender, EventArgs e)
    {
      RemoveSelectedFiles();
    }

    private void RemoveSelectedFiles()
    {
      var filesToRemove = new List<string>();
      foreach (var item in files.SelectedItems)
      {
        filesToRemove.Add(item.ToString());
      }
      RemoveFiles(filesToRemove);
    }

    private List<string> GetDuplicateFreeFiles(IEnumerable<string> fileNamesToAdd)
    {
      var fileNames = FileNames;
      var duplicateFreeFiles = new List<string>();
      foreach (var fileToAdd in fileNamesToAdd)
      {
        if (!fileNames.Contains(fileToAdd))
        {
          duplicateFreeFiles.Add(fileToAdd);
        }
      }
      return duplicateFreeFiles;
    }

    private void files_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Delete)
      {
        RemoveSelectedFiles();
      }
    }

    private void files_DragEnter(object sender, DragEventArgs e)
    {
      e.Effect = DragDropEffects.Move;
    }

    private void files_DragDrop(object sender, DragEventArgs e)
    {
      var manager = new DragDropManager();
      var allowedFileExtensions = FileFormatManager.GetQuickRouteFileExtensions();
      var fileNames = manager.GetDroppedFileNames(e, allowedFileExtensions);
      if(fileNames.Count > 0)
      {
        AddFiles(fileNames);
      }
    }

  }
}
