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
        // Thread-safe dictionary to store connected WebSocket clients with their sender IDs
        private readonly ConcurrentDictionary<WebSocket, string> _connections = new();
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WebSocketManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //handle new WebSocket connections
        public async Task HandleConnectionAsync(WebSocket webSocket, string senderId)
        {
            // Add the new WebSocket connection with the associated sender ID
            _connections.TryAdd(webSocket, senderId);

            //notify
            await BroadcastMessage($"{senderId} joined the room");
            await BroadcastMessage($"{_connections.Count} users connected");

            //receiving messages from the connected WebSocket
            await ReceiveMessagesAsync(webSocket, async (result, messageJson) =>
            {
                var messageObject = JsonSerializer.Deserialize<ChatRequest>(messageJson);

                await SaveMessageToDatabase(messageObject);
                var responseJson = JsonSerializer.Serialize(messageObject);

                await BroadcastMessage(responseJson);
            });

            // Once the WebSocket is closed, remove the connection from the dictionary
            _connections.TryRemove(webSocket, out _);

            await BroadcastMessage($"{senderId} left the room");
            await BroadcastMessage($"{_connections.Count} users connected");
        }

        //receive messages asynchronously from the WebSocket
        private async Task ReceiveMessagesAsync(WebSocket socket, Func<WebSocketReceiveResult, string, Task> handleMessage)
        {
            // Buffer to hold incoming message data
            var buffer = new byte[4 * 1024]; // 4 KB buffer

            // StringBuilder to handle fragmented messages (messages sent in parts)
            var messageBuilder = new StringBuilder();

            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    // Receive data from the WebSocket
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        // Convert the received byte data into a string
                        var messageBytes = buffer.Take(result.Count).ToArray();
                        var partialMessage = Encoding.UTF8.GetString(messageBytes);

                        // Append the partial message to the StringBuilder in case the message is fragmented
                        messageBuilder.Append(partialMessage);

                        // Check if message is fully received
                        if (result.EndOfMessage)
                        {
                            var fullMessageJson = messageBuilder.ToString();

                            // Pass the received message to the handler (deserialization and broadcasting)
                            await handleMessage(result, fullMessageJson);

                            messageBuilder.Clear(); // Reset for the next message
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

        //save message to database
        private async Task SaveMessageToDatabase(ChatRequest request)
        {
            try
            {
                var chat = new Chat();
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

        //Broadcast a message to all connected clients
        public async Task BroadcastMessage(string message)
        {
            // Convert the message into a byte array using UTF-8 encoding
            var bytes = Encoding.UTF8.GetBytes(message);

            // Prepare a list of tasks to send the message to each connected WebSocket
            var tasks = _connections.Keys
                .Where(socket => socket.State == WebSocketState.Open)
                .Select(socket =>
                {
                    // Create a byte array segment to send
                    var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);

                    // Send the message to the WebSocket as a text message
                    return socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                });

            await Task.WhenAll(tasks);  // Broadcast to all connected clients
        }
    }
}
