using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;


namespace ReportGen
{
    public static class UserLicense
    {
        #region Consts
        private const string password = "MoreBoom";
        private const string initVec = "StuffFor";
        private const string licenseFile = "CTLicense.lic";
        private const string expDate = "08/01/2014";
        private const string c_PermissionsKey = "permissions";

        //Const Properties
        public static DateTime Expiration
        {
            get
            {
                DateTime date = DateTime.Now;
                DateTime.TryParse(expDate, out date);
                return date;
            }
        }
        public static string ExpDesc
        {
            get { //return "for school year 2013-2014";
                return " beta license expires 08/01/2014 ";
            }
        }
        public static string UserName
        {
            get { return values["username"]; }
        }

        #endregion

        #region Variables
        public static Dictionary<string, string> values = new Dictionary<string, string>();
        #endregion

        #region Initialize
        public static bool Init()
        {
            try
            {
                string licensePath = Application.StartupPath + "\\" + licenseFile;

                if (!File.Exists(licensePath))
                {
                    G.ErrorShow("Unable to locate a license file.", "License file missing.");
                    return false;
                }

                string result = LicReader.ReadLicense(licensePath, password, initVec);

                if (result == "")
                {
                    G.ErrorShow("Unable to open License file. Please elevate your Windows user account privelages to read the license file.",
                        "Cannot read License file.");
                    return false;
                }

                PopulateLicenseValues(result);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        private static void PopulateLicenseValues(string licenseData)
        {
            char[] splitArr = new char[1] { '\n' };
            char[] splitPair = new char[1] { '=' };
            char[] trimChar = new char[3] { '\"', '\r', ' ' };
            string[] pairs = licenseData.Split(splitArr);

            foreach (string pair in pairs)
            {
                string[] items = pair.Split(splitPair);
                string key = items[0].Trim(trimChar);
                string value = items[1].Trim(trimChar);
                values.Add(key, value);
            }

            return;
        }


        #endregion

        #region Check License

        public static bool IsValid()
        {
            if (!Init()) //Invalid License.
                return false;

            if (DateTime.Today > Expiration)
                return false;
            else
                return true;
        }

        #endregion

        #region Document Package Permissions

        public static int[] GetDocPermissions()
        {
            string permsString = "";
            List<int> permissions = new List<int>();

            if (values.ContainsKey(c_PermissionsKey))
            {
                permsString = values[c_PermissionsKey];
                {
                    string[] perms = permsString.Split(new[] { ',' });

                    foreach (string perm in perms)
                    {
                        permissions.Add(Int32.Parse(perm.Trim()));
                    }
                }
            }

            return permissions.ToArray();
        }

        #endregion

    }
}