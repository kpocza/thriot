using System.Collections.Generic;

namespace Thriot.Reporting.Services.Dto
{
    public class FlatReportDto
    {
        public List<string> Properties { get; set; }

        public List<FlatRowDto> Rows { get; set; }
    }
}
