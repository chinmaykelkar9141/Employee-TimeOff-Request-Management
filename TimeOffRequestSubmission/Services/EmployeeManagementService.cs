using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeOffRequestSubmission.Repositories;
using TimeOffRequestSubmission.ViewModels;

namespace TimeOffRequestSubmission.Services
{
    public class EmployeeManagementService: IEmployeeManagementService
    {
        private readonly IEmployeeRepository _employeeRepository;
        
        public EmployeeManagementService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        public async Task<List<EmployeeResponse>> GetAllEmployees()
        {
            var employeeDto = await _employeeRepository.GetAllEmployees();
            return employeeDto.Select(x => new EmployeeResponse
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email
            }).ToList();
        }

        public async Task AddManager(int employeeId, int managerId)
        {
            var employee = await _employeeRepository.GetEmployeeById(employeeId);
            if (employee is null)
            {
                throw new Exception("employee doesn't exists in the system");
            }
            var manager = await _employeeRepository.GetEmployeeById(managerId);
            if (manager is null)
            {
                throw new Exception("manager doesn't exists in the system");
            }

            await _employeeRepository.AddManager(employeeId, managerId);

        }
    }
}