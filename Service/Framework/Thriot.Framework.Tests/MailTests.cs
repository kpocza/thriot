﻿using System.IO;
using System.Net.Mail;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Framework.Mails;

namespace Thriot.Framework.Tests
{
    [TestClass]
    public class MailTest
    {

        [TestInitialize]
        public void TestInitialize()
        {
            MailTemplateStore.Instance.Cleanup();
            MailTemplateStore.Instance.Add(MailTemplate.Create("Minimal", "Subject", "Text", "<html>Html</html>"));
            MailTemplateStore.Instance.Add(MailTemplate.Create("ToReplace", "S@(Model.SSS)S", "Text@(Model.TextStuff) and more", "<html>Ht @(Model.some) ml</html>"));
            MailTemplateStore.Instance.Add(MailTemplate.Create("ImageAdded", "Subject", "Text", "<html>Html<img src=\"cid:img.jpg\"></img></html>"));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            MailTemplateStore.Instance.Cleanup();
        }

        [TestMethod]
        public void SendMinimal()
        {
            var mailSender = new MailSenderStub();
            var mailSettings = new MailSettingsStub();

            var mail = new Mail(mailSender, mailSettings);

            mail.Send(Addressing.Create("user@somewhere.hu", "lfj"), "Minimal", new {});

            Assert.AreEqual("no-reply@tasclr.com", mailSender.MailMessage.From.Address);
            Assert.AreEqual("user@somewhere.hu", mailSender.MailMessage.To[0].Address);
            Assert.AreEqual("Subject", mailSender.MailMessage.Subject);
            Assert.AreEqual("<html>Html</html>", GetContent(mailSender.MailMessage.AlternateViews[0].ContentStream));
            Assert.AreEqual("Text", GetContent(mailSender.MailMessage.AlternateViews[1].ContentStream));
        }

        [TestMethod]
        public void SendWithReplacements()
        {
            var mailSender = new MailSenderStub();
            var mailSettings = new MailSettingsStub();

            var mail = new Mail(mailSender, mailSettings);

            mail.Send(Addressing.Create("user@somewhere.hu", "lfj"), "ToReplace",
                new {SSS = "X", TextStuff = "plain text", some = "some html"});

            Assert.AreEqual("no-reply@tasclr.com", mailSender.MailMessage.From.Address);
            Assert.AreEqual("user@somewhere.hu", mailSender.MailMessage.To[0].Address);
            Assert.AreEqual("SXS", mailSender.MailMessage.Subject);
            Assert.AreEqual("<html>Ht some html ml</html>", GetContent(mailSender.MailMessage.AlternateViews[0].ContentStream));
            Assert.AreEqual("Textplain text and more", GetContent(mailSender.MailMessage.AlternateViews[1].ContentStream));
        }

        [TestMethod]
        public void SendWithImageAdded()
        {
            var mailSender = new MailSenderStub();
            var mailSettings = new MailSettingsStub();

            var mail = new Mail(mailSender, mailSettings);

            mail.Send(Addressing.Create("user@somewhere.hu", "lfj"), "ImageAdded", new {});

            Assert.AreEqual("no-reply@tasclr.com", mailSender.MailMessage.From.Address);
            Assert.AreEqual("user@somewhere.hu", mailSender.MailMessage.To[0].Address);
            Assert.AreEqual("Subject", mailSender.MailMessage.Subject);
            Assert.AreEqual("<html>Html<img src=\"cid:img.jpg\"></img></html>", GetContent(mailSender.MailMessage.AlternateViews[0].ContentStream));
            Assert.AreEqual("Text", GetContent(mailSender.MailMessage.AlternateViews[1].ContentStream));

            Assert.AreEqual(1, mailSender.MailMessage.AlternateViews[0].LinkedResources.Count);
            Assert.AreEqual(5, mailSender.MailMessage.AlternateViews[0].LinkedResources[0].ContentStream.Length);
        }

        [TestMethod]
        public void MultiSend()
        {
            var mailSender = new MailSenderStub();
            var mailSender2 = new MailSenderStub();
            var mailSettings = new MailSettingsStub();

            var mail = new Mail(mailSender, mailSettings);

            mail.Send(Addressing.Create("user@somewhere.hu", "lfj"), "ToReplace",
                new { SSS = "X", TextStuff = "plain text", some = "some html" });

            var mail2 = new Mail(mailSender2, mailSettings);

            mail2.Send(Addressing.Create("user@somewhere.hu", "lfj"), "ToReplace",
                new { SSS = "X", TextStuff = "plain text", some = "some html" });

            Assert.AreEqual("no-reply@tasclr.com", mailSender2.MailMessage.From.Address);
            Assert.AreEqual("user@somewhere.hu", mailSender2.MailMessage.To[0].Address);
            Assert.AreEqual("SXS", mailSender2.MailMessage.Subject);
            Assert.AreEqual("<html>Ht some html ml</html>", GetContent(mailSender2.MailMessage.AlternateViews[0].ContentStream));
            Assert.AreEqual("Textplain text and more", GetContent(mailSender2.MailMessage.AlternateViews[1].ContentStream));
        }

        private string GetContent(Stream stream)
        {
            return Encoding.UTF8.GetString(((MemoryStream)stream).ToArray());
        }
    }

    class MailSenderStub : IMailSender
    {
        public MailMessage MailMessage { get; private set; }

        public void Send(MailMessage mailMessage)
        {
            MailMessage = mailMessage;
        }
    }

    class MailSettingsStub : IMailSettings
    {
        public MailAddress From
        {
            get { return new MailAddress("no-reply@tasclr.com", "Tasclr"); }
        }

        public string SmtpServer
        {
            get { return "smtp"; }
        }

        public string BouncesAddress
        {
            get { return "bounces@tasclr.com"; }
        }

        public Stream GetImageContent(string fileName)
        {
            var ms = new MemoryStream();
            ms.Write(new byte[] { 1, 2, 3, 4, 5 }, 0, 5);
            ms.Position = 0;

            return ms;
        }
    }
}
