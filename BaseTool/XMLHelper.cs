using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BaseTool
{
    public static class XmlHelper
    {
      
        public static string GetValue(XElement element, string key)
        {
          return (string)element.Attribute(XName.Get(key));
        }

        public static XDocument GetXDocument(string xml)
        {
            XDocument xDoc;
            using (TextReader txtReader = new StringReader(xml))
            {
                xDoc=XDocument.Load(txtReader);
                txtReader.Close();
            }
            return xDoc;
        }

        public static XElement[] GetXElements(XElement parentElement,string tag)
        {
            return string.IsNullOrEmpty(tag) ? new XElement[]{} : parentElement.Elements(XName.Get(tag)).ToArray();
        }
    }
}
