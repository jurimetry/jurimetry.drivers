using GladosSearcher.Messager;
using GladosSearcher.Messager.Domain;
using GladosSearcher.Service.Tjmg;
using System;

namespace GladosSearcher
{
    public class Program
    {
        private static readonly TjmgSearcher _tjmgSearcher = new TjmgSearcher();
        private static readonly Receiver receiver = new Receiver();

        public static void Main(string[] args)
        {
            Action<ScheduleJurimetryModel> tg = delegate (ScheduleJurimetryModel s) { _tjmgSearcher.Crawle(s); };

            receiver.Execute("schedule-jurimetry", tg);
        }
    }
}
