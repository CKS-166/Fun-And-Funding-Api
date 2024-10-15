using System.Net.WebSockets;

namespace Fun_Funding.Application.IWebSocketService
{
    public interface IWebSocketManager
    {
        Task HandleConnectionAsync(WebSocket webSocket, string senderId);
        Task BroadcastMessage(string message);
    }
}
