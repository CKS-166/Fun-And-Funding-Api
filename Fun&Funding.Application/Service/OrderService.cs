using AutoMapper;
using Azure.Core;
using Fun_Funding.Application.ExceptionHandler;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.FundingProjectDTO;
using Fun_Funding.Application.ViewModel.OrderDTO;
using Fun_Funding.Application.ViewModel.PackageBackerDTO;
using Fun_Funding.Application.ViewModel.UserDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly ClaimsPrincipal _claimsPrincipal;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IMapper _mapper;
        private readonly IDigitalKeyService _digitalKeyService;
        private readonly ICommissionFeeService _commissionFeeService;

        public OrderService(IUnitOfWork unitOfWork, UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper, IDigitalKeyService digitalKeyService, ICommissionFeeService commissionFeeService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _claimsPrincipal = httpContextAccessor.HttpContext.User;
            _roleManager = roleManager;
            _mapper = mapper;
            _digitalKeyService = digitalKeyService;
            _commissionFeeService = commissionFeeService;
        }

        public async Task<ResultDTO<string>> CreateOrder(CreateOrderRequest createOrderRequest)
        {
            try
            {
                if (_claimsPrincipal == null || !_claimsPrincipal.Identity.IsAuthenticated)
                {
                    throw new ExceptionError((int)HttpStatusCode.Unauthorized, "User not authenticated.");
                }

                var userEmailClaims = _claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (userEmailClaims == null)
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "User not found.");
                }
                var userEmail = userEmailClaims.Value;

                var user = await _unitOfWork.UserRepository.GetQueryable()
                                .AsNoTracking()
                                .Include(u => u.File)
                                .Include(u => u.Wallet)
                                .FirstOrDefaultAsync(u => u.Email == userEmail);

                if (user == null)
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "User not found.");
                }

                var wallet = await _unitOfWork.WalletRepository.GetAsync(w => w.Backer.Id == user.Id);
                if (wallet == null)
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "Wallet Not Found.");

                decimal totalCost = 0;
                foreach (var cartItem in createOrderRequest.CartItems)
                {
                    var gamePrice = cartItem.Price;
                    if (cartItem.AppliedCoupon != null)
                    {
                        ProjectCoupon projectCoupon = _unitOfWork.ProjectCouponRepository.GetById(cartItem.AppliedCoupon.Id);
                        if (projectCoupon == null)
                        {
                            throw new ExceptionError((int)HttpStatusCode.NotFound, "Project Coupon Not Found.");
                        }
                        else if (projectCoupon.Status == ProjectCouponStatus.Disable)
                        {
                            throw new ExceptionError((int)HttpStatusCode.BadRequest, "Coupon Already Used.");
                        }
                        gamePrice = gamePrice * (1 - cartItem.AppliedCoupon.DiscountRate);
                    }
                    totalCost += gamePrice;
                }

                if (wallet.Balance < totalCost)
                {
                    throw new ExceptionError((int)HttpStatusCode.BadRequest, "Insufficient Wallet Balance.");
                }

                wallet.Balance -= totalCost;

                Order order = new Order 
                {
                    Id = Guid.NewGuid(),
                    User = user,
                    TotalAmount = totalCost,
                    CreatedDate = DateTime.Now,
                    OrderDetails = new List<OrderDetail>()
                };

                Transaction purchaseTransaction = new Transaction
                {
                    TotalAmount = -totalCost,
                    TransactionType = TransactionTypes.OrderPurchase,
                    Wallet = wallet,
                    OrderId = order.Id,
                    CreatedDate = DateTime.Now,
                };

                foreach (var cartItem in createOrderRequest.CartItems)
                {
                    DigitalKey digitalKey = new DigitalKey
                    {
                        Id = Guid.NewGuid(),
                        KeyString = _digitalKeyService.GenerateGameKey(),
                        Status = KeyStatus.ACTIVE,
                        CreatedDate = DateTime.Now,
                        MarketplaceProject = _unitOfWork.MarketplaceRepository.GetById(cartItem.MarketplaceProjectId)
                    };

                    OrderDetail orderDetail = new OrderDetail
                    {
                        DigitalKey = digitalKey,
                        Order = order,
                        CreatedDate = DateTime.Now,
                    };

                    order.OrderDetails.Add(orderDetail);

                    var commissionFee = _commissionFeeService.GetAppliedCommissionFee(CommissionType.MarketingCommission)._data;
                    var recieverWallet = await _unitOfWork.WalletRepository.GetAsync(w => w.Backer.Id == user.Id);
                    if (recieverWallet == null)
                    {
                        throw new ExceptionError((int)HttpStatusCode.NotFound, "Game Owner Wallet Not Found.");
                    }
                    else
                    {
                        var recievedMoney = totalCost;
                        if (cartItem.AppliedCoupon != null)
                        {
                            ProjectCoupon projectCoupon = _unitOfWork.ProjectCouponRepository.GetById(cartItem.AppliedCoupon.Id);
                            if (projectCoupon == null)
                            {
                                throw new ExceptionError((int)HttpStatusCode.NotFound, "Project Coupon Not Found.");
                            }
                            else if (projectCoupon.Status == ProjectCouponStatus.Disable) 
                            {
                                throw new ExceptionError((int)HttpStatusCode.BadRequest, "Coupon Already Used.");
                            }
                            recievedMoney = recievedMoney * (1 - cartItem.AppliedCoupon.DiscountRate);
                            projectCoupon.Status = ProjectCouponStatus.Disable;
                            _unitOfWork.ProjectCouponRepository.Update(projectCoupon);
                        }
                        decimal marketplaceCommissionRate = commissionFee.Rate;
                        recieverWallet.Balance += (recievedMoney * (1 - marketplaceCommissionRate));
                    }

                    Transaction recieveTransaction = new Transaction
                    {
                        TotalAmount = cartItem.Price,
                        TransactionType = TransactionTypes.OrderPurchase,
                        Wallet = recieverWallet,
                        OrderDetailId = orderDetail.Id,
                        CommissionFee = await _unitOfWork.CommissionFeeRepository.GetByIdAsync(commissionFee.Id),
                        CreatedDate = DateTime.Now,
                    };

                    _unitOfWork.WalletRepository.Update(recieverWallet);
                    _unitOfWork.DigitalKeyRepository.Add(digitalKey);
                    _unitOfWork.OrderDetailRepository.Add(orderDetail);
                    _unitOfWork.TransactionRepository.Add(recieveTransaction);
                }

                _unitOfWork.WalletRepository.Update(wallet);
                _unitOfWork.OrderRepository.Add(order);
                _unitOfWork.TransactionRepository.Add(purchaseTransaction);
                await _unitOfWork.CommitAsync();

                return ResultDTO<string>.Success("", "Order added successfully!");
            }
            catch (Exception ex)
            {
                if (ex is ExceptionError exceptionError)
                {
                    throw exceptionError;
                }
                throw new ExceptionError((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<ResultDTO<PaginatedResponse<OrderInfoResponse>>> GetAllOrders(ListRequest request)
        {
            try
            {
                Expression<Func<Order, bool>> filter = null;
                Expression<Func<Order, object>> orderBy = u => u.CreatedDate;

                if (!string.IsNullOrEmpty(request.OrderBy))
                {
                    switch (request.OrderBy.ToLower())
                    {
                        case "amount":
                            orderBy = u => u.TotalAmount;
                            break;
                        default:
                            orderBy = u => u.CreatedDate;
                            break;
                    }
                }

                if (request.From != null && request.To != null)
                {
                    DateTime fromDate = (DateTime)request.From;
                    DateTime toDate = (DateTime)request.To;
                    filter = c => c.CreatedDate >= fromDate && c.CreatedDate <= toDate;
                }
                else if (request.From != null)
                {
                    DateTime fromDate = (DateTime)request.From;
                    filter = c => c.CreatedDate >= fromDate;
                }
                else if (request.To != null)
                {
                    DateTime toDate = (DateTime)request.To;
                    filter = c => c.CreatedDate <= toDate;
                }

                var list = await _unitOfWork.OrderRepository.GetAllAsync(
                       filter: filter,
                       orderBy: orderBy,
                       isAscending: request.IsAscending.Value,
                       pageIndex: request.PageIndex,
                       pageSize: request.PageSize,
                       includeProperties: "OrderDetails,OrderDetails.DigitalKey,OrderDetails.DigitalKey.MarketplaceProject");

                if (list != null && list.Count() > 0)
                {
                    var totalItems = _unitOfWork.OrderRepository.GetAll(filter).Count();
                    var totalPages = (int)Math.Ceiling((double)totalItems / (int)request.PageSize);
                    IEnumerable<OrderInfoResponse> orders = _mapper.Map<IEnumerable<OrderInfoResponse>>(list);

                    PaginatedResponse<OrderInfoResponse> response = new PaginatedResponse<OrderInfoResponse>
                    {
                        PageSize = request.PageSize.Value,
                        PageIndex = request.PageIndex.Value,
                        TotalItems = totalItems,
                        TotalPages = totalPages,
                        Items = orders
                    };

                    return ResultDTO<PaginatedResponse<OrderInfoResponse>>.Success(response, "Orders found!");
                }
                else
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "Order Not Found.");
                }
            }
            catch (Exception ex)
            {
                if (ex is ExceptionError exceptionError)
                {
                    throw exceptionError;
                }
                throw new ExceptionError((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<ResultDTO<OrderInfoResponse>> GetOrderById(Guid orderId)
        {
            try
            {
                Order? order = await _unitOfWork.OrderRepository.GetQueryable().AsNoTracking()
                    .Where(o => o.Id == orderId)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.DigitalKey)
                            .ThenInclude(m => m.MarketplaceProject)
                    .FirstOrDefaultAsync();
                if (order == null)
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "Order Not Found.");
                }
                else
                {
                    var orderResponse = _mapper.Map<OrderInfoResponse>(order);
                    return ResultDTO<OrderInfoResponse>.Success(orderResponse, "Order found!");
                }
            }
            catch (Exception ex)
            {
                if (ex is ExceptionError exceptionError)
                {
                    throw exceptionError;
                }
                throw new ExceptionError((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<ResultDTO<PaginatedResponse<OrderInfoResponse>>> GetUserOrders(ListRequest request)
        {
            try
            {
                if (_claimsPrincipal == null || !_claimsPrincipal.Identity.IsAuthenticated)
                {
                    throw new ExceptionError((int)HttpStatusCode.Unauthorized, "User not authenticated.");
                }

                var userEmailClaims = _claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (userEmailClaims == null)
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "User not found.");
                }
                var userEmail = userEmailClaims.Value;

                var user = await _unitOfWork.UserRepository.GetQueryable()
                                .AsNoTracking()
                                .Include(u => u.File)
                                .Include(u => u.Wallet)
                                .FirstOrDefaultAsync(u => u.Email == userEmail);

                if (user == null)
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "User not found.");
                }

                Expression<Func<Order, bool>> filter = o => o.UserId == user.Id;
                Expression<Func<Order, object>> orderBy = u => u.CreatedDate;

                if (!string.IsNullOrEmpty(request.OrderBy))
                {
                    switch (request.OrderBy.ToLower())
                    {
                        case "amount":
                            orderBy = u => u.TotalAmount;
                            break;
                        default:
                            orderBy = u => u.CreatedDate;
                            break;
                    }
                }

                if (request.From != null && request.To != null)
                {
                    DateTime fromDate = (DateTime)request.From;
                    DateTime toDate = (DateTime)request.To;
                    filter = c => c.CreatedDate >= fromDate && c.CreatedDate <= toDate && c.UserId == user.Id;
                }
                else if (request.From != null)
                {
                    DateTime fromDate = (DateTime)request.From;
                    filter = c => c.CreatedDate >= fromDate && c.UserId == user.Id;
                }
                else if (request.To != null)
                {
                    DateTime toDate = (DateTime)request.To;
                    filter = c => c.CreatedDate <= toDate && c.UserId == user.Id;
                }

                var list = await _unitOfWork.OrderRepository.GetAllAsync(
                       filter: filter,
                       orderBy: orderBy,
                       isAscending: request.IsAscending.Value,
                       pageIndex: request.PageIndex,
                       pageSize: request.PageSize,
                       includeProperties: "OrderDetails,OrderDetails.DigitalKey,OrderDetails.DigitalKey.MarketplaceProject");

                if (list != null && list.Count() > 0)
                {
                    var totalItems = _unitOfWork.OrderRepository.GetAll(filter).Count();
                    var totalPages = (int)Math.Ceiling((double)totalItems / (int)request.PageSize);
                    IEnumerable<OrderInfoResponse> orders = _mapper.Map<IEnumerable<OrderInfoResponse>>(list);

                    PaginatedResponse<OrderInfoResponse> response = new PaginatedResponse<OrderInfoResponse>
                    {
                        PageSize = request.PageSize.Value,
                        PageIndex = request.PageIndex.Value,
                        TotalItems = totalItems,
                        TotalPages = totalPages,
                        Items = orders
                    };

                    return ResultDTO<PaginatedResponse<OrderInfoResponse>>.Success(response, "Orders found!");
                }
                else
                {
                    throw new ExceptionError((int)HttpStatusCode.NotFound, "Order Not Found.");
                }
            }
            catch (Exception ex)
            {
                if (ex is ExceptionError exceptionError)
                {
                    throw exceptionError;
                }
                throw new ExceptionError((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
