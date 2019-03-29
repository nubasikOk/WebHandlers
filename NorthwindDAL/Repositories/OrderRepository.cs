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
    public class OrderRepository : IOrderRepository
    {
        private readonly string ConnectionString;

        private readonly  DbProviderFactory ProviderFactory;

        public OrderRepository(string connectionString, string provider)
        {
            ConnectionString = connectionString;
            ProviderFactory = DbProviderFactories.GetFactory(provider);
        }

        public IEnumerable<Order> GetAll()
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
                    command.CommandText = "select * from Orders";
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
        public Order GetByID(int id)
        {
            using (var connection = ProviderFactory.CreateConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "select * from Orders where OrderID = @id; " +
                        "select * from [Order Details] where OrderID = @id";
                    command.CommandType = CommandType.Text;

                    var paramId = SqlBuilder.Create("@id", DbType.Int32, id);
                    command.Parameters.Add(paramId);

                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        if (!reader.HasRows) return null;
                                           
                        var order = CreateOrderInstance(reader);
                        reader.NextResult();
                        order.Details = new List<OrderDetail>();

                        while (reader.Read())
                        {
                            var detail = CreateDetailInstance(reader);
                            order.Details.Add(detail);
                        }

                        return order;
                    }
                }
            }
        }


        public Order Create(Order newItem)
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
                    InsertItem(newItem, command);
                    var newid = int.Parse(command.ExecuteScalar().ToString());
                    return GetByID(newid);
                }
            }
        }

        private void InsertItem(Order newItem, DbCommand command)
        {
            newItem.OrderDate = null;
            newItem.ShippedDate = null;
            command.CommandText = $"insert into Orders (" +
                                  $"{nameof(newItem.CustomerId)}, " +
                                  $"{nameof(newItem.EmployeeId)}, " +
                                  $"{nameof(newItem.OrderDate)}, " +
                                  $"{nameof(newItem.RequiredDate)}, " +
                                  $"{nameof(newItem.ShippedDate)}, " +
                                  $"{nameof(newItem.ShipVia)}, " +
                                  $"{nameof(newItem.Freight)}, " +
                                  $"{nameof(newItem.ShipName)}, " +
                                  $"{nameof(newItem.ShipAddress)}, " +
                                  $"{nameof(newItem.ShipCity)}, " +
                                  $"{nameof(newItem.ShipRegion)}, " +
                                  $"{nameof(newItem.ShipPostalCode)}, " +
                                  $"{nameof(newItem.ShipCountry)}" +
                                  $") values (@CustomerId, @EmployeeId, @OrderDate, @RequiredDate, @ShippedDate, @ShipVia, @Freight, @ShipName, @ShipAddress, @ShipCity, @ShipRegion, @ShipPostalCode, @ShipCountry) SELECT SCOPE_IDENTITY()";
            AddParams(newItem, command);
        }
        public Order Update(Order item)
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
                    UpdateItem(item, command);
                    if (command.CommandText.Any())
                    {
                        command.ExecuteNonQuery();
                    }

                    return GetByID(item.Id);
                }
            }
        }
    
        private void UpdateItem(Order item, DbCommand command)
        {
            var currentItem = GetByID(item.Id);
            if (currentItem == null || currentItem.Status == OrderStatus.InProgress ||
                currentItem.Status == OrderStatus.Completed)
            {
                command.CommandText = "";
                return;
            }

            item.OrderDate = currentItem.OrderDate;
            item.ShippedDate = currentItem.ShippedDate;
            SetUpdateCommand(command, item);
        }
        public  bool Delete(Order item)
        {
            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection())
            {
                if (connection == null)
                {
                    return false;
                }

                connection.ConnectionString = ConnectionString;
                connection.Open();

                var deleteDetailCommand = connection.CreateCommand();
                var deleteOrderCommand = connection.CreateCommand();
                deleteDetailCommand.CommandText = $"delete from [Order Details] where OrderID = @id";
                var paramId1 = SqlBuilder.Create("@id", DbType.Int32, item.Id);
                deleteDetailCommand.Parameters.Add(paramId1);
                deleteOrderCommand.CommandText = $"delete from Orders where OrderID = @id";
                var paramId2 = SqlBuilder.Create("@id", DbType.Int32, item.Id);
                deleteOrderCommand.Parameters.Add(paramId2);
                System.Data.SqlClient.SqlTransaction tx = null;
                try
                {
                    tx = connection.BeginTransaction();
                   
                    deleteDetailCommand.Transaction = tx;
                    deleteOrderCommand.Transaction = tx;
                    
                    deleteDetailCommand.ExecuteNonQuery();
                    deleteOrderCommand.ExecuteNonQuery();
                    
                    tx.Commit();
                }
                catch 
                {
                                     
                    tx.Rollback();
                }
            }

            return GetByID(item.Id) == null;
        }

        public bool MarkShipped(int id)
        {
            using (var connection = ProviderFactory.CreateConnection())
            {
                if (connection == null)
                {
                    return false;
                }

                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    var currentItem = GetByID(id);
                    var orderDate = DateTime.Now;
                    orderDate = new DateTime(orderDate.Year, orderDate.Month, orderDate.Day, orderDate.Hour, orderDate.Minute, orderDate.Second);
                    currentItem.OrderDate = orderDate;
                    SetUpdateCommand(command, currentItem);
                    command.ExecuteNonQuery();
                    var updatedItem = GetByID(id);
                    return updatedItem?.OrderDate != null && orderDate == updatedItem.OrderDate.Value;
                }
            }
        }

        public bool MarkArrived(int id)
        {
            using (var connection = ProviderFactory.CreateConnection())
            {
                if (connection == null)
                {
                    return false;
                }

                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    var currentItem = GetByID(id);
                    var shippedDate = DateTime.Now;
                    shippedDate = new DateTime(shippedDate.Year, shippedDate.Month, shippedDate.Day, shippedDate.Hour, shippedDate.Minute, shippedDate.Second);
                    currentItem.ShippedDate = shippedDate;
                    SetUpdateCommand(command, currentItem);
                    command.ExecuteNonQuery();
                    var updatedItem = GetByID(id);
                    return updatedItem?.ShippedDate != null && shippedDate == updatedItem.ShippedDate.Value;
                }
            }
        }

        public IEnumerable<CustomerOrdersHistory> CustOrderHist(string customerId)
        {
            var OrdersHistory = new List<CustomerOrdersHistory>();
            using (var connection = ProviderFactory.CreateConnection())
            {
                if (connection == null)
                {
                    return OrdersHistory;
                }

                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"CustOrderHist {customerId}";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new CustomerOrdersHistory();
                            item.ProductName = reader[nameof(item.ProductName)] as string;
                            item.Total = (int)reader[nameof(item.Total)];
                            OrdersHistory.Add(item);
                        }
                    }
                }
            }

            return OrdersHistory;
        }

        public IEnumerable<OrderDetails> OrderDetails(int orderId)
        {
            var result = new List<OrderDetails>();
            using (var connection = ProviderFactory.CreateConnection())
            {
                if (connection == null)
                {
                    return result;
                }

                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"CustOrdersDetail {orderId}";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new OrderDetails();
                            item.ProductName = reader[nameof(item.ProductName)] as string;
                            item.Discount = (int)reader[nameof(item.Discount)];
                            item.ExtendedPrice = (decimal)reader[nameof(item.ExtendedPrice)];
                            item.Quantity = (short)reader[nameof(item.Quantity)];
                            item.UnitPrice = (decimal)reader[nameof(item.UnitPrice)];
                            result.Add(item);
                        }
                    }
                }
            }

            return result;
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


        public OrderDetail CreateDetailInstance(DbDataReader reader)
        {
            var orderDetails = new OrderDetail();
            orderDetails.Discount = (float)reader[nameof(orderDetails.Discount)];
            orderDetails.OrderId = (int)reader[nameof(orderDetails.OrderId)];
            orderDetails.ProductId = (int)reader[nameof(orderDetails.ProductId)];
            orderDetails.Quantity = (short)reader[nameof(orderDetails.Quantity)];
            orderDetails.UnitPrice = (decimal)reader[nameof(orderDetails.UnitPrice)];
            
            return orderDetails;
        }

        private void AddParams(Order item, DbCommand command)
        {
            command.Parameters.Add(SqlBuilder.Create("@id", DbType.Int32, item.Id));
            command.Parameters.Add(SqlBuilder.Create("@CustomerId", DbType.String, item.CustomerId));
            command.Parameters.Add(SqlBuilder.Create("@EmployeeId", DbType.Int32, item.EmployeeId));
            command.Parameters.Add(SqlBuilder.Create("@OrderDate", DbType.DateTime, item.OrderDate));
            command.Parameters.Add(SqlBuilder.Create("@RequiredDate", DbType.DateTime, item.RequiredDate));
            command.Parameters.Add(SqlBuilder.Create("@ShippedDate", DbType.DateTime, item.ShippedDate));
            command.Parameters.Add(SqlBuilder.Create("@ShipVia", DbType.Int32, item.ShipVia));
            command.Parameters.Add(SqlBuilder.Create("@Freight", DbType.Decimal, item.Freight));
            command.Parameters.Add(SqlBuilder.Create("@ShipName", DbType.String, item.ShipName));
            command.Parameters.Add(SqlBuilder.Create("@ShipAddress", DbType.String, item.ShipAddress));
            command.Parameters.Add(SqlBuilder.Create("@ShipCity", DbType.String, item.ShipCity));
            command.Parameters.Add(SqlBuilder.Create("@ShipRegion", DbType.String, item.ShipRegion));
            command.Parameters.Add(SqlBuilder.Create("@ShipPostalCode", DbType.String, item.ShipPostalCode));
            command.Parameters.Add(SqlBuilder.Create("@ShipCountry", DbType.String, item.ShipCountry));
        }
        private void SetUpdateCommand(DbCommand command, Order item)
        {
            command.CommandText = $"update Orders set " +
                      $"{nameof(item.CustomerId)} = @CustomerId, " +
                      $"{nameof(item.EmployeeId)} = @EmployeeId, " +
                      $"{nameof(item.ShippedDate)} = @ShippedDate, " +
                      $"{nameof(item.OrderDate)} = @OrderDate, " +
                      $"{nameof(item.RequiredDate)} = @RequiredDate, " +
                      $"{nameof(item.ShipVia)} = @ShipVia, " +
                      $"{nameof(item.Freight)} = @Freight, " +
                      $"{nameof(item.ShipName)} = @ShipName, " +
                      $"{nameof(item.ShipAddress)} = @ShipAddress, " +
                      $"{nameof(item.ShipCity)} = @ShipCity, " +
                      $"{nameof(item.ShipRegion)} =  @ShipRegion, " +
                      $"{nameof(item.ShipPostalCode)} = @ShipPostalCode, " +
                      $"{nameof(item.ShipCountry)} = @ShipCountry " +
                      $"where Orders.OrderID = @id";
            AddParams(item, command);
        }

       
    }
}
