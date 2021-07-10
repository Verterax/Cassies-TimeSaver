using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportGen
{
    [Serializable]
    public class FlatAction : FlatBranch
    {
        #region Variables

        #region Private

        private int _actionType;
        private string _tagName;

        #endregion //Private
        #region Public

        public string TagName { 
            get{return _tagName;} 
            set{_tagName = value;}
        }
        public int ActionType
        {
            get { return _actionType; }
            set { _actionType = value; }
        }

        #endregion //Public

        #endregion

        public FlatAction()
        {

        }
    }
}
