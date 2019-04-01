
using System.IO;
using System.Linq;
using System.Web;

namespace WebHandlers
{
    public class NorthwindHandler : IHttpHandler
    {
        public bool IsReusable => true;

        public void ProcessRequest(HttpContext context)
        {
            var request = context.Request;
            var filter = new OrdersCollectionWorker();
            var queryStringParser = new QueryStringParser(request.QueryString);
            var ordersCollection = filter.GetFilteredCollection(queryStringParser);
               if (ordersCollection.Count() > 0)
               {
                    context.Response.Clear();
                    var streamGenerator = new ResponseStreamGenerator(ordersCollection);

                    switch (request.ContentType)
                    {
                        case "text/xml":
                        context.Response.ContentType = "text/xml";
                        streamGenerator.GenerateXML().WriteTo(context.Response.OutputStream);
                        break;

                        case "application/xml":
                            context.Response.ContentType = "application/xml";
                            streamGenerator.GenerateXML().WriteTo(context.Response.OutputStream);
                           break;

                        default:
                            context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            streamGenerator.GenerateXLSX().WriteTo(context.Response.OutputStream);
                            break;
                    }
                context.Response.Flush();
                context.Response.Close();
               }
               else
               {
                    context.Response.Output.WriteLine("wait for parameters");
               }
            
            
        }


       
    }
}