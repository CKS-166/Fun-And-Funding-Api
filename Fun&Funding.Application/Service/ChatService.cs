using AutoMapper;
using Fun_Funding.Application.ExceptionHandler;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.IWebSocketService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.ChatDTO;
using Fun_Funding.Domain.Entity.NoSqlEntities;
using MongoDB.Driver;
using System.Net;
using System.Net.WebSockets;

namespace Fun_Funding.Application.Service
{
    public class ChatService : IChatService
    {
        private readonly IWebSocketManager _webSocketManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChatService(IWebSocketManager webSocketManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _webSocketManager = webSocketManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultDTO<IEnumerable<ChatResponse>>> GetChatConversation(Guid senderId, Guid receiverId)
        {
            try
            {
                var filter = Builders<Chat>.Filter.Or(
                    Builders<Chat>.Filter.And(
                    Builders<Chat>.Filter.Eq(m => m.SenderId, senderId),
                    Builders<Chat>.Filter.Eq(m => m.ReceiverId, receiverId)),
                    Builders<Chat>.Filter.And(
                    Builders<Chat>.Filter.Eq(m => m.SenderId, receiverId),
                    Builders<Chat>.Filter.Eq(m => m.ReceiverId, senderId)));

                var chatMessages = await _unitOfWork.ChatRepository.GetAllAsync(filter);

                var response = _mapper.Map<IEnumerable<ChatResponse>>(chatMessages);

                return ResultDTO<IEnumerable<ChatResponse>>.Success(response);
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
