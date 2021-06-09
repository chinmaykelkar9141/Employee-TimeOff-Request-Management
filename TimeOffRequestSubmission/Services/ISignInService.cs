using System.Threading.Tasks;
using TimeOffRequestSubmission.ViewModels;

namespace TimeOffRequestSubmission.Services
{
    public interface ISignInService
    {
        Task<TokenResponse> SignIn(SignIn signInRequest);
    }
}