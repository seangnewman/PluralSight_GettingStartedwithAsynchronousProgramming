using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockAnalyzer.Windows.Services;
using System.Linq;
using System.Threading;

namespace StockAnalyzer.Tests
{
    [TestClass]
    public class MockStockServiceTests
    {
        [TestMethod]
         public async void Can_Load_All_MSFT_Stocks()
        {
            var service = new MockStockService();
            var stocks = await service.GetStockPricesFor("MSFT", CancellationToken.None);

            Assert.AreEqual(stocks.Count(), 2);
            Assert.AreEqual(stocks.Sum(stock => stock.Change), 0.7m);
        }
    }
}
