using IGift.Application.Enums;

namespace IGift.Application.Responses.Notification
{
    public class NotificationResponse
    {
        public string? IdUser { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public TypeNotification Type { get; set; }
    }
}