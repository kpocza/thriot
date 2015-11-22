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
        private string _attributePrefix;
        private string _attributeSuffix;
        private string _attributeValue;

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


        public void BeginWriteAttribute(string name, string prefix, int v1, string suffix, int v2, int stg)
        {
            _attributePrefix = prefix;
            _attributeSuffix = suffix;
        }

        public void WriteAttributeValue(string name, int v1, string value, int v2, int v3, bool v4)
        {
            _attributeValue = value;
        }

        public void EndWriteAttribute()
        {
            WriteLiteral(_attributePrefix);
            WriteLiteral(_attributeValue);
            WriteLiteral(_attributeSuffix);
        }

        public override string ToString()
        {
            _builder = new StringBuilder();
            ExecuteAsync().Wait();
            return _builder.ToString();
        }
    }
}