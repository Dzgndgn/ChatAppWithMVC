using ChatApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> users { get; set; }
        public DbSet<Chat> chats { get; set; }
    }
}
