// ===========================================================================
//	PDFTableExamples				©2007 WebSupergoo. All rights reserved.
// ===========================================================================

using System;
using System.Drawing;
using System.Collections;
using System.Globalization;
using WebSupergoo.ABCpdf10;

namespace DoddleReport.AbcPdf
{
    /// <summary>
    /// PDFTable class is used to add table aligned content to pdf files
    /// </summary>

    public class PDFTable
    {


        /// <summary>
        /// The cell padding property
        /// </summary>
        public double CellPadding
        {
            get { return mCellPadding; }
            set { mCellPadding = value; }
        }
        /// <summary>
        /// Vertical alignment of the row
        /// </summary>
        public double VerticalAlignment
        {
            get { return mVerticalAlignment; }
            set
            {
                if (value > 1)
                    mVerticalAlignment = 1;
                else if (value < 0)
                    mVerticalAlignment = 0;
                else
                    mVerticalAlignment = value;
            }
        }

        /// <summary>
        /// Horizontal alignment. This property is taken directly from Doc.HPos
        /// </summary>
        public double HorizontalAlignment
        {
            get { return mDoc.HPos; }
            set { mDoc.HPos = value; }
        }

        /// <summary>
        /// Number of lines to be repeated on every page
        /// </summary>
        public int HeaderLinesCount
        {
            get { return mHeaderLinesCount; }
            set { mHeaderLinesCount = value; }
        }
        int mHeaderLinesCount = 1;


        /// <summary>
        /// If true header is repeated on every new page
        /// </summary>
        public bool RepeatHeader
        {
            get { return mRepeatHeader; }
            set { mRepeatHeader = value; }
        }
        /// <summary>
        /// If true header is framed
        /// </summary>
        public bool FrameHeader
        {
            get { return mFrameHeader; }
            set { mFrameHeader = value; }
        }
        /// <summary>
        /// The current page number - cached for speed
        /// </summary>
        public int PageNumber
        {
            get
            {
                int thePageID = mDoc.Page;
                object theObj = mPageNos[thePageID];
                if (theObj == null)
                {
                    theObj = mDoc.PageNumber;
                    mPageNos[thePageID] = theObj;
                    mPageIDs[theObj] = thePageID;
                }
                return (int)theObj;
            }
            set
            {
                object theObj = mPageIDs[value];
                if (theObj == null)
                {
                    mDoc.PageNumber = value;
                    int thePageID = mDoc.Page;
                    mPageNos[thePageID] = value;
                    mPageIDs[value] = thePageID;
                }
                else
                {
                    mDoc.Page = (int)theObj;
                }
            }
        }

        private double mCellPadding = 0;
        private double mVerticalAlignment = 0;
        private bool mRepeatHeader = false;
        private bool mFrameHeader = false;
        private Hashtable mPageNos = new Hashtable();
        private Hashtable mPageIDs = new Hashtable();

        /// <summary>
        /// Associated Doc
        /// </summary>
        private Doc mDoc = null;
        /// <summary>
        /// Current cell
        /// </summary>
        private Point mPos = new Point();
        /// <summary>
        /// Bounds of the table
        /// </summary>
        private XRect mBounds = new XRect();
        /// <summary>
        /// Relative widths of the columns
        /// </summary>
        private double[] mWidths = new double[0];
        private PDFTable mParentTable;

        /// <summary>
        /// PagePos class includes vertical position and page number
        /// </summary>
        private class PagePos
        {
            public PagePos(PDFTable inTable)
            {
                mPageHeight = inTable.mDoc.MediaBox.Height;
                PosY = inTable.mDoc.Rect.Top;
                if (inTable.PageNumber == 0)
                    PageNr = 1;
                else
                    PageNr = inTable.PageNumber;
            }
            public PagePos(PagePos p)
            {
                mPageHeight = p.mPageHeight;
                PosY = p.PosY;
                PageNr = p.PageNr;
            }
            public static bool operator <(PagePos p1, PagePos p2)
            {
                return (p1.PageNr > p2.PageNr) || (p1.PageNr == p2.PageNr && p1.PosY < p2.PosY);
            }
            public static bool operator <=(PagePos p1, PagePos p2)
            {
                return (p1.PageNr > p2.PageNr) || (p1.PageNr == p2.PageNr && p1.PosY <= p2.PosY);
            }
            public static bool operator >(PagePos p1, PagePos p2)
            {
                return !(p1.PageNr > p2.PageNr) || (p1.PageNr == p2.PageNr && p1.PosY < p2.PosY);
            }
            public static bool operator >=(PagePos p1, PagePos p2)
            {
                return !(p1.PageNr > p2.PageNr) || (p1.PageNr == p2.PageNr && p1.PosY <= p2.PosY);
            }
            public static PagePos operator -(PagePos p1, double val)
            {
                PagePos pn = new PagePos(p1);
                pn.PosY -= val;
                return pn;
            }
            public static PagePos operator +(PagePos p1, double val)
            {
                PagePos pn = new PagePos(p1);
                pn.PosY += val;
                return pn;
            }
            /// <summary>
            /// Page number
            /// </summary>
            public int PageNr = 1;
            /// <summary>
            /// Vertical position
            /// </summary>
            public double PosY = 0;
            /// <summary>
            /// Page height
            /// </summary>
            private double mPageHeight;
        }

