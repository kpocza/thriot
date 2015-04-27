namespace Thriot.Management.Dto
{
    public class InfoDto
    {
        public string ServiceProfile { get; set; }

        public string PrebuiltCompany { get; set; }

        public string PrebuiltService { get; set; }

        public bool CanCreateCompany { get; set; }

        public bool CanDeleteCompany { get; set; }

        public bool CanCreateService { get; set; }

        public bool CanDeleteService { get; set; }
    }
}
