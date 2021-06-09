using System.Collections.Generic;
using System.Threading.Tasks;
using TimeOffRequestSubmission.ViewModels;

namespace TimeOffRequestSubmission.Services
{
    public interface ITimeOffHandlerService
    {
        Task CreateRequest(int employeeId, TimeOffRequest timeOffRequest);
        Task<List<EmployeeTimeOffRequest>> GetAllTimeOffRequestsOfEmployeesUnderManager(int managerId);
        Task ApproveRequest(int timeOffRequestId, int employeeId);
        Task RejectRequest(int timeOffRequestId, int employeeId, string reason);
    }
}