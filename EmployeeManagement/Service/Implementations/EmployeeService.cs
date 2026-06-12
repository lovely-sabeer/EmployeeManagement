using EmployeeManagement.ApplicationContext;
using EmployeeManagement.Model;
using EmployeeManagement.RequestModal;
using EmployeeManagement.ResponseModal;
using EmployeeManagement.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using RestAPI.Exceptions;
using RestAPI.Service;

namespace EmployeeManagement.Service.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAsync(EmployeeManagementRm req)
        {
            var duplicate = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Email == req.Email ||
                    x.PhoneNumber == req.Phone.ToString());

            if (duplicate != null)
            {
                if (duplicate.Email == req.Email)
                    throw new AppExceptions("Employee with this email already exists.", 409);

                if (duplicate.PhoneNumber == req.Phone.ToString())
                    throw new AppExceptions("Phone number already exists.", 409);
            }

            var employee = new Employee
            {
                FirstName = req.FirstName,
                LastName = req.LastName,
                EmployeeId = "EMP-001",
                Email = req.Email,
                DateofBirth = req.Dob,
                Department = req.Department,
                PhoneNumber = req.Phone.ToString(),
                Gender = req.Gender,
                MonthlySalary = req.Salary,
                JoiningDate = req.JoiningDate,
            };

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<EmployeeRes> GetByIdAsync(Guid id)
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            return employee == null
                ? throw new AppExceptions("Employee not found.", 404)
                : new EmployeeRes
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    EmployeeId = employee.EmployeeId,
                    Email = employee.Email,
                    Dob = employee.DateofBirth.ToString("yyyy-MM-dd"),
                    Department = employee.Department.ToString(),
                    Phone= employee.PhoneNumber,
                    Gender = employee.Gender,
                    Salary = employee.MonthlySalary,
                    JoiningDate = employee.JoiningDate.ToString("yyyy-MM-dd"),
                };
        }

        public async Task<bool> UpdateAsync(Guid id, EmployeeManagementRm req)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (employee == null)
                throw new AppExceptions("Employee not found.", 404);

            var duplicate = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id != id &&
                    (
                        x.Email == req.Email ||
                        x.PhoneNumber == req.Phone.ToString()
                    ));

            if (duplicate != null)
            {
                if (duplicate.Email == req.Email)
                    throw new AppExceptions("Employee with this email already exists.", 409);

                if (duplicate.PhoneNumber == req.Phone.ToString())
                    throw new AppExceptions("Phone number already exists.", 409);
            }

            employee.FirstName = req.FirstName;
            employee.LastName = req.LastName;
            employee.Email = req.Email;
            employee.DateofBirth = req.Dob;
            employee.Department = req.Department;
            employee.PhoneNumber = req.Phone.ToString();
            employee.Gender = req.Gender;
            employee.MonthlySalary = req.Salary;
            employee.JoiningDate = req.JoiningDate;
            employee.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteListAsync(List<Guid> ids)
        {
            if (ids == null || !ids.Any())
                throw new AppExceptions("Employee ids are required.", 400);

            var affectedRows = await _context.Employees
                .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.IsDeleted, true)
                    .SetProperty(x => x.DeletedAt, DateTime.UtcNow));

            if (affectedRows == 0)
                throw new AppExceptions("No employees found.", 404);

            return true;
        }

        public async Task<CommonPaginatedResponse<EmployeeDto>> GetListAsync( string? search, string? department, int page, int pageSize)
        {
            var query = _context.Employees
                .AsNoTracking()
                .Where(x => !x.IsDeleted);
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(x =>
                    x.FirstName.Contains(search) ||
                    x.LastName.Contains(search) ||
                    x.Email.Contains(search) ||
                    x.EmployeeId.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(department) && department != "all")
            {
                query = query.Where(x => x.Department.ToString() == department);
            }

            var totalCount = await query.CountAsync();

            var employees = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new EmployeeDto
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
                    Name = $"{x.FirstName} {x.LastName}",
                    Department = x.Department.ToString(),
                    Email = x.Email
                })
                .ToListAsync();
            return new CommonPaginatedResponse<EmployeeDto>(employees,totalCount,page,pageSize);
        }

        public async Task<byte[]> ExportAsync(string? department,string? search)
        {
            var result = await GetListAsync( search, department, 1, int.MaxValue);
            var exportData = result.Data.Select(x => new
            {
                x.EmployeeId,
                x.Name,
                x.Department,
                x.Email
            });
            return CsvExporter.ToCsv(exportData);
        }
    }
}
