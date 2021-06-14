using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TimeOffRequestSubmission.DataModels;
using TimeOffRequestSubmission.Repositories;
using TimeOffRequestSubmission.ViewModels;

namespace TimeOffRequestSubmission.Services
{
    public class SignUpService: ISignUpService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRoleRepository _roleRepository;

        public SignUpService(IEmployeeRepository employeeRepository, IRoleRepository roleRepository)
        {
            _employeeRepository = employeeRepository;
            _roleRepository = roleRepository;
        }
        
        // register in the system
          public async Task SignUp(SignUp signUpRequest)
        {
            Validate(signUpRequest);
            var existingEmployee =
                await _employeeRepository.GetEmployeeByUserNameAndPassword(signUpRequest.UserName, signUpRequest.Password);
            if (existingEmployee != null)
            {
                throw new Exception(
                    "Employee with provided credentials is already present in the system. Please login to access the system");
            }
            
            var rolesId = await _roleRepository.GetRoleIdByRoleName(signUpRequest.Role);
            if (rolesId is null) // if role is not in three specified roles, throw exception
            {
                throw new Exception("Provided role is not present in the system");  
            }

            var newEmployee = new Employee
            {
                FirstName = signUpRequest.FirstName,
                LastName = signUpRequest.LastName,
                Email = signUpRequest.Email,
                UserName = signUpRequest.UserName,
                Password = signUpRequest.Password,
                RolesId = rolesId.Value
            };

            await _employeeRepository.SignUp(newEmployee);
        }

        private static void Validate(SignUp signUpRequest)
        {
            if (string.IsNullOrEmpty(signUpRequest.FirstName))
            {
                throw new ArgumentNullException(nameof(signUpRequest.FirstName));
            }
            
            if (string.IsNullOrEmpty(signUpRequest.LastName))
            {
                throw new ArgumentNullException(nameof(signUpRequest.LastName));
            }
            
            if (string.IsNullOrEmpty(signUpRequest.UserName))
            {
                throw new ArgumentNullException(nameof(signUpRequest.UserName));
            }
            
            if (string.IsNullOrEmpty(signUpRequest.Password))
            {
                throw new ArgumentNullException(nameof(signUpRequest.Password));
            }
            
            if (string.IsNullOrEmpty(signUpRequest.Email))
            {
                throw new ArgumentNullException(nameof(signUpRequest.Email));
            }

            var emailAddressAttribute = new EmailAddressAttribute();
            if (!emailAddressAttribute.IsValid(signUpRequest.Email))
            {
                throw new Exception("Invalid email address");
            }
            
            if (string.IsNullOrEmpty(signUpRequest.Role))
            {
                throw new ArgumentNullException(nameof(signUpRequest.Role));
            }
        }
    }
}