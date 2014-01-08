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
        private int _liNesting = 0;

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
                        if (_liNesting > 0)
                        {
                            _sb.AppendLine();
                        }

                        _sb.Append("- ");
                        _liNesting++;
                        ProcessNodes(node.ChildNodes);
                        _liNesting--;
                        _sb.AppendLine();
                        break;

                    case "h3":
                        _sb.AppendLine();
                        _sb.Append("## ");
                        ProcessNodes(node.ChildNodes);
                        _sb.AppendLine();
                        break;

                    case "strong":
                        _sb.Append("**");
                        ProcessNodes(node.ChildNodes);
                        _sb.Append("**");
                        break;

                    case "em":
                        // They tend to go along with <strong>, so ignore them
                        ProcessNodes(node.ChildNodes);
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

                    case "pre":
                        _sb.AppendLine();

                        string classAttrib = node.GetAttributeValue("class", "");
                        if (classAttrib.Contains("csharp"))
                        {
                            _sb.AppendLine("{% highlight c# %}");
                            ProcessNodes(node.ChildNodes);
                            _sb.AppendLine("{% endhighlight %}");
                        }
                        else
                        {
                            _sb.AppendLine("```");
                            ProcessNodes(node.ChildNodes);
                            _sb.AppendLine();
                            _sb.AppendLine("```");
                        }
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
                            text = text.Replace("&gt;", ">");
                            text = text.Replace("&lt;", "<");
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
