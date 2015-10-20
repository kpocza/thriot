using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Thriot.TestHelpers
{
    public class EnvironmentFactory
    {
        public IManagementEnvironment ManagementEnvironment { get; private set; }
        public IMessagingEnvironment MessagingEnvironment { get; private set; }
        public ITelemetryEnvironment TelemetryEnvironment { get; private set; }
        public IQueueEnvironment QueueEnvironment { get; private set; }

        public EnvironmentFactory(IManagementEnvironment managementEnvironment, IMessagingEnvironment messagingEnvironment, ITelemetryEnvironment telemetryEnvironment, IQueueEnvironment queueEnvironment)
        {
            ManagementEnvironment = managementEnvironment;
            MessagingEnvironment = messagingEnvironment;
            TelemetryEnvironment = telemetryEnvironment;
            QueueEnvironment = queueEnvironment;
        }
    }
}
