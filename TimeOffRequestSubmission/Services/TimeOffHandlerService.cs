using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeOffRequestSubmission.Enums;
using TimeOffRequestSubmission.Repositories;
using TimeOffRequestSubmission.ViewModels;

namespace TimeOffRequestSubmission.Services
{
    public class TimeOffHandlerService: ITimeOffHandlerService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailService _emailService;
        private readonly ITimeOffRequestRepository _timeOffRequestRepository;

        public TimeOffHandlerService(IEmployeeRepository employeeRepository,
            IEmailService emailService,
            ITimeOffRequestRepository timeOffRequestRepository)
        {
            _employeeRepository = employeeRepository;
            _emailService = emailService;
            _timeOffRequestRepository = timeOffRequestRepository;
        }
        
        public async Task CreateRequest(int employeeId, TimeOffRequest timeOffRequest)
        {
            Validate(timeOffRequest);
            var employee = await _employeeRepository.GetEmployeeInformation(employeeId);
            var manager = employee?.Manager;
            if (manager is null)
            {
                throw new Exception("Manager not assigned");
            }

            var timeOffRequestDto = new DataModels.TimeoffRequest
            {
                EmployeeId = employee.Id,
                StartDate = timeOffRequest.Start,   
                Enddate = timeOffRequest.End,
                ApprovalStatus = EApprovalStatus.Pending,
                Reason = timeOffRequest.Reason
            };
            await _timeOffRequestRepository.CreateRequest(timeOffRequestDto);

            var emailRequest = new EmailRequest
            {
                From = employee.Email,
                To = manager.Email,
                Subject = $"Requesting time-off from {timeOffRequest.Start} to {timeOffRequest.End}",
                Body = $"<div>Hello {manager.FirstName},</div>" +
                       $"<div>I am requesting a time-off due to {timeOffRequest.Reason}. Please consider it.</div>" +
                       $"<div> Thank you,</div> " +
                       $"<div> {employee.FirstName}</div>"
            };
            await _emailService.SendAsync(emailRequest);
        }

        private static void Validate(TimeOffRequest request)
        {
            if (string.IsNullOrEmpty(request.Reason))
            {
                throw new ArgumentNullException(nameof(request.Reason));
            }

            if (request.End < request.Start)
            {
                throw new Exception("End Date can't be be before Start Date");
            }
            
            if (request.End < DateTime.Now ||  request.Start < DateTime.Now)
            {
                throw new Exception("Please add dates in future");
            }
        }

        public async Task<List<EmployeeTimeOffRequest>> GetAllPendingTimeOffRequestsOfEmployeesUnderManager(int managerId)
        {
            var timeOffRequestsDto =
                await _timeOffRequestRepository.GetAllPendingTimeOffRequestsOfEmployeesUnderManager(managerId);
            return timeOffRequestsDto.Select(x => new EmployeeTimeOffRequest
            {
                TimeOffRequestId = x.Id,
                EmployeeId = x.EmployeeId,
                FirstName = x.Employee.FirstName,
                LastName = x.Employee.LastName,
                Start = x.StartDate,
                End = x.Enddate,
                Reason = x.Reason,
            }).ToList();
        }

        //approve employee request
        public async Task ApproveRequest(int timeOffRequestId, int employeeId)
        {
            var timeOffRequest =
                await _timeOffRequestRepository.GetTimeOffRequestByIdAndEmployeeId(timeOffRequestId, employeeId);
            if (timeOffRequest is null)
            {
                throw new Exception("Time off request is not assigned to employee");
            }

            var employee = await _employeeRepository.GetEmployeeInformation(employeeId);
            var manager = employee.Manager;

            await _timeOffRequestRepository.ApproveRequest(timeOffRequestId,EApprovalStatus.Approved);
            
            var emailRequest = new EmailRequest
            {
                From = manager.Email,
                To = employee.Email,
                Subject = "Time off request approved",
                Body = $"<div>Hello {employee.FirstName},</div>" +
                       $"<div>Your time off request from {timeOffRequest.StartDate} to {timeOffRequest.Enddate} has been approved. Enjoy your vacation </div>" +
                       "<div> Thank you,</div>" +
                       $"<div> {manager.FirstName}</div>"
            };
            await _emailService.SendAsync(emailRequest);
        }

        // reject employee request
        public async Task RejectRequest(int timeOffRequestId, int employeeId, string reason)
        {
            if (string.IsNullOrEmpty(reason))
            {
                throw new Exception("Please mention reject reason");
            }
            var timeOffRequest =
                await _timeOffRequestRepository.GetTimeOffRequestByIdAndEmployeeId(timeOffRequestId, employeeId);
            if (timeOffRequest is null)
            {
                throw new Exception("Time off request is not assigned to employee");
            }
            
            var employee = await _employeeRepository.GetEmployeeInformation(employeeId);
            var manager = employee.Manager;
            
            await _timeOffRequestRepository.RejectRequest(timeOffRequestId,EApprovalStatus.Denied);
            
            var emailRequest = new EmailRequest
            {
                From = manager.Email,
                To = employee.Email,
                Subject = "Time off request denied",
                Body = $"<div>Hello {employee.FirstName},</div>" +
                       $"<div>Your time off request from {timeOffRequest.StartDate} to {timeOffRequest.Enddate} has been denied due to {reason}. Please contact me for further questions </div>" +
                       $"<div> Thank you,</div> " +
                       $"<div> {manager.FirstName}</div>"
            };
            await _emailService.SendAsync(emailRequest);
            
        }
    }
}