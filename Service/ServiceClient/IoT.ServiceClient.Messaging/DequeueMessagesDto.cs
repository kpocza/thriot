using System.Collections.Generic;

namespace Thriot.ServiceClient.Messaging
{
    public class DequeueMessagesDto
    {
        public List<DequeueMessageDto> Messages { get; set; }
    }
}
