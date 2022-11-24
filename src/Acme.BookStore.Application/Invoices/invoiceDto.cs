using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.MultiTenancy;

namespace Acme.BookStore.Invoices
{
    public class InvoiceDto : EntityDto<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; }
        public long? Number { get; }

        public InvoiceDto(Guid? tenantId) 
        {
            TenantId = tenantId;
        }

       
    }
}
