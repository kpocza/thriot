namespace Thriot.Management.Services
{
    public interface ICapabilityProvider
    {
        bool CanCreateCompany { get; }

        bool CanDeleteCompany { get; }

        bool CanCreateService { get; }

        bool CanDeleteService { get; }
    }
}
