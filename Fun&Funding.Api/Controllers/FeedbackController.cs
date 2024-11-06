using Fun_Funding.Application.Interfaces.IEntityService;
using Fun_Funding.Application.ViewModel.FeedbackDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/feedbacks")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }
        [HttpGet]
        public async Task<IActionResult> GetRandomFeedback()
        {
            var result = await _feedbackService.Get4RandomFeedback();
            if (!result._isSuccess)
            {
                NotFound();
            }
            return Ok(result);
        } 
        [HttpPost]
        public async Task<IActionResult> CreateFeedback(FeedbackRequest resquest)
        {
            var result = await _feedbackService.CreateFeedBack(resquest);
            if (!result._isSuccess)
            {
                NotFound();
            }
            return Ok(result);
        }
    }
}
