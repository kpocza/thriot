namespace Thriot.Framework.Mails
{
    public class Addressing
    {
        public static Addressing Create(string toAddress, string toName)
        {
            return new Addressing
            {
                ToAddress = toAddress,
                ToName = toName
            };
        }

        public string ToAddress { get; private set; }

        public string ToName { get; private set; }
    }
}
