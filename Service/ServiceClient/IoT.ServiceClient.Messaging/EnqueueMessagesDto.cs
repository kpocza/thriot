using System.Collections.Generic;

namespace Thriot.ServiceClient.Messaging
{
    public class EnqueueMessagesDto
    {
        public List<EnqueueMessageDto> Messages { get; set; }
    }
}
