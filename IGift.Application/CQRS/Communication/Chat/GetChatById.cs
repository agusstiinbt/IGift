namespace IGift.Application.CQRS.Communication.Chat
{
    /// <summary>
    /// Esta clase se usa para traernos el chat que queremos visualizar 
    /// </summary>
    public class SearchChatById
    {
        public string ToUserId { get; set; }

        public string FromUserId { get; set; }

        public DateTime LastMessageDate { get; set; }

        public required bool IsFirstTime { get; set; }

    }
}
