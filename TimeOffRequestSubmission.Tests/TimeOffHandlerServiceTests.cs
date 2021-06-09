using System;
using System.Threading.Tasks;
using Moq;
using TimeOffRequestSubmission.DataModels;
using TimeOffRequestSubmission.Enums;
using TimeOffRequestSubmission.Repositories;
using TimeOffRequestSubmission.Services;
using TimeOffRequestSubmission.ViewModels;
using Xunit;

namespace TimeOffRequestSubmission.Tests
{
    public class TimeOffHandlerServiceTests
    {
        private Mock<IEmployeeRepository> _employeeRepositoryMock;
        private Mock<ITimeOffRequestRepository> _timeOffRequestRepositoryMock;
        private Mock<IEmailService> _emailServiceMock;
        private ITimeOffHandlerService _timeOffHandlerService;

        public TimeOffHandlerServiceTests()
        {
            _emailServiceMock = new Mock<IEmailService>(MockBehavior.Strict);
            _timeOffRequestRepositoryMock = new Mock<ITimeOffRequestRepository>(MockBehavior.Strict);
            _employeeRepositoryMock = new Mock<IEmployeeRepository>(MockBehavior.Strict);
            _timeOffHandlerService = new TimeOffHandlerService(_employeeRepositoryMock.Object, _emailServiceMock.Object,
                _timeOffRequestRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateRequest_Should_Throw_Exception_When_Manager_Is_Not_Assigned_To_Employee()
        {
            const int employeeId = 10;
            var timeOffRequest = new TimeOffRequest
            {
                Start = DateTime.Now.AddDays(1),
                End = DateTime.Now.AddDays(4),
                Reason = "sick"
            };
            
            _employeeRepositoryMock.Setup(x => x.GetEmployeeInformation(employeeId)).ReturnsAsync(new Employee
            {
                Manager = null
            });
            var exception = await Assert.ThrowsAsync<Exception>(() => _timeOffHandlerService.CreateRequest(employeeId, timeOffRequest));
            Assert.Equal("Manager not assigned",exception.Message);
        }
        
        [Fact]
        public async Task CreateRequest_Should_Throw_Exception_When_Employee_Doesnt_Submit_Reason()
        {
            const int employeeId = 10;
            var timeOffRequest = new TimeOffRequest
            {
                Start = DateTime.Now.AddDays(1),
                End = DateTime.Now.AddDays(4),
                Reason = string.Empty
            };
            _employeeRepositoryMock.Setup(x => x.GetEmployeeInformation(employeeId)).ReturnsAsync(new Employee
            {
                Manager = new Employee()
            }); 
            await Assert.ThrowsAsync<ArgumentNullException>(() => _timeOffHandlerService.CreateRequest(employeeId, timeOffRequest));
        }
        
        [Fact]
        public async Task CreateRequest_Should_Throw_Exception_When_EndDate_Before_StartDate()
        {
            const int employeeId = 10;
            var timeOffRequest = new TimeOffRequest
            {
                Start = DateTime.Now.AddDays(1),
                End = DateTime.Now.AddDays(-4),
                Reason = "sick"
            };
            _employeeRepositoryMock.Setup(x => x.GetEmployeeInformation(employeeId)).ReturnsAsync(new Employee
            {
                Manager = new Employee()
            }); 
            var exception = await Assert.ThrowsAsync<Exception>(() => _timeOffHandlerService.CreateRequest(employeeId, timeOffRequest));
            Assert.Equal("End Date can't be be before Start Date", exception.Message);
        }
        
        [Fact]
        public async Task CreateRequest_Should_Throw_Exception_When_EndDate_And_StartDate_Are_In_Past()
        {
            const int employeeId = 10;
            var timeOffRequest = new TimeOffRequest
            {
                Start = DateTime.Now.AddDays(-6),
                End = DateTime.Now.AddDays(-4),
                Reason = "sick"
            };
            _employeeRepositoryMock.Setup(x => x.GetEmployeeInformation(employeeId)).ReturnsAsync(new Employee
            {
                Manager = new Employee()
            }); 
            var exception = await Assert.ThrowsAsync<Exception>(() => _timeOffHandlerService.CreateRequest(employeeId, timeOffRequest));
            Assert.Equal("Please add dates in future", exception.Message);
        }

        [Fact]
        public async Task CreateRequest_Should_Create_Record_In_The_Table()
        {
            const int employeeId = 10;
            var timeOffRequest = new TimeOffRequest
            {
                Start = DateTime.Now.AddDays(6),
                End = DateTime.Now.AddDays(8),
                Reason = "sick"
            };

            var employee = new Employee
            {
                Id = 10,
                Email = "a@fmail.com",
                Manager = new Employee
                {
                    Email = "b@gmail.com"
                }
            };
            _employeeRepositoryMock.Setup(x => x.GetEmployeeInformation(employeeId)).ReturnsAsync(employee);

            _timeOffRequestRepositoryMock.Setup(x => x.CreateRequest(It.IsAny<TimeoffRequest>()))
                .Returns(Task.CompletedTask);
            
            _emailServiceMock.Setup(x => x.SendAsync(It.IsAny<EmailRequest>()))
                .Returns(Task.CompletedTask);

            await _timeOffHandlerService.CreateRequest(employeeId, timeOffRequest);

            _timeOffRequestRepositoryMock.Verify(x => x.CreateRequest(It.Is<TimeoffRequest>(
                request => request.EmployeeId == employeeId && request.StartDate == timeOffRequest.Start &&
                           request.Enddate == timeOffRequest.End)), Times.Once);
            
            _emailServiceMock.Verify(x => x.SendAsync(It.Is<EmailRequest>(
                request => request.From == employee.Email && request.To == employee.Manager.Email)), Times.Once);
        }

        [Fact]
        public async Task ApproveRequest_Should_Throw_Exception_When_RequestId_And_EmployeeId_Doesnt_Match()
        {
            const int timeOffRequestId = 1;
            const int employeeId = 10;
            _timeOffRequestRepositoryMock.Setup(t => t.GetTimeOffRequestByIdAndEmployeeId(timeOffRequestId, employeeId))
                .ReturnsAsync((TimeoffRequest) null);
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _timeOffHandlerService.ApproveRequest(timeOffRequestId, employeeId));
            Assert.Equal("Time off request is not assigned to employee",exception.Message);
        }

        [Fact]
        public async Task ApproveRequest_Should_Update_Record_In_The_Database_As_Approved_And_Send_Email_To_Employee_From_Manager()
        {
            const int timeOffRequestId = 1;
            const int employeeId = 10;
            var timeOffRequest = new TimeoffRequest
            {
                Id = 1,
                StartDate = DateTime.Now.AddDays(6),
                Enddate = DateTime.Now.AddDays(8),
                Reason = "sick"
            }; 
            
            var employee = new Employee
            {
                Id = 10,
                Email = "a@fmail.com",
                Manager = new Employee
                {
                    Email = "b@gmail.com"
                }
            };
            
            _timeOffRequestRepositoryMock.Setup(t => t.GetTimeOffRequestByIdAndEmployeeId(timeOffRequestId, employeeId))
                .ReturnsAsync(timeOffRequest);
            
            _timeOffRequestRepositoryMock.Setup(t => t.ApproveRequest(timeOffRequestId, EApprovalStatus.Approved))
                .Returns(Task.CompletedTask);

            _employeeRepositoryMock.Setup(e => e.GetEmployeeInformation(employeeId)).ReturnsAsync(employee);
            
            _emailServiceMock.Setup(x => x.SendAsync(It.IsAny<EmailRequest>()))
                .Returns(Task.CompletedTask);

            await _timeOffHandlerService.ApproveRequest(timeOffRequestId, employeeId);
            
            _timeOffRequestRepositoryMock.Verify(x=>x.ApproveRequest(It.Is<int>(id=>id == timeOffRequestId),It.Is<EApprovalStatus>(status => status == EApprovalStatus.Approved)), Times.Once);

            _emailServiceMock.Verify(x => x.SendAsync(It.Is<EmailRequest>(
                request => request.From == employee.Manager.Email && request.To == employee.Email)), Times.Once);

        }

        [Fact]
        public async Task RejectRequest_Should_Throw_Exception_When_Reject_Reason_Not_Specified_By_Manager()
        {
            const int timeOffRequestId = 1;
            const int employeeId = 10;
            var reason = string.Empty;
            var exception = await Assert.ThrowsAsync<Exception>(() =>  _timeOffHandlerService.RejectRequest(timeOffRequestId, employeeId, reason));
            Assert.Equal("Please mention reject reason",exception.Message);
        }
        
        [Fact]
        public async Task RejectRequest_Should_Throw_Exception_When_RequestId_And_EmployeeId_Doesnt_Match()
        {
            const int timeOffRequestId = 1;
            const int employeeId = 10;
            const string reason = "too much workload";
            _timeOffRequestRepositoryMock.Setup(t => t.GetTimeOffRequestByIdAndEmployeeId(timeOffRequestId, employeeId))
                .ReturnsAsync((TimeoffRequest) null);
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _timeOffHandlerService.RejectRequest(timeOffRequestId, employeeId, reason));
            Assert.Equal("Time off request is not assigned to employee",exception.Message);
        }
        
