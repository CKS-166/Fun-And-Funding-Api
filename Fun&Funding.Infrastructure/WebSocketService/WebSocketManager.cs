using AutoMapper;
using Fun_Funding.Application;
using Fun_Funding.Application.IWebSocketService;
using Fun_Funding.Application.ViewModel.ChatDTO;
using Fun_Funding.Domain.Entity.NoSqlEntities;
using Microsoft.Extensions.DependencyInjection;  // Import this for IServiceScopeFactory
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Fun_Funding.Infrastructure.WebSocketService
{
    public class WebSocketManager : IWebSocketManager
    {
        private readonly ConcurrentDictionary<WebSocket, string> _connections = new();
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory;  // Use IServiceScopeFactory for scoped DbContext

        public WebSocketManager(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
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
                await SaveMessageToDatabase(messageObject);  // Save message
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
            var messageBuilder = new StringBuilder();

            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var messageBytes = buffer.Take(result.Count).ToArray();
                        var partialMessage = Encoding.UTF8.GetString(messageBytes);
                        messageBuilder.Append(partialMessage);

                        if (result.EndOfMessage)
                        {
                            var fullMessageJson = messageBuilder.ToString();
                            await handleMessage(result, fullMessageJson);
                            messageBuilder.Clear();
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in WebSocket connection: {ex.Message}");
                await socket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Error occurred", CancellationToken.None);
            }
        }

        // Modify SaveMessageToDatabase to create a scoped instance of IUnitOfWork
        private async Task SaveMessageToDatabase(ChatRequest request)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var chat = new Chat
                {
                    SenderId = request.SenderId,
                    ReceiverId = request.ReceiverId,
                    Message = request.Message,
                    CreatedDate = DateTime.Now
                };

                try
                {
                    await unitOfWork.ChatRepository.CreateAsync(chat);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
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

            await Task.WhenAll(tasks);
        }
    }
}
