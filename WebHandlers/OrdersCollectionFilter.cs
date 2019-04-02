using NorthwindDAL.Entities;
using NorthwindDAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;


namespace WebHandlers
{
    public class OrdersCollectionFilter
    {
        private OrderRepository dbContext;

        public OrdersCollectionFilter(string connectionString, string providerName)
        {
            dbContext = new OrderRepository(connectionString, providerName);
        }


        public IEnumerable<Order> GetFilteredCollection(QueryStringParser constraints)
        {
           
            var resultCollection = dbContext.GetAllByCustomer(constraints.CustID); 
           
            if (constraints.IsDateFromExist())
            {
                resultCollection = resultCollection.Where(o => o.OrderDate >= constraints.DateFrom);
            }

            if (constraints.IsDateToExist())
            {
                resultCollection = resultCollection.Where(o => o.OrderDate <= constraints.DateTo);
            }

            if (constraints.IsSkipExist())
            {
                resultCollection = resultCollection.Skip(constraints.Skip);
            }

            if (constraints.IsTakeExist())
            {
                resultCollection = resultCollection.Take(constraints.Take);
            }

            return resultCollection;
        }
    }

}