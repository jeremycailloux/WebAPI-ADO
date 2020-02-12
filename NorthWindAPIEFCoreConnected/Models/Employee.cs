using System;
using System.Collections.Generic;

namespace NorthWindAPIEFCoreConnected.Models
{
    public partial class Employee
    {
        public Employee()
        {
            EmployeeTerritory = new HashSet<EmployeeTerritory>();
            InverseReportsToNavigation = new HashSet<Employee>();
            Orders = new HashSet<Orders>();
        }

        public int EmployeeId { get; set; }
        public Guid AddressId { get; set; }
        public int? ReportsTo { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Title { get; set; }
        public string TitleOfCourtesy { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? HireDate { get; set; }
        public byte[] Photo { get; set; }
        public string Notes { get; set; }

        public virtual Address Address { get; set; }
        public virtual Employee ReportsToNavigation { get; set; }
        public virtual ICollection<EmployeeTerritory> EmployeeTerritory { get; set; }
        public virtual ICollection<Employee> InverseReportsToNavigation { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
