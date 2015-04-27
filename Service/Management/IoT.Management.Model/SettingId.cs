namespace Thriot.Management.Model
{
    public class SettingId
    {
        public string Category { get; private set; }
        public string Config { get; private set; }

        public SettingId(string category, string config)
        {
            Category = category;
            Config = config;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", Category, Config);
        }

        public static SettingId GetConnection(string connectionStringName)
        {
            return new SettingId("Connection", connectionStringName);
        }
    }
}
