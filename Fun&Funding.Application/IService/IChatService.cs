using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.ChatDTO;
using System.Net.WebSockets;

namespace Fun_Funding.Application.IService
{
    public interface IChatService
    {
        Task HandleWebSocketConnectionAsync(WebSocket webSocket, string senderId);
        Task<ResultDTO<IEnumerable<ChatResponse>>> GetChatConversation(Guid senderId, Guid receiverId);
    }
}
