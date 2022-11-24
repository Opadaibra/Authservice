using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Acme.BookStore.Invoices;

public class Invoice:Entity<Guid>,IMultiTenant
{ 
    public Guid? TenantId { get; }
    public long Number { get; set; }
    

    public Invoice(Guid? tenantId)
    {
        TenantId = tenantId;
    }
    

   
}