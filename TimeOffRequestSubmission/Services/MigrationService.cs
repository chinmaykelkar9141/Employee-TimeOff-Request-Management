using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TimeOffRequestSubmission.Services
{
    public static class MigrationService
    {
        public static void Migrate(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                serviceScope.ServiceProvider.GetService<EmployeeManagementContext>()?.Database.Migrate();
            }
        }
        
    }
}