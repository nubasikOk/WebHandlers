
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System;
using Moq;
using NorthwindDAL.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;

namespace WebHandlers.Tests
{
   
    [TestClass]
    public class ConstraintTests
    {
        
        private OrdersCollectionFilter filter;
        private QueryStringParser querryParser;

        private string connectionString =
           @"data source=EPBYGROW0110;initial catalog=Northwind;integrated security=True;MultipleActiveResultSets=True";

        private string providerName = "System.Data.SqlClient";

        [TestInitialize]
        public void Test_Init()
        {
            filter = new OrdersCollectionFilter(connectionString, providerName);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public void Test_If_TakeConstraint_GetCorrectResult(int take)
        {
            var constraints = new NameValueCollection();
            constraints.Add("custID", "ALFKI");
            constraints.Add("take", take.ToString());
            constraints.Add("dateTo", "01-01-2005");

            var ordersCollection = GetCollectionWithConstraints(constraints).ToList();
            Assert.AreEqual(ordersCollection.Count, take);
        }


        private IEnumerable<Order> GetCollectionWithConstraints(NameValueCollection constraints)
        {
            querryParser = new QueryStringParser(constraints);
            return filter.GetFilteredCollection(querryParser);
           
        }
    }
}
