using System.Collections.Generic;

namespace Thriot.Reporting.Dto
{
    public class FlatReportDto
    {
        public List<string> Properties { get; set; }

        public List<FlatRowDto> Rows { get; set; }
    }
}
