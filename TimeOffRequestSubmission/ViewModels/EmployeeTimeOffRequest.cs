using System;

namespace TimeOffRequestSubmission.ViewModels
{
    public class EmployeeTimeOffRequest
    {
        public int TimeOffRequestId { get; set; }
        
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Reason { get; set; }
    }
}
