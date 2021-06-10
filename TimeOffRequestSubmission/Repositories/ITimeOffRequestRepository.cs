using System.Collections.Generic;
using System.Threading.Tasks;
using TimeOffRequestSubmission.DataModels;
using TimeOffRequestSubmission.Enums;
using TimeOffRequestSubmission.ViewModels;

namespace TimeOffRequestSubmission.Repositories
{
    public interface ITimeOffRequestRepository
    {
        Task CreateRequest(TimeoffRequest timeOffRequest);
        Task<ICollection<TimeoffRequest>> GetAllPendingTimeOffRequestsOfEmployeesUnderManager(int managerId);
        Task<TimeoffRequest> GetTimeOffRequestByIdAndEmployeeId(int timeOffRequestId, int employeeId);
        Task ApproveRequest(int timeOffRequestId, EApprovalStatus approved);
        Task RejectRequest(int timeOffRequestId, EApprovalStatus denied);
    }
}