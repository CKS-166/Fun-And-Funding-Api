using System.Net.WebSockets;

namespace Fun_Funding.Application.IWebSocketService
{
    public interface IWebSocketManager
    {
        Task HandleConnectionAsync(WebSocket webSocket, string senderId, string receiverId);
        Task BroadcastMessage(string message, string senderId, string receiverId);
    }
}
