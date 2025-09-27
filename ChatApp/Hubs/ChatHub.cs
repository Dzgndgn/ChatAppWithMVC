using ChatApp.Context;
using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Hubs
{
    public  class ChatHub(AppDbContext _context) : Hub
    {
        public static Dictionary<string, Guid> Users = new();

        public async Task Connect(Guid userId)
        {
            Users.Add(Context.ConnectionId, userId);
            User user = await _context.users.FindAsync(userId);
            if (user is not null)
            {
                user.Status = "online";
                await _context.SaveChangesAsync();
                Clients.All.SendAsync("notifyAll", user);
            }
        }

        //public async override Task OnConnectedAsync(Guid userId)
        //{

        //    Users.Add(Context.ConnectionId, userId);
        //    User user = await _context.users.FindAsync(userId);
        //    if (user is not null)
        //    {
        //        user.Status = "online";
        //        await _context.SaveChangesAsync();
        //       await Clients.All.SendAsync("ReceiveMessage", user);
        //    }

        //}
        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            Users.TryGetValue(Context.ConnectionId, out Guid userID);
            User user = await _context.users.FindAsync(userID);
            if(user is not null)
            {
                user.Status = "offline";
                await _context.SaveChangesAsync();
                Clients.All.SendAsync("ReceiveMessage", user);
            }
        
        }
    }
}
