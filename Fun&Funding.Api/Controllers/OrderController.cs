using Fun_Funding.Application.IService;
using Fun_Funding.Application.Service;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.OrderDTO;
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
        [HttpGet]
        public async Task<IActionResult> GetUserOrders([FromQuery] ListRequest request)
        {
            var response = await _orderService.GetUserOrders(request);
            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById([FromQuery] Guid orderId)
        {
            var response = await _orderService.GetOrderById(orderId);
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromForm] CreateOrderRequest createOrderRequest)
        {
            var response = await _orderService.CreateOrder(createOrderRequest);
            return Ok(response);
        }
        [HttpGet("all-orders")]
        public async Task<IActionResult> GetAllOrders([FromQuery] ListRequest request)
        {
            var response = await _orderService.GetAllOrders(request);
            return Ok(response);
        }
    }
}
