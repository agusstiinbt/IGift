namespace IGift.Application.CQRS.Communication.Chat
{
    /// <summary>
    /// Esta clase se usa para traernos el chat que queremos visualizar 
    /// </summary>
    public class SearchChatById
    {
        /// <summary>
        /// El Id del chat que se desea traer
        /// </summary>
        public string ToUserId { get; set; }

        public string FromUserId { get; set; }

        public bool IsFirstTime { get; set; }

        public SearchChatById(string ToUserId, string fromUserId)
        {
            this.ToUserId = ToUserId;
            FromUserId = fromUserId;
        }
    }
}
