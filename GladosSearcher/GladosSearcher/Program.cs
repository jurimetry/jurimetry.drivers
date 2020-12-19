using GladosSearcher.Service.Tjmg;
using System;

namespace GladosSearcher
{
    public class Program
    {
        private static readonly TjmgSearcher _tjmgSearcher = new TjmgSearcher();
        static void Main(string[] args)
        {
            _tjmgSearcher.Crawle();
            Console.WriteLine("Hello World!");
        }
    }
}
