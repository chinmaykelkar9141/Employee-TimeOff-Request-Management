using System.Collections.Generic;
using System.Threading.Tasks;
using TimeOffRequestSubmission.ViewModels;

namespace TimeOffRequestSubmission.Services
{
    public interface IEmployeeManagementService
    {
        Task<List<EmployeeResponse>> GetAllEmployees();
        Task AddManager(int employeeId, int managerId);
    }
}