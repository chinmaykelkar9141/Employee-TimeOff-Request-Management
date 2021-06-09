using System.Threading.Tasks;

namespace TimeOffRequestSubmission.Repositories
{
    public interface IRoleRepository
    {
        public Task<int?> GetRoleIdByRoleName(string roleName);
    }
}