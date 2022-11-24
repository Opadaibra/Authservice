using System;
using Volo.Abp.Account;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.BookStore.Authentication;

public class MyTenant:AuditedAggregateRoot<Guid>
{
    public Guid TenantId { get; set; }
    public string Name { get; set; }
    public string CompanyName { get; set; }
    public string PhoneNumber { get; set; }
    public string BusinessType { get; set; }
    public string Address { get; set; }
    public bool IsFree { get; set; }
    public DateTime ExpirationDate { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; }

 

}