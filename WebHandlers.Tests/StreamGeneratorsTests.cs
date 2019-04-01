using System;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace WebHandlers.Tests
{
    [TestClass]
    public class StreamGeneratorsTests
    {
        private ResponseStreamGenerator streamGenerator;
        private OrdersCollectionWorker collWorker;
        private QueryStringParser stringParser;


        [TestInitialize]
        public void Test_Init()
        {
            var constraints = new NameValueCollection();
            constraints.Add("custID", "ALFKI");
            constraints.Add("skip", "2");
            constraints.Add("take", "1");
            stringParser = new QueryStringParser(constraints);
            collWorker = new OrdersCollectionWorker();
        }

        [TestMethod]
        public void Test_If_XMLStream_GenerateCorrect()
        {
            OrdersCollectionWorker collWorker = new OrdersCollectionWorker();
            var orders = collWorker.GetFilteredCollection(stringParser);
            streamGenerator = new ResponseStreamGenerator(orders);
            var stream = streamGenerator.GenerateXML();
            Assert.IsNotNull(stream);
        }


        [TestMethod]
        public void Test_If_XLSXStream_GenerateCorrect()
        {
            OrdersCollectionWorker collWorker = new OrdersCollectionWorker();
            var orders = collWorker.GetFilteredCollection(stringParser);
            streamGenerator = new ResponseStreamGenerator(orders);
            var stream = streamGenerator.GenerateXLSX();
            Assert.IsNotNull(stream);
        }
    }
}
