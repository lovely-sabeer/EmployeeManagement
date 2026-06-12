using EmployeeManagement.ApplicationContext;
using EmployeeManagement.Model;
using EmployeeManagement.RequestModal;
using EmployeeManagement.ResponseModal;
using EmployeeManagement.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using RestAPI.Service;

namespace EmployeeManagement.Service.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        public readonly AppDbContext _context;
        public AttendanceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AttendanceStatCardRes> GetStatCardsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            var totalEmployees = await _context.Employees.Where(i => !i.IsDeleted).CountAsync();

            var presentToday = await _context.Attendences
                .CountAsync(a =>
                    a.AttendenceUpdateTime.Date == today &&
                    a.Status == AttendenceStatus.Present);

            var absentToday = await _context.Attendences
                .CountAsync(a =>
                    a.AttendenceUpdateTime.Date == today &&
                    a.Status == AttendenceStatus.Absent);

            var continuousLeaveCount = await _context.Employees
                .CountAsync(e =>
                    _context.Attendences.Any(a =>
                        a.EmployeeId == e.Id &&
                        a.Status == AttendenceStatus.Absent &&
                        a.AttendenceUpdateTime.Date == today)
                    &&
                    _context.Attendences.Any(a =>
                        a.EmployeeId == e.Id &&
                        a.Status == AttendenceStatus.Absent &&
                        a.AttendenceUpdateTime.Date == yesterday));

            return new AttendanceStatCardRes
            {
                TotalEmployees = totalEmployees,
                PresentCount = presentToday,
                AbsentCount = absentToday,
                ContinuousLeaveCount = continuousLeaveCount
            };
        }


        public async Task<CommonPaginatedResponse<AttendanceRes>> GetListAsync(AttendanceListReq req)
        {
            var today = DateTime.UtcNow.Date;
            var query = _context.Attendences
                .Include(x => x.Employee)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(req.Search))
                query = query.Where(x => x.Employee.FirstName.Contains(req.Search) || x.Employee.LastName.Contains(req.Search) || x.Employee.EmployeeId.Contains(req.Search));

            if (!string.IsNullOrWhiteSpace(req.Department) && req.Department != "all")
                query = query.Where(x => x.Employee.Department.ToString() == req.Department);

            if (!string.IsNullOrWhiteSpace(req.Status) && req.Status != "all")
                query = query.Where(x => x.Status.ToString() == req.Status);

            if (req.Date == AttendanceDateFilter.Today)
                query = query.Where(x => x.AttendenceUpdateTime.Date == today);

            if (req.Date == AttendanceDateFilter.ThisWeek)
                query = query.Where(x => x.AttendenceUpdateTime >= today.AddDays(-7));

            if (req.Date == AttendanceDateFilter.ThisMonth)
                query = query.Where(x => x.AttendenceUpdateTime.Month == today.Month && x.AttendenceUpdateTime.Year == today.Year);

            if (req.Date == AttendanceDateFilter.ThisYear)
                query = query.Where(x => x.AttendenceUpdateTime.Year == today.Year);

            var totalCount = await query.CountAsync();

            var rows = await query
                .OrderByDescending(x => x.AttendenceUpdateTime)
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(x => new AttendanceRes
                {
                    Id = x.Id,
                    EmployeeId = x.Employee.EmployeeId,
                    Name = $"{x.Employee.FirstName} {x.Employee.LastName}",
                    Department = x.Employee.Department.ToString(),
                    Date = x.AttendenceUpdateTime.Date,
                    Status = x.Status.ToString()
                })
                .ToListAsync();
            return new CommonPaginatedResponse<AttendanceRes>(rows, totalCount, req.Page, req.PageSize);
        }

        public async Task<bool> BulkAttendanceAsync(BulkAttendanceReq req)
        {
            var employees = await _context.Employees
                .Where(x => req.EmployeeIds.Contains(x.Id))
                .ToListAsync();

            var employeeIds = employees.Select(x => x.Id).ToList();

            var attendances = await _context.Attendences
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .ToListAsync();

            foreach (var employee in employees)
            {
                var attendance = attendances
                    .FirstOrDefault(x => x.EmployeeId == employee.Id);

                if (attendance == null)
                {
                    await _context.Attendences.AddAsync(new Attendence
                    {
                        EmployeeId = employee.Id,
                        Status = req.Status,
                        AttendenceUpdateTime = DateTime.Now
                    });
                }
                else
                {
                    attendance.Status = req.Status;
                    attendance.AttendenceUpdateTime = DateTime.Now;
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<byte[]> ExportAsync(AttendanceListReq req)
        {
            req.Page = 1;
            req.PageSize = int.MaxValue;
            var result = await GetListAsync(req);
            var exportData = result.Data.Select(x => new
            {
                x.EmployeeId,
                x.Name,
                x.Department,
                Date = x.Date.ToString("dd-MM-yyyy"),
                x.Status
            }).ToList();
            var csvBytes = CsvExporter.ToCsv(result.Data);
            return csvBytes;
        }
    }
}
