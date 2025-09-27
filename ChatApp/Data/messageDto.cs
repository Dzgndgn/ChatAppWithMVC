namespace ChatApp.Data
{
    public record messageDto
    {
        public string message { get; set; }
        public Guid UserId { get; set; }
        public Guid receivedId { get; set; }
    }
}
