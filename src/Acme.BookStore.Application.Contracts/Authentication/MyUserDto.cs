using System;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.Authentication;

public class MyUserDto:AuditedEntityDto<Guid>
{
    public Guid IdentityUserId { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public bool Statues { get; set; }
    public DateTime LastSeen { get; set; }
    public string LastMessage { get; set; }
    public SubscriptionType SubscriptionType { get; set; }
}