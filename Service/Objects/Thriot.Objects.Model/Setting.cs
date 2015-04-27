using System.ComponentModel.DataAnnotations;

namespace Thriot.Objects.Model
{
    public class Setting
    {
        private const string CategoryMicroservice = "Microservice";

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

        public static readonly SettingId TelemetrySetupServiceEndpoint = new SettingId(CategoryMicroservice, "TelemetrySetupServiceEndpoint");
        public static readonly SettingId TelemetrySetupServiceApiKey = new SettingId(CategoryMicroservice, "TelemetrySetupServiceApiKey");

        public static readonly SettingId MessagingServiceEndpoint = new SettingId(CategoryMicroservice, "MessagingServiceEndpoint");
        public static readonly SettingId MessagingServiceApiKey = new SettingId(CategoryMicroservice, "MessagingServiceApiKey");
    }
}
