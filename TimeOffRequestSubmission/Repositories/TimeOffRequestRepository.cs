using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeOffRequestSubmission.DataModels;
using TimeOffRequestSubmission.Enums;

namespace TimeOffRequestSubmission.Repositories
{
    
    public class TimeOffRequestRepository: ITimeOffRequestRepository
    {
        private readonly EmployeeManagementContext _context;

        public TimeOffRequestRepository(EmployeeManagementContext context)
        {
            _context = context;
        }
        public async Task CreateRequest(TimeoffRequest timeOffRequest)
        {
            await _context.AddAsync(timeOffRequest);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<TimeoffRequest>> GetAllTimeOffRequestsOfEmployeesUnderManager(int managerId)
        {
            return await _context.Employees
                .AsNoTracking()
                .Include(e => e.TimeOffRequests)
                .ThenInclude(t=>t.Employee)
                .Where(e => e.ManagerId == managerId)
                .Select(e => e.TimeOffRequests)
                .FirstOrDefaultAsync();
        }

        public async Task<TimeoffRequest> GetTimeOffRequestByIdAndEmployeeId(int timeOffRequestId, int employeeId)
        {
            return await _context.TimeoffRequests.FirstOrDefaultAsync(x => x.Id == timeOffRequestId && x.EmployeeId == employeeId);
        }

        public async Task ApproveRequest(int timeOffRequestId, EApprovalStatus approved)
        {
            var request = await _context.TimeoffRequests.FirstAsync(x => x.Id == timeOffRequestId);
            request.ApprovalStatus = approved;
            _context.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task RejectRequest(int timeOffRequestId, EApprovalStatus denied)
        {
            var request = await _context.TimeoffRequests.FirstAsync(x => x.Id == timeOffRequestId);
            request.ApprovalStatus = denied;
            _context.Update(request);
            await _context.SaveChangesAsync();
        }
    }
}