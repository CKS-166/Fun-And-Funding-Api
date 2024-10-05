using Fun_Funding.Api.Exception;
using Fun_Funding.Application;
using Fun_Funding.Application.IRepository;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel.LikeDTO;
using Fun_Funding.Domain.Entity.NoSqlEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Fun_Funding.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly ILikeService _likeService;
        private readonly IUnitOfWork _unitOfWork;

        public LikeController( ILikeService likeService,IUnitOfWork unitOfWork)
        {
            _likeService = likeService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("get-all-liked-projects")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _likeService.GetAll();
            return Ok(result);
        }
        [HttpPost("like-project")]
        public async Task<IActionResult> likeProject([FromBody] LikeRequest likeRequest)
        {
            var result = await _likeService.LikeProject(likeRequest);
            if (!result._isSuccess)
            {
                return BadRequest(result._message);
            }
            return Ok(result);
        }
        [HttpGet("get-like/{id}")]
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
        [HttpGet("check-user-like/{id}")]
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
