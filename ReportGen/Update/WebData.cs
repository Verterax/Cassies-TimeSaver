using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.ComponentModel;

namespace ReportGen
{
    public enum DLReason
    {
        None, Manifest, DocFile, AppFile
    };

    public class DownloaderArgs
    {
        #region Variables
        public string URL;
        public string FileName;
        public string SaveTo;
        public string ErrCode;
        public DLReason Reason;
        #endregion

        #region Constructor / Init
        public DownloaderArgs()
        {
            Init();
        }

        public DownloaderArgs(
            string url, 
            string fileName, 
            string saveTo, 
            DLReason reason)
        {
            Init();

            URL = url;
            FileName = fileName;
            SaveTo = saveTo;
            Reason = reason;
        }

        private void Init()
        {
            URL = "";
            FileName = "";
            SaveTo = "";
            ErrCode = "";
            Reason = DLReason.None;
        }
        #endregion
    }

    public class WebData
    {
        //public static event BytesDownloadedEventHandler BytesDownloaded;

        public static string Download(
            string URL, 
            string fileName, 
            string saveTo, 
            BackgroundWorker bw)
        {

            if (saveTo == "")
                saveTo = G.GetUpdateFolder();

            string ErrCode = G.c_NoErr;
            byte[] download = new byte[0];
            byte[] dataBuffer = new byte[1024];
            int dataLength = 0;

            try
            {
                //Open request to URL
                WebRequest webReq = WebRequest.Create(URL.Replace(' ', '_'));
                WebResponse webResponse = webReq.GetResponse();
                Stream dataStream = webResponse.GetResponseStream();
                MemoryStream memStream = new MemoryStream();

                //get total size of download
                dataLength = (int)webResponse.ContentLength;

                //Declare downloaded bytes event args
                ByteArgs DLArgs = new ByteArgs(dataLength);
                int totalBytesDownloaded = 0;


                while (true)
                {
                    int bytesFromStream = dataStream.Read(dataBuffer, 0, dataBuffer.Length);
                    totalBytesDownloaded += bytesFromStream;
                    int percent = (int)((double)((double)totalBytesDownloaded/ (double)DLArgs.Total) * 100.0);

                    if (bytesFromStream > 0)
                    {
                        //Add new DL Data to Mem stream.
                        memStream.Write(dataBuffer, 0, bytesFromStream);

                        //Update Event Args
                        DLArgs.Downloaded = bytesFromStream;
                        if (bw != null)
                        {
                            //Console.WriteLine("Downloaded: " + totalBytesDownloaded + " Total: " + DLArgs.Total);
                            bw.ReportProgress(percent);
                        }
                    }
                    else //No bytes left to download
                    {
                        DLArgs.Downloaded = dataLength;

                        if (bw != null)
                            bw.ReportProgress(percent);

                        break;
                    }
                }

                //Covert stream to byte array.
                download = memStream.ToArray();

                //Release resources
                dataStream.Close();
                memStream.Close();

                //Write DL to File.
                FileStream fStream = new FileStream(saveTo, FileMode.Create);
                fStream.Write(download, 0, download.Length);
                fStream.Close();

            }
            catch (Exception ex)
            {
                ErrCode = ex.Message;
            }


            return ErrCode;
        }
    }
}
