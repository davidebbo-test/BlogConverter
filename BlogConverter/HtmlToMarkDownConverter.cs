using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogConverter
{
    class HtmlToMarkDownConverter
    {
        private HtmlDocument _doc;
        private StringBuilder _sb = new StringBuilder();

        public HtmlToMarkDownConverter(string html)
        {
            html = Util.FixQuotes(html);

            _doc = new HtmlDocument();
            _doc.LoadHtml(html);
        }

        public string Convert()
        {
            ProcessNodes(_doc.DocumentNode.ChildNodes);

            return _sb.ToString();
        }

        void ProcessNodes(HtmlNodeCollection nodes)
        {
            foreach (HtmlNode node in nodes)
            {
                switch (node.Name)
                {
                    case "p":
                    case "ul":
                        _sb.AppendLine();
                        ProcessNodes(node.ChildNodes);
                        _sb.AppendLine();
                        break;

                    case "li":
                        _sb.Append("- ");
                        ProcessNodes(node.ChildNodes);
                        _sb.AppendLine();
                        break;

                    case "h3":
                        _sb.Append("### ");
                        ProcessNodes(node.ChildNodes);
                        _sb.AppendLine();
                        break;

                    case "strong":
                    case "em":
                        _sb.Append("**");
                        ProcessNodes(node.ChildNodes);
                        _sb.Append("**");
                        break;

                    case "a":
                        if (String.IsNullOrWhiteSpace(node.InnerText))
                        {
                            ProcessNodes(node.ChildNodes);
                        }
                        else
                        {
                            _sb.Append(String.Format("[{0}]({1})", node.InnerText, node.GetAttributeValue("href", "")));
                        }
                        break;

                    case "img":
                        string alt = node.GetAttributeValue("alt", "");
                        string src = node.GetAttributeValue("src", "");
                        _sb.Append(String.Format("![{0}]({1})", alt, src));
                        break;

                    default:
                        if (node.HasChildNodes)
                        {
                            ProcessNodes(node.ChildNodes);
                        }
                        else
                        {
                            string text = node.InnerText;
                            text = text.Replace("&nbsp;", "");
                            if (!String.IsNullOrWhiteSpace(node.InnerText))
                            {
                                _sb.Append(text);
                            }
                        }
                        break;
                }
            }
        }
    }
}
