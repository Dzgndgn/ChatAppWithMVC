using ChatApp.Context;
using ChatApp.Data;
using ChatApp.Hubs;
using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers
{
    public class ChatsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatsController(AppDbContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        [HttpGet]
        public async Task<IActionResult> getChat(Guid Id,Guid receivedUserId, CancellationToken cancellationToken)
        {

            var chats = await _context.chats
                .Where(x =>
                       (x.UserId == Id && x.ReceiverUserId == receivedUserId) ||
                       (x.UserId == receivedUserId && x.ReceiverUserId == Id))
                .OrderBy(x => x.Date)
                .ToListAsync(cancellationToken);

            ViewBag.UserList = await _context.users.Where(x => x.Id != Id).ToListAsync();
            ViewBag.UserId = Id;
            ViewBag.ReceivedUserId = receivedUserId;

            return View("Index",chats);
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid Id, Guid receivedUserId, CancellationToken cancellationToken)
        {
        //    var chats = await _context.chats
        //        .Where(x =>
        //               (x.UserId == Id && x.ReceiverUserId == receivedUserId) ||
        //               (x.UserId == receivedUserId && x.ReceiverUserId == Id))
        //        .OrderBy(x => x.Date)
        //        .ToListAsync(cancellationToken);

            ViewBag.UserList = await _context.users.Where(x => x.Id != Id).ToListAsync();
            ViewBag.UserId = Id;
           // ViewBag.ReceivedUserId = receivedUserId;

            //return View(chats); 
            return View(new List<Chat>());
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage( [FromBody]messageDto messagedto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                
                return RedirectToAction(nameof(Index), new
                {
                    userId = messagedto.UserId,
                    receivedUserId = messagedto.receivedId
                });
            }

            var chat = new Chat
            {
                UserId = messagedto.UserId,
                ReceiverUserId = messagedto.receivedId,
                Messages = messagedto.message,
                Date = DateTime.Now
            };

            await _context.AddAsync(chat, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            
            var UserconnectionId = ChatHub.Users.FirstOrDefault(x => x.Value == chat.UserId).Key;
            var receiverConnectionId = ChatHub.Users.FirstOrDefault(x => x.Value == chat.ReceiverUserId).Key;

            if (!string.IsNullOrEmpty(UserconnectionId) && !string.IsNullOrEmpty(receiverConnectionId))
            {
                await _hubContext.Clients.Client(UserconnectionId).SendAsync("Messages", chat);
                await _hubContext.Clients.Client(receiverConnectionId).SendAsync("Messages", chat);
            }

            TempData["AuthMessage"] = "Mesaj gönderildi.";


            return Ok(chat);
        }
    }

}