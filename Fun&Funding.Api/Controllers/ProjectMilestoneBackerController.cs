using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectMilestoneBackerController : ControllerBase
    {
        // api post projectmilestonebacker
        [HttpPost]
        public async Task<IActionResult> PostMilestoneReview()
        {
            throw new NotImplementedException();
        }
    }
}
