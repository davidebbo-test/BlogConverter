using HtmlAgilityPack;
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
                string title = entry.Element(ns + "title").Value;
                title = Util.FixQuotes(title);

                var link = entry.Elements(ns + "link").Where(e => e.Attribute("rel").Value == "alternate").FirstOrDefault();
                if (link == null) continue;
                string fileName = Path.GetFileNameWithoutExtension(link.Attribute("href").Value);

                DateTime publishedDate = DateTime.Parse(entry.Element(ns + "published").Value);
                string publishedString = publishedDate.ToString("yyyy-MM-dd");

                var tags = from category in entry.Elements(ns + "category")
                           where category.Attribute("scheme").Value.Contains("ns#")
                           select category.Attribute("term").Value;

                string tagString = String.Join(" ", tags);
                Console.WriteLine(tagString);

                string content = entry.Element(ns + "content").Value;

                var converter = new HtmlToMarkDownConverter(content);
                string markDown = converter.Convert();

                string fullFileName = String.Format("{0}-{1}.markdown", publishedString, fileName);
                Console.WriteLine(fullFileName);

                string contentTemplate=
@"---
layout: post
title:  ""{0}""
comments: true
categories: {1}
---

{2}
";

                File.WriteAllText(fullFileName, String.Format(contentTemplate, title, tagString, markDown));
            }
        }
    }
}
