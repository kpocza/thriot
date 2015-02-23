namespace IoT.Client.DotNet.Management.Dto
{
    public class RegistrationResult
    {
        public bool NeedsActivation { get; set; }

        public string AuthToken { get; set; }
    }
}