        /// <summary>
        /// RowVerticalBounds class includes row top and bottom coordinates
        /// </summary>
        private class RowVerticalBounds
        {
            public PagePos Top;
            public PagePos Bottom;
        }

        /// <summary>
        /// Bounds of the all existing rows
        /// </summary>
        private RowVerticalBounds[] mRowPositions = new RowVerticalBounds[0];

        /// <summary>
        /// Top of the current row
        /// </summary>
        private PagePos RowTop;
        /// <summary>
        /// Bottom of the current row
        /// </summary>
        private PagePos mRowBottom;
        /// <summary>
        /// Row bottom
        /// </summary>
        private PagePos RowBottom
        {
            get { return mRowBottom; }
            set
            {
                mRowBottom = value;
                if (mParentTable != null)
                {
                    if (mParentTable.RowBottom > mRowBottom - CellPadding)
                        mParentTable.RowBottom = mRowBottom - CellPadding;
                }
            }
        }

        /// <summary>
        /// RowObject class describes row object (string or image)
        /// </summary>
        private class RowObject
        {
            public RowObject() { }
            public RowObject(RowObject inObj)
            {
                id = inObj.id;
                obj = inObj.obj;
                rect.String = inObj.rect.String;
                textStyle = inObj.textStyle;
                pageNr = inObj.pageNr;
                font = inObj.font;
            }
            /// <summary>
            /// Pdf object id
            /// </summary>
            public int id = 0;
            /// <summary>
            /// String or image object
            /// </summary>
            public object obj = new object();
            /// <summary>
            /// Bounding rectangle
            /// </summary>
            public XRect rect = new XRect();
            /// <summary>
            /// Text style
            /// </summary>
            public string textStyle = "";
            /// <summary>
            /// Page number
            /// </summary>
            public int pageNr = 0;
            /// <summary>
            /// Font ID
            /// </summary>
            public int font = 0;
        }

        /// <summary>
        /// Contains current row objects added directly via AddHtml or AddImage methods
        /// </summary>
        private ArrayList mOwnRowObjects = new ArrayList();
        /// <summary>
        /// Contains current row objects added to the child tables
        /// </summary>
        private ArrayList mChildRowObjects = new ArrayList();

        /// <summary>
        /// Save object description
        /// </summary>
        /// <param name="id">id of the added object</param>
        /// <param name="obj">string or XImage</param>
        /// <returns></returns>
        private string SaveRowObject(int id, object obj)
        {
            RowObject newObj = new RowObject();
            newObj.id = id;
            newObj.obj = obj;
            newObj.rect.String = mDoc.GetInfo(id, "rect");
            newObj.textStyle = mDoc.TextStyle.String;
            newObj.pageNr = PageNumber;
            newObj.font = mDoc.Font;
            mOwnRowObjects.Add(newObj);
            if (mParentTable != null)
                mParentTable.SaveChildRowObject(newObj);
            return newObj.rect.String;
        }

        private void SaveChildRowObject(RowObject obj)
        {
            RowObject newObj = new RowObject(obj);
            newObj.rect.String = obj.rect.String;
            newObj.rect.Inset(-CellPadding, -CellPadding);
            mChildRowObjects.Add(newObj);
            if (mParentTable != null)
                mParentTable.SaveChildRowObject(newObj);
        }


        /// <summary>
        /// Header position
        /// </summary>
        private PagePos mHeaderPos;
        /// <summary>
        /// Header content
        /// </summary>
        private ArrayList mHeaderObjects = new ArrayList();

        /// <summary>
        ///  Focus on the document and assign the relevant number of columns
        /// </summary>
        /// <param name="doc">Parent Doc</param>
        /// <param name="columns">Number of columns</param>
        public PDFTable(Doc doc, int columns)
        {
            mDoc = doc;
            RowTop = new PagePos(this);
            mRowBottom = new PagePos(this);
            SetRect(mDoc.Rect.String);
            SetColumns(columns);
        }

        /// <summary>
        ///  Focus on the document and assign the relevant number of columns and padding
        /// </summary>
        /// <param name="doc">Parent Doc</param>
        /// <param name="columns">Number of columns</param>
        /// <param name="padding">Padding amount</param>
        public PDFTable(Doc doc, int columns, double padding)
        {
            mDoc = doc;
            RowTop = new PagePos(this);
            mRowBottom = new PagePos(this);
            SetRect(mDoc.Rect.String);
            SetColumns(columns);
            CellPadding = padding;
        }

