namespace IoT.Management.Dto
{
    public class RegistrationResultDto
    {
        public bool NeedsActivation { get; set; }

        public string AuthToken { get; set; }
    }
}