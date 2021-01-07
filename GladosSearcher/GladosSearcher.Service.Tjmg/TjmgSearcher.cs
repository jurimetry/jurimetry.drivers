using GladosSearcher.Domain;
using GladosSearcher.Messager;
using GladosSearcher.Messager.Domain;
using GorticLib;
using GorticLib.RequestParameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GladosSearcher.Service.Tjmg
{
    public class TjmgSearcher
    {
        private readonly GorticLibCrawler libCrawler = new GorticLibCrawler();

        private const string baseUrl = "https://www5.tjmg.jus.br/jurisprudencia";
        private readonly string searchUlr = $"{baseUrl}/pesquisaPalavrasEspelhoAcordao.do";
        private List<CourtJurisprudenceModel> courtJurisprudenceModels;
        private readonly Producer producer = new Producer();

        public IReadOnlyList<CourtJurisprudenceModel> CourtJurisprudences => courtJurisprudenceModels;

        public void Crawle(ScheduleJurimetryModel message = null)
        {
            courtJurisprudenceModels = new List<CourtJurisprudenceModel>();
            var urls = GetMatterUrls(message);
            foreach (var url in urls) 
            {
                var resultMatter = Navigate(url);
                
                if (string.IsNullOrEmpty(resultMatter))
                    continue;

                var parser = new TjmgParser(resultMatter);
                var result = parser.CreateModelByPage();

                courtJurisprudenceModels.Add(result);

            }

            producer.Publish(courtJurisprudenceModels);
        }

        private List<string> GetMatterUrls(ScheduleJurimetryModel message = null)
        {
#if DEBUG
            var list = new List<string>()
            {
                "1-1",
                "1-2",
                "1-3",
                "1-4",
                "1-5",
                "1-7"
            };

            var list2 = new List<string>()
            {
                "2-1232313",
                "2-1528561",
                "2-2607893",
                "0-12070"
            };
#endif
            var searchPage = NavigateToSearchPage();
            var parser = new TjmgParser(searchPage);


#if DEBUG
            var courts = parser.GetAllCourtDecisorsNames().Where(x => list2.Contains(x));
            var sessions = parser.GetAllCourtSessionsNames().Where(x => list.Contains(x));
#else
            var courts = parser.GetAllCourtDecisorsNames();
            var sessions = parser.GetAllCourtSessionsNames();
#endif
            var listMatterUrls = new List<string>();
            foreach (var session in sessions)
            {
                foreach (var court in courts)
                {
                    var resultListPage = NavigateToResultPage(session, court, message);
#if DEBUG
                    Console.WriteLine($"{session} - {court} - {!IsMatterResultPage(resultListPage)}");
#endif
                    if (IsMatterResultPage(resultListPage))
                        continue;

                    parser = new TjmgParser(resultListPage);
                    listMatterUrls.AddRange(parser.GetUrlFromMatterList());
                }
            }

            return listMatterUrls;
        }

        private bool IsMatterResultPage(string resultListPage) 
        {
            return resultListPage.Contains("Nenhum Espelho do Ac") || resultListPage.Contains("Ocorreu um erro");
        }

        private string NavigateToResultPage(string courtSessions, string courtDecisors, ScheduleJurimetryModel message = null) 
        {
            var url = $"{searchUlr}?{CreateParametersSearch(courtSessions, courtDecisors, message)}";

            return Navigate(url);
        }

#region Parameters
        private const string pagesLimit = "linhasPorPagina=";
        private const string searchThearm = "palavras=";
        private const string searchComplement = "pesquisarPor=acordao&orderByData=2";
        private const string searchComplementFinalPt1 = "classe=&codigoAssunto=&dataPublicacaoInicial=&dataPublicacaoFinal=";
        private const string searchComplementFinalPt2 = "siglaLegislativa=&referenciaLegislativa=Clique+na+lupa+para+pesquisar+as+refer%EAncias+cadastradas...&numeroRefLegislativa=&anoRefLegislativa=&legislacao=&norma=&descNorma=&complemento_1=&listaPesquisa=&descricaoTextosLegais=&observacoes=";
#endregion

        private const string thearm = "tutela";

        private string CreateParametersSearch(string courtSessions, string courtDecisors, ScheduleJurimetryModel message = null) 
        {
            var courtSessionsPost = $"listaOrgaoJulgador={courtSessions}";
            var courtDecisorsPost = $"listaRelator={courtDecisors}";
            var searchLimitation = GetSearchLimitation(message);

            var searchParameters = $"numeroRegistro=1&totalLinhas=1&{searchThearm}{thearm}";
            searchParameters += $"&{searchComplement}&codigoOrgaoJulgador=&{courtSessionsPost}&codigoCompostoRelator=&{courtDecisorsPost}";
            searchParameters += $"&{searchComplementFinalPt1}&{searchLimitation}&{searchComplementFinalPt2}&{pagesLimit}50&pesquisaPalavras=Pesquisar";

            return searchParameters;
        }

        private string GetSearchLimitation(ScheduleJurimetryModel message = null) 
        {
            if (message == null || message.RequiredDate == null || message.RequiredDate == default)
                return "dataJulgamentoInicial=&dataJulgamentoFinal=";

            var date = $"{message.RequiredDate.Date.Day.ToString().PadLeft(2)}%2F{message.RequiredDate.Date.Month.ToString().PadLeft(2)}%2F{message.RequiredDate.Date.Year.ToString()}";

            return $"dataJulgamentoInicial={date}&dataJulgamentoFinal={date}";
        }

        private string NavigateToSearchPage() => Navigate(searchUlr);

        private string Navigate(string url) 
        {
            if (!url.StartsWith(baseUrl))
                url = $"{baseUrl}/{url.TrimStart('/')}";

            var parameters = new GorticLibParameters();
            parameters.Url = url;
            parameters.Timeout = 100000;
            parameters.Enconding = GorticLib.Helpers.RequestHelper.Enconding.ISO8859;
            libCrawler.GorticClientProperties.SetRequestParameters(parameters);

            var response = libCrawler.GorticClientProperties.MakeRequest();

            if (response.Item1 != HttpStatusCode.OK) return string.Empty;

            return response.Item2;
        }
    }
}
