namespace IGift.Application.CQRS.Communication.Chat
{
    /// <summary>
    /// Esta clase se usa para traernos el chat que queremos visualizar 
    /// </summary>
    public class GetChatById
    {
        /// <summary>
        /// El Id del chat que se desea traer
        /// </summary>
        public string ToUserId { get; set; }

        public GetChatById(string ToUserId)
        {
            this.ToUserId = ToUserId;
        }
    }
}
