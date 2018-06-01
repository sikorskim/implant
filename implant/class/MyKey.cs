using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace implant
{
    class MyKey
{
        public static string name { get; set; }
        public static string privPubKey { get; set; }
        public static string note { get; set; }
        static readonly string path = FileService.getCurrentDirPath() + @"\myKeys";
        static readonly string xElementName = "myKey";
        static readonly string xAttributeName = "name";

        public static void add()
        {
            XElement xMyKey = new XElement(xElementName);

            XAttribute xName = new XAttribute("name", name);
            XAttribute xPrivPubKey = new XAttribute("privPubKey", privPubKey);
            XAttribute xNote = new XAttribute("note", note);

            xMyKey.Add(xName);
            xMyKey.Add(xPrivPubKey);
            xMyKey.Add(xNote);

            XMLService.addElement(path, xMyKey);
        }

        public static void remove(string myKeyName)
        {
            XMLService.deleteElement(path, xElementName, xAttributeName, myKeyName);
        }

        public static void get(string myKeyName)
        {
            XElement xMyKey = XMLService.getElement(path, xElementName, xAttributeName, myKeyName);

            name = myKeyName;
            privPubKey = xMyKey.Attribute("privPubKey").Value;
            note = xMyKey.Attribute("note").Value;
        }

        public static IEnumerable getData()
        {
            IEnumerable data = null;
            try
            {
                XDocument doc = XMLService.getDocument(path);
                var xElements = doc.Element("root").Elements(xElementName);
                var xmlData = xElements.Select(p => new { Nazwa = p.Attribute("name").Value, Klucz = p.Attribute("privPubKey").Value, Notatka = p.Attribute("note").Value });
                data = xmlData.ToList();
            }
            catch (Exception ex)
            {
                LogService.add(ex.ToString());
                data = null;
            }


            return data;
        }

        public static bool checkName(string value)
        {
            bool unique = XMLService.checkDublicates(path, xElementName, xAttributeName, value);
            return unique;
        }

        public static List<string> getKeyNames()
        {
            List<string> names = new List<string>();

            XDocument doc = XMLService.getDocument(path);
            var xElements = doc.Element("root").Elements(xElementName);

            foreach (XElement element in xElements)
            {
                names.Add(element.Attribute("name").Value);
            }

            return names;
        }
    }
}
