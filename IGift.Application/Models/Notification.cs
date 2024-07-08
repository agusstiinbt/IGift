﻿using IGift.Application.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace IGift.Application.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string IdUser { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public TypeNotification Type { get; set; }
    }
}
