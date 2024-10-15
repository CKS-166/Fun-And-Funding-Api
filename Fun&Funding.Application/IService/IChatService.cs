using System.Net.WebSockets;

namespace Fun_Funding.Application.IService
{
    public interface IChatService
    {
        Task HandleWebSocketConnectionAsync(WebSocket webSocket, string senderId);
    }
}
