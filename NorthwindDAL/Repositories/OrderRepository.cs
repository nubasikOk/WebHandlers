using NorthwindDAL.Entities;
using NorthwindDAL.Entities.Enums;
using NorthwindDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace NorthwindDAL.Repositories
{
    public class OrderRepository 
    {
        private readonly string ConnectionString;

        private readonly  DbProviderFactory ProviderFactory;

        public OrderRepository(string connectionString, string provider)
        {
            ConnectionString = connectionString;
            ProviderFactory = DbProviderFactories.GetFactory(provider);
        }


        public IEnumerable<Order> GetAllByCustomer(string custID)
        {

            using (var connection = ProviderFactory.CreateConnection())
            {
                if (connection == null)
                {
                    throw new ArgumentNullException();
                }

                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select * from Orders where CustomerID=@id order by OrderID";
                    command.CommandType = CommandType.Text;

                    var paramId = SqlBuilder.Create("@id", DbType.Int32, custID);
                    command.Parameters.Add(paramId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return CreateOrderInstance(reader);
                        }
                    }
                }
            }
        }



        public Order CreateOrderInstance(DbDataReader reader)
        {
            var order = new Order();
            order.Id = (int)reader["OrderID"];
            order.CustomerId = reader[nameof(order.CustomerId)] as string;
            order.EmployeeId = reader[nameof(order.EmployeeId)] as int?;
            order.OrderDate = reader[nameof(order.OrderDate)] as DateTime?;
            order.RequiredDate = reader[nameof(order.RequiredDate)] as DateTime?;
            order.ShippedDate = reader[nameof(order.ShippedDate)] as DateTime?;
            order.ShipVia = reader[nameof(order.ShipVia)] as int?;
            order.Freight = reader[nameof(order.Freight)] as decimal?;
            order.ShipName = reader[nameof(order.ShipName)] as string;
            order.ShipAddress = reader[nameof(order.ShipAddress)] as string;
            order.ShipCity = reader[nameof(order.ShipCity)] as string;
            order.ShipRegion = reader[nameof(order.ShipRegion)] as string;
            order.ShipPostalCode = reader[nameof(order.ShipPostalCode)] as string;
            order.ShipCountry = reader[nameof(order.ShipCountry)] as string;
            return order;
        }


    }
}
