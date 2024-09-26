using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Fun_Funding.Application.IStorageService;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel.FundingProjectDTO;
using Fun_Funding.Application.ViewModel.FundingFileDTO;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Domain.Enum;
namespace Fun_Funding.Api.Controllers
{
    [Route("api/funding-projects")]
    [ApiController]
    public class FundingProjectController : ControllerBase
    {
        private IAzureService _storageService;
        private IFundingProjectService _fundingProjectService;
        public FundingProjectController(IAzureService storageService
            , IFundingProjectService fundingProjectService)
        {
            _storageService = storageService;
            _fundingProjectService = fundingProjectService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromForm] FundingProjectAddRequest req)
        {
            var response = await _fundingProjectService.CreateFundingProject(req);
            return Ok(response);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetProjectById([FromQuery] Guid id)
        {
            var response = await _fundingProjectService.GetProjectById(id);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProject([FromForm] FundingProjectUpdateRequest req)
        {
            var response = await _fundingProjectService.UpdateFundingProject(req);
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetFundingProjects([FromQuery] ListRequest request, string? categoryName, ProjectStatus status, decimal? fromTarget, decimal? toTarget)
        {
            var response = await _fundingProjectService.GetFundingProjects(request, categoryName, status, fromTarget, toTarget);
            return Ok(response);
        }
    }
}
