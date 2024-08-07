namespace IGift.Application.Models.Chat
{
    public class ChatHistory
    {
        public long Id { get; set; }
        public string FromUserId { get; set; } = string.Empty;
        public string ToUserId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string FromUserImageUrl { get; set; } = string.Empty;
        public string ToUserImageUrl { get; set; } = string.Empty;
    }
}
