using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using QuickRoute.Resources;

namespace QuickRoute.UI.Classes
{
  public class LapGridViewPrinter
  {
    // The DataGridView Control which will be printed.
    private readonly DataGridView dataGridView;
    // The PrintDocument to be used for printing.
    private PrintDocument printDocument;
    // The class that will do the printing process.
    private DataGridViewPrinter dataGridViewPrinter;

    public LapGridViewPrinter(DataGridView dataGridView)
    {
      this.dataGridView = dataGridView;
    }

    // The printing setup function
    private bool Setup(string title)
    {
      var printDialog = new PrintDialog
                            {
                              AllowCurrentPage = false,
                              AllowPrintToFile = false,
                              AllowSelection = false,
                              AllowSomePages = false,
                              PrintToFile = false,
                              ShowHelp = false,
                              ShowNetwork = false
                            };

      if (printDialog.ShowDialog() != DialogResult.OK)
        return false;

      printDocument = new PrintDocument
                        {
                          DocumentName = string.Format("{0} - {1}", Strings.QuickRoute, title),
                          PrinterSettings = printDialog.PrinterSettings,
                          DefaultPageSettings = printDialog.PrinterSettings.DefaultPageSettings
                        };
      printDocument.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);
      printDocument.PrintPage += printDocument_PrintPage;

      dataGridViewPrinter = new DataGridViewPrinter(
        dataGridView,
        printDocument, 
        false, 
        true,
        title, 
        new Font("Calibri", 10, FontStyle.Bold, GraphicsUnit.Point), 
        Color.Black, 
        false);

      return true;
    }


    public void Print(string title)
    {
      if (Setup(title)) printDocument.Print();
    }


    // The PrintPage action for the PrintDocument control
    private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
    {
      bool more = dataGridViewPrinter.DrawDataGridView(e.Graphics);
      if (more) e.HasMorePages = true;
    }


    // The Print Preview Button
    public void Preview(string title)
    {
      if (Setup(title))
      {
        var myPrintPreviewDialog = new PrintPreviewDialog {Document = printDocument};
        myPrintPreviewDialog.ShowDialog();
      }
    }


  }

}
