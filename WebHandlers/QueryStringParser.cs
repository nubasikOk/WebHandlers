using System;
using System.Collections.Specialized;


namespace WebHandlers
{
    public class QueryStringParser
    {
        public string CustID { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime DateFrom { get; set; }

        public QueryStringParser(NameValueCollection queryString)
        {
            CustID = queryString["custID"];
            int take=int.MinValue, skip=int.MinValue;
            int.TryParse(queryString["take"],out take);
            int.TryParse(queryString["skip"], out skip);
            Take = take;
            Skip = skip;
            DateTime dateTo, dateFrom;
            DateTime.TryParse(queryString["dateTo"], out dateTo);
            DateTime.TryParse(queryString["dateFrom"], out dateFrom);
            DateTo = dateTo;
            DateFrom = dateFrom;
        }

        public bool IsTakeExist()
        {
            return Take!=int.MinValue;
        }

        public bool IsSkipExist()
        {
            return Skip != int.MinValue;
        }
        public bool IsDateToExist()
        {
            return DateTo!=DateTime.MinValue;
        }
        public bool IsDateFromExist()
        {
            return DateFrom!= DateTime.MinValue;
        }
    }
}