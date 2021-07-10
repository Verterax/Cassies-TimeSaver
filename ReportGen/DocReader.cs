using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Novacode;
using Code7248.word_reader;
using iTextSharp.text.pdf;
using System.Windows.Forms;
using System.IO;


namespace ReportGen
{
    public static class DocReader
    {
        public static string ReadFileContents(string filePath)
        {
            string fileContents = string.Empty;
            FileInfo fileInfo = null;

            fileInfo = new FileInfo(filePath);

            if (fileInfo.Extension == ".doc")
                fileContents = DocReader.ReadDoc(filePath); //Read a .doc file.
            else if (fileInfo.Extension == ".docx")
                fileContents = DocReader.ReadDoc(filePath); //Read a .docx file.
            else if (fileInfo.Extension == ".rtf")
                fileContents = DocReader.ReadRTF(filePath); //Read a .rtf file.
            else if (fileInfo.Extension == ".pdf")
                fileContents = DocReader.ReadPDF(filePath); //Read a .PDF File.
            else
                fileContents = DocReader.ReadAnyFile(filePath);

            return fileContents;
        }


        #region Read File into meaningful Text

        public static string ReadDoc(string filePath)
        {

            #region Open Doc in MS Word Code (Dead)
            //Microsoft.Office.Interop.Word.Application wordApp
            // = new Microsoft.Office.Interop.Word.Application();
            //wordApp.Visible = false;

            //Microsoft.Office.Interop.Word.Document thisDoc =
            //    wordApp.Documents.Open(filePath);

            //thisDoc.ActiveWindow.Selection.WholeStory();
            //thisDoc.ActiveWindow.Selection.Copy();

            //IDataObject data = System.Windows.Forms.Clipboard.GetDataObject();
            //string fileText = data.GetData(System.Windows.Forms.DataFormats.Text).ToString();
            //System.Windows.Forms.Clipboard.SetDataObject(string.Empty);

            //thisDoc.Close();

            //wordApp.Quit();

            #endregion

            try
            {
                TextExtractor extractor = new TextExtractor(filePath);

                string fileText = string.Empty;

                fileText += extractor.ExtractText();
                fileText = TrimMetaChars(fileText);

                return fileText;
            }
            catch 
            {
                return "Unable to open " + filePath +
                    ". Make sure the file is closed and not in use by another program.";
            }
        }

        public static string ReadRTF(string filePath)
        {
            RichTextBox textBox = new RichTextBox();

            textBox.Rtf = System.IO.File.ReadAllText(filePath);

            return textBox.Text;
        }

        public static string ReadPDF(string filePath)
        {
            string text = string.Empty;

            PDFParser reader = new PDFParser();
            text = reader.ExtractText(filePath);

            return text;
        }

        public static string ReadAnyFile(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            string fileContents = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();

            return fileContents;
        }

        private static string TrimMetaChars(string text)
        {
            char tabChar = (char)7;
            char nlChar = (char)12;

            for (char i = (char)1; i < 5; i++)
                text = text.Replace(i, ' ');
            for (char i = (char)19; i < 22; i++)
                text = text.Replace(i, ' ');


            text = text.Replace(tabChar, '\t');
            text = text.Replace(nlChar, '\n');

            return text;
        }

        #endregion
    }

    public class PDFParser
    {

        #region Fields

        #region _numberOfCharsToKeep
        /// <summary>
        /// The number of characters to keep, when extracting text.
        /// </summary>
        private static int _numberOfCharsToKeep = 15;
        #endregion

        #endregion

        #region ExtractText
        /// <summary>
        /// Extracts a text from a PDF file.
        /// </summary>
        /// <param name="inFileName">the full path to the pdf file.</param>
        /// <param name="outFileName">the output file name.</param>
        /// <returns>the extracted text</returns>
        public string ExtractText(string inFileName)//, string outFileName)
        {
            //StreamWriter outFile = null;
            StringBuilder builder = new StringBuilder();
            try
            {
                // Create a reader for the given PDF file
                PdfReader reader = new PdfReader(inFileName);
                //outFile = File.CreateText(outFileName);
                //outFile = new StreamWriter(outFileName, false, System.Text.Encoding.UTF8);
               
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    builder.Append(ExtractTextFromPDFBytes(reader.GetPageContent(page)) + " ");
                }

                return builder.ToString();
            }
            catch (Exception ex)
            {
                return "Error Reading PDF " + ex.Message;
            }
        }
        #endregion

