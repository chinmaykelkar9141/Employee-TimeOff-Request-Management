using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeOffRequestSubmission.DataModels;

namespace TimeOffRequestSubmission.Repositories
{
    public class EmployeeRepository: IEmployeeRepository
    {
        private readonly EmployeeManagementContext _context;

        public EmployeeRepository(EmployeeManagementContext context)
        {
            _context = context;
        }
        
        public async Task<Employee> GetEmployeeByUserNameAndPassword(string username, string password)
        {
            return await _context.Employees
                .AsNoTracking()
                .Include(e=>e.Roles)
                .FirstOrDefaultAsync(r => r.UserName.Contains(username) && r.Password.Contains(password));
        }

        public async Task<Employee> GetEmployeeById(int id)
        {
            return await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public Task<Employee> GetEmployeeInformation(int employeeId)
        {
            return _context.Employees
                .Include(x => x.Manager)
                .Where(x => x.Id == employeeId)
                .FirstOrDefaultAsync();
        }

        public async Task SignUp(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }
        
        public async Task<List<Employee>> GetAllEmployees()
        {
            return await _context.Employees
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddManager(int employeeId, int managerId)
        {
            var currentEmployeeRecord = await _context.Employees.FirstAsync(x => x.Id == employeeId);
            currentEmployeeRecord.ManagerId = managerId;
            _context.Update(currentEmployeeRecord);
            await _context.SaveChangesAsync();
        }
        
    }
}