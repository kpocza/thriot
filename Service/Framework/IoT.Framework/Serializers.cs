using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;

namespace Thriot.Framework
{
    public static class Serializers
    {
        public static T DeepClone<T>(object entity)
        {
            return (T) FromJsonString<T>(ToJsonString(entity));
        }

        public static string ToJsonString(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None, SerializerSettings);
        }

        public static T FromJsonString<T>(string str)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(str, SerializerSettings);
            }
            catch
            {
                return default(T);
            }
        }

        private readonly static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None
        };
    }
}
