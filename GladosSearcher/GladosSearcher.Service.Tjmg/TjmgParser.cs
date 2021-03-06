﻿using GladosSearcher.Domain;
using GladosSearcher.Helper;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace GladosSearcher.Service.Tjmg
{
    public class TjmgParser
    {
        #region Constantes
        private const string formValuesXpath = "//select[contains(@name, '{0}')]/option/@value";
        private const string courtAbreviation = "tjmg";
        #endregion

        private readonly static HtmlDocument _doc = new HtmlDocument();
        
        public TjmgParser(string html)
        {
            _doc.LoadHtml(html);
        }

        public CourtJurisprudenceModel CreateModelByPage() 
        {
            var courtModel = new CourtJurisprudenceModel();

            courtModel.Class = GetMatterClass();
            courtModel.CourtDecisor = GetMatterCourtDecisor();
            courtModel.CourtAbreviation = courtAbreviation;
            courtModel.CourtSession = GetMatterCourtSession();
            courtModel.CourtEntry = GetMatterEntry();
            courtModel.CourtSumary = GetMatterSumary();
            courtModel.AllJurisprudenceText = GetMatterAllText();
            courtModel.DecisionDate = GetDecisionDate();

            return courtModel;
        }

        
        private DateTime GetDecisionDate() 
        {
            var decisionDate = GetMatterTextDecisionDate();

            return StringUtils.ConvertStringToDateTime(decisionDate);
        }

        private string GetMatterTextDecisionDate() => GetStringByXpath(ConstructXpath("Data de Julgamento"));

        private string GetMatterAllText() => GetStringByXpath("//td[@class='corpo']//./div[@class='cabecalho'][//*[contains(text(), 'Inteiro Teor')]]/following-sibling::div[@id='panel1']/div[contains(@style, 'justify')]");

        private string GetMatterSumary() => GetStringByXpath(ConstructXpath("mula"));

        private string GetMatterEntry() => GetStringByXpath(ConstructXpath("Ementa"));

        /// <summary>
        /// The original therm in html is 'Órgão Julgador / Câmara'
        /// </summary>
        private string GetMatterCourtSession() => GetStringByXpath(ConstructXpath("o Julgador / C"));

        private string GetMatterCourtDecisor() => GetStringByXpath(ConstructXpath("Relator"));

        private const string xpathClass = "//td[@class='corpo']//./div[@class='cabecalho']/following-sibling::div/text()[1]";

        private readonly Regex regexMatterClass = new Regex(@"");
        private string GetMatterClass() 
        {
            return GetStringByXpath(xpathClass);
        }

        private string ConstructXpath(string textFilter) => $"//td[@class='corpo']//./div[@class='cabecalho'][contains(text(), '{textFilter}')]/following-sibling::div";

        private string GetStringByXpath(string xpath) 
        {
            var node = _doc.DocumentNode.SelectSingleNode(xpath);

            if (node == null) return string.Empty;

            return node.GetClearTextFromNode();
        }

        public List<string> GetUrlFromMatterList() 
        {
            var nodes = _doc.DocumentNode.SelectNodes("//div[contains(@class, 'caixa_processo')]/a");
            
            return nodes
                    .Select(x => x.GetAttributeValue("href", string.Empty))
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();
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
