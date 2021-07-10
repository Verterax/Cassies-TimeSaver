using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportGen
{

    [Serializable]
    public class FlatBranch : FlatButton
    {
        #region Variables

        #region Private

        private FlatIf _ifBranch;
        private FlatExprs _result;

        #endregion //Private
        #region Public

        public FlatIf IfBranch
        {get {return _ifBranch;}
         set {_ifBranch = value;}
        }
        public FlatExprs Result
        {
            get { return _result; }
            set { _result = value; }
        }

        #endregion //Public

        #endregion

        public FlatBranch()
        {

        }


    }
}
