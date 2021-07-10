using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportGen
{
    [Serializable]
    public class FlatExprs : FlatButton
    {
        #region Variables

        #region Private

        private string _exprs;

        #endregion //Private
        #region Public

        public string Exprs { 
            get{return _exprs;} 
            set{_exprs = value;}
        }


        #endregion //Public

        #endregion

        public FlatExprs()
        {

        }


    }
}

