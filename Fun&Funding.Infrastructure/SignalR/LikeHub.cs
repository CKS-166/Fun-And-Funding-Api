// LikeHub.cs
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class LikeHub : Hub
{
    public async Task SendLikeNotification(string message)
    {
        await Clients.All.SendAsync("receivelikenotification", message);
    }
}