        /// <summary>
        /// Assign a new table rectangle and reset the current table position
        /// </summary>
        /// <param name="rect">New table rectangle</param>

        protected void SetRect(string rect)
        {
            mDoc.Rect.String = rect;
            mBounds.String = rect;
            mPos.Y = -1;
            mPos.X = -1;
        }

        /// <summary>
        /// Change the number of columns in the table
        /// </summary>
        /// <param name="inNum">Number of columns</param>
        private void SetColumns(int inNum)
        {
            if (inNum > 0)
            {
                mWidths = new double[inNum];
                for (int i = 0; i < mWidths.Length; i++)
                    mWidths[i] = 1;
            }
        }

        /// <summary>
        /// Get the current row - a zero based index
        /// </summary>
        public int Row { get { return mPos.Y; } }
        /// <summary>
        /// Get the current column - a zero based index
        /// </summary>
        public int Column { get { return mPos.X; } }

        /// <summary>
        /// Change a column width
        /// </summary>
        /// <param name="i">Column index</param>
        /// <param name="inWidth">Relative width of the column</param>
        public void SetColumnWidth(int i, double inWidth)
        {
            if ((i >= 0) && (i < mWidths.Length))
                mWidths[i] = inWidth;
        }
        /// <summary>
        /// Change column widths
        /// </summary>
        /// <param name="inWidths">Array of column widths</param>
        public void SetColumnWidths(double[] inWidths)
        {
            inWidths.CopyTo(mWidths, 0);
        }

        /// <summary>
        /// Move to the next cell
        /// </summary>
        public void NextCell()
        {
            if (mPos.Y == -1)
                NextRow();
            mPos.X = mPos.X + 1;
            if (mPos.X >= mWidths.Length)
            {
                NextRow();
                mPos.X = 0;
            }
            if (mPos.X < 0)
                mPos.X = 0;
            SelectCurrentCell();
        }
        /// <summary>
        /// Move by the number of cells
        /// </summary>
        /// <param name="count"> Number of cells to move by</param>
        public void NextCell(int count)
        {
            for (int i = 0; i < count; i++)
                NextCell();
        }

        /// <summary>
        /// Move to the next row
        /// </summary>
        public void NextRow()
        {
            FixRowPosition();

            if (VerticalAlignment > 0)
                AlignVertically();

            if (RepeatHeader && mPos.Y == 0 && HeaderLinesCount != 0)
            {
                mHeaderObjects = new ArrayList();
                mHeaderPos = RowTop;
            }

            if (RepeatHeader && mPos.Y >= 0 && mPos.Y < HeaderLinesCount)
            {
                ArrayList rowList = (ArrayList)mOwnRowObjects.Clone();
                for (int i = 0; i < rowList.Count; i++)
                    mHeaderObjects.Add(rowList[i]);
            }

            if (RepeatHeader && FrameHeader && mPos.Y == HeaderLinesCount - 1)
            {
                FrameCells(0, 0, mWidths.Length - 1, HeaderLinesCount - 1);
            }

            if (mPos.Y >= 0)
                RowTop = RowBottom - CellPadding;
            else
                RowTop = RowBottom;

            if (RowTop.PosY < mBounds.Bottom)
                RowTop.PosY = mBounds.Bottom;

            mDoc.Rect.String = mBounds.String;
            RowBottom = RowTop;
            mDoc.Rect.Top = RowTop.PosY;

            mChildRowObjects.Clear();
            mOwnRowObjects.Clear();

            mPos.Y++;
            mPos.X = -1;
        }

        /// <summary>
        /// Add text to the current cell
        /// </summary>
        /// <param name="inText">Text of the cell</param>
        /// <returns>id of the created pdf object</returns>
        public int AddText(string inText)
        {
            while (inText.Length > 0 && Char.IsWhiteSpace(inText[inText.Length - 1]))
                inText = inText.Remove(inText.Length - 1, 1);

            string theRect = mDoc.Rect.String;

            mDoc.Rect.Inset(CellPadding, CellPadding);
            PageNumber = RowTop.PageNr;
            int id = mDoc.AddText(inText);

            int theDrawn = 0;
            if (id > 0)
                theDrawn = mDoc.GetInfoInt(id, "Characters");
            if (theDrawn < inText.Length)
            {
                if (id != 0)
                    mDoc.Delete(id);
                if (mRowPositions.Length >= 2)
                    mRowPositions[mRowPositions.Length - 1].Bottom.PosY
                        = mRowPositions[mRowPositions.Length - 2].Bottom.PosY;

                MoveRowToNextPage();
                mDoc.Rect.String = theRect;
                mDoc.Rect.Top = RowTop.PosY;
                AddText(inText);
            }
            else
            {
                XRect drawnRect = new XRect();
                drawnRect.String = SaveRowObject(id, inText);

                PagePos thePos = new PagePos(this);
                thePos.PosY = (double)(mDoc.Pos.Y - mDoc.FontSize);
                if (thePos < RowBottom) RowBottom = thePos;

                mDoc.Rect.String = theRect;
            }

            return id;
        }

