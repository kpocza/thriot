using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace IoT.Framework
{
    public static class JTokenExtensions
    {
        public static void EnsureRecognizableFormat(this JToken jToken)
        {
            var toRemove = new List<JToken>();

            foreach (JToken child in jToken.Children())
            {
                var prop = child as JProperty;
                if (prop == null)
                {
                    toRemove.Add(child);
                    continue;
                }

                if (!IsValidType(prop.Value.Type))
                {
                    toRemove.Add(child);
                }
            }

            toRemove.ForEach(e => e.Remove());
        }

        private static readonly IEnumerable<JTokenType> ValidTypes = new [] {JTokenType.Boolean, JTokenType.Date, JTokenType.Float, JTokenType.Guid, JTokenType.Integer, JTokenType.String};

        private static bool IsValidType(JTokenType type)
        {
            return ValidTypes.Contains(type);
        }
    }
}
