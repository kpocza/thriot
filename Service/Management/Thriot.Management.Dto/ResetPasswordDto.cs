﻿namespace Thriot.Management.Dto
{
    public class ResetPasswordDto
    {
        public string UserId { get; set; }

        public string ConfirmationCode { get; set; }

        public string Password { get; set; }
    }
}
