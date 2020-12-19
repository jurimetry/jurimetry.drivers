using System;

namespace GladosSearcher.Domain
{
    public class CourtJurisprudenceModel
    {
        public DateTime DecisionDate { get; set; }
        public string CourtAbreviation { get; set; }
        public string CourtDecisor { get; set; }
        public string CourtSession { get; set; }
        public string Class { get; set; }
        public string Decision { get; set; }
    }
}
