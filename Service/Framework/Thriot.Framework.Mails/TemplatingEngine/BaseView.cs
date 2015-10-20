using System;
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

        public void WriteAttribute(string name, Tuple<string, int> prefix, Tuple<string, int> suffix,
            Tuple<Tuple<string, int>, Tuple<object, int>, bool> value)
        {
            WriteLiteral(prefix.Item1);
            WriteLiteral(value.Item2.Item1);
            WriteLiteral(suffix.Item1);
        }

        public override string ToString()
        {
            _builder = new StringBuilder();
            ExecuteAsync().Wait();
            return _builder.ToString();
        }
    }
}