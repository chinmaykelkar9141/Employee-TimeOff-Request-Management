using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TimeOffRequestSubmission.Repositories
{
    public class RoleRepository: IRoleRepository
    {
        private readonly EmployeeManagementContext _context;

        public RoleRepository(EmployeeManagementContext context)
        {
            _context = context;
        }
        
        public async Task<int?> GetRoleIdByRoleName(string roleName)
        {
            return await _context.Roles
                .AsNoTracking()
                .Where(r => r.Name.Contains(roleName))
                .Select(x=>x.Id)
                .FirstOrDefaultAsync();
        }
    }
}