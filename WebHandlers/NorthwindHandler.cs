
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
            var ordersStream = new MemoryStream();
            var streamGenerator = new ResponseStreamGenerator(ordersCollection);
            try
            {
                if (ordersCollection.Count() > 0)
                {
                    context.Response.Clear();

                    switch (request.ContentType)
                    {
                        case "text/xml":
                            context.Response.ContentType = "text/xml";
                            ordersStream = streamGenerator.GenerateXML();
                            break;
                        case "application/xml":
                            context.Response.ContentType = "application/xml";
                            ordersStream = streamGenerator.GenerateXML();
                            break;
                        default:
                            context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            ordersStream = streamGenerator.GenerateXLSX();
                            break;
                    }
                    ordersStream.WriteTo(context.Response.OutputStream);
                    context.Response.Flush();
                    context.Response.Close();
                }

                else
                {
                    context.Response.Output.WriteLine("wait for parameters");
                }
            }
            finally
            {
                ordersStream.Dispose();
            }

        }
    }
}