using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Thriot.Reporting.Dto;

namespace Thriot.Reporting.WebApi.Formatters
{
    public static class CsvFormatter
    {
        public static HttpResponseMessage ToHttpResponseMessage(FlatReportDto flatReport)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            using (var stringWriter = new StringWriter())
            {
                if (flatReport != null)
                {
                    stringWriter.WriteLine(string.Join<string>(",",
                        new[] {"DeviceId", "Name", "Time",}.Union(flatReport.Properties.Select(Escaped))));

                    var properties = flatReport.Properties;
                    int propertyCount = properties.Count();

                    foreach (var row in flatReport.Rows)
                    {
                        var valueLine = Escaped(row.DeviceId) + "," + Escaped(row.Name) + "," +
                                        Escaped(row.Timestamp.ToString(CultureInfo.InvariantCulture)) +
                                        (propertyCount > 0 ? "," : "");

                        for (var p = 0; p < propertyCount; p++)
                        {
                            var prop = properties[p];

                            var field = row.Fields.SingleOrDefault(f => f.Key == prop);
                            var escaped = Escaped(field != null ? field.Value : null);
                            valueLine = string.Concat(valueLine, escaped, p < propertyCount - 1 ? "," : "");
                        }

                        stringWriter.WriteLine(valueLine);
                    }
                }
                response.Content = new StringContent(stringWriter.ToString());
            }

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "telemetry.csv"
            };

            return response;
        }

        private static string Escaped(string val)
        {
            if (val == null)
                return string.Empty;

            if (val.Contains(","))
                val = string.Concat("\"", val, "\"");

            if (val.Contains("\r"))
                val = val.Replace("\r", " ");
            if (val.Contains("\n"))
                val = val.Replace("\n", " ");

            return val;
        }
    }
}