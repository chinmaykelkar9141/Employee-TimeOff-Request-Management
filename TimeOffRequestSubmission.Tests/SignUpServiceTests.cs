using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using TimeOffRequestSubmission.DataModels;
using TimeOffRequestSubmission.Repositories;
using TimeOffRequestSubmission.Services;
using TimeOffRequestSubmission.ViewModels;
using Xunit;

namespace TimeOffRequestSubmission.Tests
{
    public class SignUpServiceTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly ISignUpService _signUpService;
        
        public static IEnumerable<object[]> InputWithEmptyValues =>
            new List<object[]>
            {
                new object[] {new SignUp {FirstName = string.Empty}},
                new object[] {new SignUp {LastName = string.Empty}},
                new object[] {new SignUp {UserName = string.Empty}},
                new object[] {new SignUp {Password = string.Empty}},
                new object[] {new SignUp {Email = string.Empty}},
                new object[] {new SignUp {Role = string.Empty}},
            };
        
        public static IEnumerable<object[]> InputWithNullValues =>
            new List<object[]>
            {
                new object[] {new SignUp {FirstName = null}},
                new object[] {new SignUp {LastName = null}},
                new object[] {new SignUp {UserName = null}},
                new object[] {new SignUp {Password = null}},
                new object[] {new SignUp {Email = null}},
                new object[] {new SignUp {Role = null}},
            };
        
        public SignUpServiceTests()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>(MockBehavior.Strict);
            _roleRepositoryMock = new Mock<IRoleRepository>(MockBehavior.Strict);
            _signUpService = new SignUpService(_employeeRepositoryMock.Object, _roleRepositoryMock.Object);
        }

        [Theory]
        [MemberData(nameof(InputWithEmptyValues))]
        public async Task Signup_Should_Throw_Exceptions_When_Input_Provided_Is_Empty(SignUp signUpRequest)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _signUpService.SignUp(signUpRequest));
        }
        
        [Theory]
        [MemberData(nameof(InputWithNullValues))]
        public async Task Signup_Should_Throw_Exceptions_When_Input_Provided_Is_Null(SignUp signUpRequest)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _signUpService.SignUp(signUpRequest));
        }

        [Fact]
        public async Task SignUp_Should_Throw_Exception_When_Provided_Role_Is_Not_Present_In_The_System()
        {
            var input = new SignUp
            {
                FirstName = "Apurva",
                LastName = "Kelkar",
                UserName = "amande1",
                Password = "ajit@1968",
                Email = "amande@gmail.com",
                Role = "testrole"
            };

            _employeeRepositoryMock.Setup(e => e.GetEmployeeByUserNameAndPassword(input.UserName, input.Password)).ReturnsAsync((Employee)null);
            _roleRepositoryMock.Setup(e => e.GetRoleIdByRoleName(input.Role)).ReturnsAsync((int?) null);
            var exception = await Assert.ThrowsAsync<Exception>(() => _signUpService.SignUp(input));
            Assert.Equal("Provided role is not present in the system",exception.Message);
        }

        [Fact]
        public async Task SignUp_Should_Throw_Exception_When_Invalid_Email_Is_Provided()
        {
            var input = new SignUp
            {
                FirstName = "Apurva",
                LastName = "Kelkar",
                UserName = "amande1",
                Password = "ajit@1968",
                Email = "amande",
                Role = "SystemAdmin"
            };
            
            var exception = await Assert.ThrowsAsync<Exception>(() => _signUpService.SignUp(input));
            Assert.Equal("Invalid email address",exception.Message);
        }
        
        [Fact]
        public async Task SignUp_Should_Throw_Exception_When_Employee_Is_Already_Present_In_The_System()
        {
            var input = new SignUp
            {
                FirstName = "Apurva",
                LastName = "Kelkar",
                UserName = "amande1",
                Password = "ajit@1968",
                Email = "amande@gmail.com",
                Role = "SystemAdmin"
            };

            _employeeRepositoryMock.Setup(x => x.GetEmployeeByUserNameAndPassword(input.UserName, input.Password))
                .ReturnsAsync(new Employee
                {
                    FirstName = "Apurva",
                    LastName = "Kelkar",
                    UserName = "amande1",
                    Password = "ajit@1968",
                    Email = "amande@gmail.com",
                    RolesId = 1
                });

            var exception = await Assert.ThrowsAsync<Exception>(() => _signUpService.SignUp(input));
            Assert.Equal("Employee with provided credentials is already present in the system. Please login to access the system", exception.Message);
        }

        [Fact]
        public async Task SignUp_Should_Create_Employee_When_Input_Is_Valid()
        {
            var input = new SignUp
            {
                FirstName = "Apurva",
                LastName = "Kelkar",
                UserName = "amande1",
                Password = "ajit@1968",
                Email = "amande@gmail.com",
                Role = "SystemAdmin"
            };

            _employeeRepositoryMock.Setup(x => x.GetEmployeeByUserNameAndPassword(input.UserName, input.Password))
                .ReturnsAsync((Employee) null);

            _roleRepositoryMock.Setup(x => x.GetRoleIdByRoleName(input.Role)).ReturnsAsync(1);
            _employeeRepositoryMock.Setup(x => x.SignUp(It.IsAny<Employee>())).Returns(Task.CompletedTask);

            await _signUpService.SignUp(input);
            
            _employeeRepositoryMock.Verify(x=>x.SignUp(It.IsAny<Employee>()), Times.Once);
            
        }
    }
}