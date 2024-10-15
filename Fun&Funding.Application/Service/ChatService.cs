using Fun_Funding.Application.ExceptionHandler;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.IWebSocketService;
using System.Net;
using System.Net.WebSockets;

namespace Fun_Funding.Application.Service
{
    public class ChatService : IChatService
    {
        private readonly IWebSocketManager _webSocketManager;

        public ChatService(IWebSocketManager webSocketManager)
        {
            _webSocketManager = webSocketManager;
        }
        public async Task HandleWebSocketConnectionAsync(WebSocket webSocket, string senderId)
        {
            try
            {
                await _webSocketManager.HandleConnectionAsync(webSocket, senderId);
            }
            catch (Exception ex)
            {
                throw new ExceptionError((int)HttpStatusCode.InternalServerError, ex.Message);
            }

        }
    }
}
