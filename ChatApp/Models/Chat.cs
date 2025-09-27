namespace ChatApp.Models
{
    public class Chat
    {
        public Guid Id { get; set; }

        public Chat()
        {
            Id = Guid.NewGuid();
        }

        public Guid UserId { get; set; }

        public Guid ReceiverUserId { get; set; }

        public string Messages { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
