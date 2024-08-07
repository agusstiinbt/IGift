namespace IGift.Application.Models.Chat
{
    public class ChatHistory
    {
        public long Id { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}