        /// <summary>
        /// Add text to the current cell
        /// </summary>
        /// <param name="inText">Text of the cell</param>
        /// <param name="inFontSize">Font size</param>
        /// <returns>id of the created text object</returns>
        public int AddText(string inText, int inFontSize)
        {
            int oldFontSize = mDoc.FontSize;
            mDoc.FontSize = inFontSize;
            int id = AddText(inText);
            mDoc.FontSize = oldFontSize;
            return id;
        }

        /// <summary>
        /// Add enumerable data (strings or images) to the table
        /// </summary>
        /// <param name="data">Enumerable collection of strings and images</param>
        public void AddEnumerableData(IEnumerable data)
        {
            IEnumerator it = data.GetEnumerator();

            it.Reset();
            bool bNoneEmpty = it.MoveNext();

            while (bNoneEmpty)
            {
                NextCell();

                if (it.Current is string)
                    AddHtml((string)it.Current);
                else
                    if (it.Current is XImage)
                        AddImage((XImage)it.Current, true);

                bNoneEmpty = it.MoveNext();
            }
        }

        /// <summary>
        /// Add enumerable html data
        /// </summary>
        /// <param name="data">Enumerable collection of strings and images</param>
        public void AddHtml(IEnumerable data)
        {
            AddEnumerableData(data);
        }

        /// <summary>
        /// Fill table with rectangular array of strings
        /// </summary>
        /// <param name="inTextArrays">Rectangular array of strings</param>
        public void AddHtml(string[][] inTextArrays)
        {
            if (inTextArrays.Length == mWidths.Length)
            {
                bool bLast = false;
                int RowCount = 0;

                while (!bLast)
                {
                    NextRow();
                    bLast = true;
                    for (int i = 0; i < inTextArrays.Length; i++)
                    {
                        NextCell();

                        if (inTextArrays[i].Length > RowCount + 1)
                            bLast = false;

                        if (inTextArrays[i].Length > RowCount)
                            AddHtml(inTextArrays[i][RowCount]);
                    }
                    RowCount++;
                }
            }
        }

        /// <summary>
        /// Depth of recursion into AddHtml function
        /// </summary>
        private int HtmlDepth = 0;
        /// <summary>
        /// Maximum allowed depth of recursion into AddHtml function
        /// </summary>
        private const int MaxHtmlDepth = 10;

        /// <summary>
        /// Add Html to the current cell
        /// </summary>
        /// <param name="inHtml">Html to put into the cell</param>
        /// <returns>id of the created pdf object</returns>
        public int AddHtml(string inHtml)
        {
            return AddHtml(inHtml, true);
        }

        private int AddHtml(string inHtml, bool inMoveRows)
        {
            int id = 0;
            HtmlDepth++;

            if (HtmlDepth < MaxHtmlDepth)
            {
                inHtml = inHtml.TrimEnd(null);
                if (inHtml.Length > 0)
                {
                    string theRect = mDoc.Rect.String;

                    mDoc.Rect.Inset(CellPadding, CellPadding);
                    PageNumber = RowTop.PageNr;

                    id = mDoc.AddHtml(inHtml);
                    if ((id == 0) || (mDoc.Chainable(id)))
                    {
                        if (inMoveRows)
                        {
                            if (id != 0)
                                mDoc.Delete(id);
                            if (mRowPositions.Length >= 2)
                                mRowPositions[mRowPositions.Length - 1].Bottom.PosY
                                    = mRowPositions[mRowPositions.Length - 2].Bottom.PosY;

                            MoveRowToNextPage();
                            mDoc.Rect.String = theRect;
                            mDoc.Rect.Top = RowTop.PosY;
                            id = AddHtml(inHtml);
                        }
                    }
                    else
                    {
                        XRect drawnRect = new XRect();
                        drawnRect.String = SaveRowObject(id, inHtml);

                        PagePos thePos = new PagePos(this);
                        thePos.PosY = drawnRect.Bottom;
                        if (thePos < RowBottom)
                            RowBottom = thePos;
                        mDoc.Rect.String = theRect;
                    }
                }
            }

            HtmlDepth--;
            return id;
        }


        /// <summary>
        /// Add nested table with the specified number of columns
        /// </summary>
        /// <param name="inColumns">Number of columns</param>
        /// <returns></returns>
        public PDFTable AddTable(int inColumns)
        {
            PageNumber = RowTop.PageNr;
            string theRect = mDoc.Rect.String;
            mDoc.Rect.Bottom = mBounds.Bottom;
            PDFTable nestedTable = new PDFTable(mDoc, inColumns);
            nestedTable.mParentTable = this;
            nestedTable.CellPadding = CellPadding;
            nestedTable.VerticalAlignment = VerticalAlignment;

            mDoc.Rect.String = theRect;
            return nestedTable;
        }
        /// <summary>
        /// Add nested table with the specified number of columns and cellpadding
        /// </summary>
        /// <param name="inColumns">Number of columns</param>
        /// <param name="padding">Padding amount</param>
        /// <returns>Created PDFTable object</returns>
        public PDFTable AddTable(int inColumns, double padding)
        {
            PDFTable result = AddTable(inColumns);
            result.CellPadding = padding;
            return result;
        }

