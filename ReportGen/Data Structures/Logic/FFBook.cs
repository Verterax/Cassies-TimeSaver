using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ReportGen
{
    public class FFBook
    {
        #region Variables
        public string lastTemplateFilePath;
        public List<FormAndFuncs> leftPane;
        public List<FormAndFuncs> rightPane;
        #endregion

        #region Constructors/Init

        public FFBook()
        {
            Init();
        }

        public FFBook(TabControl leftPane, TabControl rightPane, string lastTemplatePath)
        {
            Init();

            lastTemplateFilePath = LocateDocTemplate(lastTemplatePath);

            foreach (FormAndFuncs ff in leftPane.Controls)
                this.leftPane.Add(ff);

            foreach (FormAndFuncs ff in rightPane.Controls)
                this.rightPane.Add(ff);
        }

        public FFBook(string filePath)
        {
            Init();
            this.Deserialize(filePath);     
        }

        private void Init()
        {
            this.lastTemplateFilePath = string.Empty;
            this.leftPane = new List<FormAndFuncs>();
            this.rightPane = new List<FormAndFuncs>();
        }

        #endregion

        #region Serialize

        public string Serialize(string saveAs)
        {
            string errCode = G.c_NoErr;

            try
            {
                //First flatted FFs into flatFFs
                FlatBook flatCol = this.Flatten();

                Type typeLayout = typeof(FlatBook);
                XmlSerializer flatColSerializer = new XmlSerializer(typeLayout);

                using(FileStream stream = new FileStream(saveAs, FileMode.Create))
                {
                    flatColSerializer.Serialize(stream, flatCol);
                }

            }
            catch (Exception ex)
            {
                errCode = "Unable to save: " + saveAs + ". "
                    + ex.Message;
               
            }

           return errCode;
        }

        #endregion

        #region Deserialize

        private void Deserialize(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FlatBook));
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                object obj = serializer.Deserialize(stream);
                FlatBook fafFromFile = (FlatBook)obj;

                this.lastTemplateFilePath = LocateDocTemplate(fafFromFile.lastTemplateFilePath);

                foreach (FlatFormAndFuncs flatFF in fafFromFile.flatLeftPane)
                {
                    FormAndFuncs newLeftPage = flatFF.ToFF();
                    this.leftPane.Add(newLeftPage);
                    newLeftPage.SetTabOrder(newLeftPage, 20);
                }

                foreach (FlatFormAndFuncs flatFF in fafFromFile.flatRightPane)
                {
                    FormAndFuncs newRightPage = flatFF.ToFF();
                    this.rightPane.Add(newRightPage);
                    newRightPage.SetTabOrder(newRightPage, 20);
                }
            }

        }

        #endregion

        #region Flatten

        public FlatBook Flatten()
        {
            FlatBook flatBook = new FlatBook();
            string lastPath = lastTemplateFilePath;
            string myDocsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Point scroll = Point.Empty;

            //Path is contained in the My Documents Folder, save as relative.
            if (lastPath.Contains(myDocsDir))
                flatBook.lastTemplateFilePath = lastPath.Replace(myDocsDir, "");
            else //Path is not in My Docs, save as absolute.
                flatBook.lastTemplateFilePath = lastPath;


            foreach (FormAndFuncs page in leftPane)
            {
                scroll = new Point(-page.AutoScrollPosition.X, -page.AutoScrollPosition.Y);
                page.AutoScrollPosition = new Point(0, 0);
                flatBook.flatLeftPane.Add(page.Flatten());
                page.AutoScrollPosition = scroll;

            }

            foreach (FormAndFuncs page in rightPane)
            {
                scroll = new Point(-page.AutoScrollPosition.X, -page.AutoScrollPosition.Y);
                page.AutoScrollPosition = new Point(0, 0);
                flatBook.flatRightPane.Add(page.Flatten());
                page.AutoScrollPosition = scroll;
            }

            return flatBook;
        }

        #endregion

        #region Inflate

        #endregion

        #region Associated Document Template

        private string LocateDocTemplate(string lastKnownPath)
        {

            string templatePath = string.Empty;
            string myDocsFolder =  Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


            if (lastKnownPath.Length == 0) return "";

            //It's relative path.
            if (lastKnownPath[0] == Path.DirectorySeparatorChar)
            {
                templatePath = myDocsFolder + lastKnownPath;

                if (System.IO.File.Exists(templatePath))
                    return templatePath;
                else
                {
                    //The path is relative, but the file is not where we expected.
                    //Try to locate it elsewhere in MyDocuments.
                    templatePath = G.RecursiveFileSearch(myDocsFolder, Path.GetFileName(lastKnownPath));

                    //Return whether the path is found or not. Handle file not found later with error msg.
                    return templatePath;
                }
            }
            else //It's an absolute path, return it. If missing handle in error later.
            {
                return lastKnownPath;
            }
        }

        #endregion
   }
}
