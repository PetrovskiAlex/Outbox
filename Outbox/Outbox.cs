using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Outbox
{
    [Table("_Outbox")]
    public class Outbox
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string Content { get; set; }

        public int Retries { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public OutboxMessageStatus Status { get; set; }

        public string ErrorMessage { get; set; }

        public string Error { get; set; }

        public DateTime? Updated { get; set; }

        public OutboxMessageType OutboxMessageType { get; set; }
    }
}