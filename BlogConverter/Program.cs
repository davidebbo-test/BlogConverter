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
                var title = entry.Element(ns + "title").Value;
                title = FixQuotes(title);

                var link = entry.Elements(ns + "link").Where(e => e.Attribute("rel").Value == "alternate").FirstOrDefault();
                if (link == null) continue;
                string fileName = Path.GetFileNameWithoutExtension(link.Attribute("href").Value);
                //Console.WriteLine(fileName);

                DateTime publishedDate = DateTime.Parse(entry.Element(ns + "published").Value);
                string publishedString = publishedDate.ToString("yyyy-MM-dd");
                //Console.WriteLine(publishedString);

                var tags = from category in entry.Elements(ns + "category")
                           where category.Attribute("scheme").Value.Contains("ns#")
                           select category.Attribute("term").Value;

                string tagString = String.Join(" ", tags);
                Console.WriteLine(tagString);

                string fullFileName = String.Format("{0}-{1}.markdown", publishedString, fileName);
                Console.WriteLine(fullFileName);

                string contentTemplate=
@"---
layout: post
title:  ""{0}""
categories: {1}
---

Enter content here!
";

                File.WriteAllText(fullFileName, String.Format(contentTemplate, title, tagString));
            }
        }

        static string FixQuotes(string s)
        {
            return s.Replace('‘', '\'').Replace('’', '\'');
        }
    }
}
