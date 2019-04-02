using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;

namespace WebHandlers.Tests
{
    
    [TestClass]
    public class IntegrationTest
    {
        private WebRequest webRequest;

        [TestInitialize]
        public void TestInit()
        {
            webRequest = WebRequest.Create("http://localhost:65105/?custID=COMMI&&take=2");
        }


        [TestMethod]
        [DataRow("application/xml")]
        [DataRow("text/xml")]
        [DataRow("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        public void TestIsResponseNotNull(string contentType)
        {
            webRequest.Method = "POST";
            webRequest.ContentType = contentType;
            webRequest.ContentLength = 0;
            var webResponse = webRequest.GetResponse();
            var stream = webResponse.GetResponseStream();
            var streamReader = new StreamReader(stream);
            Assert.IsNotNull(streamReader.ReadToEnd());
        }
    }
}
