using EmployeeManagement.Model;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EmployeeManagement.RequestModal
{
    public class RequestModals
    {
    }


    public class AdminRm
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class LoginRes
    {
        public string Token { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public class LoginReq
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class AttendanceListReq
    {
        public string? Search{get; set;}
        public string? Department{get; set;}
        public string? Status{get; set;}
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AttendanceDateFilter? Date {get; set;}
        public int Page{get; set;}
        public int PageSize { get; set;}
    }
    public class SalaryReq
    {
        public List<Guid> EmployeeIds { get; set; } = [];
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentStatus Status { get; set; }
    }

    public class EmployeeManagementRm
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public long Phone{ get; set; }
        public string Gender { get; set; }
        public DateTime Dob { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Department Department { get; set; }
        public decimal Salary { get; set; }
        public DateTime JoiningDate { get; set; }
    }




    public class AttendanceRes
    {
        public Guid Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty; 
    }

    public class AttendanceStatCardRes
    {
        public int TotalEmployees { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int ContinuousLeaveCount { get; set; }
    }


    public class BulkAttendanceReq
    {
        public List<Guid> EmployeeIds { get; set; } = [];
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AttendenceStatus Status { get; set; }
    }

    public class MainDashboardDto
    {
        public int TotalEmployees { get; set; }
        public decimal PaidThisMonth { get; set; }
        public int NewJoinees { get; set; }
        public int PresentToday { get; set; }
        public List<MonthlyTrendDto> RecruitmentTrends { get; set; } = new();
        public List<DepartmentCountDto> DepartmentHub { get; set; } = new();
        public List<RecentJoineeDto> RecentNewJoinees { get; set; } = new();
    }

    public class MonthlyTrendDto
    {
        public string Month { get; set; } 
        public int RecruitmentCount { get; set; }
        public int ResignationCount { get; set; }
    }

    public class DepartmentCountDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int Headcount { get; set; } 
    }

    public class RecentJoineeDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
    }

    public class PayrollRm
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

}
