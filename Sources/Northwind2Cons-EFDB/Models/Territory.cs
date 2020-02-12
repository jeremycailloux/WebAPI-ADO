using System;
using System.Collections.Generic;

namespace Northwind2Cons_EFDB.Models
{
    public partial class Territory
    {
        public Territory()
        {
            EmployeeTerritory = new HashSet<EmployeeTerritory>();
        }

        public string TerritoryId { get; set; }
        public int RegionId { get; set; }
        public string Name { get; set; }

        public virtual Region Region { get; set; }
        public virtual ICollection<EmployeeTerritory> EmployeeTerritory { get; set; }
    }
}
