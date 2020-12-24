using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladosSearcher.Helper
{
    public static class HtmlUtils
    {
        public static string GetClearTextFromNode(this HtmlNode node) 
        {
            if (node == null) return string.Empty;

            node = node.ClearNode();

            var sb = new StringBuilder();

            if (node.Name.Equals("#text"))
            {
                node.AppendNodeText(sb);
            }
            else
            {
                foreach (var childN in node.ChildNodes) 
                {
                    childN.AppendNodeText(sb);
                }
            }

            return sb.ToString().GetStringWihoutSpace();
        }

        private static void AppendNodeText(this HtmlNode node, StringBuilder sb)
        {
            if (node.ChildNodes.Any())
            {
                foreach (var childN in node.ChildNodes)
                    AppendNodeText(childN, sb);
            }
            else
            {
                if (node.Name.Equals("br"))
                    sb.AppendLine();
                else
                    sb.Append(node.InnerText);
            }
        }

        #region ClearNode
        private static readonly List<string> thearmsToReplace = new List<string>()
        {
            "script",
            "style",
            "#comment",
            "button",
            "i"
        };

        private static HtmlNode ClearNode(this HtmlNode node) 
        {
            if (node == null) return node;

            node.Descendants().ToList().RemoveAll(nd => thearmsToReplace.Contains(nd.Name));

            return node;
        }
        #endregion
    }
}
