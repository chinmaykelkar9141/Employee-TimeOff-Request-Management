using System.Threading.Tasks;
using TimeOffRequestSubmission.ViewModels;

namespace TimeOffRequestSubmission.Services
{
    public interface ISignUpService
    {
        Task SignUp(SignUp signUpRequest);
    }
}