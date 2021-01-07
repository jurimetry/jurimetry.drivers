using GladosSearcher.Service.Tjmg;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GladosSearcher.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private static readonly TjmgSearcher _tjmgSearcher = new TjmgSearcher();

        [TestMethod]
        public void TestMethod1()
        {
            _tjmgSearcher.Crawle();
             
            
        }
    }
}
