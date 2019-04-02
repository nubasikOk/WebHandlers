
using NorthwindDAL.Entities;
using System;
using System.Collections.Generic;
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
            var generator = new ResponseGenerator();
            using (var response = generator.GetResponseForCurrentContext(context))
            {
                response.WriteTo(context.Response.OutputStream);
                context.Response.OutputStream.Flush();
                context.Response.OutputStream.Close();
            }
        }
    }
}