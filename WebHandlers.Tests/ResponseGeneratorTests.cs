using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebHandlers.Tests
{
    [TestClass]
    public class ResponseGeneratorTests
    {
        [TestMethod]
        
        public void TestIsCorrectResponseGeneration()
        {
            
            var stream = new MemoryStream();
            HttpRequest request = new HttpRequest("","http://localhost:65105/", "custID=ALFKI&&take=1");

            StringWriter sr = new StringWriter();
            HttpResponse resp= new HttpResponse(sr);
            HttpContext.Current = new HttpContext(
                request,     
                resp
                );

            var generator = new ResponseGenerator();
            var response = generator.GetResponseForCurrentContext(HttpContext.Current);
           
            Assert.IsNotNull(response);
        }
    }
}
