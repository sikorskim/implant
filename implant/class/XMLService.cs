using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace implant
{
    public static class XMLService
    {
        static readonly string xAesIv = "C'0/.O6g2p}sc]-W";
        static readonly string xSalt = "YLs&bJuqsJ]I|uc^qVx_+odsk(>U?pX>stFbLpg:Ed]Syr#Ve9pFr81gf}9Z-AR#";
            

        /// <summary>
        /// Writes new relationshio data to ~/_rels/document.xml.rels file.
        /// This operation is required to avoid deletion of encrypted file after OOXML file modification.
        /// </summary>
        public static void writeRels(string ext)
        {
            try
            {
                XMLdata data = getRelsPath(ext);
                string path = data.relsPath;
                string type = data.relType;

                if (!File.Exists(path))
                {
                    XmlTextWriter writer = new XmlTextWriter(path, null);
                    writer.WriteStartElement("Relationships");
                    writer.WriteEndElement();
                    writer.Close();
                }

                XNamespace xmlns = XNamespace.Get("http://schemas.openxmlformats.org/package/2006/relationships");

                XElement xml = XElement.Load(path);
                xml.Add(
                new XElement(xmlns + "Relationship",
                new XAttribute("Id", "rId99"),
                new XAttribute("Type", type),
                new XAttribute("Target", "temp.xml"))
                );
                xml.Save(path);
            }
            catch (Exception ex)
            {
                LogService.add(ex.ToString());
            }
        }

        public static void writeContentTypes(string ext)
        {
            try
            {
                XMLdata data = getRelsPath(ext);
                string path = data.contentTypesPath;

                //if (!File.Exists(path))
                //{
                //    XmlTextWriter writer = new XmlTextWriter(path, null);
                //    writer.WriteStartElement("Relationships");
                //    writer.WriteEndElement();
                //    writer.Close();
                //}

                XNamespace xmlns = XNamespace.Get("http://schemas.openxmlformats.org/package/2006/content-types");

                XElement xml = XElement.Load(path);
                xml.Add(
                new XElement(xmlns + "Override",
                new XAttribute("PartName", data.partName),
                new XAttribute("ContentType", data.contentType)
                ));
                xml.Save(path);
            }
            catch (Exception ex)
            {
                LogService.add(ex.ToString());
            }
        }

        static XMLdata getRelsPath(string ext)
        {
            XMLdata data = new XMLdata();
            data.contentTypesPath =@"temp\[Content_Types].xml";

            if (ext == "*.docx")
            {
                data.relsPath = "temp\\word\\_rels\\document.xml.rels";

                //do poprawy - sprawdzić ścieżkę w pliku docx.
                data.relType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/custom";
                data.partName = "/word/temp.xml";
                data.contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.temp+xml";
            }

            if (ext == "*.xlsx")
            {
                data.relsPath = "temp\\xl\\_rels\\workbook.xml.rels";
                data.relType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet";
            }
            else if (ext == "*.pptx")
            {
                data.relsPath = "temp\\ppt\\_rels\\presentation.xml.rels";
                data.relType = "http://schemas.openxmlformats.org/package/2006/relationships";
            }

            return data;
        }

        public static void writeImplant(string ext, string text, string sign)
        {
            try
            {
                string path = FileService.getImplantPath(ext);

                XDocument doc = new XDocument(new XElement("root", new XElement("msg", text), new XElement("sign", sign)));
                doc.Save(path);
            }
            catch (Exception ex)
            {
                LogService.add(ex.ToString());
            }
        }

        public static string readImplant(string ext)
        {
            string ciphertext = null;

            try
            {
                string path = FileService.getImplantPath(ext);
                XDocument doc = XDocument.Load(path);
                ciphertext = doc.Element("root").Element("msg").Value.ToString();
            }
            catch (Exception ex)
            {
                LogService.add(ex.ToString());
            }

            return ciphertext;
        }

        public static string readSign(string ext)
        {
            string sign = null;

            try
            {
                string path = FileService.getImplantPath(ext);
                XDocument doc = XDocument.Load(path);
                sign = doc.Element("root").Element("sign").Value.ToString();
            }
            catch (Exception ex)
            {
                LogService.add(ex.ToString());
            }

            return sign;
        }

        public static bool checkRSAkey(string key)
        {
            bool valid = true;

            try
            {
                XDocument doc = XDocument.Parse(key);
                if (doc.Element("RSAKeyValue").Element("Modulus").Value == null || doc.Element("RSAKeyValue").Element("Exponent").Value == null)
                {
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                valid = false;
                LogService.add(ex.ToString());
            }

            return valid;
        }

        static void createXml(string path)
        {
            XDocument doc = new XDocument();
            XElement xElement = new XElement("root");
            doc.Add(xElement);
            doc.Save(path);
        }

        public static XDocument getDocument(string path)
        {
            XDocument doc = null;
            try
            {
                doc = XDocument.Load(path);
            string tmp = doc.Element("root").Value.ToString();
            if (tmp != string.Empty)
            {
                tmp = decrypt(tmp);
                doc.Element("root").Value = string.Empty;
                string tmpDoc = doc.ToString();

                tmpDoc = tmpDoc.Insert(6, tmp);
                doc = XDocument.Parse(tmpDoc);
            }
        }
            catch (Exception ex)
            {
                LogService.add(ex.ToString());
                doc = null;
            }
    Console.WriteLine("getDoc: " + doc);
            return doc;
        }

        public static XElement getElement(string path, string name)
        {
            XElement element = null;
            try
            {
                XDocument doc = getDocument(path);
                element = doc.Element("root").Element(name);
            }
            catch (Exception ex)
            {
                LogService.add(ex.ToString());
            }

            return element;
        }

        public static XElement getElement(string path, string name, string attribute, string value)
        {
            XElement element = null;
            try
            {
                XDocument doc = XDocument.Load(path);
                string tmpElem = doc.Element("root").Value.ToString();
                tmpElem = decrypt(tmpElem);
                doc.Element("root").Value = string.Empty;
                string tmpDoc = doc.ToString();
                tmpDoc = tmpDoc.Insert(6, tmpElem);
                doc = XDocument.Parse(tmpDoc);
                element = doc.Element("root").Elements(name).Single(p => p.Attribute(attribute).Value == value);
            }
            catch (Exception ex)
            {
                LogService.add(ex.ToString());
            }

            return element;
        }

        public static void addElement(string path, XElement xElement)
        {
            XDocument doc = XDocument.Load(path);
            XElement rootElement = doc.Element("root");
            string tmp = rootElement.Value;

            if (tmp != string.Empty)
            {
                tmp = rootElement.Value;
                tmp = decrypt(tmp);
            }

            tmp += xElement.ToString();
            tmp = encrypt(tmp);
            rootElement.Value = tmp;
            doc.Save(path);
        }

        public static void editElement(string path, string element, XElement newElement)
        {
            XDocument doc = getDocument(path);
            XElement rootElement = doc.Element("root");
            XElement oldElement = rootElement.Element(element);

            foreach (XAttribute oldAtr in oldElement.Attributes())
            {
                var newAtr = newElement.Attributes().Single(p => p.Name == oldAtr.Name);
                oldAtr.SetValue(newAtr.Value);
            }

            string tmp = oldElement.ToString();
            tmp = encrypt(tmp);
            rootElement.Value = tmp;
            doc.Save(path);
        }

        //public static void editElement(string path, string element, string attribute, string value, XElement newElement)
        //{
        //    XDocument doc = getDocument(path);
        //    XElement oldElement = doc.Element("root").Elements(element).Single(p => p.Attribute(attribute).Value == value);

        //    foreach (XAttribute oldAtr in oldElement.Attributes())
        //    {
        //        var newAtr = newElement.Attributes().Single(p => p.Name == oldAtr.Name);
        //        oldAtr.SetValue(newAtr.Value);
        //    }

        //    doc.Save(path);
        //}
        public static void deleteElement(string path, string element, string attribute, string value)
        {
            XDocument doc = getDocument(path);
            XElement rootElement = doc.Element("root");

            XElement elementToDel = rootElement.Elements(element).Single(p => p.Attribute(attribute).Value == value);
            elementToDel.Remove();
            string tmpElem = rootElement.Value.ToString();

            if (tmpElem != string.Empty)
            {
                tmpElem = encrypt(tmpElem);
                rootElement.Value = tmpElem;
            }

            doc.Save(path);
        }

        public static void initialSetup()
        {
            string currentDir = FileService.getCurrentDirPath();
            createXml(currentDir + @"\settings");
            Setting.initialSetup();
            createXml(currentDir + @"\myKeys");
            createXml(currentDir + @"\trustedContacts");
            createXml(currentDir + @"\logs");
        }

        public static bool checkDublicates(string path, string elementName, string attributeName, string value)
        {
            bool unique = false;

            try
            {
                XDocument doc = getDocument(path);
                XElement element = doc.Element("root").Elements(elementName).Single(p => p.Attribute(attributeName).Value == value);
            }
            catch (Exception)
            {
                unique = true;
            }

            return unique;
        }
        static string encrypt(string input)
        {
            string key = Setting.userPassword;

            AESservice aes = new AESservice(xSalt, xAesIv);
            CryptObject cryptObject = aes.encrypt(input, key);
            string output = cryptObject.value;

            return output;
        }

        static string decrypt(string input)
        {
            string output = null;
            string key = Setting.userPassword;

            AESservice aes = new AESservice(xSalt, xAesIv);
            CryptObject cryptObject = aes.decrypt(input, key);

            if (cryptObject.operationSuccess)
            {
                output = cryptObject.value;
            }
            else
            {
                output = null;
            }

            return output;
        }

        public static bool checkPassword(string input)
        {
            bool valid = true;
            Setting.userPassword = HashService.compute(input, "SHA512");
            try
            {
                string temp = getDocument("settings").ToString();
            }
            catch (Exception)
            {
                valid = false;
            }

            return valid;
        }
    }
}
