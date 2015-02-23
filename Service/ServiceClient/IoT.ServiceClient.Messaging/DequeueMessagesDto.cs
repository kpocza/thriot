using System.Collections.Generic;

namespace IoT.ServiceClient.Messaging
{
    public class DequeueMessagesDto
    {
        public List<DequeueMessageDto> Messages { get; set; }
    }
}
