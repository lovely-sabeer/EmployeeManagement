using EmployeeManagement.Model;
using EmployeeManagement.RequestModal;
using EmployeeManagement.ResponseModal;

namespace EmployeeManagement.Service.Interfaces
{
    public interface IEmployeeService
    {
        Task<bool> CreateAsync(EmployeeManagementRm req);
        Task<CommonPaginatedResponse<EmployeeDto>> GetListAsync(string? search,string? department,int page,int pageSize);
        Task<bool> DeleteListAsync(List<Guid> ids);
        Task<byte[]> ExportAsync(string? department,string? search);
        Task<EmployeeRes> GetByIdAsync(Guid id);
        Task<bool> UpdateAsync(Guid id, EmployeeManagementRm req);
    }
}
