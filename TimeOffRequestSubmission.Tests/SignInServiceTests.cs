using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using TimeOffRequestSubmission.DataModels;
using TimeOffRequestSubmission.Repositories;
using TimeOffRequestSubmission.Services;
using TimeOffRequestSubmission.ViewModels;
using Xunit;

namespace TimeOffRequestSubmission.Tests
{
    public class SignInServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly ISignInService _signInService;
        
        public SignInServiceTests()
        {
            var configurationMock = new Mock<IConfiguration>(MockBehavior.Loose);
            _employeeRepositoryMock = new Mock<IEmployeeRepository>(MockBehavior.Strict);
            _signInService = new SignInService(_employeeRepositoryMock.Object, configurationMock.Object);
        }
        [Fact]
        public async Task SignIn_Should_Throw_Unauthorized_Exception_When_Employee_Doesnt_Exists_In_The_System()
        {
            var signInRequest = new SignIn
            {
                UserName = "test1243",
                Password = "test1234"
            };

            _employeeRepositoryMock.Setup(
                    x => x.GetEmployeeByUserNameAndPassword(signInRequest.UserName, signInRequest.Password))
                .ReturnsAsync((Employee) null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _signInService.SignIn(signInRequest));
        }
    }
}