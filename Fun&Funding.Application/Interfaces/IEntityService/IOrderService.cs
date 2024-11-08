using Fun_Funding.Application.ViewModel.UserDTO;
using Fun_Funding.Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fun_Funding.Application.ViewModel.OrderDTO;

namespace Fun_Funding.Application.IService
{
    public interface IOrderService
    {
        Task<ResultDTO<PaginatedResponse<OrderInfoResponse>>> GetUserOrders(ListRequest request);
        Task<ResultDTO<Guid>> CreateOrder(CreateOrderRequest createOrderRequest);
        Task<ResultDTO<PaginatedResponse<OrderInfoResponse>>> GetAllOrders(ListRequest request);
        Task<ResultDTO<OrderInfoResponse>> GetOrderById(Guid orderId);
    }
}
