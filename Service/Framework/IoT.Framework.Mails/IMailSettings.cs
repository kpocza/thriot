﻿using System.IO;
using System.Net.Mail;

namespace IoT.Framework.Mails
{
    public interface IMailSettings
    {
        MailAddress From { get; }
        string SmtpServer { get; }
        string BouncesAddress { get; }
        Stream GetImageContent(string fileName);
    }
}
