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
        Task<ResultDTO<List<OrderInfoResponse>>> GetUserCart();
        Task<ResultDTO<List<OrderInfoResponse>>> GetUserOrders();
        Task<ResultDTO<string>> AddProjectToCart();
        Task<ResultDTO<string>> DeleteProjectFromCart();
        Task<ResultDTO<string>> ClearUserCart();
        Task<ResultDTO<string>> CheckoutCart();
        Task<ResultDTO<string>> GetAllPurchasedOrders();
    }
}
