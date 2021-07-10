using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportGen
{
    [Serializable]
    public class FlatButton
    {

        #region Variables

        #region Private

        private string _label;
        private string _type;

        #endregion //Private
        #region Public

        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        #endregion //Public

        #endregion

        public FlatButton()
        {
            
        }

    }

}
