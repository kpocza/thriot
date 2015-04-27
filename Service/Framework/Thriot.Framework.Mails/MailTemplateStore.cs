using System.Collections.Generic;
using System.Linq;

namespace Thriot.Framework.Mails
{
    public class MailTemplateStore
    {
        public static readonly MailTemplateStore Instance = new MailTemplateStore();

        private readonly List<MailTemplate> _mailTemplates = new List<MailTemplate>();

        private MailTemplateStore()
        {
        }

        public void Add(MailTemplate templates)
        {
            _mailTemplates.Add(templates);
        }

        public MailTemplate Resolve(string name)
        {
            return _mailTemplates.Single(mt =>  mt.Name == name);
        }

        public void Cleanup()
        {
            _mailTemplates.Clear();
        }
    }
}
