using System;
using Volo.Abp.Account;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.Authentication;

public class MyTenantDto:AuditedEntityDto<Guid>
{
    
    //public string TenantId { get; set; } // Represent forging key from @abp-Tenant
    public string Name { get; set; }
    public string CompanyName { get; set; }
    public string PhoneNumber { get; set; }
    public string BusinessType { get; set; }
    public string Address { get; set; }
    public bool IsFree { get; set; }
    public DateTime ExpirationDate { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; }
    
}