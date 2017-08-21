using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Utility
{
    internal class ConcurrencyModeFixedRewriter
    {
        private static void Main(string[] args)
        {
            string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var files = Directory.GetFiles(directoryPath, "*.edmx");
            foreach (var file in files)
            {
                XDocument xmlDoc = XDocument.Load(file);
                XAttribute concurrencyAttribute = new XAttribute("ConcurrencyMode", "Fixed");

                IEnumerable<XElement> versionColumns =
                    from el in xmlDoc.Descendants()
                    where (string)el.Attribute("Name") == "RowVersion"
                    && (string)el.Attribute("Type") == "Binary"
                    select el;
                bool modified = false;
                foreach (XElement el in versionColumns)
                {
                    if (el.Attribute("ConcurrencyMode") == null) el.Add(concurrencyAttribute);
                    else el.SetAttributeValue("ConcurrencyMode", "Fixed");
                    modified = true;
                }
                if (modified)
                    xmlDoc.Save(file);
            }
        }
    }
}