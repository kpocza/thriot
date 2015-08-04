using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Razor;
using Microsoft.AspNet.Razor.CodeGenerators;
using Microsoft.CSharp;

namespace Thriot.Framework.Mails.TemplatingEngine
{
    class RazorTemplater
    {
        private readonly string _template;
        private readonly object _model;
        private static readonly object _lock = new object();

        public RazorTemplater(string template, object model)
        {
            _template = template;
            _model = model;
        }

        public string GetResult()
        {
            // no idea (yet) if this stuff is thread safe or not. 
            // For safetiness the critical part is put into a lock.
            lock (_lock)
            {
                var razorResult = GenerateCode();

                var compilerResults = CompileCode(razorResult);

                if (compilerResults.Errors.HasErrors)
                    return "!!!INTERNAL TEMPLATE COMPILATION ERROR!!!";

                var substituted = ExecuteCode(compilerResults);

                return substituted.Replace("<nl/>", "").Replace("<nl />", "");
            }
        }

        private GeneratorResults GenerateCode()
        {
            var codeLang = new CSharpRazorCodeLanguage();

            var host = new RazorEngineHost(codeLang)
            {
                DefaultBaseClass = "Thriot.Framework.Mails.TemplatingEngine.BaseView",
                DefaultClassName = "Template",
                DefaultNamespace = "CompiledRazor",
            };

            var engine = new RazorTemplateEngine(host);

            return engine.GenerateCode(new StringReader(_template));
        }

        private static CompilerResults CompileCode(GeneratorResults razorResult)
        {
            var referencedAssemblies = new List<string>
            {
                Assembly.GetExecutingAssembly().Location,
                typeof (Microsoft.CSharp.RuntimeBinder.Binder).Assembly.Location,
                typeof (System.Runtime.CompilerServices.CallSite).Assembly.Location
            };

            var compilerParameters = new CompilerParameters(referencedAssemblies.Distinct().ToArray());
            var codeProvider = new CSharpCodeProvider();

            return codeProvider.CompileAssemblyFromSource(compilerParameters, razorResult.GeneratedCode);
        }

        private string ExecuteCode(CompilerResults compilerResults)
        {
            var templateInstance = (BaseView)compilerResults.CompiledAssembly.CreateInstance("CompiledRazor.Template");

            IDictionary<string, object> expandoModel = new ExpandoObject();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(_model.GetType()))
                expandoModel.Add(property.Name, property.GetValue(_model));

            templateInstance.Model = expandoModel;

            return templateInstance.ToString();
        }
    }
}
