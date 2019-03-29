using NorthwindDAL.Entities;
using NorthwindDAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindDAL.Repositories
{
    public interface IOrderRepository
    {
       

        Order GetByID(int id);

        Order Create(Order newItem);

        Order Update(Order item);

        bool MarkShipped(int id);

        bool MarkArrived(int id);

        bool Delete(Order item);

        IEnumerable<Order> GetAll();

        IEnumerable<CustomerOrdersHistory> CustOrderHist(string customerId);

        IEnumerable<OrderDetails> OrderDetails(int orderId);




    }
}
