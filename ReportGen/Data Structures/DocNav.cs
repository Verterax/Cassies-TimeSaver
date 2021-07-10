using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Novacode;

namespace ReportGen
{

    public class DocLoc
    {       
        public int T { get; set; }
        public int R { get; set; }
        public int C { get; set; }
        public int P { get; set; }

        public bool IsNull
        {
            get
            {
                return !(
                    T == int.MaxValue &&
                    R == int.MaxValue &&
                    C == int.MaxValue &&
                    P == int.MaxValue);
            }
        }

        public DocLoc(int t, int r, int c, int p)
        {
            this.T = t;
            this.R = r;
            this.C = c;
            this.P = p;
        }

        public DocLoc()
        {
            T = int.MaxValue;
            R = int.MaxValue;
            C = int.MaxValue;
            P = int.MaxValue;
        }

        

    }

    public class DocNav
    {
        public const int c_Null = int.MaxValue;

        public DocX doc { get; private set; }
        
        public DocNav(DocX document)
        {
            this.doc = document;
        }

        #region Contains Checks



        #endregion

        #region Get

        #region Get Resource

        /// <summary>
        /// Gets a Table using the stored index.
        /// </summary>
        /// <param name="navigator"></param>
        /// <returns>A Table, or null if return not possible.</returns>
        public Table GetTable(DocLoc loc)
        {
            if (loc.T == c_Null)
                return null;

            if (doc.Tables.Count > loc.T)
                return doc.Tables[loc.T];

            return null;
        }

        /// <summary>
        /// Gets a Row using the stored index.
        /// </summary>
        /// <param name="navigator"></param>
        /// <returns>A Row, or null if return not possible.</returns>
        public Row GetRow(DocLoc loc)
        {
            if (loc.T == c_Null ||
                loc.R == c_Null)
                return null;

            Table currentTable = GetTable(loc);
            if (currentTable == null) 
                return null;

            if (currentTable.Rows.Count > loc.R)
                return currentTable.Rows[loc.R];

            return null;
        }

        /// <summary>
        /// Gets a Cell using the stored index.
        /// </summary>
        /// <param name="navigator"></param>
        /// <returns>A Cell, or null if return not possible.</returns>
        public Cell GetCell(DocLoc loc)
        {
            if (loc.T == c_Null ||
                loc.R == c_Null ||
                loc.C == c_Null)
                return null;

            Row currentRow = GetRow(loc);
            if (currentRow == null) return null;

            if (currentRow.Cells.Count > loc.C)
                return currentRow.Cells[loc.C];

            return null;
        }


        /// <summary>
        /// Gets a Paragraph using the stored index.
        /// </summary>
        /// <param name="navigator"></param>
        /// <returns>A Paragraph, or null if return not possible.</returns>
        public Paragraph GetParagraph(List<Paragraph> paragraphs, DocLoc loc)
        {
            if (loc.T == c_Null ||
                loc.R == c_Null ||
                loc.C == c_Null ||
                loc.P == c_Null)
                return null;

            Cell currentCell = GetCell(loc);
            if (currentCell == null) return null;

            if (currentCell.Paragraphs.Count > loc.P)
                return currentCell.Paragraphs[loc.P];

            return null;
        }

        public int GetParagraphIndexWith(string search, int tryBeginFrom)
        {
            int count = doc.Paragraphs.Count;
            List<Paragraph> paragraphs = doc.Paragraphs;
            if (tryBeginFrom >= count) 
                tryBeginFrom = 0;

            for (int i = 0; i < count; i++)
            {
                if (paragraphs[i].Text.Contains(search))
                    return i;
            }

            return -1;
        }

        public static int GetParagraphIndexWith(List<Paragraph> paragraphs, string search, int tryBeginFrom = 0)
        {
            int count = paragraphs.Count;
            if (tryBeginFrom >= count) tryBeginFrom = 0;
                
            for (int i = tryBeginFrom; i < count; i++)
                if (paragraphs[i].Text.Contains(search))
                    return i;

            return -1;
        }

        #endregion

        #region Get Type Locs Containing

        #endregion

        #region Get Type Continaing


        public Table GetTableWith(string text)
        {
            DocLoc tLoc = GetLocOf(doc.Tables, text);

            if (tLoc == null)
                return null;

            return GetTable(tLoc);
        }

        public Row GetRowWith(string text)
        {
            DocLoc rLoc = GetLocOf(doc.Tables, text);

            if (rLoc == null)
                return null;

            return GetRow(rLoc);
        }

        public Cell GetCellWith(string text)
        {
            DocLoc cLoc = GetLocOf(doc.Tables, text);

            if (cLoc == null)
                return null;

            return GetCell(cLoc);
        }

        public DocLoc GetColWith(string text)
        {
            return GetLocOf(doc.Tables, text);
        }

        public Paragraph GetParagraphWith(List<Paragraph> paragraphs, string text)
        {
            //First search tables.
            DocLoc pLoc = GetLocOf(doc.Tables, text);

            if (pLoc == null)
            {
                if (doc.Text.Contains(text))
                {
                    //If not in table, must be outside a table.
                    foreach (Paragraph p in paragraphs)
                        if (p.MagicText != null)
                            if (p.Text.Contains(text))
                                return p;
                }
            }
            else
            {
                return GetParagraph(paragraphs, pLoc);
            }

            return null;
        }


        //public List<Table> GetTablesWith(string text)
        //{
        //    List<Table> tables = new List<Table>();
        //    List<DocLoc> tLocs = GetTLocsWith(text);

