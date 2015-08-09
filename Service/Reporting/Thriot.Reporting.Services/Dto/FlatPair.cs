namespace Thriot.Reporting.Services.Dto
{
    public class FlatPair
    {
        public FlatPair(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; private set; }
        public string Value { get; private set; }
    }
}
