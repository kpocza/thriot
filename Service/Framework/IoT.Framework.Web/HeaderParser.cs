using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace IoT.Framework.Web
{
    public static class HeaderParser
    {
        public static IDictionary<string, string> ParseAllOrNothing(HttpRequestHeaders headers, params string[] fields)
        {
            var result = new Dictionary<string, string>();
            IEnumerable<string> values;

            foreach (var field in fields)
            {
                if (headers.TryGetValues(field, out values) && values.Count() == 1)
                {
                    result.Add(field, values.Single());
                }
                else
                {
                    return null;
                }
            }

            return result;
        }
    }
}
