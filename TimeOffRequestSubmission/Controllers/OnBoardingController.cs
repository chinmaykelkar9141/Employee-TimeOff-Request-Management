using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeOffRequestSubmission.Services;
using TimeOffRequestSubmission.ViewModels;

namespace TimeOffRequestSubmission.Controllers
{
    [ApiController]
    public class OnBoardingController: ControllerBase
    {
        private readonly ISignInService _signInService;
        private readonly ISignUpService _signUpService;

        public OnBoardingController(ISignInService signInService, ISignUpService signUpService)
        {
            _signInService = signInService;
            _signUpService = signUpService;
        }
        
        [AllowAnonymous]
        [HttpPost("[Controller]/[Action]")]
        public Task<TokenResponse> SignIn([FromBody] SignIn signInRequest)
        {
            return _signInService.SignIn(signInRequest);
        }
        
        [AllowAnonymous]
        [HttpPost("[Controller]/[Action]")]
        public async Task<IActionResult> SignUp([FromBody] SignUp signUpRequest)
        {
            await _signUpService.SignUp(signUpRequest);
            return Ok("Employee added successfully");
        }
    }
}