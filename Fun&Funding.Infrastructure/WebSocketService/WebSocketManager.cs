using AutoMapper;
using Fun_Funding.Application;
using Fun_Funding.Application.IWebSocketService;
using Fun_Funding.Application.ViewModel.ChatDTO;
using Fun_Funding.Domain.Entity.NoSqlEntities;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Fun_Funding.Infrastructure.WebSocketService
{
    public class WebSocketManager : IWebSocketManager
    {
        private readonly ConcurrentDictionary<WebSocket, string> _connections = new();
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WebSocketManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task HandleConnectionAsync(WebSocket webSocket, string senderId)
        {
            _connections.TryAdd(webSocket, senderId);

            await BroadcastMessage($"{senderId} joined the room");
            await BroadcastMessage($"{_connections.Count} users connected");

            await ReceiveMessagesAsync(webSocket, async (result, messageJson) =>
            {
                var messageObject = JsonSerializer.Deserialize<ChatRequest>(messageJson);

                await SaveMessageToDatabase(messageObject);
                var responseJson = JsonSerializer.Serialize(messageObject);
                await BroadcastMessage(responseJson);
            });

            _connections.TryRemove(webSocket, out _);
            await BroadcastMessage($"{senderId} left the room");
            await BroadcastMessage($"{_connections.Count} users connected");
        }

        private async Task ReceiveMessagesAsync(WebSocket socket, Func<WebSocketReceiveResult, string, Task> handleMessage)
        {
            var buffer = new byte[4 * 1024];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var messageBytes = buffer.Take(result.Count).ToArray();
                    var messageJson = Encoding.UTF8.GetString(messageBytes);
                    await handleMessage(result, messageJson);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    break;
                }
            }
        }

        private async Task SaveMessageToDatabase(ChatRequest request)
        {
            try
            {
                var chat = _mapper.Map<Chat>(request);
                chat.SenderId = request.SenderId;
                chat.ReceiverId = request.ReceiverId;
                chat.Message = request.Message;
                chat.CreatedDate = DateTime.Now;

                await _unitOfWork.ChatRepository.CreateAsync(chat);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task BroadcastMessage(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            var tasks = _connections.Keys
                .Where(socket => socket.State == WebSocketState.Open)
                .Select(socket =>
                {
                    var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                    return socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                });

            await Task.WhenAll(tasks);  // Broadcast to all connected clients
        }
    }
}
