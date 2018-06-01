using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;

namespace implant
{
    public static class ZIPService
    {
        static string workingDirectory = "temp";

        public static void unzip(string path)
        {
            try
            {
                ZipFile.ExtractToDirectory(path, workingDirectory);
            }
            catch (Exception ex)
            {
                LogService.add(ex.ToString());
            }
        }

        public static void zip(string sourceDirectory, string destinationFile)
        {
            try
            {
                ZipFile.CreateFromDirectory(sourceDirectory, destinationFile);
            }
            catch (Exception ex)
            {
                LogService.add(ex.ToString());
            }
        }
    }
}
