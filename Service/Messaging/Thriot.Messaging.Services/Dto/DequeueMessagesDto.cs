using System.Collections.Generic;

namespace Thriot.Messaging.Services.Dto
{
    public class DequeueMessagesDto
    {
        public List<DequeueMessageDto> Messages { get; set; }
    }
}
