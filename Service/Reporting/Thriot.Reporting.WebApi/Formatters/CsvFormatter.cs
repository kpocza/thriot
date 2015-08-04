using Microsoft.AspNet.Mvc;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Thriot.Reporting.Dto;

namespace Thriot.Reporting.WebApi.Formatters
{
    public static class CsvFormatter
    {
        public static IActionResult ToHttpResponseMessage(FlatReportDto flatReport)
        {
            using (var stringWriter = new StringWriter())
            {
                stringWriter.NewLine = "\r\n";
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
                var fileContentResult = new FileContentResult(Encoding.UTF8.GetBytes(stringWriter.ToString()), "text/csv");
                fileContentResult.FileDownloadName = "telemetry.csv";
                return fileContentResult;
            }
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