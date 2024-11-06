using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel.FundingProjectDTO;
using Fun_Funding.Application.ViewModel.FundingFileDTO;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Domain.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Fun_Funding.Domain.Constrain;
using Fun_Funding.Application.Interfaces.IExternalServices;

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
        //[Authorize(Roles = Role.GameOwner)]
        public async Task<IActionResult> CreateProject([FromForm] FundingProjectAddRequest req)
        {
            var response = await _fundingProjectService.CreateFundingProject(req);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(Guid id)
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
        public async Task<IActionResult> GetFundingProjects([FromQuery] ListRequest request, string? categoryName, ProjectStatus? status, decimal? fromTarget, decimal? toTarget)
        {
            var response = await _fundingProjectService.GetFundingProjects(request, categoryName, status, fromTarget, toTarget);
            return Ok(response);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateProjectStatus([FromRoute] Guid id, [FromQuery] ProjectStatus status)
        {
            var response = await _fundingProjectService.UpdateFundingProjectStatus(id, status);           
            return Ok(response);
        }

        [HttpGet("project-owner")]
        [Authorize]
        public async Task<IActionResult> CheckProjectOwner([FromQuery] Guid projectId)
        {
            var response = await _fundingProjectService.CheckProjectOwner(projectId);
            return Ok(response);
        }

        [HttpGet("top3")]
        public async Task<IActionResult> GetTop3MostFundedOngoingProject()
        {
            var response = await _fundingProjectService.GetTop3MostFundedOngoingProject();
            return Ok(response);
        }
    }
}