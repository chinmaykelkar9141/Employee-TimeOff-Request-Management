using System;
using TimeOffRequestSubmission.Enums;

#nullable disable

namespace TimeOffRequestSubmission.DataModels
{
    public class TimeoffRequest
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime Enddate { get; set; }
        public string Reason { get; set; }
        public EApprovalStatus ApprovalStatus { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