        /// <summary>
        /// Add image 
        /// </summary>
        /// <param name="inImage">XImage to put into the cell</param>
        /// <param name="bStretch">If bStretch is true inserted image will have the same width with the cell</param>
        /// <returns></returns>
        public int AddImage(XImage inImage, bool inStretch)
        {
            if (inImage != null)
            {
                int id = 0;
                string theRect = mDoc.Rect.String;
                mDoc.Rect.Inset(CellPadding, CellPadding);

                double scale = 1;
                if (inStretch)
                    scale = mDoc.Rect.Width / inImage.Width;

                mDoc.Rect.Width = inImage.Width * scale;
                PageNumber = RowTop.PageNr;

                if (mDoc.Rect.Top - inImage.Height * scale >= mBounds.Bottom)
                {
                    mDoc.Rect.Bottom = mDoc.Rect.Top - inImage.Height * scale;

                    PagePos thePos = new PagePos(this);
                    id = mDoc.AddImageObject(inImage);
                    thePos -= inImage.Height * scale;
                    if (thePos < RowBottom) RowBottom = thePos;
                    SaveRowObject(id, inImage);
                }
                else
                {
                    MoveRowToNextPage();
                    mDoc.Rect.String = theRect;
                    mDoc.Rect.Top = RowTop.PosY;
                    id = AddImage(inImage, inStretch);
                }

                mDoc.Rect.String = theRect;
                return id;
            }
            return 0;
        }


        /// <summary>
        /// Moves vertically the input rect of the cell
        /// </summary>
        /// <param name="advance">Advance amount</param>
        public void Advance(double advance)
        {
            if (mDoc.Rect.Top - advance >= mBounds.Bottom)
                mDoc.Rect.Top -= advance;
            else
                mDoc.Rect.Top = mBounds.Bottom;

            if (RowBottom.PosY - advance >= mBounds.Bottom)
                RowBottom -= advance;
            else
                RowBottom.PosY = mBounds.Bottom;
        }

        /// <summary>
        /// Returns top bound of the top parent table
        /// </summary>
        /// <returns>top bound of the top parent table</returns>
        private double GetParentTopBounds()
        {
            if (mParentTable == null)
                return mBounds.Top;
            else
                return mParentTable.GetParentTopBounds();
        }

        /// <summary>
        /// Align content vertically
        /// </summary>
        private void AlignVertically()
        {
            ArrayList objectList = new ArrayList(mOwnRowObjects);
            mOwnRowObjects.Clear();

            for (int i = 0; i < objectList.Count; i++)
            {
                RowObject oldObj = (RowObject)objectList[i];
                double bottom = Math.Round(RowBottom.PosY - CellPadding, 6);

                XRect bbox = new XRect();
                bbox.String = mDoc.GetInfo(oldObj.id, "rect");
                bbox.Inset(-CellPadding, -CellPadding);


                if (bbox.Bottom > bottom && oldObj.pageNr == RowBottom.PageNr)
                {
                    double alpha = VerticalAlignment;
                    if (VerticalAlignment > 1)
                        alpha = 1;
                    double offset = alpha * (bbox.Bottom - bottom);
                    double width = 0;
                    if (oldObj.obj is XImage)
                        width = Double.Parse(mDoc.GetInfo(oldObj.id, "Width"), new CultureInfo("en-US"));
                    mDoc.Delete(oldObj.id);
                    XRect newRect = new XRect();
                    newRect.String = bbox.String;
                    newRect.Move(0, -offset);
                    if (newRect.Bottom > bottom)
                        newRect.Bottom = bottom;

                    mDoc.Rect.String = newRect.String;

                    string theTextStyle = mDoc.TextStyle.String;

                    mDoc.TextStyle.String = oldObj.textStyle;
                    int iFont = mDoc.Font;
                    mDoc.Font = oldObj.font;

                    if (oldObj.obj is string)
                        AddHtml((string)(oldObj.obj));
                    else
                        if (oldObj.obj is XImage)
                        {
                            XImage image = (XImage)(oldObj.obj);
                            if (width == image.Width)
                                AddImage(image, false);
                            else
                                AddImage(image, true);
                        }

                    mDoc.Font = iFont;
                    mDoc.TextStyle.String = theTextStyle;
                }
                else
                    mOwnRowObjects.Add(oldObj);

            }
        }

