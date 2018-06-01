using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace implant
{
    class TrustedContact
{
        public static string name { get; set; }
        public static string pubKey { get; set; }
        public static string note { get; set; }
        static readonly string path = FileService.getCurrentDirPath() + @"\trustedContacts";
        static readonly string xElementName = "trustedContact";
        static readonly string xAttributeName = "name";

        public static void add()
        {
            XElement xTrustedContact = new XElement("trustedContact");

            XAttribute xName = new XAttribute("name", name);
            XAttribute xPrivPubKey = new XAttribute("pubKey", pubKey);
            XAttribute xNote = new XAttribute("note", note);

            xTrustedContact.Add(xName);
            xTrustedContact.Add(xPrivPubKey);
            xTrustedContact.Add(xNote);

            XMLService.addElement(path, xTrustedContact);
        }

        //public static void update(string trustedContactName)
        //{
        //    XElement xTrustedContact = new XElement("myKey");

        //    XAttribute xName = new XAttribute("name", name);
        //    XAttribute xPubKey = new XAttribute("pubKey", pubKey);
        //    XAttribute xNote = new XAttribute("note", note);

        //    xTrustedContact.Add(xName);
        //    xTrustedContact.Add(xPubKey);
        //    xTrustedContact.Add(xNote);

        //    XMLService.editElement(path, xElementName, xAttributeName, trustedContactName, xTrustedContact);
        //}

        public static void remove(string trustedContactName)
        {
            XMLService.deleteElement(path, xElementName, xAttributeName, trustedContactName);
        }

        public static void get(string trustedContactName)
        {
            XElement xMyKey = XMLService.getElement(path, xElementName, xAttributeName, trustedContactName);

            name = trustedContactName;
            pubKey = xMyKey.Attribute("pubKey").Value;
            note = xMyKey.Attribute("note").Value;
        }

        public static IEnumerable getData()
        {
            IEnumerable data = null;
            try
            {
                XDocument doc = XMLService.getDocument(path);
                XElement rootElement = doc.Element("root");
                    var xElements = doc.Element("root").Elements(xElementName);
                    var xmlData = xElements.Select(p => new { Nazwa = p.Attribute("name").Value, Klucz = p.Attribute("pubKey").Value, Notatka = p.Attribute("note").Value });
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
    }
}
