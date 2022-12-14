using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Acme.BookStore.Reporting;

public class Proxy:Entity<Guid> ,IMultiTenant
{
    public DateTime LastActiveTime { get; set; }
    public bool Status { get; set; }
    public string LastMessage { get; set; }
    public string[] AssignedNumbers { get; set; }
    public Guid? TenantId { get; }
    
   

    
}