        /// <summary>
        /// Move row content to the next page
        /// </summary>
        private void MoveRowToNextPage()
        {
            ArrayList oldRowObjects = new ArrayList(mOwnRowObjects);
            oldRowObjects.AddRange(mChildRowObjects);
            mOwnRowObjects.Clear();
            mChildRowObjects.Clear();

            if (PageNumber == mDoc.PageCount)
                mDoc.Page = mDoc.AddPage();
            else
                PageNumber = PageNumber + 1;

            mDoc.Rect.String = mBounds.String;
            mDoc.Rect.Top = GetParentTopBounds();

            PagePos oldRowTop = RowTop;
            RowTop = new PagePos(this);
            RowBottom = RowTop;
            double headerOffset = 0;

            if (RepeatHeader)
            {
                int posX = mPos.X;
                MoveRowToNextPage(mHeaderObjects, GetParentTopBounds() - mHeaderPos.PosY, false);
                if (FrameHeader)
                    FrameRow(mPos.Y);
                NextRow();
                mPos.X = posX;
                headerOffset = mRowPositions[mPos.Y - 1].Top.PosY - mRowPositions[mPos.Y - 1].Bottom.PosY;
            }

            MoveRowToNextPage(oldRowObjects, GetParentTopBounds() - oldRowTop.PosY - headerOffset, true);
        }

        private bool MoveRowToNextPage(ArrayList objectList, double offset, bool bRemove)
        {
            bool ok = true;
            for (int i = 0; i < objectList.Count; i++)
            {
                RowObject oldObj = (RowObject)objectList[i];
                double width = 0;
                if (oldObj.obj is XImage)
                    width = Double.Parse(mDoc.GetInfo(oldObj.id, "Width"), new CultureInfo("en-US"));
                mDoc.Rect.String = mDoc.GetInfo(oldObj.id, "rect");
                if (bRemove)
                    mDoc.Delete(oldObj.id);
                mDoc.Rect.Move(0, offset);
                mDoc.Rect.Inset(-CellPadding, -CellPadding);
                string theTextStyle = mDoc.TextStyle.String;

                mDoc.TextStyle.String = oldObj.textStyle;
                mDoc.Rect.Bottom -= mDoc.TextStyle.LineSpacing;
                int iFont = mDoc.Font;
                mDoc.Font = oldObj.font;

                if (oldObj.obj is string)
                {
                    if (AddHtml((string)(oldObj.obj)) == 0)
                        ok = false;
                }
                else if (oldObj.obj is XImage)
                {
                    XImage image = (XImage)(oldObj.obj);
                    if (AddImage(image, width != image.Width) == 0)
                        ok = false;
                }

                mDoc.Font = iFont;
                mDoc.TextStyle.String = theTextStyle;
            }
            return ok;
        }

        /// <summary>
        /// Frame specified rectangle
        /// </summary>
        /// <param name="inX1">Bottom left X coordinate</param>
        /// <param name="inY1">Bottom left Y coordinate</param>
        /// <param name="inX2">Top right X coordinate</param>
        /// <param name="inY2">Top right Y coordinate</param>
        public void FrameCells(int inX1, int inY1, int inX2, int inY2)
        {
            FrameCells(inX1, inY1, inX2, inY2, true, true, true, true);
        }

        /// <summary>
        /// Frame specified rectangle
        /// </summary>
        /// <param name="inX1">Bottom left X coordinate</param>
        /// <param name="inY1">Bottom left Y coordinate</param>
        /// <param name="inX2">Top right X coordinate</param>
        /// <param name="inY2">Top right Y coordinate</param>
        /// <param name="inTop">If true, line top side</param>
        /// <param name="inBottom">If true, line bottom side</param>
        /// <param name="inLeft">If true, line left side</param>
        /// <param name="inRight">If true, line right side</param>
        public void FrameCells(int inX1, int inY1, int inX2, int inY2, bool inTop, bool inBottom, bool inLeft, bool inRight)
        {
            // check inputs
            if (inX1 > inX2)
                return;
            if (inY1 > inY2)
                return;

            double theTotal = 0;
            double left = 0, right = 0;

            for (int i = 0; i < inX1; i++)
                left += mWidths[i];

            for (int i = 0; i <= inX2; i++)
                right += mWidths[i];

            for (int i = 0; i < mWidths.Length; i++)
                theTotal += mWidths[i];

            left = mBounds.Left + left * mBounds.Width / theTotal;
            right = mBounds.Left + right * mBounds.Width / theTotal;

            PagePos top = mRowPositions[inY1].Top;
            PagePos bottom = mRowPositions[inY2].Bottom;

            int pageNr = PageNumber;

            double tempTop = top.PosY;
            double tempBottom = 0;
            int curPageNr = top.PageNr;

            do
            {
                if (curPageNr == bottom.PageNr)
                    tempBottom = bottom.PosY;
                else
                    tempBottom = mBounds.Bottom;

                if (curPageNr == top.PageNr)
                    tempTop = top.PosY;
                else
                    tempTop = GetParentTopBounds();

                PageNumber = curPageNr++;

                if (inLeft)
                    mDoc.AddLine(left, tempBottom, left, tempTop);
                if (inTop)
                    mDoc.AddLine(left, tempTop, right, tempTop);
                if (inRight)
                    mDoc.AddLine(right, tempTop, right, tempBottom);
                if (inBottom)
                    mDoc.AddLine(right, tempBottom, left, tempBottom);
            }
            while (curPageNr <= bottom.PageNr);

            PageNumber = pageNr;
        }

