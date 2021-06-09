using System;

namespace TimeOffRequestSubmission.ViewModels
{
    public class TimeOffRequest
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Reason { get; set; }
    }
}