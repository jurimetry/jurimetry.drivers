using GladosSearcher.Domain;
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

        public IReadOnlyList<CourtJurisprudenceModel> CourtJurisprudences => courtJurisprudenceModels;

        public void Crawle(ScheduleJurimetryModel message)
        {
            courtJurisprudenceModels = new List<CourtJurisprudenceModel>();
            var urls = GetMatterUrls();
            foreach (var url in urls) 
            {
                var resultMatter = Navigate(url);
                
                if (string.IsNullOrEmpty(resultMatter))
                    continue;

                var parser = new TjmgParser(resultMatter);
                var result = parser.CreateModelByPage();

                courtJurisprudenceModels.Add(result);
            }
        }

        private List<string> GetMatterUrls()
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
                    var resultListPage = NavigateToResultPage(session, court);
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

        private string NavigateToResultPage(string courtSessions, string courtDecisors) 
        {
            var url = $"{searchUlr}?{CreateParametersSearch(courtSessions, courtDecisors)}";

            return Navigate(url);
        }

#region Parameters
        private const string pagesLimit = "linhasPorPagina=";
        private const string searchThearm = "palavras=";
        private const string searchComplement = "pesquisarPor=acordao&orderByData=2";
        private const string searchComplementFinal = "&classe=&codigoAssunto=&dataPublicacaoInicial=&dataPublicacaoFinal=&dataJulgamentoInicial=&dataJulgamentoFinal=&siglaLegislativa=&referenciaLegislativa=Clique+na+lupa+para+pesquisar+as+refer%EAncias+cadastradas...&numeroRefLegislativa=&anoRefLegislativa=&legislacao=&norma=&descNorma=&complemento_1=&listaPesquisa=&descricaoTextosLegais=&observacoes=";
#endregion

        private const string thearm = "tutela";

        private string CreateParametersSearch(string courtSessions, string courtDecisors) 
        {
            var courtSessionsPost = $"listaOrgaoJulgador={courtSessions}";
            var courtDecisorsPost = $"listaRelator={courtDecisors}";

            var searchParameters = $"numeroRegistro=1&totalLinhas=1&{searchThearm}{thearm}";
            searchParameters += $"&{searchComplement}&codigoOrgaoJulgador=&{courtSessionsPost}&codigoCompostoRelator=&{courtDecisorsPost}";
            searchParameters += $"&{searchComplementFinal}&{pagesLimit}50&pesquisaPalavras=Pesquisar";

            return searchParameters;
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
