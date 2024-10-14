using Fun_Funding.Api.Exception;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel.ProjectMilestoneDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/project-milestones")]
    [ApiController]
    public class ProjectMilestoneController : ControllerBase
    {
        private IProjectMilestoneService _projectMilestoneService;
        public ProjectMilestoneController(IProjectMilestoneService projectMilestoneService) {
            _projectMilestoneService = projectMilestoneService;
        }
        //api create milestone request
        [HttpPost]
        public IActionResult CreateMilestoneRequest([FromBody]ProjectMilestoneRequest request)
        {
            try
            {
                var res = _projectMilestoneService.CreateProjectMilestoneRequest(request);
                return Ok(res);
            }
            catch(ExceptionError ex)
            {
                return Ok(ex.InnerException);
            }
           
        }

        // api get list projectmilestone


        // api update status projectmilestone


        // api get detail projectmilestone
        [HttpGet("{id}")]
        public IActionResult GetProjectMilestoneById(Guid id)
        {
            var res = _projectMilestoneService.GetProjectMilestoneRequest(id);
            return Ok(res);
        }
        
    }
}
