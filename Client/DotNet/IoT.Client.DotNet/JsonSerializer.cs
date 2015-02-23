using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace IoT.Client.DotNet
{
    internal static class JsonSerializer
    {
        internal static string Serialize<T>(T obj, bool useSimpleDictionaryFormat = false)
        {
            var settings = new DataContractJsonSerializerSettings();
            settings.UseSimpleDictionaryFormat = useSimpleDictionaryFormat;

            var jsonSerializer = new DataContractJsonSerializer(typeof (T), settings);
            using (var ms = new MemoryStream())
            {
                jsonSerializer.WriteObject(ms, obj);

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        internal static T Deserialize<T>(string content)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(T));

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                ms.Position = 0;
                return (T)jsonSerializer.ReadObject(ms);
            }
        }
    }
}
