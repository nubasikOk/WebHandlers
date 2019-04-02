using NorthwindDAL.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using System.Xml.Serialization;
using System.Data;
using System.Web;

namespace WebHandlers
{
    public class StreamGenerator
    {
        private readonly IEnumerable<Order> orders;
        public StreamGenerator(IEnumerable<Order> orders)
        {
            this.orders = orders;
        }

        public MemoryStream GenerateStreamByContentType(string contentType, out string responseContentType)
        {
            
            switch (contentType)
            {
                case "text/xml":
                    responseContentType = "text/xml";
                    return GenerateXML();

                case "application/xml":
                    responseContentType = "application/xml";
                    return GenerateXML();

                default:
                    responseContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    return GenerateXLSX();

            }
        }


        private MemoryStream GenerateXML()
        {
            var stream = new MemoryStream();
            try
            {
                var formatter = new XmlSerializer(typeof(List<Order>));
                formatter.Serialize(stream, orders.ToList());
                return stream;
            }
            catch
            {
                stream.Dispose();
                throw;
            }
            
        }

        private MemoryStream GenerateXLSX()
        {
            
            using (var workbook = new XLWorkbook())
            {
                var stream = new MemoryStream();
                var dataTable = CreateOrdersDataTable();
                var worksheet = workbook.Worksheets.Add("report");
                worksheet.Cell(2, 1).Value = dataTable.AsEnumerable();

                for (var i = 0; i < dataTable.Columns.Count; i++)
                {
                    worksheet.Cell(1, i + 1).Value = dataTable.Columns[i].ColumnName;
                }

                workbook.SaveAs(stream, new SaveOptions { });
                return stream;
                            
            }

        }

        

        private DataTable CreateOrdersDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("CustomerId", typeof(string));
            table.Columns.Add("EmployeeId", typeof(int));
            table.Columns.Add("OrderDate", typeof(DateTime));
            table.Columns.Add("RequiredDate", typeof(DateTime));
            table.Columns.Add("Freight", typeof(decimal));
            table.Columns.Add("ShipName", typeof(string));
            table.Columns.Add("ShipCity", typeof(string));
            table.Columns.Add("ShipPostalCode", typeof(string));
            table.Columns.Add("ShipCountry", typeof(string));

            foreach (var order in orders)
            {
                table.Rows.Add(
                    order.Id.ToString(),
                    order.CustomerId,
                    order.EmployeeId,
                    order.OrderDate,
                    order.RequiredDate,
                    order.Freight,
                    order.ShipName,
                    order.ShipCity,
                    order.ShipPostalCode,
                    order.ShipCountry
                    );
            }

            return table;
        }
    }
}