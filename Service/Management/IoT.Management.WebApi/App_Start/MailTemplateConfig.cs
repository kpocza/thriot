using System.Web;
using IoT.Framework.Mails;

namespace IoT.Management.WebApi
{
    public class MailTemplateConfig
    {
        public static void Register()
        {
            MailTemplateStore.Instance.Add(GetTemplate("Activation"));
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