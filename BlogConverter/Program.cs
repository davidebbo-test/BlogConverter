using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BlogConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            var root = XElement.Load(args[0]);
            XNamespace ns = "http://www.w3.org/2005/Atom";

            var entries = from entry in root.Elements(ns + "entry")
                          where entry.Element(ns + "category").Attribute("term").Value.Contains("kind#post")
                          select entry;
            foreach (var entry in entries)
            {
                var title = entry.Element(ns + "title");
                var content = entry.Element(ns + "content");
                Console.WriteLine(title.Value);
                Console.WriteLine(content);
            }


            //IEnumerable<XElement> tests =
            //    from el in root.Elements("Test")
            //    where (string)el.Element("CommandLine") == "Examp2.EXE"
            //    select el;
            //foreach (XElement el in tests)
            //    Console.WriteLine((string)el.Attribute("TestId"));
        }
    }
}
