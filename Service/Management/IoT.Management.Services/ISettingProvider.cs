using IoT.Management.Model;

namespace IoT.Management.Services
{
    public interface ISettingProvider
    {
        ServiceProfile ServiceProfile { get; }

        bool EmailActivation { get; }

        string PrebuiltCompany { get; set; }

        string PrebuiltService { get; set; }

        string UserForPrebuiltEntity { get; set; }

        string WebsiteUrl { get; }

        string ManagementApiUrl { get; }

        string PlatformApiUrl { get; }

        string PlatformWsUrl { get; }

        string ReportingApiUrl { get; }
    }
}