using Fun_Funding.Api.Exception;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel.CommentDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }
        [HttpGet("get-all-comment-projects")]
        public async Task<IActionResult> AllCommentProjects()
        {
            var result = await _commentService.GetAllComment();
            return Ok(result);
        }
        [HttpPost("comment-project")]
        public async Task<IActionResult> commentProject([FromBody] CommentRequest request)
        {
            var result = await _commentService.CommentProject(request);
            if (!result._isSuccess)
            {
                return BadRequest(result._message);
            }
            return Ok(result);
        }
        [HttpGet("get-comments/{id}")]
        public async Task<IActionResult> GetProjecComment(Guid id)
        {
            try
            {
                var result = _commentService.GetCommentsByProject(id);
                return Ok(result);
            }
            catch (ExceptionError ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("delete-comments/{id}")]
        public async Task<IActionResult> DeleteProjecComment(Guid id)
        {
            try
            {
                var result = await _commentService.DeleteComment(id);
                return Ok(result);
            }
            catch (ExceptionError ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
