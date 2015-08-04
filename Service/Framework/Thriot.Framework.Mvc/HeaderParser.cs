using Microsoft.AspNet.Http;
using System.Collections.Generic;
using System.Linq;

namespace Thriot.Framework.Mvc
{
    public static class HeaderParser
    {
        public static IDictionary<string, string> ParseAllOrNothing(IHeaderDictionary headers, params string[] fields)
        {
            var result = new Dictionary<string, string>();
            string[] values;

            foreach (var field in fields)
            {
                if (headers.TryGetValue(field, out values) && values.Count() == 1)
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
