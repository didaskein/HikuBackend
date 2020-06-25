using Hiku.Framework.Events;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hiku.Services.Service.ServiceBus
{
    public class ServiceBusMessageFormatter
    {
        public Message GetBrokeredMessage(IntegrationEvent domainEvent)
        {
            var serializedEvent = JsonConvert.SerializeObject(domainEvent);
            var bytes = Encoding.UTF8.GetBytes(serializedEvent);

            return new Message(bytes)
            {
                UserProperties =
                {
                    { "eventId", domainEvent.EventId },
                    { "parentEventId", domainEvent.ParentEventId },
                    { "createdAt", domainEvent.CreatedAt },
                    { "id", domainEvent.Id },
                    { "name", domainEvent.Name}
                },
                ContentType = "application/json"
            };
        }
    }
}
