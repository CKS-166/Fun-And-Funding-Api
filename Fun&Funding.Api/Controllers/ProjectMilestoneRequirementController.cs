using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel.ProjectMilestoneRequirementDTO;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/project-milestone-requirements")]
    [ApiController]
    public class ProjectMilestoneRequirementController : ControllerBase
    {
        private IProjectMilestoneRequirementService _projectMilestoneRequirementService;
        public ProjectMilestoneRequirementController(IProjectMilestoneRequirementService projectMilestoneRequirementService) 
        {
            _projectMilestoneRequirementService = projectMilestoneRequirementService;
        }

        [HttpPost]
        public IActionResult CreateMilestoneRequirements([FromForm] List<ProjectMilestoneRequirementRequest> request)
        {
            var result =_projectMilestoneRequirementService.CreateMilestoneRequirements(request);

            return Ok(result);
        }

        [HttpPut]
        public IActionResult UpdateMilestoneRequirements([FromForm] List<ProjectMilestoneRequirementUpdateRequest> request)
        {
            var result = _projectMilestoneRequirementService.UpdateMilestoneRequirements(request);
            return Ok(result);
        }
    }
}
