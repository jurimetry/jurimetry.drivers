using System;

namespace GladosSearcher.Domain
{
    public class CourtJurisprudenceModel
    {
        /// <summary>
        /// Data da decisão
        /// </summary>
        public DateTime DecisionDate { get; set; }
        
        /// <summary>
        /// Nome do Tribunal
        /// </summary>
        public string CourtAbreviation { get; set; }
        
        /// <summary>
        /// Decisor / Juiz
        /// </summary>
        public string CourtDecisor { get; set; }
        
        /// <summary>
        /// Órgão Julgador / Câmara
        /// </summary>
        public string CourtSession { get; set; }
        
        /// <summary>
        /// Classe Processual
        /// </summary>
        public string Class { get; set; }
        
        /// <summary>
        /// Ementa
        /// </summary>
        public string CourtEntry { get; set; }
        
        /// <summary>
        /// Súmula do tribunal
        /// </summary>
        public string CourtSumary { get; set; }

        /// <summary>
        /// Inteiro Teor
        /// </summary>
        public string AllJurisprudenceText { get; set; }
    }
}
