using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TimeOffRequestSubmission.DataModels;
using TimeOffRequestSubmission.Repositories;
using TimeOffRequestSubmission.ViewModels;

namespace TimeOffRequestSubmission.Services
{
    public class SignInService: ISignInService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IConfiguration _configuration;

        public SignInService(IEmployeeRepository employeeRepository, IConfiguration configuration)
        {
            _employeeRepository = employeeRepository;
            _configuration = configuration;
        }
        
        public async Task<TokenResponse> SignIn(SignIn signInRequest)
        {
            var existingEmployee = 
                await _employeeRepository.GetEmployeeByUserNameAndPassword(signInRequest.UserName, signInRequest.Password);
            if (existingEmployee is null)
            {
                throw new UnauthorizedAccessException();
            }
            
            var token = CreateToken(existingEmployee);

            return new TokenResponse
            {
                FirstName = existingEmployee.FirstName,
                LastName = existingEmployee.LastName,
                Email = existingEmployee.Email,
                Token = token
            };
        }
        
        private string CreateToken(Employee existingEmployee)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("Secret"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {new("employeeId", existingEmployee.Id.ToString()), new(ClaimTypes.Role, existingEmployee.Roles.Name)}),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        
    }
}