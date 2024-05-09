﻿using System.ComponentModel.DataAnnotations;

namespace IGift.Shared.Operations.Login
{
    public class LoginModel
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
