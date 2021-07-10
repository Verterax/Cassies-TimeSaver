using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace ReportGen
{
    class Updates
    {
        public static string DownloadTestFile(string URL, string fileName)
        {
            string updateFolder = G.GetUpdateFolder();

            if (!Directory.Exists(updateFolder))
                Directory.CreateDirectory(updateFolder);

            string ErrCode = ""; //WebData.Download(URL, fileName);
            return ErrCode;           
        }
    }

    #region Web Data

    public delegate void BytesDownloadedEventHandler(ByteArgs e);

    public class ByteArgs : EventArgs
    {

        #region Properties
        private int _downloaded;
        private int _total;
        public int Downloaded
        {
            get { return _downloaded; }
            set { _downloaded = value; }
        }
        public int Total
        {
            get { return _total; }
            set { _total = value; }
        }

        #endregion

        #region Init / Load


        public ByteArgs(int dataLength)
        {
            Init();
            this.Total = dataLength;
        }

        private void Init()
        {
            this.Downloaded = 0;
            this.Total = 0;
        }

        #endregion

    }

 



    #endregion
}
