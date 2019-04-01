using System;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace WebHandlers.Tests
{
    [TestClass]
    public class StreamGeneratorsTests
    {
        private ResponseStreamGenerator streamGenerator;
        private OrdersCollectionWorker collWorker;
        private QueryStringParser querryParser;

        private string connectionString =
           @"data source=EPBYGROW0110;initial catalog=Northwind;integrated security=True;MultipleActiveResultSets=True";

        private string providerName = "System.Data.SqlClient";


        [TestInitialize]
        public void Test_Init()
        {
            var constraints = new NameValueCollection();
            constraints.Add("custID", "ALFKI");
            constraints.Add("skip", "2");
            constraints.Add("take", "1");
            constraints.Add("dateFrom", "01-01-1990");
            InitializeWithConstraints(constraints);
        }

        [TestMethod]
        public void Test_If_XMLStream_GenerateCorrect()
        { 
            var stream = streamGenerator.GenerateXML();
            Assert.IsNotNull(stream);
        }


        [TestMethod]
        public void Test_If_XLSXStream_GenerateCorrect()
        {
            var stream = streamGenerator.GenerateXLSX();
            Assert.IsNotNull(stream);
        }


        [TestMethod]
        [DataRow(1)]
        public void Test_If_TakeConstraint_GetCorrectResult(int take)
        {
            var constraints = new NameValueCollection();
            constraints.Add("custID", "ALFKI");
            constraints.Add("take", take.ToString());
            InitializeWithConstraints(constraints);
            var ordersCollection = collWorker.GetFilteredCollection(querryParser).ToList();
            Assert.AreEqual(ordersCollection.Count,take);
        }

        [TestMethod]
        [DataRow(-1)]
        public void Test_If_TakeConstraintIncorrect_GetCorrectResult(int take)
        {
            var constraints = new NameValueCollection();
            constraints.Add("custID", "ANATR");
            constraints.Add("take", take.ToString());
            InitializeWithConstraints(constraints);
            var ordersCollection = collWorker.GetFilteredCollection(querryParser).ToList();
            Assert.AreEqual(ordersCollection.Count, 0);
        }

        private void InitializeWithConstraints(NameValueCollection constraints)
        {
            querryParser = new QueryStringParser(constraints);
            collWorker = new OrdersCollectionWorker(connectionString, providerName);
            var orders = collWorker.GetFilteredCollection(querryParser);
            streamGenerator = new ResponseStreamGenerator(orders);
        }

    }
}
