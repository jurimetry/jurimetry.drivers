using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GladosSearcher.Service.Tjmg
{
    public class TjmgParser
    {
        private readonly static HtmlDocument _doc = new HtmlDocument();
        private const string formValuesXpath = "//select[contains(@name, '{0}')]/option/@value";
        public TjmgParser(string html)
        {
            _doc.LoadHtml(html);
        }

        public List<string> GetAllCourtSessionsNames()
        {
            return GetValuesFormByXpath(string.Format(formValuesXpath, "codigoOrgaoJulgador"));
        }

        public List<string> GetAllCourtDecisorsNames()
        {
            return GetValuesFormByXpath(string.Format(formValuesXpath, "codigoCompostoRelator"));
        }

        private List<string> GetValuesFormByXpath(string xpath) 
        {
            var valueNodes = _doc.DocumentNode.SelectNodes(xpath);
            if (valueNodes == null)
                Console.WriteLine("Deu ruim");

            return valueNodes
                        .Select(x => x.GetAttributeValue("Value", string.Empty))
                        ?.Where(x => !string.IsNullOrEmpty(x))
                        .ToList() ?? new List<string>();
        }
    }
}
