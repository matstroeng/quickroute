using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace QuickRoute.UI.Classes
{
  // using code from http://www.codeproject.com/KB/printing/datagridviewprinter.aspx
  public class DataGridViewPrinter
  {
    private static int currentRow;
    // A static parameter that keep track on which Row (in the DataGridView control) that should be printed

    private static int pageNumber;
    private int bottomMargin;
    private int columnPoint;
    private List<int[]> columnPoints;
    private List<float> columnPointsWidth;
    private List<float> columnsWidth;

    private float currentY;
    // A parameter that keep track on the y coordinate of the page, so the next object to be printed will start from this y coordinate

    private DataGridView dataGridView; // The DataGridView Control which will be printed
    private float dataGridViewWidth;
    private bool isCenterOnPage; // Determine if the report will be printed in the Top-Center of the page
    private bool isWithPaging; // Determine if paging is used
    private bool isWithTitle; // Determine if the page contain title text

    private int leftMargin;
    private int pageHeight;
    private int pageWidth;
    private PrintDocument printDocument; // The PrintDocument to be used for printing
    private int rightMargin;

    private float rowHeaderHeight;
    private List<float> rowsHeight;
    private Color titleColor; // The color to be used with the title text (if isWithTitle is set to true)
    private Font titleFont; // The font to be used with the title text (if isWithTitle is set to true)
    private string titleText; // The title text to be printed in each page (if isWithTitle is set to true)
    private int topMargin;

    // The class constructor
    public DataGridViewPrinter(DataGridView aDataGridView, PrintDocument aPrintDocument, bool centerOnPage,
                               bool withTitle, string titleText, Font titleFont, Color aTitleColor, bool withPaging)
    {
      dataGridView = aDataGridView;
      printDocument = aPrintDocument;
      isCenterOnPage = centerOnPage;
      isWithTitle = withTitle;
      this.titleText = titleText;
      this.titleFont = titleFont;
      titleColor = aTitleColor;
      isWithPaging = withPaging;

      pageNumber = 0;

      rowsHeight = new List<float>();
      columnsWidth = new List<float>();

      columnPoints = new List<int[]>();
      columnPointsWidth = new List<float>();

      // Claculating the pageWidth and the pageHeight
      if (!printDocument.DefaultPageSettings.Landscape)
      {
        pageWidth = printDocument.DefaultPageSettings.PaperSize.Width;
        pageHeight = printDocument.DefaultPageSettings.PaperSize.Height;
      }
      else
      {
        pageHeight = printDocument.DefaultPageSettings.PaperSize.Width;
        pageWidth = printDocument.DefaultPageSettings.PaperSize.Height;
      }

      // Claculating the page margins
      leftMargin = printDocument.DefaultPageSettings.Margins.Left;
      topMargin = printDocument.DefaultPageSettings.Margins.Top;
      rightMargin = printDocument.DefaultPageSettings.Margins.Right;
      bottomMargin = printDocument.DefaultPageSettings.Margins.Bottom;

      // First, the current row to be printed is the first row in the DataGridView control
      currentRow = 0;
    }

    // The function that calculate the height of each row (including the header row), the width of each column (according to the longest text in all its cells including the header cell), and the whole DataGridView width
    private void Calculate(Graphics g)
    {
      if (pageNumber == 0) // Just calculate once
      {
        var tmpSize = new SizeF();
        Font tmpFont;
        float tmpWidth;

        dataGridViewWidth = 0;
        for (int i = 0; i < dataGridView.Columns.Count; i++)
        {
          tmpFont = dataGridView.ColumnHeadersDefaultCellStyle.Font;
          if (tmpFont == null) // If there is no special HeaderFont style, then use the default DataGridView font style
            tmpFont = dataGridView.DefaultCellStyle.Font;

          tmpSize = g.MeasureString(dataGridView.Columns[i].HeaderText, tmpFont);
          tmpWidth = tmpSize.Height;
          rowHeaderHeight = Math.Max(tmpSize.Width, rowHeaderHeight);

          for (int j = 0; j < dataGridView.Rows.Count; j++)
          {
            tmpFont = dataGridView.Rows[j].DefaultCellStyle.Font;
            if (tmpFont == null)
              // If the there is no special font style of the currentRow, then use the default one associated with the DataGridView control
              tmpFont = dataGridView.DefaultCellStyle.Font;

            tmpSize = g.MeasureString("Anything", tmpFont);
            rowsHeight.Add(tmpSize.Height);

            tmpSize = g.MeasureString(dataGridView.Rows[j].Cells[i].EditedFormattedValue.ToString(), tmpFont);
            if (tmpSize.Width > tmpWidth)
              tmpWidth = tmpSize.Width;
          }
          if (dataGridView.Columns[i].Visible)
            dataGridViewWidth += tmpWidth;
          columnsWidth.Add(tmpWidth);
        }

        // Define the start/stop column points based on the page width and the DataGridView Width
        // We will use this to determine the columns which are drawn on each page and how wrapping will be handled
        // By default, the wrapping will occurr such that the maximum number of columns for a page will be determine
        int k;

        int mStartPoint = 0;
        for (k = 0; k < dataGridView.Columns.Count; k++)
          if (dataGridView.Columns[k].Visible)
          {
            mStartPoint = k;
            break;
          }

        int mEndPoint = dataGridView.Columns.Count;
        for (k = dataGridView.Columns.Count - 1; k >= 0; k--)
          if (dataGridView.Columns[k].Visible)
          {
            mEndPoint = k + 1;
            break;
          }

        float mTempWidth = dataGridViewWidth;
        float mTempPrintArea = (float) pageWidth - (float) leftMargin - (float) rightMargin;

        // We only care about handling where the total datagridview width is bigger then the print area
        if (dataGridViewWidth > mTempPrintArea)
        {
          mTempWidth = 0.0F;
          for (k = 0; k < dataGridView.Columns.Count; k++)
          {
            if (dataGridView.Columns[k].Visible)
            {
              mTempWidth += columnsWidth[k];
              // If the width is bigger than the page area, then define a new column print range
              if (mTempWidth > mTempPrintArea)
              {
                mTempWidth -= columnsWidth[k];
                columnPoints.Add(new int[] {mStartPoint, mEndPoint});
                columnPointsWidth.Add(mTempWidth);
                mStartPoint = k;
                mTempWidth = columnsWidth[k];
              }
            }
            // Our end point is actually one index above the current index
            mEndPoint = k + 1;
          }
        }
        // Add the last set of columns
        columnPoints.Add(new int[] {mStartPoint, mEndPoint});
        columnPointsWidth.Add(mTempWidth);
        columnPoint = 0;
      }
    }

    // The funtion that print the title, page number, and the header row
    private void DrawHeader(Graphics g)
    {
      currentY = (float) topMargin;

      // Printing the page number (if isWithPaging is set to true)
      if (isWithPaging)
      {
        pageNumber++;
        string PageString = "Page " + pageNumber.ToString();

        var PageStringFormat = new StringFormat();
        PageStringFormat.Trimming = StringTrimming.Word;
        PageStringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
        PageStringFormat.Alignment = StringAlignment.Far;

        var PageStringFont = new Font("Calibri", 8, FontStyle.Regular, GraphicsUnit.Point);

        var PageStringRectangle = new RectangleF((float) leftMargin, currentY,
                                                 (float) pageWidth - (float) rightMargin - (float) leftMargin,
                                                 g.MeasureString(PageString, PageStringFont).Height);

        g.DrawString(PageString, PageStringFont, new SolidBrush(Color.Black), PageStringRectangle, PageStringFormat);

        currentY += g.MeasureString(PageString, PageStringFont).Height;
      }

      // Printing the title (if isWithTitle is set to true)
      if (isWithTitle)
      {
        var TitleFormat = new StringFormat();
        TitleFormat.Trimming = StringTrimming.Word;
        TitleFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
        if (isCenterOnPage)
          TitleFormat.Alignment = StringAlignment.Center;
        else
          TitleFormat.Alignment = StringAlignment.Near;

        var TitleRectangle = new RectangleF((float) leftMargin, currentY,
                                            (float) pageWidth - (float) rightMargin - (float) leftMargin,
                                            g.MeasureString(titleText, titleFont).Height);

        g.DrawString(titleText, titleFont, new SolidBrush(titleColor), TitleRectangle, TitleFormat);

        currentY += g.MeasureString(titleText, titleFont).Height;
      }

      // Calculating the starting x coordinate that the printing process will start from
      var CurrentX = (float) leftMargin;
      if (isCenterOnPage)
        CurrentX += (((float) pageWidth - (float) rightMargin - (float) leftMargin) - columnPointsWidth[columnPoint])/
                    2.0F;

      // Setting the HeaderFore style
      Color HeaderForeColor = dataGridView.ColumnHeadersDefaultCellStyle.ForeColor;
      if (HeaderForeColor.IsEmpty) // If there is no special HeaderFore style, then use the default DataGridView style
        HeaderForeColor = dataGridView.DefaultCellStyle.ForeColor;
      var HeaderForeBrush = new SolidBrush(HeaderForeColor);

      // Setting the HeaderBack style
      Color HeaderBackColor = dataGridView.ColumnHeadersDefaultCellStyle.BackColor;
      if (HeaderBackColor.IsEmpty) // If there is no special HeaderBack style, then use the default DataGridView style
        HeaderBackColor = dataGridView.DefaultCellStyle.BackColor;
      var HeaderBackBrush = new SolidBrush(HeaderBackColor);

      // Setting the LinePen that will be used to draw lines and rectangles (derived from the GridColor property of the DataGridView control)
      var TheLinePen = new Pen(dataGridView.GridColor, 1);

      // Setting the HeaderFont style
      Font HeaderFont = dataGridView.ColumnHeadersDefaultCellStyle.Font;
      if (HeaderFont == null) // If there is no special HeaderFont style, then use the default DataGridView font style
        HeaderFont = dataGridView.DefaultCellStyle.Font;

      // Calculating and drawing the HeaderBounds        
      var HeaderBounds = new RectangleF(CurrentX, currentY, columnPointsWidth[columnPoint], rowHeaderHeight);
      g.FillRectangle(HeaderBackBrush, HeaderBounds);

      // Setting the format that will be used to print each cell of the header row
      var CellFormat = new StringFormat();
      CellFormat.Trimming = StringTrimming.Word;
      CellFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;

      // Printing each visible cell of the header row
      RectangleF CellBounds;
      float ColumnWidth;
      for (var i = (int) columnPoints[columnPoint].GetValue(0); i < (int) columnPoints[columnPoint].GetValue(1); i++)
      {
        if (!dataGridView.Columns[i].Visible) continue; // If the column is not visible then ignore this iteration

        ColumnWidth = columnsWidth[i];

        // Check the CurrentCell alignment and apply it to the CellFormat
        if (dataGridView.ColumnHeadersDefaultCellStyle.Alignment.ToString().Contains("Right"))
          CellFormat.Alignment = StringAlignment.Far;
        else if (dataGridView.ColumnHeadersDefaultCellStyle.Alignment.ToString().Contains("Center"))
          CellFormat.Alignment = StringAlignment.Center;
        else
          CellFormat.Alignment = StringAlignment.Far;

        CellFormat.FormatFlags = StringFormatFlags.DirectionVertical;
        CellFormat.LineAlignment = StringAlignment.Center;

        CellBounds = new RectangleF(CurrentX, currentY, ColumnWidth, rowHeaderHeight);

        // Printing the cell text
        g.DrawString(dataGridView.Columns[i].HeaderText, HeaderFont, HeaderForeBrush, CellBounds, CellFormat);

        // Drawing the cell bounds
        if (dataGridView.RowHeadersBorderStyle != DataGridViewHeaderBorderStyle.None)
          // Draw the cell border only if the HeaderBorderStyle is not None
          g.DrawRectangle(TheLinePen, CurrentX, currentY, ColumnWidth, rowHeaderHeight);

        CurrentX += ColumnWidth;
      }

      currentY += rowHeaderHeight;
    }

    // The function that print a bunch of rows that fit in one page
    // When it returns true, meaning that there are more rows still not printed, so another PagePrint action is required
    // When it returns false, meaning that all rows are printed (the CureentRow parameter reaches the last row of the DataGridView control) and no further PagePrint action is required
    private bool DrawRows(Graphics g)
    {
      // Setting the LinePen that will be used to draw lines and rectangles (derived from the GridColor property of the DataGridView control)
      var TheLinePen = new Pen(dataGridView.GridColor, 1);

      // The style paramters that will be used to print each cell
      Font RowFont;
      Color RowForeColor;
      Color RowBackColor;
      SolidBrush RowForeBrush;
      SolidBrush RowBackBrush;
      SolidBrush RowAlternatingBackBrush;

      // Setting the format that will be used to print each cell
      var CellFormat = new StringFormat();
      CellFormat.Trimming = StringTrimming.Word;
      CellFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;

      // Printing each visible cell
      RectangleF RowBounds;
      float CurrentX;
      float ColumnWidth;
      while (currentRow < dataGridView.Rows.Count)
      {
        if (dataGridView.Rows[currentRow].Visible) // Print the cells of the currentRow only if that row is visible
        {
          // Setting the row font style
          RowFont = dataGridView.Rows[currentRow].DefaultCellStyle.Font;
          if (RowFont == null)
            // If the there is no special font style of the currentRow, then use the default one associated with the DataGridView control
            RowFont = dataGridView.DefaultCellStyle.Font;

          // Setting the RowFore style
          RowForeColor = dataGridView.Rows[currentRow].DefaultCellStyle.ForeColor;
          if (RowForeColor.IsEmpty)
            // If the there is no special RowFore style of the currentRow, then use the default one associated with the DataGridView control
            RowForeColor = dataGridView.DefaultCellStyle.ForeColor;
          RowForeBrush = new SolidBrush(RowForeColor);

          // Setting the RowBack (for even rows) and the RowAlternatingBack (for odd rows) styles
          RowBackColor = dataGridView.Rows[currentRow].DefaultCellStyle.BackColor;
          if (RowBackColor.IsEmpty)
            // If the there is no special RowBack style of the currentRow, then use the default one associated with the DataGridView control
          {
            RowBackBrush = new SolidBrush(dataGridView.DefaultCellStyle.BackColor);
            RowAlternatingBackBrush = new SolidBrush(dataGridView.AlternatingRowsDefaultCellStyle.BackColor);
          }
          else
            // If the there is a special RowBack style of the currentRow, then use it for both the RowBack and the RowAlternatingBack styles
          {
            RowBackBrush = new SolidBrush(RowBackColor);
            RowAlternatingBackBrush = new SolidBrush(RowBackColor);
          }

          // Calculating the starting x coordinate that the printing process will start from
          CurrentX = (float) leftMargin;
          if (isCenterOnPage)
            CurrentX += (((float) pageWidth - (float) rightMargin - (float) leftMargin) - columnPointsWidth[columnPoint])/
                        2.0F;

          // Calculating the entire currentRow bounds                
          RowBounds = new RectangleF(CurrentX, currentY, columnPointsWidth[columnPoint], rowsHeight[currentRow]);

          // Filling the back of the currentRow
          if (currentRow%2 == 0)
            g.FillRectangle(RowBackBrush, RowBounds);
          else
            g.FillRectangle(RowAlternatingBackBrush, RowBounds);

          // Printing each visible cell of the currentRow                
          for (var CurrentCell = (int) columnPoints[columnPoint].GetValue(0);
               CurrentCell < (int) columnPoints[columnPoint].GetValue(1);
               CurrentCell++)
          {
            if (!dataGridView.Columns[CurrentCell].Visible)
              continue; // If the cell is belong to invisible column, then ignore this iteration

            // Check the CurrentCell alignment and apply it to the CellFormat
            if (dataGridView.Columns[CurrentCell].DefaultCellStyle.Alignment.ToString().Contains("Left"))
              CellFormat.Alignment = StringAlignment.Near;
            else if (dataGridView.Columns[CurrentCell].DefaultCellStyle.Alignment.ToString().Contains("Center"))
              CellFormat.Alignment = StringAlignment.Center;
            else
              CellFormat.Alignment = StringAlignment.Far;

            ColumnWidth = columnsWidth[CurrentCell];
            CellFormat.LineAlignment = StringAlignment.Center;
            var CellBounds = new RectangleF(CurrentX, currentY, ColumnWidth, rowsHeight[currentRow]);

            // Printing the cell text
            g.DrawString(dataGridView.Rows[currentRow].Cells[CurrentCell].EditedFormattedValue.ToString(), RowFont,
                         RowForeBrush, CellBounds, CellFormat);

            // Drawing the cell bounds
            if (dataGridView.CellBorderStyle != DataGridViewCellBorderStyle.None)
              // Draw the cell border only if the CellBorderStyle is not None
              g.DrawRectangle(TheLinePen, CurrentX, currentY, ColumnWidth, rowsHeight[currentRow]);

            CurrentX += ColumnWidth;
          }
          currentY += rowsHeight[currentRow];

          // Checking if the currentY is exceeds the page boundries
          // If so then exit the function and returning true meaning another PagePrint action is required
          if ((int) currentY > (pageHeight - topMargin - bottomMargin))
          {
            currentRow++;
            return true;
          }
        }
        currentRow++;
      }

      currentRow = 0;
      columnPoint++; // Continue to print the next group of columns

      if (columnPoint == columnPoints.Count) // Which means all columns are printed
      {
        columnPoint = 0;
        return false;
      }
      else
        return true;
    }

    // The method that calls all other functions
    public bool DrawDataGridView(Graphics g)
    {
      try
      {
        Calculate(g);
        DrawHeader(g);
        bool bContinue = DrawRows(g);
        return bContinue;
      }
      catch (Exception ex)
      {
        MessageBox.Show("Operation failed: " + ex.Message.ToString(), Application.ProductName + " - Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
      }
    }
  }
}