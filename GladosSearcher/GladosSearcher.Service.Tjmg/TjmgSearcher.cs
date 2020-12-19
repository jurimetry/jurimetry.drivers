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
        private const string searchUlr = "https://www5.tjmg.jus.br/jurisprudencia/pesquisaPalavrasEspelhoAcordao.do";

        public void Crawle()
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

            foreach (var session in sessions) 
            {
                foreach (var court in courts)
                {
                    var resultListPage = NavigateToResultPage(session, court);
                    Console.WriteLine($"{session} - {court} - {!resultListPage.Contains("Nenhum Espelho do Ac")}");
                }
            }
        }

        

        private string NavigateToResultPage(string courtSessions, string courtDecisors) 
        {
            var url = $"{searchUlr}?{CreateParametersSearch(courtSessions, courtDecisors)}";

            return Navigate(url);
        }

#region Parameters
        private const string pagesLimit = "linhasPorPagina=";
        private const string searchThearm = "palavras=";
        private const string searchComplement = "pesquisarPor=ementa&orderByData=2";
        private const string searchComplementFinal = "referenciaLegislativa=Clique%20na%20lupa%20para%20pesquisar%20as%20refer%EAncias%20cadastradas...&pesquisaPalavras=Pesquisar&";
#endregion

        private const string thearm = "tutela";

        private string CreateParametersSearch(string courtSessions, string courtDecisors) 
        {
            var courtSessionsPost = $"listaRelator={courtSessions}";
            var courtDecisorsPost = $"listaOrgaoJulgador={courtDecisors}";

            var searchParameters = $"{pagesLimit}50&paginaNumero=1&{searchThearm}{thearm}";
            searchParameters += $"{searchComplement}&{courtDecisorsPost}&{courtSessionsPost}{searchComplementFinal}";
            //var searchParameters = $"{baseSearchPost}{thearm}&{complementSearchPost}";
            //searchParameters += $"&{courtSessionsPost}&codigoCompostoRelator=&{courtDecisorsPost}";
            //searchParameters += $"&{complementPostFinal}&{pagesLimit}50&pesquisaPalavras=Pesquisar";

            //var searchParameters = $"{pagesLimit}10&{pageNumber}&{searchThearm}{thearm}";
            //searchParameters += $"&{searchComplement}&{courtSessionsPost}&{courtDecisorsPost}{searchComplement2}";


            return searchParameters;
        }

        private string NavigateToSearchPage() => Navigate(searchUlr);

        private string Navigate(string url) 
        {
            var parameters = new GorticLibParameters();
            parameters.Url = url;
            parameters.Timeout = 100000;
            libCrawler.GorticClientProperties.SetRequestParameters(parameters);

            var response = libCrawler.GorticClientProperties.MakeRequest();

            if (response.Item1 != HttpStatusCode.OK) return string.Empty;

            return response.Item2;
        }
    }
}
