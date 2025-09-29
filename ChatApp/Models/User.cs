using Microsoft.AspNetCore.Identity;

namespace ChatApp.Models
{
    public class User : IdentityUser<Guid>
    {
       // public User()
       // {
       //     Id = Guid.NewGuid();
       // }
       //// public override string Id { get => base.Id; set => base.Id = value; }
       // public override Guid Id { get; set; }
       // public string userName { get; set; } = string.Empty;
       // public string password { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
