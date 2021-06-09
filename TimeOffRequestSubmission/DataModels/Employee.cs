using System.Collections.Generic;

#nullable disable

namespace TimeOffRequestSubmission.DataModels
{
    public class Employee
    {
        public Employee()
        {
            Managers = new HashSet<Employee>();
            TimeOffRequests = new HashSet<TimeoffRequest>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int RolesId { get; set; }
        public int? ManagerId { get; set; }

        public virtual Employee Manager { get; set; }
        public virtual Role Roles { get; set; }
        public virtual ICollection<Employee> Managers { get; set; }
        public virtual ICollection<TimeoffRequest> TimeOffRequests { get; set; }
    }
}
