using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportGen
{
    [Serializable]
    public class FlatFormAndFuncs
    {
        public string lastActionTagSelected;
        public string lastRegexTagSelected;
        public FlatControl flatFF;
        public List<FlatAction> actionsList;
        public List<FlatRegexField> regexes;

        public FlatFormAndFuncs()
        {
            this.lastActionTagSelected = string.Empty;
            this.lastRegexTagSelected = string.Empty;
            this.flatFF = new FlatControl();
            this.actionsList = new List<FlatAction>();
            this.regexes = new List<FlatRegexField>();
        }

        public FormAndFuncs ToFF()
        {
            FormAndFuncs newFF = (FormAndFuncs)flatFF.RecurseCreateControls(flatFF);
            newFF.AddAutoLabels(newFF);
            //newFF.SetTabOrder(newFF, 20);
            newFF.PopulateActions(actionsList);
            newFF.lastActionTagSelected = this.lastActionTagSelected;
            newFF.lastRegexTagSelected = this.lastRegexTagSelected;

            return newFF;
        }
    }
}