        /// <summary>
        /// Frame table
        /// </summary>
        public void Frame()
        {
            FixRowPosition();
            FrameCells(0, 0, mWidths.Length - 1, mRowPositions.Length - 1);
        }

        /// <summary>
        /// Frame Row
        /// </summary>
        /// <param name="index">Row index</param>
        public void FrameRow(int index)
        {
            FixRowPosition();
            FrameCells(0, index, mWidths.Length - 1, index);
        }

        /// <summary>
        /// Frame all rows
        /// </summary>
        public void FrameRows()
        {
            FixRowPosition();
            for (int i = 0; i < mRowPositions.Length; i++)
                FrameRow(i);
        }

        /// <summary>
        /// Frame column
        /// </summary>
        /// <param name="index">Column index</param>
        public void FrameColumn(int index)
        {
            FixRowPosition();
            FrameCells(index, 0, index, mRowPositions.Length - 1);
        }

        /// <summary>
        /// Frame all columns
        /// </summary>
        public void FrameColumns()
        {
            FixRowPosition();
            for (int i = 0; i < mWidths.Length; i++)
                FrameCells(i, 0, i, mRowPositions.Length - 1);
        }

        /// <summary>
        /// Fill rectangle with specified color
        /// </summary>
        /// <param name="inColor">Fill color</param>
        /// <param name="inX1">Bottom left X coordinate</param>
        /// <param name="inY1">Bottom left Y coordinate</param>
        /// <param name="inX2">Top right X coordinate</param>
        /// <param name="inY2">Top right Y coordinate</param>
        public void FillCells(string inColor, int inX1, int inY1, int inX2, int inY2)
        {
            // check inputs
            if (inX1 > inX2)
                return;
            if (inY1 > inY2)
                return;

            double theTotal = 0;
            double left = 0, right = 0;

            for (int i = 0; i < inX1; i++)
                left += mWidths[i];

            for (int i = 0; i <= inX2; i++)
                right += mWidths[i];

            for (int i = 0; i < mWidths.Length; i++)
                theTotal += mWidths[i];

            left = mBounds.Left + left * mBounds.Width / theTotal;
            right = mBounds.Left + right * mBounds.Width / theTotal;

            PagePos top = mRowPositions[inY1].Top;
            PagePos bottom = mRowPositions[inY2].Bottom;

            int pageNr = PageNumber;

            double tempTop = top.PosY;
            double tempBottom = 0;
            int curPageNr = top.PageNr;

            do
            {
                if (curPageNr == bottom.PageNr)
                    tempBottom = bottom.PosY;
                else
                    tempBottom = mBounds.Bottom;

                if (curPageNr == top.PageNr)
                    tempTop = top.PosY;
                else
                    tempTop = GetParentTopBounds();

                PageNumber = curPageNr++;

                //mDoc.Rect.Left = left;
                //mDoc.Rect.Right = right;
                //mDoc.Rect.Top = tempTop;
                //mDoc.Rect.Bottom = tempBottom;

                mDoc.Rect.Left = left;
                mDoc.Rect.Bottom = tempBottom;
                mDoc.Rect.Height = 1;
                mDoc.Rect.Width = right;
                //mDoc.Rect.Right = right;
                //mDoc.Rect.Top = tempBottom + 1;


                int theLayer = mDoc.Layer;
                string theColor = mDoc.Color.String;
                mDoc.Layer = mDoc.LayerCount + 1;
                mDoc.Color.String = inColor;
                int id = mDoc.FillRect();
                mDoc.Color.String = theColor;
                mDoc.Layer = theLayer;
            }
            while (curPageNr <= bottom.PageNr);

            PageNumber = pageNr;
        }


        /// <summary>
        /// Underline the rectangle with specified color
        /// </summary>
        /// <param name="inColor">Fill color</param>
        /// <param name="index">Row index</param>
        public void UnderlineRow(string inColor, int index)
        {
            if (index > 0)
            {
                FixRowPosition();
                UnderlineCell(inColor, 0, index, mWidths.Length - 1, index);
            }
        }

