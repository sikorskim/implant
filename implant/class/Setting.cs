using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace implant
{
    class Setting
    {
        public static string aesIv { get; set; }
        public static string salt { get; set; }
        public static int genPassLength { get; set; }
        public static int rsaKeyLength { get; set; }
        public static bool autoSignMsg { get; set; }
        public static string autoSignName { get; set; }
        public static string userPassword { get; set; }

        static readonly string path = FileService.getCurrentDirPath()+ @"\settings";
        static readonly string xElementName = "setting";

        public static void initialSetup()
        {
            XElement xSetting = new XElement("setting");

            string newAesIv = string.Empty;
            string newSalt = string.Empty;
            int newGenPassLength = 32;
            int newRsaKeyLength = 2048;
            bool newAutoSignMsg = false;
            string newAutoSignName = string.Empty;
            string newUserPassword = string.Empty;

            XAttribute xAesIv = new XAttribute("aesIv", newAesIv);
            XAttribute xSalt = new XAttribute("salt", newSalt);
            XAttribute xGenPassLength = new XAttribute("genPassLength", newGenPassLength);
            XAttribute xRsaKeyLength = new XAttribute("rsaKeyLength", newRsaKeyLength);
            XAttribute xAutoSignMsg = new XAttribute("autoSignMsg", newAutoSignMsg);
            XAttribute xAutoSignName = new XAttribute("autoSignName", newAutoSignName);
            XAttribute xUserPassword = new XAttribute("userPassword", newUserPassword);

            xSetting.Add(xAesIv);
            xSetting.Add(xSalt);
            xSetting.Add(xGenPassLength);
            xSetting.Add(xRsaKeyLength);
            xSetting.Add(xAutoSignMsg);
            xSetting.Add(xAutoSignName);
            xSetting.Add(xUserPassword);

            XMLService.addElement(path, xSetting);
        }

        public static bool checkSettingsExist()
        {
            bool exist = File.Exists(path);

            return exist;
        }

        public static void set()
        {
            XElement xSetting = new XElement("setting");

            XAttribute xAesIv = new XAttribute("aesIv", aesIv);
            XAttribute xSalt = new XAttribute("salt", salt);
            XAttribute xGenPassLength = new XAttribute("genPassLength", genPassLength);
            XAttribute xRsaKeyLength = new XAttribute("rsaKeyLength", rsaKeyLength);
            XAttribute xAutoSignMsg = new XAttribute("autoSignMsg", autoSignMsg);
            XAttribute xAutoSignName = new XAttribute("autoSignName", autoSignName);
            XAttribute xUserPassword = new XAttribute("userPassword", userPassword);

            xSetting.Add(xAesIv);
            xSetting.Add(xSalt);
            xSetting.Add(xGenPassLength);
            xSetting.Add(xRsaKeyLength);
            xSetting.Add(xAutoSignMsg);
            xSetting.Add(xAutoSignName);
            xSetting.Add(xUserPassword);


            XMLService.editElement(path, xElementName, xSetting);
        }

        public static void get()
        {
            try
            {
                XElement xSetting = XMLService.getElement(path, xElementName);

                aesIv = xSetting.Attribute("aesIv").Value;
                salt = xSetting.Attribute("salt").Value;
                genPassLength = Int32.Parse(xSetting.Attribute("genPassLength").Value);
                rsaKeyLength = Int32.Parse(xSetting.Attribute("rsaKeyLength").Value);
                autoSignMsg = Boolean.Parse(xSetting.Attribute("autoSignMsg").Value);
                autoSignName = xSetting.Attribute("autoSignName").Value;
                userPassword = xSetting.Attribute("userPassword").Value;
            }
            catch (Exception ex)
            {
                LogService.add(ex.ToString());
            }
        }
    }
}
