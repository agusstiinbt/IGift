﻿using IGift.Application.Interfaces.Chat;

namespace IGift.Application.Models
{
    public class Chat<TUser> : IChatHistory<TUser> where TUser : IChatUser
    {
        public long Id { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual TUser FromUser { get; set; }
        public virtual TUser ToUser { get; set; }
    }
}
