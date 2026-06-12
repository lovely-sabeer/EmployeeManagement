using EmployeeManagement.RequestModal;
using EmployeeManagement.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalaryController : ControllerBase
    {
        private readonly ISalaryService _salaryService;

        public SalaryController(ISalaryService salaryService)
        {
            _salaryService = salaryService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetList( string? search, string? department, int month, int year, int page = 1, int pageSize = 10)
        {
            var result = await _salaryService.GetSalaryListAsync(search,department,month,year,page,pageSize);
            return Ok(result);
        }

        [HttpPost("pay")]
        public async Task<IActionResult> Pay(SalaryReq request)
        {
            await _salaryService.CreatePaymentAsync(request);
            return Ok(new
            {
                Message = "Salary processed successfully."
            });
        }

        [HttpGet("export")]
        public async Task<IActionResult> Export(string? search,string? department,int month,int year)
        {
            var file = await _salaryService.ExportAsync(search,department,month,year);
            return File(file,"text/csv",$"SalaryReport_{year}_{month}.csv");
        }
    }
}
