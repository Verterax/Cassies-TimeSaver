using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportGen
{
    [Serializable]
    public class FlatIf : FlatBranch
    {
        #region Variables

        #region Private

        private FlatExprs _ifExprs;
        private FlatBranch _else;

        #endregion //Private
        #region Public

        public List<FlatIf> ElseIfs;
        public FlatExprs IfExprs
        {
            get { return _ifExprs; }
            set { _ifExprs = value; }
        }
        public FlatBranch Else
        {
            get { return _else; }
            set { _else = value; }
        }

        #endregion //Public

        #endregion

        public FlatIf()
        {
            
        }


    }
}
