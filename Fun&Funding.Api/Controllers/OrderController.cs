using Fun_Funding.Application.IService;
using Fun_Funding.Application.Service;
using Fun_Funding.Application.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        public IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet("cart")]
        public async Task<IActionResult> GetUserCart()
        {
            var response = await _orderService.GetUserCart();
            return Ok(response);
        }
        [HttpGet("orders")]
        public async Task<IActionResult> GetUserOrders()
        {
            var response = await _orderService.GetUserOrders();
            return Ok(response);
        }
        [HttpPost("cart")]
        public async Task<IActionResult> AddProjectToCart()
        {
            var response = await _orderService.AddProjectToCart();
            return Ok(response);
        }
        [HttpDelete("cart")]
        public async Task<IActionResult> DeleteProjectFromCart()
        {
            var response = await _orderService.DeleteProjectFromCart();
            return Ok(response);
        }
        [HttpDelete("cart/all")]
        public async Task<IActionResult> ClearUserCart()
        {
            var response = await _orderService.ClearUserCart();
            return Ok(response);
        }
        [HttpPatch("cart")]
        public async Task<IActionResult> CheckoutCart()
        {
            var response = await _orderService.CheckoutCart();
            return Ok(response);
        }
        [HttpGet("all-orders")]
        public async Task<IActionResult> GetAllPurchasedOrders()
        {
            var response = await _orderService.GetAllPurchasedOrders();
            return Ok(response);
        }
    }
}
