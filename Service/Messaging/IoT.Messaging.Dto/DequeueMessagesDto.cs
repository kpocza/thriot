using System.Collections.Generic;

namespace Thriot.Messaging.Dto
{
    public class DequeueMessagesDto
    {
        public List<DequeueMessageDto> Messages { get; set; }
    }
}
