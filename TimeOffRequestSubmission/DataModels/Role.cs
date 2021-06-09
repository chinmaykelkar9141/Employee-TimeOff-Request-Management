using System.Collections.Generic;

#nullable disable

namespace TimeOffRequestSubmission.DataModels
{
    public class Role
    {
        public Role()
        {
            Employees = new HashSet<Employee>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
