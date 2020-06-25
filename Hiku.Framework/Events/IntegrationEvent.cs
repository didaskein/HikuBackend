using System;
using System.Collections.Generic;
using System.Text;

namespace Hiku.Framework.Events
{
    public enum DomainEventAuthor
    {
        Unknown = 0,
        Admin = 1,
        System = 2
    }

    public class IntegrationEvent
    {

        protected IntegrationEvent(string name, DomainEventAuthor creator, Guid? id, Guid? creatorId)
        {
            EventId = Guid.NewGuid();
            Name = name;
            Author = creator;
            CreatedAt = DateTimeOffset.Now;
            Id = id;
            AuthorId = creatorId;
        }

        protected IntegrationEvent(string name, DomainEventAuthor creator, Guid? id, Guid? creatorId, Guid? parentEventId) :
            this(name, creator, id, creatorId)
        {
            ParentEventId = parentEventId;
        }


        public DateTime CreationDate { get; }
        public Guid EventId { get; set; }
        public Guid? ParentEventId { get; set; }
        public string Name { get; set; }
        public Guid? Id { get; set; }
        public DomainEventAuthor Author { get; set; }
        public Guid? AuthorId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
