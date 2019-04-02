using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NorthwindDAL.Entities;

namespace WebHandlers.Tests
{
    [TestClass]
    public class StreamGeneratorsTests
    {
        
        [TestMethod]
        [DataRow("application/xml")]
        [DataRow("text/xml")]
        [DataRow("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        public void TestIsCorrectStreamGeneration(string requestType)
        {
            List<Order> collection = new List<Order>()
            {
                new Order()
                {
                    CustomerId="testID",
                    OrderDate=Convert.ToDateTime("06-06-1998"),
                    Id=1,
                    ShipAddress="test address"

                }
            };
            StreamGenerator generator = new StreamGenerator(collection);
            Assert.IsNotNull(generator.GenerateStreamByContentType(requestType, out string responseType));
        }
    }
}