        #region ExtractTextFromPDFBytes
        /// <summary>
        /// This method processes an uncompressed Adobe (text) object 
        /// and extracts text.
        /// </summary>
        /// <param name="input">uncompressed</param>
        /// <returns></returns>
        private string ExtractTextFromPDFBytes(byte[] input)
        {
            if (input == null || input.Length == 0) return "";

            try
            {
                string resultString = "";

                // Flag showing if we are we currently inside a text object
                bool inTextObject = false;

                // Flag showing if the next character is literal 
                // e.g. '\\' to get a '\' character or '\(' to get '('
                bool nextLiteral = false;

                // () Bracket nesting level. Text appears inside ()
                int bracketDepth = 0;

                // Keep previous chars to get extract numbers etc.:
                char[] previousCharacters = new char[_numberOfCharsToKeep];
                for (int j = 0; j < _numberOfCharsToKeep; j++) previousCharacters[j] = ' ';


                for (int i = 0; i < input.Length; i++)
                {
                    char c = (char)input[i];

                    if (inTextObject)
                    {
                        // Position the text
                        if (bracketDepth == 0)
                        {
                            if (CheckToken(new string[] { "TD", "Td" }, previousCharacters))
                            {
                                resultString += "\n\r";
                            }
                            else
                            {
                                if (CheckToken(new string[] { "'", "T*", "\"" }, previousCharacters))
                                {
                                    resultString += "\n";
                                }
                                else
                                {
                                    if (CheckToken(new string[] { "Tj" }, previousCharacters))
                                    {
                                        resultString += " ";
                                    }
                                }
                            }
                        }

                        // End of a text object, also go to a new line.
                        if (bracketDepth == 0 &&
                            CheckToken(new string[] { "ET" }, previousCharacters))
                        {

                            inTextObject = false;
                            resultString += " ";
                        }
                        else
                        {
                            // Start outputting text
                            if ((c == '(') && (bracketDepth == 0) && (!nextLiteral))
                            {
                                bracketDepth = 1;
                            }
                            else
                            {
                                // Stop outputting text
                                if ((c == ')') && (bracketDepth == 1) && (!nextLiteral))
                                {
                                    bracketDepth = 0;
                                }
                                else
                                {
                                    // Just a normal text character:
                                    if (bracketDepth == 1)
                                    {
                                        // Only print out next character no matter what. 
                                        // Do not interpret.
                                        if (c == '\\' && !nextLiteral)
                                        {
                                            nextLiteral = true;
                                        }
                                        else
                                        {
                                            if (((c >= ' ') && (c <= '~')) ||
                                                ((c >= 128) && (c < 255)))
                                            {
                                                resultString += c.ToString();
                                            }

                                            nextLiteral = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Store the recent characters for 
                    // when we have to go back for a checking
                    for (int j = 0; j < _numberOfCharsToKeep - 1; j++)
                    {
                        previousCharacters[j] = previousCharacters[j + 1];
                    }
                    previousCharacters[_numberOfCharsToKeep - 1] = c;

                    // Start of a text object
                    if (!inTextObject && CheckToken(new string[] { "BT" }, previousCharacters))
                    {
                        inTextObject = true;
                    }
                }
                return resultString;
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region CheckToken
        /// <summary>
        /// Check if a certain 2 character token just came along (e.g. BT)
        /// </summary>
        /// <param name="search">the searched token</param>
        /// <param name="recent">the recent character array</param>
        /// <returns></returns>
        private bool CheckToken(string[] tokens, char[] recent)
        {
            foreach (string token in tokens)
            {
                if ((recent[_numberOfCharsToKeep - 3] == token[0]) &&
                    (recent[_numberOfCharsToKeep - 2] == token[1]) &&
                    ((recent[_numberOfCharsToKeep - 1] == ' ') ||
                    (recent[_numberOfCharsToKeep - 1] == 0x0d) ||
                    (recent[_numberOfCharsToKeep - 1] == 0x0a)) &&
                    ((recent[_numberOfCharsToKeep - 4] == ' ') ||
                    (recent[_numberOfCharsToKeep - 4] == 0x0d) ||
                    (recent[_numberOfCharsToKeep - 4] == 0x0a))
                    )
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }

}