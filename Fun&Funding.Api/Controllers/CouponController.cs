using Fun_Funding.Application.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly IProjectCouponService _couponService;

        public CouponController(IProjectCouponService couponService)
        {
            _couponService = couponService;
        }
        [HttpPost]
        public async Task<IActionResult> ImportFile(IFormFile formFile, Guid projectId)
        {
            var result = await _couponService.ImportFile(formFile, projectId);
            if(!result._isSuccess) return BadRequest(result);
            return Ok(result);
        }
    }
}
