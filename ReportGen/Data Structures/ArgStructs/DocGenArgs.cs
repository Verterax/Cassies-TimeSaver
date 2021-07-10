using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportGen
{
    public class DocGenArgs
    {

        #region Variables
        public string errCode;
        public string templateFilePath;
        public string newDocFilePath;
        public List<FormAndFuncs> formFuncs;
        public Fields fields;

        #endregion

        #region Constructors

        private void Init()
        {
            errCode = string.Empty;
            templateFilePath = string.Empty;
            newDocFilePath = string.Empty;
            formFuncs = new List<FormAndFuncs>();
            fields = new Fields();
        }

        public DocGenArgs()
        {
            Init();
        }

        public DocGenArgs(string templateFilePath, 
            List<FormAndFuncs> formFuncs, Fields fields)
        {
            Init();

            this.fields = fields;
            this.templateFilePath = templateFilePath;
            this.formFuncs = formFuncs;
        }

        #endregion //Constructors
    }
}
