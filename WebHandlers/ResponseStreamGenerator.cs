﻿using ClosedXML.Excel;
using NorthwindDAL.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WebHandlers
{
    public class ResponseStreamGenerator
    {
        private readonly IEnumerable<Order> orders;
        public ResponseStreamGenerator(IEnumerable<Order> orders)
        {
            this.orders = orders;
        }

        public MemoryStream GenerateXML()
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new XmlSerializer(typeof(List<Order>));

                formatter.Serialize(stream, orders.ToList());

                return stream;
            }

        }

        public MemoryStream GenerateXLSX()
        {
            
            using (var workbook = new XLWorkbook())
            {
                using (var stream = new MemoryStream())
                {
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
            table.Columns.Add("ShipPostalCode", typeof(int));
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