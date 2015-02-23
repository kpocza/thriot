using System.Collections.Generic;

namespace IoT.ServiceClient.Messaging
{
    public class EnqueueMessagesDto
    {
        public List<EnqueueMessageDto> Messages { get; set; }
    }
}
