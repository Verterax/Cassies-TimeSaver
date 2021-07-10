using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportGen
{
    [Serializable]
    public class FlatBook
    {
        public string lastTemplateFilePath;
        public List<FlatFormAndFuncs> flatLeftPane;
        public List<FlatFormAndFuncs> flatRightPane;

        public FlatBook()
        {
            Init();
        }

        private void Init()
        {
            lastTemplateFilePath = string.Empty;
            flatLeftPane = new List<FlatFormAndFuncs>();
            flatRightPane = new List<FlatFormAndFuncs>();
        }
    }
}
