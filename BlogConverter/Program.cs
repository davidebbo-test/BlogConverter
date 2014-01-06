using System;
using System.Collections.Generic;
using System.IO;
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

                var link = entry.Elements(ns + "link").Where(e => e.Attribute("rel").Value == "alternate").FirstOrDefault();
                if (link == null) continue;
                string fileName = Path.GetFileNameWithoutExtension(link.Attribute("href").Value);
                Console.WriteLine(fileName);

                var tags = from category in entry.Elements(ns + "category")
                           where category.Attribute("scheme").Value.Contains("ns#")
                           select category.Attribute("term").Value;

                foreach (var tag in tags)
                {
                    Console.Write(tag + " ");
                }

                //entry.Elements(ns + "category").Where(e => e.Attribute("scheme").Value.Contains("ns#")).Select(e=>)
            }
        }
    }
}
