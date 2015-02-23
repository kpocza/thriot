using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
using RazorEngine;

namespace IoT.Framework.Mails
{
    public class Mail
    {
        private readonly IMailSettings _mailSettings;
        private readonly IMailSender _mailSender;

        private Addressing _addressing;
        private string _templateName;
        private object _model;
        private MailMessage _mailMessage;

        public Mail(IMailSender mailSender, IMailSettings mailSettings)
        {
            _mailSender = mailSender;
            _mailSettings = mailSettings;
        }

        public void Send(Addressing addressing, string templateName,  object model)
        {
            _mailMessage = new MailMessage();

            _addressing = addressing;
            _templateName = templateName;
            _model = model;

            PrepareMessage();

            _mailSender.Send(_mailMessage);
        }

        private void PrepareMessage()
        {
            var mailTemplate = MailTemplateStore.Instance.Resolve(_templateName);

            _mailMessage.From = _mailSettings.From;
            _mailMessage.To.Add(new MailAddress(_addressing.ToAddress, _addressing.ToName));
            _mailMessage.Subject = GetSubstitutedContent(mailTemplate.Subject);

            var htmlContent = GetSubstitutedContent(mailTemplate.Html);
            var textContent = GetSubstitutedContent(mailTemplate.Text);

            var htmlAlternate = CreateAlternateView(htmlContent, "text/html");
            var textAlternate = CreateAlternateView(textContent, "text/plain");

            ResolveEmbeddedImages(htmlContent, htmlAlternate);

            _mailMessage.AlternateViews.Add(htmlAlternate);
            _mailMessage.AlternateViews.Add(textAlternate);

            AddBouncesHeaders();
        }

        private void AddBouncesHeaders()
        {
            var bouncesAddress = _mailSettings.BouncesAddress;

            if (!string.IsNullOrWhiteSpace(bouncesAddress))
            {
                _mailMessage.Headers.Add("X-Errors-To", bouncesAddress);
                _mailMessage.Headers.Add("Return-Path", bouncesAddress);
                _mailMessage.Headers.Add("Errors-To", bouncesAddress);
                _mailMessage.Headers.Add("Auto-Submitted", "auto-generated");
                _mailMessage.Headers.Add("Message-ID", GenerateMailMessageId(_mailSettings.SmtpServer));
                _mailMessage.Headers.Add("X-Auto-Response-Suppress", "OOF, DR, RN, NRN, AutoReply");
            }
        }

        private static AlternateView CreateAlternateView(string message, string mimeType)
        {
            string contenttype = mimeType + "; charset=UTF-8";
            
            AlternateView alternate = AlternateView.CreateAlternateViewFromString(message, new ContentType(contenttype));
            alternate.TransferEncoding = TransferEncoding.QuotedPrintable;

            return alternate;
        }

        private void ResolveEmbeddedImages(string htmlContent, AlternateView htmlAlternate)
        {
            Regex contentIdRegex = new Regex(@"""cid:([\w\.]*?)""", RegexOptions.Compiled);

            var embeddedImages = new Dictionary<string, string>();

            var matches = contentIdRegex.Matches(htmlContent);
            foreach (Match match in matches)
            {
                var imgName = match.Groups[1].Value;
                embeddedImages[imgName.Replace(".", "")] = imgName;
            }

            foreach (var embeddedImage in embeddedImages)
            {
                var fileName = embeddedImage.Value;

                var imageStream = _mailSettings.GetImageContent(fileName);

                if (imageStream != null)
                {
                    var mimeType = GetMediaType(fileName);

                    if (mimeType != null)
                    {
                        var linkedResource = new LinkedResource(imageStream, mimeType);
                        linkedResource.ContentId = embeddedImage.Value;
                        linkedResource.ContentType.Name = embeddedImage.Value;
                        htmlAlternate.LinkedResources.Add(linkedResource);
                    }
                }
            }
        }

        private static string GetMediaType(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            if (ext != null)
            {
                switch (ext.ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                        return "image/jpeg";
                    case ".png":
                        return "image/png";
                }
            }
            return null;
        }

        private string GetSubstitutedContent(string text)
        {
            return Razor.Parse(text, _model).Replace("<nl/>", "").Replace("<nl />", "");
        }

        private static string GenerateMailMessageId(string mailServerName)
        {
            return String.Format("<mail.{0}.{1}@{2}>", DateTime.UtcNow.Ticks, Guid.NewGuid().ToString(), mailServerName);
        }
    }
}
