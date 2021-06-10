using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeOffRequestSubmission.Services;
using TimeOffRequestSubmission.ViewModels;

namespace TimeOffRequestSubmission.Controllers
{
    
    [ApiController]
    public class TimeOffHandlerController: ControllerBase
    {
        private readonly ITimeOffHandlerService _timeOffHandlerService;

        public TimeOffHandlerController(ITimeOffHandlerService timeOffHandlerService)
        {
            _timeOffHandlerService = timeOffHandlerService;
        }
        
        [Authorize(Roles = Role.Employee)]
        [HttpPost("[Controller]/[Action]")]
        public async Task CreateRequest([FromBody] TimeOffRequest timeOffRequest)
        {
            var employeeId = int.Parse(User.Claims.First(x => x.Type == "employeeId").Value);
            await _timeOffHandlerService.CreateRequest(employeeId, timeOffRequest);
        }

        [Authorize(Roles = Role.Manager)]
        [HttpGet("[Controller]/[Action]")]
        public async Task<List<EmployeeTimeOffRequest>> GetAllPendingTimeOffRequestsOfEmployeesUnderManager()
        {
            var managerId = int.Parse(User.Claims.First(x => x.Type == "employeeId").Value);
            return await _timeOffHandlerService.GetAllPendingTimeOffRequestsOfEmployeesUnderManager(managerId);
        }
        
        [Authorize(Roles = Role.Manager)]
        [HttpPut("[Controller]/[Action]")]
        public async Task ApproveRequest([FromQuery]int timeOffRequestId, [FromQuery]int employeeId)
        {
            await _timeOffHandlerService.ApproveRequest(timeOffRequestId, employeeId);
        }
        
        [Authorize(Roles = Role.Manager)]
        [HttpPut("[Controller]/[Action]")]
        public async Task RejectRequest([FromQuery]int timeOffRequestId, [FromQuery]int employeeId, [FromQuery] string reason)
        {
            await _timeOffHandlerService.RejectRequest(timeOffRequestId, employeeId, reason);
        }
        
    }
}