        //    foreach (DocLoc tLoc in tLocs)
        //        tables.Add(GetTable(tLoc));

        //    return tables;
        //}

        //public List<Row> GetRowsWith(string text)
        //{
        //    List<Row> rows = new List<Row>();
        //    List<DocLoc> rLocs = GetRLocsWith(text);

        //    foreach (DocLoc rLoc in rLocs)
        //        rows.Add(GetRow(rLoc));

        //    return rows;
        //}

        //public List<Cell> GetCellsWith(string text)
        //{
        //    List<Cell> cells = new List<Cell>();
        //    List<DocLoc> cLocs = GetCLocsWith(text);

        //    foreach (DocLoc cLoc in cLocs)
        //        cells.Add(GetCell(cLoc));

        //    return cells;
        //}

        //public List<Paragraph> GetParagraphsWith(string text)
        //{
        //    List<Paragraph> paragraphs = new List<Paragraph>();
        //    List<DocLoc> pLocs = GetPLocsWith(text);

        //    foreach (DocLoc pLoc in pLocs)
        //        paragraphs.Add(GetParagraph(pLoc));

        //    return paragraphs;
        //}

        #endregion

        #region Get Text Index

        public int GetTextIndexOf(string match)
        {
            string rawText = doc.Text.Replace("\n", "");
            rawText = rawText.Replace("\t", "");
            rawText = rawText.Replace("\r", "");

            return rawText.IndexOf(match);
        }

        #endregion


        #endregion

        #region Matching Helpers

        private Paragraph FindParagraph(string text)
        {
            foreach (Paragraph p in doc.Paragraphs)
                if (p.Text.Contains(text))
                    return p;

            return null;
        }

        private bool PContainsText(List<Paragraph> paragraphs, string text)
        {
            foreach (Paragraph p in paragraphs)
                if (p.Text.Contains(text))
                    return true;

            return false;
        }

        public DocLoc GetLocOf(List<Table> tables, string text, int tableDepth = 0)
        {
            DocLoc foundAt = null;

            for (int t = 0; t < tables.Count; t++)
            {
                if (PContainsText(tables[t].Paragraphs, text)) //Table contains text.
                {
                    List<Row> rows = tables[t].Rows;

                    for (int r = 0; r < rows.Count; r++)
                    {
                        if (PContainsText(rows[r].Paragraphs, text)) //Row contains text.
                        {
                            List<Cell> cells = rows[r].Cells;
                            
                            for (int c = 0; c < cells.Count; c++)
                            {
                                if (PContainsText(cells[c].Paragraphs, text)) //Cell contains text.
                                {
                                    List<Paragraph> paragraphs = cells[c].Paragraphs;

                                    //Is the paragraph in a deeper table?
                                    if (cells[c].Tables.Count > 0)
                                    {
                                        foundAt = GetLocOf(cells[c].Tables, text, t + (tableDepth + 1) );                                       
                                    }

                                    //Its not deeper, it's here.
                                    if (foundAt == null)
                                    {
                                        for (int p = 0; p < paragraphs.Count; p++)
                                        {
                                            if (paragraphs[p].Text.Contains(text)) //Paragraph contains text.
                                                foundAt = new DocLoc(t + tableDepth, r, c, p);
                                        }
                                    }
                                }
                            }         
                        }
                    }
                }
            }

            return foundAt;
        }

       
        #endregion

        #region Misc

        public string PrintParagraphs(List<Paragraph> paragraphs)
        {
            string str = string.Empty;
            //str += "Paragraphs: = " + paragraphs.Count + Environment.NewLine;
            for (int p = 0; p < paragraphs.Count; p++)
            {
                Paragraph P = paragraphs[p];
                List<DocProperty> props = P.DocumentProperties;

                if (props != null)
                    if (props.Count > 0)
                    {
                        for (int i = 0; i < props.Count; i++)
                        {
                            DocProperty prop = props[i];

                            str += string.Format("Name: {0} = {1}", prop.Name, prop.Xml) + Environment.NewLine; ;
                        }
                    }

                str += string.Format("P{0} = {1}", p, P.Text) + Environment.NewLine;
            }
            return str;
        }

        public string PrintTables(List<Table> tables)
        {
            string str = string.Empty;

            for (int t = 0; t < tables.Count; t++)
                str += Environment.NewLine + string.Format("TABLE( {0} ): Index( {1} ):", t,tables[t].Index) + Environment.NewLine 
                    + PrintRows(tables[t].Rows);

            return str;
        }

        public string PrintRows(List<Row> rows)
        {
            string str = string.Empty;

            for (int r = 0; r < rows.Count; r++)
                str += Environment.NewLine + string.Format("ROW({0}): ", r)
                    + Environment.NewLine + PrintCells(rows[r].Cells);

            return str;
        }

        public string PrintCells(List<Cell> cells)
        {
            string str = string.Empty;

            for (int c = 0; c < cells.Count; c++)
            {
                str += Environment.NewLine + string.Format("CELL({0}): ", c)
                    + Environment.NewLine + PrintParagraphs(cells[c].Paragraphs);

                if (cells[c].Tables.Count > 0)
                    str += PrintTables(cells[c].Tables);
            }

            return str;
        }

        public string PrintLayout()
        {
            string str = string.Empty;//PrintParagraphs(doc.Paragraphs);

            str += PrintTables(doc.Tables);

            return str;
        }

        #endregion

    }
}
