using Fun_Funding.Api.Exception;
using Fun_Funding.Application;
using Fun_Funding.Application.IRepository;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel.LikeDTO;
using Fun_Funding.Domain.Entity.NoSqlEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Fun_Funding.Api.Controllers
{
    [Route("api/like")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly ILikeService _likeService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<LikeHub> _hubContext;

        public LikeController(ILikeService likeService,IUnitOfWork unitOfWork, IHubContext<LikeHub> hubContext)
        {
            _likeService = likeService;
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _likeService.GetAll();
            return Ok(result);
        }
        [HttpPost("like")]
        public async Task<IActionResult> likeProject([FromBody] LikeRequest likeRequest)
        {
            var result = await _likeService.LikeProject(likeRequest);
            if (!result._isSuccess)
            {
                return BadRequest(result._message);
            }
            // Send a real-time notification through SignalR
            await _hubContext.Clients.All.SendAsync("receivelikenotification", $"Project {likeRequest.ProjectId} received a like!");
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectLike(Guid id)
        {
            try
            {
                var result = _likeService.GetLikesByProject(id);
                return Ok(result);
            }
            catch (ExceptionError ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("user-like/{id}")]
        [Authorize]
        public async Task<IActionResult> CheckUserLike(Guid id)
        {
            try
            {
                var result = _likeService.CheckUserLike(id);
                return Ok(result);
            }
            catch (ExceptionError ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
