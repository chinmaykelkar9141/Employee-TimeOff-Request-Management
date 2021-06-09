using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeOffRequestSubmission.Services;
using TimeOffRequestSubmission.ViewModels;

namespace TimeOffRequestSubmission.Controllers
{
    [ApiController]
    public class EmployeeManagementController
    {
        private readonly IEmployeeManagementService _employeeManagementService;

        public EmployeeManagementController(IEmployeeManagementService employeeManagementService)
        {
            _employeeManagementService = employeeManagementService;
        }
        
        [Authorize(Roles = Role.SystemAdmin)]
        [HttpPost("[Controller]/[Action]")]
        public async Task AddManager([FromQuery] int employeeId, [FromQuery] int managerId)
        {
            await _employeeManagementService.AddManager(employeeId, managerId);
        }
        
        [Authorize(Roles = Role.SystemAdmin)]
        [HttpGet("[Controller]/[Action]")]
        public async Task<List<EmployeeResponse>> GetAllEmployees()
        {
            return await _employeeManagementService.GetAllEmployees();
        }
    }
}