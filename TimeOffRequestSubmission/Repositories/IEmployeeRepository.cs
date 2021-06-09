using System.Collections.Generic;
using System.Threading.Tasks;
using TimeOffRequestSubmission.DataModels;

namespace TimeOffRequestSubmission.Repositories
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAllEmployees();
        Task AddManager(int employeeId, int managerId);
        Task SignUp(Employee employee);
        Task<Employee> GetEmployeeByUserNameAndPassword(string username, string password);
        Task<Employee> GetEmployeeById(int id);
        Task<Employee> GetEmployeeInformation(int employeeId);
    }
}