        /// <summary>
        /// Underline the rectangle with specified color
        /// </summary>
        /// <param name="inColor">Fill color</param>
        /// <param name="inX1">Bottom left X coordinate</param>
        /// <param name="inY1">Bottom left Y coordinate</param>
        /// <param name="inX2">Top right X coordinate</param>
        /// <param name="inY2">Top right Y coordinate</param>
        public void UnderlineCell(string inColor, int inX1, int inY1, int inX2, int inY2)
        {
            // check inputs
            if (inX1 > inX2)
                return;
            if (inY1 > inY2)
                return;

            double theTotal = 0;
            double left = 0, right = 0;

            for (int i = 0; i < inX1; i++)
                left += mWidths[i];

            for (int i = 0; i <= inX2; i++)
                right += mWidths[i];

            for (int i = 0; i < mWidths.Length; i++)
                theTotal += mWidths[i];

            left = mBounds.Left + left * mBounds.Width / theTotal;
            right = mBounds.Left + right * mBounds.Width / theTotal;

            PagePos top = mRowPositions[inY1].Top;
            PagePos bottom = mRowPositions[inY2].Bottom;

            int pageNr = PageNumber;

            double tempTop = top.PosY;
            double tempBottom = 0;
            int curPageNr = top.PageNr;

            do
            {
                if (curPageNr == bottom.PageNr)
                    tempBottom = bottom.PosY;
                else
                    tempBottom = mBounds.Bottom;

                if (curPageNr == top.PageNr)
                    tempTop = top.PosY;
                else
                    tempTop = GetParentTopBounds();

                PageNumber = curPageNr++;

                mDoc.Rect.Left = left;
                mDoc.Rect.Bottom = tempBottom;
                mDoc.Rect.Height = 0.005;
                mDoc.Rect.Width = right - 20;

                int theLayer = mDoc.Layer;
                string theColor = mDoc.Color.String;
                mDoc.Layer = mDoc.LayerCount + 1;
                mDoc.Color.String = inColor;
                int id = mDoc.FillRect();
                mDoc.Color.String = theColor;
                mDoc.Layer = theLayer;
            }
            while (curPageNr <= bottom.PageNr);

            PageNumber = pageNr;
        }

        /// <summary>
        /// Fill table with specified color
        /// </summary>
        /// <param name="inColor">Fill color</param>
        public void Fill(string inColor)
        {
            FixRowPosition();
            FillCells(inColor, 0, 0, mWidths.Length - 1, mRowPositions.Length - 1);
        }

        /// <summary>
        /// Fill column with specified color
        /// </summary>
        /// <param name="inColor">Fill color</param>
        /// <param name="index">Column index</param>
        public void FillColumn(string inColor, int index)
        {
            FixRowPosition();
            FillCells(inColor, index, 0, index, mRowPositions.Length - 1);
        }

        /// <summary>
        /// Fill row with specified color
        /// </summary>
        /// <param name="inColor">Fill color</param>
        /// <param name="index">Row index</param>
        public void FillRow(string inColor, int index)
        {
            FixRowPosition();
            FillCells(inColor, 0, index, mWidths.Length - 1, index);
        }

        /// <summary>
        /// Fix current row position
        /// </summary>
        private void FixRowPosition()
        {
            if (mPos.Y >= 0)
            {
                if (mPos.Y > mRowPositions.Length - 1)
                {
                    RowVerticalBounds[] theCopy = new RowVerticalBounds[mPos.Y + 1];
                    mRowPositions.CopyTo(theCopy, 0);
                    mRowPositions = theCopy;
                }

                if (RowBottom.PosY < mBounds.Bottom)
                    RowBottom.PosY = mBounds.Bottom;

                mRowPositions[mPos.Y] = new RowVerticalBounds();
                mRowPositions[mPos.Y].Top = RowTop;
                mRowPositions[mPos.Y].Bottom = RowBottom - CellPadding;
            }
        }

        /// <summary>
        /// Select current cell
        /// </summary>
        private void SelectCurrentCell()
        {
            double theTotal = 0, thePos = 0;
            if ((mPos.X >= 0) && (mPos.X < mWidths.Length))
            {
                // get the x offset and width of the cell
                for (int i = 0; i < mWidths.Length; i++)
                {
                    theTotal = theTotal + mWidths[i];
                    if (i < mPos.X) thePos = thePos + mWidths[i];
                }
                thePos = thePos * (mBounds.Width / theTotal);
                double theWidth = mWidths[mPos.X] * (mBounds.Width / theTotal);
                // position the cell
                mDoc.Rect.Top = RowTop.PosY;
                mDoc.Rect.Left = mBounds.Left + thePos;
                mDoc.Rect.Width = theWidth;
            }
        }

        /// <summary>
        ///  Delete all the objects drawn part of the current row
        /// </summary>
        public void DeleteLastRow()
        {
            for (int i = 0; i < mOwnRowObjects.Count; i++)
                mDoc.Delete(((RowObject)mOwnRowObjects[i]).id);
            for (int i = 0; i < mChildRowObjects.Count; i++)
                mDoc.Delete(((RowObject)mOwnRowObjects[i]).id);
        }
    }

}
