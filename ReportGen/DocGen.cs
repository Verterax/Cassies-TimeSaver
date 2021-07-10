using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Novacode;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace ReportGen
{
    public static class DocGen
    {

        #region Consts


        public const string c_OpTag = "<";
        public const string c_ClTag = ">";

       // public const string newDocFolderName = @"GeneratedDocs\";
        //public const string c_TemplatesFolder = @"Templates\";
        //public const string c_LibrariesFolder = @"Libraries\";
        public const string c_DocXFileName = "DocX.dll";
        public const string c_DocReaderFileName = "reader.dll";

        //Loaded into the hash on instantiation along with doc name.
        //Make sure to add any new docs into the Initialize() function.
        public const string c_RSPInitFileName = "RSP Initial Template.docx";
        public const string c_RSPInitReportName = "RSP-Initial";

        public const string c_RSPTriFileName = "RSP Tri Template.docx";
        public const string c_RSPTriReportName = "RSP-Triennial";

        public const string c_PsychInitFileName = "Psych Initial Template.docx";
        public const string c_PsychInitReportName = "Psych-Initial";

        public const string c_PsychTriFileName = "Psych Tri Template.docx";
        public const string c_PsychTriReportName = "Psych-Triennial";

        public const string c_CustomFileName = "Get Custom Template Path";
        public const string c_CustomReport = "Custom Template";

        public const string c_TestFileName = "regexTest.docx";
        public const string c_TestReportName = "Regex Test";
        public const string c_docxExt = ".docx";

        public const string c_RegexTagWrapClo = @"</(.*?)>";
        public const string c_RegexTagWrapSL = @"<(.*?)>.*?</\1>";
        public const string c_RegexDelPar = @".*?<delPar>.*?";

        #endregion

        #region DocGen Variables

        //DocGen Progress Reporting.
        public static int progress1;
        public static int progress2;
        public static string msg1;
        public static string msg2;
        public static BackgroundWorker bgw;

        #endregion

        #region Constructor

        //public DocGen()
        //{
        //    //add Docs Available to create.

        //    Initialize();
        //}

        //private bool Initialize()
        //{
        //      return true;
        //}

        #endregion

        #region Check DocGen Files exist

         public static string hasDocX()
         {
             if (!System.IO.File.Exists(c_DocXFileName))
             {
                 return G.GetRoot(); //Return where docX should be.
             }
             else
                 return "0";
         }

        #endregion

        #region Misc DocGen Functions

        private static string GetPad(int len)
        {
            string padding = "";
            for (int i = 0; i < (len/7); i++)
            {
                padding += "\t";
            }
            return padding;
        }

        //Wraps a string in < >
        public static string DocTag(string content)
        {
            return c_OpTag + content + c_ClTag;
        }

        public static string DocTagClo(string content)
        {
            return c_OpTag + '/' + content + c_ClTag;
        }

        private static string GetNewDocName(DocGenArgs args)
        {
            string docName = string.Empty;

            foreach (FormAndFuncs ff in args.formFuncs)
                if (ff.actions.ContainsKey(Action.c_FileName))
                {
                    Action fileNameAction = ff.actions[Action.c_FileName];
                    docName = fileNameAction.EvalStructure(ref args.fields);

                   
                    if (docName != string.Empty)
                        break;
                }

            if (docName == string.Empty)
            {
                Random rand = new Random();
                docName = "Custom Doc " + rand.Next();
            }

            docName = docName.Replace('/', '-');
            return G.CleanFileName(docName);
        }

        private static string CopyDocFromTemplate(string templateFilePath, string newDocPath)
        {
            try
            {
                System.IO.File.Copy(templateFilePath, newDocPath, true);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        public static string OpenDocProgram(string docToOpen)
        {
            try
            {
                System.Diagnostics.Process.Start(docToOpen);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return G.c_NoErr;
        }

        public static string GetErrFiller(string find)
        {
            string repeatString = "EXPRESSION ERROR -- ";
            string filler = string.Empty;
            while (filler.Length <= find.Length)
            {
                filler += repeatString;
            }

            return filler;
        }

        #endregion

        #region Progress Callback

        public static void UpdateMain(
            int pbr1,
            int pbr2,
            string message1,
            string message2)
        {
            progress1 = pbr1;
            progress2 = pbr2;
            msg1 = message1;
            msg2 = message2;
            bgw.ReportProgress(0);
        }

        private static void MainProgressChanged(int pbr1)
        {
            progress1 = pbr1;
            bgw.ReportProgress(0);
        }

        private static void SecondaryProgressChanged(int pbr2)
        {
            progress2 = pbr2;
            bgw.ReportProgress(0);
        }

        private static void MainMsgChanged(string message1)
        {
            msg1 = message1;
            bgw.ReportProgress(0);
        }

        private static void SecondaryMsgChanged(string message2)
        {
            msg2 = message2;
            bgw.ReportProgress(0);
        }

        #endregion

        #region Generate Documents

        public static string GenCustom(ref DocGenArgs args, BackgroundWorker bg)
        {
            string templateFilePath = args.templateFilePath;

            string ErrCode = string.Empty;
            string newDocPath = string.Empty;
            Fields fields = args.fields;

            //Set BGW for UpdateMain
            bgw = bg;
            UpdateMain(0, 0, "", "");

            newDocPath = G.GetUserDocsPath() + G.c_Output_Folder_Name + GetNewDocName(args) + G.c_TemplateExt;

            args.newDocFilePath = newDocPath;
            ErrCode = CopyDocFromTemplate(templateFilePath, newDocPath);


            //Ensure file is not already in use.
            try
            {
                StreamReader reader = new StreamReader(newDocPath);
                reader.Close();
                reader.Dispose();
            }
            catch (IOException)
            {
                ErrCode = "Please close a document by the name of: " + G.GetFileName(newDocPath) +
                    " and try again. A new document could not be generated because a file by its name is locked " +
                    "for editing by another program--most likely Microsoft Word.";
                args.errCode = ErrCode;
                return ErrCode;
            }



            if (System.IO.File.Exists(newDocPath))
            {
                using (DocX fillDoc = DocX.Load(newDocPath))
                {
                    Dictionary<string, string> actionTags = new Dictionary<string, string>();
                    Dictionary<string, string> showTags = new Dictionary<string, string>();
                    DocNav docNav = new DocNav(fillDoc);


                    //Pre-calculate Actions and Shows, add to fields.
                    UpdateMain(0, 0, "", "");
                    ErrCode += PrecalculateActions(docNav, args);
                    
                    ////Handle Show Action sections. Remove leftover Show sections. Including in P tags.
                    UpdateMain(10, 0, "", "");
                    ErrCode += HandleShowHideSections(docNav, args);

                    if (bgw.CancellationPending)
                    {
                        fillDoc.Save();
                        return G.c_Cancel;
                    }

                    ////Replace Fields 
                    UpdateMain(40, 0, "", "");
                    ErrCode += ReplaceTags(docNav, args.fields);

                    if (bgw.CancellationPending)
                    {
                        fillDoc.Save();
                        return G.c_Cancel;
                    }

                    //Replace Suffix Modded Tags (Able to Mod both fields and actions.
                    //Console.WriteLine("Replacing Suffix Mods.");
                    UpdateMain(65, 0, "", "");
                    ErrCode += ReplaceSuffixModTags(docNav, args);

                    if (bgw.CancellationPending)
                    {
                        fillDoc.Save();
                        return G.c_Cancel;
                    }

                    //Console.WriteLine("Removings Tables, Rows, Columns, and Shrinking...");
                    UpdateMain(80, 0, "", "");
                    ErrCode += ApplyCommandTags(docNav);

                    ////Save
                    UpdateMain(97, 50, "Saving Document.", "Almost done...");
                    //Console.WriteLine("Saving..");
                    fillDoc.Save();

                    UpdateMain(100, 100, "All Done!", "All Done!");
                }
            }
            else
            {
                ErrCode = newDocPath + " could not be found or created. Check to make sure this program has file creation privileges.";
                return ErrCode;
            }

            //If no errors, return no errors code.
            if (ErrCode == string.Empty)
                ErrCode = G.c_NoErr;

            args.errCode = ErrCode;
            return ErrCode;
        }

        #endregion

        #region High Level Doc Manip Functions


        private static string CalcTagsOfType(DocGenArgs args, ref Dictionary<string, string> dictionary, ActionType aType)
        {
            string ErrCode = G.c_NoErr;

            foreach (FormAndFuncs ff in args.formFuncs)
                foreach (string aKey in ff.actions.Keys)
                    if (ff.actions[aKey].actionType == aType)
                        dictionary.Add(
                            ff.actions[aKey].tagName,
                            ff.actions[aKey].EvalStructure(ref args.fields));

            if (ErrCode == G.c_NoErr)
                return string.Empty;
            else
                return Environment.NewLine + ErrCode;
        }

        /// <summary>
        /// Adds the results of any Actions to the dictionary of fields.
        /// </summary>
        /// <param name="docNav"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string PrecalculateActions(DocNav docNav, DocGenArgs args)
        {
            string ErrCode = G.c_NoErr;

            Dictionary<string, string> showTags = new Dictionary<string,string>();
            Dictionary<string, string> actionTags = new Dictionary<string,string>();
            Dictionary<string, Action> actions = new Dictionary<string,Action>();

            MainMsgChanged("Evaluating Actions");

            SecondaryMsgChanged("Gathering Actions.");
            //Get a dictionary of all the actions.
            foreach (FormAndFuncs ff in args.formFuncs)
                    foreach (string key in ff.actions.Keys)
                        if (!actions.ContainsKey(key))
                            actions.Add(key, ff.actions[key]);

            //do a double calculate for shows.
            SecondaryMsgChanged("First Pass Evaluate.");
            SecondaryProgressChanged(25);
            for (int i = 0; i < 2; i++)
            {
                foreach (string key in actions.Keys) //Foreach Fill and Show action.
                    if (actions[key].actionType == ActionType.Show)
                    {
                        Action action = actions[key];
                        //Console.WriteLine("Adding Show: " + key);
                        string result = action.EvalStructure(ref args.fields);

                        if (result == Fields.c_EmptyField)
                            result = RPNCalc.c_False;

                        if (args.fields.Keys.Contains(key))
                            args.fields[key] = result;
                        else
                            args.fields.Add(key, result);
                    }
                if (bgw.CancellationPending) return ErrCode;
                SecondaryMsgChanged("Second Pass Evaluate");
                SecondaryProgressChanged(50);                                    
            }

            SecondaryMsgChanged("Consolidating Actions.");
            SecondaryProgressChanged(75);  
            //Add Fill Actions.
            foreach (string key in actions.Keys) //Foreach Fill and Show action.
                if (actions[key].actionType == ActionType.Fill)
                    if (!args.fields.Keys.Contains(key)) //If this result isn't present in the fieldsdictionary.
                    {
                        Action action = actions[key];
                        string result = action.EvalStructure(ref args.fields);
                        args.fields.Add(key, result);
                    }

            SecondaryMsgChanged("Actions Evaluated.");
            SecondaryProgressChanged(100); 

            if (ErrCode == G.c_NoErr)
                return string.Empty;
            else
                return ErrCode;
        }

        private static string HandleShowHideSections(DocNav docNav, DocGenArgs args)
        {
            string ErrCode = string.Empty;
            List<string> evalErrTags = new List<string>();
            List<string> missingOTags = new List<string>();
            List<string> otherErr = new List<string>();
            List<string> matchesSL = null;
            List<string> matchesMP = null;
            Paragraph pTagO = null;
            Paragraph pTagClo = null;
            string tag = string.Empty;
            string tagClo = string.Empty;
            int openPIndex = -1;
            int closePIndex = -1;
            int scanFrom = 0;
            bool showSection = false;
            bool evalErr = false;

            //Calc Show/Hide Actions
            Dictionary<string, string> showTags = new Dictionary<string, string>();
            ErrCode += CalcTagsOfType(args, ref showTags, ActionType.Show);

            #region Hide/Show Inline Tags
            MainMsgChanged("Show / Hiding Inline Regions");

            // Hide or Show all tags in the showTags Dic
            matchesSL = docNav.doc.FindUniqueByPattern(c_RegexTagWrapSL, RegexOptions.Singleline);
            int count = matchesSL.Count;


            for (int i = 0; i < count; i++)
            {
                string match = matchesSL[i];

                #region Reset Match Variables
                tag = match.Replace("/", "");
                tagClo = match;

                showSection = false;
                evalErr = false;

                string key = match.Substring(1, match.IndexOf('>') - 1);

                if (showTags.Keys.Contains(key))
                {
                    if (showTags[key] == RPNCalc.c_True)
                        showSection = true;
                    else if (showTags[key] == RPNCalc.c_False)
                        showSection = false;
                    else
                        evalErr = true;
                }
                else
                    showSection = false;
                #endregion

                string tagO = G.TagWrap(key);

                if (showSection)
                {
                    //Only remove tags.
                    docNav.doc.ReplaceText(tagO, string.Empty);
                    docNav.doc.ReplaceText(G.TagCloWrap(key), string.Empty);
                    SecondaryMsgChanged("Showing " + tagO + " area."); 
                }
                else//Remove entire find.
                {
                    List<Paragraph> paragraphs = docNav.doc.Paragraphs;
                    Paragraph remP = docNav.GetParagraphWith(paragraphs, tagO);            
                    docNav.doc.ReplaceText(match, string.Empty);
                    SecondaryMsgChanged("Hiding " + tagO + " area.");
                }

                if (bgw.CancellationPending) return ErrCode;
                SecondaryProgressChanged(((100 / count) * i) / 2);

                if (evalErr)
                    evalErrTags.Add(tag);
            }
            #endregion

            matchesMP = docNav.doc.FindUniqueByPattern(c_RegexTagWrapClo, RegexOptions.Singleline);
            count = matchesMP.Count;

            //Handle separate paragraph tag wraps. 
            #region Hide/Show Separate Paragraph Tag Wraps

            MainMsgChanged("Show / Hiding Paragraph Regions");

            for (int i = 0; i < count; i++)
            {
                if (bgw.CancellationPending) return ErrCode;

                bool keepSearching = true;
                string match = matchesMP[i];

                while (keepSearching)
                {
                    #region Reset Match Variables
                    //construct tag appearance
                    tag = match.Replace("/", "");
                    tagClo = match;

                    showSection = false;
                    evalErr = false;

                    string key = G.TagUnwrap(tag);

                    if (showTags.Keys.Contains(key))
                    {
                        if (showTags[key] == RPNCalc.c_True)
                            showSection = true;
                        else if (showTags[key] == RPNCalc.c_False)
                            showSection = false;
                        else
                            evalErr = true;
                    }
                    else
                        showSection = false;
                    #endregion

                    openPIndex = docNav.GetParagraphIndexWith(tag, scanFrom);
                    closePIndex = docNav.GetParagraphIndexWith(tagClo, openPIndex);
                             
                    if (openPIndex == -1 &&
                        closePIndex == -1)
                    {
                        keepSearching = false;
                        break;
                    } 
                    else if (openPIndex == -1 && closePIndex != -1)
                    {                        
                        Paragraph delP = docNav.doc.Paragraphs[closePIndex];

                        if (delP != null && delP.MagicText != null)
                            if (delP.Text.Contains(tagClo))
                            {
                                missingOTags.Add(tag); // Remove close tag and keep going.
                                delP.ReplaceText(tagClo, "");
                            }

                        keepSearching = false;
                        break;
                    }
                    if (closePIndex == -1)
                    {
                        keepSearching = false;
                        break;
                    }

                    //Console.WriteLine("Removing " + tag + " region.");

                    //Set  that openPIndex was the last place scanned to find a close open tag.
                    scanFrom = openPIndex; 

                    pTagO = docNav.doc.Paragraphs[openPIndex];
                    pTagClo = docNav.doc.Paragraphs[closePIndex];

                    if (closePIndex < openPIndex)
                    {
                        //This is backwards.
                        pTagO.ReplaceText(tag, "");
                        pTagClo.ReplaceText(tagClo, "");
                        otherErr.Add("Found closing tag: " + tagClo + " before opening tag: " + tag);
                        continue;
                    }

                    if (showSection)
                    {
                        //Remove just the tag references.
                        SecondaryMsgChanged("Showing " + tag + " area.");
                        pTagO.ReplaceText(tag, "");
                        pTagClo.ReplaceText(tagClo, "");                      
                    }
                    else
                    {
                        #region Remove Whole Section Tables and Paragraphs

                        //Remove the whole section.
                        //Find tables to remove within the range of text indexes.

                        int textIndexOfOpen = docNav.GetTextIndexOf(tag);
                        int textIndexOfClose = docNav.GetTextIndexOf(tagClo);
                        List<Table> tablesToRemove = new List<Table>();
                        foreach (Table table in docNav.doc.Tables)
                            if (table.Index >= textIndexOfOpen &&
                                table.Index <= textIndexOfClose)
                                tablesToRemove.Add(table);

                        //Remove tables.
                        int remTableCount = tablesToRemove.Count;
                        int tableTrack = 1;
                        //SecondaryMsgChanged("Hiding " + tag + " area. (" + remTableCount +" tables)");

                        for (int t = remTableCount; t > 0; t--)
                        {
                            if (bgw.CancellationPending) return ErrCode;
                            SecondaryMsgChanged(string.Format("Hiding {0} area. ({1} of {2} tables)", tag, tableTrack++, remTableCount));
                            tablesToRemove[t - 1].Remove();
                        }

                        //Remove Paragraphs between open/close tags.
                        //Re-acquire the range of Paragraphs
                        openPIndex = scanFrom = docNav.GetParagraphIndexWith(tag, scanFrom);
                        closePIndex = docNav.GetParagraphIndexWith(tagClo, openPIndex);
                        //docNav.doc.Paragraphs.RemoveRange(openPIndex, closePIndex - openPIndex);
                        int rangeP = (closePIndex - 1 - openPIndex);
                        int parTrack = 1;
                        for (int p = closePIndex - 1; p > openPIndex; p--)
                        {
                            if (bgw.CancellationPending) return ErrCode;
                            SecondaryMsgChanged(string.Format("Hiding {0} area. ({1} of {2} paragraphs)", tag, parTrack++, rangeP));
                            docNav.doc.Paragraphs[p].Remove(false);
                        }

                        //Remove the
                        int openAt = pTagO.Text.IndexOf(tag);

                        //If all text in the P comes after the tag, delete the whole P.
                        if (openAt == 0)
                            pTagO.Remove(false);
                        else //Else delete all text after.
                            pTagO.RemoveText(openAt, pTagO.Text.Length - openAt);

                        int closeAt = pTagClo.Text.IndexOf(tagClo);

                        if (closeAt == (pTagClo.Text.Length - tagClo.Length))
                            pTagClo.Remove(false);
                        else //if (closeAt > 0)
                        {
                            string removeText = pTagClo.Text.Substring(0, closeAt + tagClo.Length);

                            if (pTagClo.MagicText != null)
                            {
                                pTagClo.ReplaceText(removeText, "");
                            }
                            else
                                Console.WriteLine("Unable to remove text " + removeText + " from " + pTagClo.Text);
                        }
                        #endregion
                    }
                }

                SecondaryProgressChanged(50 + (((100 / count) * i) / 2));

            }

            #endregion

            //Add Errors here.
            //
            #region Compile Any error message.

            if (evalErrTags.Count > 0)
            {
                ErrCode += "The following Show Actions did not evaulate to (true) or (false) as expected. " +
                    "Try checking the Action for syntactical mistakes: ";
                foreach (string badEval in evalErrTags)
                    ErrCode += badEval + ", ";
                ErrCode.TrimEnd(new char[] { ',', ' ' });
            }
            if (missingOTags.Count > 0)
            {

                if (ErrCode != string.Empty)
                    ErrCode += Environment.NewLine + Environment.NewLine;

                ErrCode += "The document template: " + Path.GetFileName(args.templateFilePath) +
                    " contains orphaned Show tag(s) with no openings. Make sure the opening tags " +
                "are complete and not possibly nested within another Show tag region: ";
                foreach (string oTagMiss in missingOTags)
                    ErrCode += oTagMiss + ", ";
                ErrCode.TrimEnd(new char[] { ',', ' ' });
                
            }
            if (otherErr.Count > 0)
            {
                if (ErrCode != string.Empty)
                    ErrCode += Environment.NewLine + Environment.NewLine;

                foreach (string miscErr in otherErr)
                    ErrCode += miscErr + Environment.NewLine;
            }
            #endregion

            SecondaryMsgChanged("Show Tag Areas Handled!");
            SecondaryProgressChanged(100);

            if (ErrCode == string.Empty)
                return string.Empty;
            else
                return ErrCode;
        }

        private static string ReplaceTags(DocNav docNav, Dictionary<string, string> dictionary, bool checkExists = false)
        {
            string ErrCode = G.c_NoErr;

            List<string> replaceAgain = new List<string>();

            MainMsgChanged("Replacing Fill & Action Tags");

            int count = dictionary.Keys.Count;
            double dblCount = (double)count;
            double progress = 0;
            int prgs = 0;
            List<string> keys = dictionary.Keys.ToList();

            for (int i = 0; i < count; i++)
            {
                string key = keys[i];
                string tag = G.TagWrap(key);
                string value = dictionary[key];

                if (value.Contains('<'))
                {
                    foreach (string otherKey in dictionary.Keys)
                    {
                        if (key != otherKey)
                            if (value.Contains(G.TagWrap(otherKey)))
                                replaceAgain.Add(otherKey);
                    }
                }

                progress = (100 / dblCount) * i;
                prgs = (int)progress;
                if (prgs != progress2)
                {
                    SecondaryProgressChanged(prgs);
                    if (bgw.CancellationPending) return ErrCode;
                } 

                SecondaryMsgChanged("Filling: " + tag);

                docNav.doc.ReplaceText(tag, dictionary[key]);          
            }

            if (bgw.CancellationPending) return ErrCode;
            SecondaryMsgChanged("Second Pass Filling...");
            foreach (string otherKey in replaceAgain)
                docNav.doc.ReplaceText(G.TagWrap(otherKey), dictionary[otherKey]);


            //Fill License Specific information.
            foreach (string otherKey in UserLicense.values.Keys)
                docNav.doc.ReplaceText(G.TagWrap(otherKey), UserLicense.values[otherKey]);


            SecondaryProgressChanged(100);
            SecondaryMsgChanged("Tags Filled!");
            MainMsgChanged("Tags Filled!");

            if (ErrCode == G.c_NoErr)
                return string.Empty;
            else
                return Environment.NewLine + ErrCode;
        }

        private static string ReplaceSuffixModTags(DocNav docNav, DocGenArgs args)
        {
            string ErrCode = G.c_NoErr;

            List<Action> sufMods = new List<Action>();

            foreach (FormAndFuncs ff in args.formFuncs)
                foreach (string key in ff.actions.Keys)
                    if (ff.actions[key].actionType == ActionType.SfxMod)
                        sufMods.Add(ff.actions[key]);

            MainMsgChanged("Replacing Suffix Modifiers");

            int count = sufMods.Count;
            double dblCount = (double)count;
            int pgrs = 0;
            double progress = 0;

            for (int i = 0; i < count; i++)
            {
                
                Action sufMod = sufMods[i];
                string wildcardSuffix = sufMod.GetWildcardSuffix();
                wildcardSuffix = Regex.Escape(wildcardSuffix);
                string wildcardRegex = @"<\S+?" + wildcardSuffix + ">";

                List<string> matchesWildcardAction = docNav.doc.FindUniqueByPattern(
                    wildcardRegex, RegexOptions.Singleline);

                int matchCount = matchesWildcardAction.Count;
                if (matchCount > 0)
                {
                    SecondaryMsgChanged("Replacing " + sufMod.tagName + "( " + matchCount + " )");
                }

                foreach (string match in matchesWildcardAction)
                {
                    //Exchange wildcard for the fieldname it sits next to. 
                    string wildcardPrefix = G.TagGetPrefix(match, wildcardSuffix);

                    if (args.fields.ContainsKey(wildcardPrefix))
                    {
                        args.fields.anyField = args.fields[wildcardPrefix];
                        string result = sufMod.EvalStructure(ref args.fields);
                        docNav.doc.ReplaceText(match, result);

                        if (bgw.CancellationPending) return ErrCode;
                    }
                }

                progress = (100 / dblCount) * i;
                pgrs = (int)progress;
                SecondaryProgressChanged(pgrs);
            }

            SecondaryProgressChanged(100);
            SecondaryMsgChanged("All Suffixes Replaced!");

            if (ErrCode == G.c_NoErr)
                return string.Empty;
            else
                return Environment.NewLine + ErrCode;
        }

        private static string ApplyCommandTags(DocNav docNav)
        {
            string ErrCode = G.c_NoErr;
            string delTable = G.TagWrap(Fields.c_DelTable);
            string delCol = G.TagWrap(Fields.c_DelCol);
            string delRow = G.TagWrap(Fields.c_DelRow);
            string delPar = G.TagWrap(Fields.c_DelPar);
            string shrinkRow = G.TagWrap(Fields.c_Shrink);

            MainMsgChanged("Applying Command Tags");

            SecondaryMsgChanged("Removing Tables.");
            //Remove Table
            Table remTable = docNav.GetTableWith(delTable);
            while (remTable != null)
            {
                remTable.Remove();
                remTable = docNav.GetTableWith(delTable);
            }

            if (bgw.CancellationPending) return ErrCode;

            SecondaryProgressChanged(20);
            SecondaryMsgChanged("Removing Columns.");
            // Remove Column
            DocLoc remCol = docNav.GetColWith(delCol);
            while (remCol != null)
            {
                Table table = docNav.GetTable(remCol);
                table.RemoveColumn(remCol.C);
                remCol = docNav.GetColWith(delCol);
            }

            if (bgw.CancellationPending) return ErrCode;

            SecondaryProgressChanged(40);
            SecondaryMsgChanged("Removing Rows.");
            //Remove Row
            Row remRow = docNav.GetRowWith(delRow);
            while (remRow != null)
            {
                remRow.Remove();
                remRow = docNav.GetRowWith(delRow);
            }

            if (bgw.CancellationPending) return ErrCode;

            SecondaryProgressChanged(60);
            SecondaryMsgChanged("Removing Paragraphs.");
            //Remove Paragraph
            List<Paragraph> paragraphs = docNav.doc.Paragraphs;
            Paragraph remPar = docNav.GetParagraphWith(paragraphs, delPar);
            while (remPar != null)
            {
                remPar.Remove(false);
                remPar = docNav.GetParagraphWith(paragraphs, delPar);
            }

            if (bgw.CancellationPending) return ErrCode;

            //Find Remove Paragraph within "text"
            List<string> matches = docNav.doc.FindUniqueByPattern(".*?<delPar>.*?", RegexOptions.Multiline);
            foreach (string match in matches)
            {
                string remStr = match;

                if (remStr[0] == '\t')
                    remStr = remStr.TrimStart('\t');

                docNav.doc.ReplaceText(remStr, "");
            }

            if (bgw.CancellationPending) return ErrCode;

            SecondaryProgressChanged(80);
            SecondaryMsgChanged("Shrinking Rows.");
            //AutoFit Row
            Row fitRow = docNav.GetRowWith(shrinkRow);
            while (fitRow != null)
            {
                bool hasShrink = false;
                foreach (Paragraph p in fitRow.Paragraphs)
                    if (p.Text.Contains(shrinkRow))
                        hasShrink = true;

                if (hasShrink)
                {
                    fitRow.ReplaceText(shrinkRow, "");
                    fitRow.Height = 0;
                    fitRow = docNav.GetRowWith(shrinkRow);
                }
                else break;
            }

            SecondaryProgressChanged(100);
            MainMsgChanged("Done Applying Commands!");
            SecondaryMsgChanged("Done Applying Commands!");

            if (ErrCode == G.c_NoErr)
                return string.Empty;
            else
                return Environment.NewLine + ErrCode;
        }


        #endregion


    }
}
