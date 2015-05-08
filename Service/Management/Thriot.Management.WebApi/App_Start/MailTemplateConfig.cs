using System.Web;
using Thriot.Framework.Mails;

namespace Thriot.Management.WebApi
{
    public class MailTemplateConfig
    {
        public static void Register()
        {
            MailTemplateStore.Instance.Add(GetTemplate("Activation"));
            MailTemplateStore.Instance.Add(GetTemplate("ResetPassword"));
        }

        private static MailTemplate GetTemplate(string name)
        {
            return MailTemplate.Create(name, LoadContent(name, "subject"), LoadContent(name, "txt"), LoadContent(name, "html"));
        }

        private static string LoadContent(string name, string extension)
        {
            var mailTemplatesPath = HttpContext.Current.Server.MapPath("~/MailTemplates");

            var pathToRead = System.IO.Path.Combine(mailTemplatesPath, name + "." + extension);

            return System.IO.File.ReadAllText(pathToRead);
        }
    }
}