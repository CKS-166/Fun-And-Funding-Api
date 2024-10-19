using AutoMapper;
using Fun_Funding.Application.ExceptionHandler;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.IWebSocketService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Domain.Entity.NoSqlEntities;
using Microsoft.Extensions.DependencyInjection;  // Import for IServiceScopeFactory
using MongoDB.Driver;
using System.Net;
using System.Net.WebSockets;

namespace Fun_Funding.Application.Service
{
    public class ChatService : IChatService
    {
        private readonly IWebSocketManager _webSocketManager;
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory;  // Inject IServiceScopeFactory

        public ChatService(IWebSocketManager webSocketManager, IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _webSocketManager = webSocketManager;
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        public async Task<ResultDTO<IEnumerable<Chat>>> GetChatConversation(Guid senderId, Guid receiverId)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var filter = Builders<Chat>.Filter.Or(
                        Builders<Chat>.Filter.And(
                            Builders<Chat>.Filter.Eq(m => m.SenderId, senderId),
                            Builders<Chat>.Filter.Eq(m => m.ReceiverId, receiverId)),
                        Builders<Chat>.Filter.And(
                            Builders<Chat>.Filter.Eq(m => m.SenderId, receiverId),
                            Builders<Chat>.Filter.Eq(m => m.ReceiverId, senderId)));

                    var chatMessages = await unitOfWork.ChatRepository.GetAllAsync(filter);

                    return ResultDTO<IEnumerable<Chat>>.Success(chatMessages);
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

        public async Task HandleWebSocketConnectionAsync(WebSocket webSocket, string senderId)
        {
            try
            {
                await _webSocketManager.HandleConnectionAsync(webSocket, senderId);
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
