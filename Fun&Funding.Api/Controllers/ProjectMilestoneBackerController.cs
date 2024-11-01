using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel.ProjectMilestoneBackerDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/project-milestone-backer")]
    [ApiController]
    public class ProjectMilestoneBackerController : ControllerBase
    {
        private readonly IProjectMilestoneBackerService _projectMilestoneBackerService;

        public ProjectMilestoneBackerController(IProjectMilestoneBackerService projectMilestoneBackerService)
        {
            _projectMilestoneBackerService = projectMilestoneBackerService;
        }

        [HttpPost]
        public async Task<IActionResult> PostMilestoneReview(ProjectMilestoneBackerRequest request)
        {
            var result = await _projectMilestoneBackerService.CreateNewProjectMilestoneBackerReview(request);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjectMilestoneReview(Guid projectMilestoneId)
        {
            var result = await _projectMilestoneBackerService.GetAllMilestoneReview(projectMilestoneId);
            return Ok(result);
        }
    }
}
