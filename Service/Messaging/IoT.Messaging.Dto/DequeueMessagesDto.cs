using System.Collections.Generic;

namespace IoT.Messaging.Dto
{
    public class DequeueMessagesDto
    {
        public List<DequeueMessageDto> Messages { get; set; }
    }
}
