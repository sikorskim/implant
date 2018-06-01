using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace implant
{
    public static class FileService
    {
        public static void save(string input, string path)
        {
            try
            {  
                File.WriteAllText(path, input, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                LogService.add(ex.ToString());
            }
        }

        public static string openTxt(string path)
        {
            string text = null;

            try
            {
                text = File.ReadAllText(path, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                LogService.add(ex.ToString());
            }

            return text;
        }

        public static void deleteTemporaryFiles()
        {
            try
            {
                string path = "temp";
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception ex)
            {
                LogService.add(ex.ToString());
            }
        }

        public static bool checkForImplants(string ext)
        {
            bool exist = false;

            try
            {
                string path = getImplantPath(ext);

                if (File.Exists(path))
                {
                    exist = true;
                }
            }
            catch (Exception ex)
            {
                LogService.add(ex.ToString());
            }

            return exist;
        }

        public static string getImplantPath(string ext)
        {
            string savePath = null;

            if (ext == "*.docx")
            {
                savePath = "temp\\word\\temp.xml";
            }
            else if (ext == "*.xlsx")
            {
                savePath= "temp\\xl\\temp.xml";
            }
            else if (ext == "*.pptx")
            {
                savePath= "temp\\ppt\\temp.xml";
            }

            return savePath;
        }

        public static string getCurrentDirPath()
        {
            string path = Directory.GetCurrentDirectory();
            return path;
        }
    }
}
