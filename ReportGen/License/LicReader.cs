using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace ReportGen
{
    public static class LicReader
    {
        public static string ReadLicense(string inputFilePath, string password, string initVec)
        {
            string result = string.Empty;

            try
            {
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);
                byte[] vec = UE.GetBytes(initVec);
                result = string.Empty;

                FileStream fsCrypt = new FileStream(inputFilePath, FileMode.Open);
                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateDecryptor(key, vec),
                    CryptoStreamMode.Read);

                StreamReader streamRead = new StreamReader(cs);
                result = streamRead.ReadToEnd();

                streamRead.Close();
                cs.Close();
                fsCrypt.Close();

            }
            catch (Exception ex)
            {
                return "The Error says: " + ex.Message;
            }

            return result;
        }
    }
}
