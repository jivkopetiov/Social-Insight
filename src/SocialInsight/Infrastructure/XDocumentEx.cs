using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SocialInsight
{
    internal static class XDocumentEx
    {
        public static XNamespace GetMediaNamespace(this XDocument doc)
        {
            var attribute = doc.Root.Attribute(XNamespace.Xmlns + "media");

            if (attribute == null)
                return XNamespace.None;
            else
                return attribute.Value;
        }

        public static XNamespace GetXmlns(this XDocument doc)
        {
            var attribute = doc.Root.Attribute("xmlns");
            if (attribute == null)
                return XNamespace.None;

            return attribute.Value;
        }

        public static XNamespace GetNamespace(this XDocument doc, string namespaceName)
        {
            var attribute = doc.Root.Attribute(XNamespace.Xmlns + namespaceName);
            if (attribute == null)
                throw new InvalidOperationException(
                    string.Format("Missing namespace {0} from the root of the xml document", namespaceName));

            return attribute.Value;
        }

        public static XDocument LoadFromStream(Stream stream)
        {
            return LoadFromStream(stream, Encoding.UTF8);
        }

        public static XDocument LoadFromStream(Stream stream, Encoding encoding)
        {
            using (var reader = new StreamReader(stream, encoding))
            {
                var doc = XDocument.Load(reader);
                return doc;
            }
        }

        public static string ToStringWithDeclaration(this XDocument doc)
        {
            if (doc.Declaration == null)
            {
                doc.Declaration = new XDeclaration("1.0", "utf-8", null);
            }

            return doc.Declaration.ToString() + "\r\n" + doc.ToString();
        }

        public static byte[] ToBytes(this XDocument doc)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(memoryStream, new XmlWriterSettings() { Indent = true }))
                {
                    doc.Save(xmlWriter);
                }

                memoryStream.Position = 0;

                return memoryStream.ToArray();
            }
        }
    }
}
