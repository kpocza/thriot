using System.Text;
using System.Threading.Tasks;

namespace Thriot.Framework.Mails.TemplatingEngine
{
    public abstract class BaseView
    {
        public virtual Task ExecuteAsync()
        {
            return Task.FromResult(0);
        }

        private StringBuilder _builder;

        public dynamic Model { get; set; }

        public void Write(object value)
        {
            WriteLiteral(value);
        }

        public void WriteLiteral(object value)
        {
            _builder.Append(value);
        }

        public string Raw(string value)
        {
            return value;
        }

        public override string ToString()
        {
            _builder = new StringBuilder();
            ExecuteAsync().Wait();
            return _builder.ToString();
        }
    }
}