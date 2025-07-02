using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace QuangThienDungRazorPages.Hubs
{
    [Authorize]
    public class NewsHub : Hub
    {
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendNewsUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveNewsUpdate", Context.User?.Identity?.Name, message);
        }

        public async Task NotifyNewsCreated(string newsId, string title)
        {
            await Clients.All.SendAsync("NewsCreated", newsId, title, Context.User?.Identity?.Name);
        }

        public async Task NotifyNewsUpdated(string newsId, string title)
        {
            await Clients.All.SendAsync("NewsUpdated", newsId, title, Context.User?.Identity?.Name);
        }

        public async Task NotifyNewsDeleted(string newsId, string title)
        {
            await Clients.All.SendAsync("NewsDeleted", newsId, title, Context.User?.Identity?.Name);
        }
    }
}
