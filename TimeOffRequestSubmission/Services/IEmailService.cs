using System.Threading.Tasks;
using TimeOffRequestSubmission.ViewModels;

namespace TimeOffRequestSubmission.Services
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest emailRequest);
    }
}