using NorthwindDAL.Entities;
using NorthwindDAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;


namespace WebHandlers
{
    public class OrdersCollectionWorker
    {
        const string ConnectionString =
           @"data source=EPBYGROW0110;initial catalog=Northwind;integrated security=True;MultipleActiveResultSets=True";

        const string ProviderName = "System.Data.SqlClient";


        public IEnumerable<Order> GetFilteredCollection(QueryStringParser constraints)
        {

            var dbContext = new OrderRepository(ConnectionString, ProviderName);
            var resultCollection = dbContext.GetAllByCustomer(constraints.CustID); ;
            if (constraints.IsDateExist())
            {
                if (constraints.IsDateFromExist())
                {
                    resultCollection = resultCollection.Where(o => o.OrderDate >= Convert.ToDateTime(constraints.DateFrom));
                }
                if (constraints.IsDateToExist())
                {
                    resultCollection = resultCollection.Where(o => o.OrderDate <= Convert.ToDateTime(constraints.DateTo));
                }
            }


            if (constraints.IsSkipExist())
            {
                resultCollection = resultCollection.Skip(Convert.ToInt32(constraints.Skip));
            }
            if (constraints.IsTakeExist())
            {
                resultCollection = resultCollection.Take(Convert.ToInt32(constraints.Take));
            }

            return resultCollection;
        }
    }

}