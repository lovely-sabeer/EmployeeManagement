using EmployeeManagement.Model;

namespace EmployeeManagement.ResponseModal
{
    public class SalaryDto
    {
        public Guid Id { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public decimal Amount { get; set; }
        public string? PaidDate { get; set; }
        public string? Status { get; set; }
    }

    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Email { get; set; }
    }

    public class EmployeeRes
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone{ get; set; }
        public string EmployeeId { get; set; }
        public string Gender { get; set; }
        public string Dob { get; set; }
        public string Department { get; set; }
        public decimal Salary { get; set; }
        public string JoiningDate { get; set; }
    }
}
