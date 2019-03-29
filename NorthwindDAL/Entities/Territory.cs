using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindDAL.Entities
{
    public class Territory
    {
        public string Id { get; set; }

        public string TerritoryDescription { get; set; }

        public int RegionId { get; set; }
    }
}
