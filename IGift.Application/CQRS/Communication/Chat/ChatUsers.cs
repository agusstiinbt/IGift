﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGift.Application.CQRS.Communication.Chat
{
    /// <summary>
    /// Esta clase se usa para mostrar en el costado del chat room los chats que tenemos con otros usuarios
    /// </summary>
    public class ChatUsers
    {
        public long ChatId { get; set; }
        public required string UserName { get; set; }
        public string? UserProfileImageUrl { get; set; }
        public required string LastMessage { get; set; }
        public bool Seen { get; set; }
    }
}
