using Fun_Funding.Application.Interfaces.IEntityService;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.Services.EntityServices;
using Fun_Funding.Application.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/carts")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCartInfo()
        {
            var response = await _cartService.GetUserCartInfo();
            return Ok(response);
        }
        [HttpPatch("add/{marketplaceProjectId}")]
        public async Task<IActionResult> AddGameToUserCart([FromRoute]Guid marketplaceProjectId)
        {
            var response = await _cartService.AddGameToUserCart(marketplaceProjectId);
            return Ok(response);
        }
        [HttpPatch("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var response = await _cartService.ClearCart();
            return Ok(response);
        }
        [HttpPatch("delete/{marketplaceProjectId}")]
        public async Task<IActionResult> DeleteCartItem([FromRoute] Guid marketplaceProjectId)
        {
            var response = await _cartService.DeleteCartItem(marketplaceProjectId);
            return Ok(response);
        }
    }
}
