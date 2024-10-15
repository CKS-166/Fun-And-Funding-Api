using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.OrderDTO;
using Fun_Funding.Application.ViewModel.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Service
{
    public class OrderService : IOrderService
    {
        public Task<ResultDTO<string>> AddProjectToCart()
        {
            throw new NotImplementedException();
        }

        public Task<ResultDTO<string>> CheckoutCart()
        {
            throw new NotImplementedException();
        }

        public Task<ResultDTO<string>> ClearUserCart()
        {
            throw new NotImplementedException();
        }

        public Task<ResultDTO<string>> DeleteProjectFromCart()
        {
            throw new NotImplementedException();
        }

        public Task<ResultDTO<string>> GetAllPurchasedOrders()
        {
            throw new NotImplementedException();
        }

        public Task<ResultDTO<List<OrderInfoResponse>>> GetUserCart()
        {
            throw new NotImplementedException();
        }

        public Task<ResultDTO<List<OrderInfoResponse>>> GetUserOrders()
        {
            throw new NotImplementedException();
        }
    }
}
