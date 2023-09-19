using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Payloads
{
    public partial class ChangePasswordOTPPayload
    {        
        public string LoginName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string OTPPasscode { get; set; } = string.Empty;
        public string OTPPrefix { get; set; } = string.Empty;
    }
}