using System.ComponentModel.DataAnnotations;

namespace Thriot.Management.Model
{
    public class Setting
    {
        private const string CategoryRuntime = "Runtime";
        private const string CategoryServiceProfile = "ServiceProfile";
        private const string CategoryMicroservice = "Microservice";
        private const string PublicUrl = "PublicUrl";

        [StringLength(32)]
        [Key]
        public string Category { get; set; }

        [StringLength(32)]
        [Key]
        public string Config { get; set; }

        [MaxLength(2048)]
        [Required]
        public string Value { get; set; }

        public Setting()
        {
        }

        public Setting(SettingId id, string value)
        {
            Category = id.Category;
            Config = id.Config;
            Value = value;
        }

        public SettingId Id
        {
            get { return new SettingId(Category, Config); }
        }

        public static readonly SettingId ServiceProfile = new SettingId(CategoryRuntime, "ServiceProfile");
        public static readonly SettingId EmailActivation = new SettingId(CategoryRuntime, "EmailActivation");

        public static readonly SettingId PrebuiltCompany = new SettingId(CategoryServiceProfile, "PrebuiltCompany");
        public static readonly SettingId PrebuiltService = new SettingId(CategoryServiceProfile, "PrebuiltService");
        public static readonly SettingId UserForPrebuiltEntity = new SettingId(CategoryServiceProfile, "UserForPrebuiltEntity");

        public static readonly SettingId TelemetrySetupServiceEndpoint = new SettingId(CategoryMicroservice, "TelemetrySetupServiceEndpoint");
        public static readonly SettingId TelemetrySetupServiceApiKey = new SettingId(CategoryMicroservice, "TelemetrySetupServiceApiKey");

        public static readonly SettingId MessagingServiceEndpoint = new SettingId(CategoryMicroservice, "MessagingServiceEndpoint");
        public static readonly SettingId MessagingServiceApiKey = new SettingId(CategoryMicroservice, "MessagingServiceApiKey");

        public static readonly SettingId WebsiteUrl = new SettingId(PublicUrl, "WebsiteUrl");
        public static readonly SettingId ManagementApiUrl = new SettingId(PublicUrl, "ManagementApiUrl");
        public static readonly SettingId PlatformApiUrl = new SettingId(PublicUrl, "PlatformApiUrl");
        public static readonly SettingId PlatformWsUrl = new SettingId(PublicUrl, "PlatformWsUrl");
        public static readonly SettingId ReportingApiUrl = new SettingId(PublicUrl, "ReportingApiUrl");
    }
}
