using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebSerivcesPlugin;

namespace UniOrm.WebServiceTest
{
    [TestClass]
    public class RadioSerMngTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            RadioSerMng radioSerMng = new RadioSerMng();
            var allxmls = radioSerMng.Getcountries();
        }
    }
}
