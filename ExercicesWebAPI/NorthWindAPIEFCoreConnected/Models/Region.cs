using System;
using System.Collections.Generic;

namespace NorthWindAPIEFCoreConnected.Models
{
    public partial class Region
    {
        public Region()
        {
            Territory = new HashSet<Territory>();
        }

        public int RegionId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Territory> Territory { get; set; }
    }
}
