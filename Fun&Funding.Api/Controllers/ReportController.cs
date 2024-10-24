using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.ReportDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ListRequest request)
        {
            var result = await _reportService.GetAllReport(request);
            if (!result._isSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateReport(ReportRequest request)
        {
            var result = await _reportService.CreateReportRequest(request);
            if (!result._isSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpPatch]
        public async Task<IActionResult> UpdateReport(HandleRequest request)
        {
            var result = await _reportService.UpdateHandleReport(request);
            if (!result._isSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
