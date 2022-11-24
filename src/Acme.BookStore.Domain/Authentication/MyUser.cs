using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.BookStore.Authentication;

public class MyUser:AuditedAggregateRoot<Guid>
{
    public Guid? IdentityUserId { get; set; }
    public string UserName { get; set; }
    public string Type { get; set; }
    public bool Statues { get; set; }
    public DateTime LastSeen { get; set; }
    public string LastMessage { get; set; }
    public SubscriptionType SubscriptionType { get; set; }
    public string[] Numbers { get; set; }
}