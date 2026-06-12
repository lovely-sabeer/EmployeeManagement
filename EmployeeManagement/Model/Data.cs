namespace EmployeeManagement.Model
{
    public enum Department
    {
        Engineering,
        Marketing, 
        Design
    }
    public enum AttendanceDateFilter
    {
        Today,
        ThisWeek,
        ThisMonth,
        ThisYear
    }
    public enum AttendenceStatus
    {
        Present,
        Absent,
        HalfDay
    }

    public enum PaymentStatus
    {
        Unpaid,
        Paid,
        Hold
    }
    public class BaseEntity
    {
        
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } 
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
    
    public class Admin: BaseEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }


    public class Employee: BaseEntity
    {
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string EmployeeId { get; set; }
        public string Gender { get; set; }
        public DateTime DateofBirth { get; set; }
        public Department Department { get; set; }
        public decimal MonthlySalary { get; set; }
        public DateTime JoiningDate { get; set; }
        public ICollection<Attendence>? Attendences { get; set; }
        public ICollection<Payment>? Payments { get; set; }
        
    }



    public class Attendence: BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public AttendenceStatus Status { get; set;  }
        public DateTime AttendenceUpdateTime { get; set; }

    }

    public class Payment: BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
