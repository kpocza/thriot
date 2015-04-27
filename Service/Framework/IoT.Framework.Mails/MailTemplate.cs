namespace Thriot.Framework.Mails
{
    public class MailTemplate
    {
        public static MailTemplate Create(string name, string subject, string text, string html)
        {
            return new MailTemplate
            {
                Name = name,
                Subject = subject,
                Text = text,
                Html = html
            };
        }

        public string Name { get; private set; }
        public string Subject { get; private set; }
        public string Text { get; private set; }
        public string Html { get; private set; }
    }
}
