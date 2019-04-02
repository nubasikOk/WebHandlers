using NorthwindDAL.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;

namespace WebHandlers
{
    public class ResponseGenerator
    {
        private string connectionString =
           @"data source=EPBYGROW0110;initial catalog=Northwind;integrated security=True;MultipleActiveResultSets=True";

        private string providerName = "System.Data.SqlClient";


        public MemoryStream GetResponseForCurrentContext(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var contentType = request.ContentType;
            var responseStream = new MemoryStream();
            response.Clear();
            var ordersCollection = InitCollectionByQuerry(request.QueryString).ToList();
            if (ordersCollection.Count() > 0)
            {
                var streamGenerator = new StreamGenerator(ordersCollection);
                responseStream = streamGenerator.GenerateStreamByContentType(request.ContentType, out string responseType);
                response.ContentType = responseType;
                
            }
            else
            {
                response.Output.Write("wait for parameters");
            }
            return responseStream;
        }

        private IEnumerable<Order> InitCollectionByQuerry(NameValueCollection queryString)
        {
            var filter = new OrdersCollectionFilter(connectionString, providerName);
            var constraintParser = new QueryStringParser(queryString);
            return filter.GetFilteredCollection(constraintParser);
        }

    }
}