        [Fact]
        public async Task RejectRequest_Should_Update_Record_In_The_Database_As_Denied_And_Send_Email_To_Employee_From_Manager()
        {
            const int timeOffRequestId = 1;
            const int employeeId = 10;
            const string reason = "too much workload";
            var timeOffRequest = new TimeoffRequest
            {
                Id = 1,
                StartDate = DateTime.Now.AddDays(6),
                Enddate = DateTime.Now.AddDays(8),
                Reason = "sick"
            }; 
            
            var employee = new Employee
            {
                Id = 10,
                Email = "a@fmail.com",
                Manager = new Employee
                {
                    Email = "b@gmail.com"
                }
            };
            
            _timeOffRequestRepositoryMock.Setup(t => t.GetTimeOffRequestByIdAndEmployeeId(timeOffRequestId, employeeId))
                .ReturnsAsync(timeOffRequest);
            
            _timeOffRequestRepositoryMock.Setup(t => t.RejectRequest(timeOffRequestId, EApprovalStatus.Denied))
                .Returns(Task.CompletedTask);

            _employeeRepositoryMock.Setup(e => e.GetEmployeeInformation(employeeId)).ReturnsAsync(employee);
            
            _emailServiceMock.Setup(x => x.SendAsync(It.IsAny<EmailRequest>()))
                .Returns(Task.CompletedTask);

            await _timeOffHandlerService.RejectRequest(timeOffRequestId, employeeId, reason);
            
            _timeOffRequestRepositoryMock.Verify(x=>x.RejectRequest(It.Is<int>(id=>id == timeOffRequestId),It.Is<EApprovalStatus>(status => status == EApprovalStatus.Denied)), Times.Once);

            _emailServiceMock.Verify(x => x.SendAsync(It.Is<EmailRequest>(
                request => request.From == employee.Manager.Email && request.To == employee.Email)), Times.Once);

        }
        
    }
}