using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace implant
{
    public static class LogService
    {
        static readonly string path = FileService.getCurrentDirPath() + @"\logs";
        static readonly string xElementName = "log";

        public static void add(string content)
        {
            try
            {
                XElement xLog = new XElement(xElementName);

                XAttribute xTime = new XAttribute("time", DateTime.Now);
                XAttribute xContent = new XAttribute("content", content);

                xLog.Add(xTime);
                xLog.Add(xContent);

                XDocument doc = XDocument.Load(path);
                XElement rootElement = doc.Element("root");
                rootElement.Add(xLog);
                doc.Save(path);
            }
            catch (Exception)
            {}
        }

        public static IEnumerable getData()
        {
            XDocument doc = XDocument.Load(path);
            var xElements = doc.Element("root").Elements(xElementName);
            var xmlData = xElements.Select(p => new { Czas = DateTime.Parse(p.Attribute("time").Value).ToLocalTime(), Treść = p.Attribute("content").Value});
            IEnumerable data = xmlData.ToList();
            return data;
        }
    }
}

