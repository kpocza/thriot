using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;

namespace Thriot.Objects.Common.CachingOperations
{
    public class ServiceOperations : CachingBase<Service>, IServiceOperations
    {
        private readonly IPersistedServiceOperations _serviceOperations;

        protected override string Prefix
        {
            get { return "Service"; }
        }

        public ServiceOperations(IPersistedServiceOperations serviceOperations)
        {
            _serviceOperations = serviceOperations;
        }

        public Service Get(string id)
        {
            return Get(id, internalId => _serviceOperations.Get((string)internalId));
        }
    }
}
