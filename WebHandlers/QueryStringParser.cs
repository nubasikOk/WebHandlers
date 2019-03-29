using System;
using System.Collections.Specialized;


namespace WebHandlers
{
    public class QueryStringParser
    {
        public string CustID { get; set; }
        public string Take { get; set; }
        public string Skip { get; set; }
        public string DateTo { get; set; }

        public string DateFrom { get; set; }
        public QueryStringParser(NameValueCollection queryString)
        {
            CustID = queryString["custID"];
            Take = queryString["take"];
            Skip = queryString["skip"];
            DateTo = queryString["dateTo"];
            DateFrom = queryString["dateFrom"];
        }


        public bool IsDateExist()
        {
            return !String.IsNullOrEmpty(DateTo) && !String.IsNullOrEmpty(DateFrom);
        }

        public bool IsTakeExist()
        {
            return !String.IsNullOrEmpty(Take);
        }

        public bool IsSkipExist()
        {
            return !String.IsNullOrEmpty(Skip);
        }
        public bool IsDateToExist()
        {
            return !String.IsNullOrEmpty(DateTo);
        }
        public bool IsDateFromExist()
        {
            return !String.IsNullOrEmpty(DateFrom);
        }
    }
}