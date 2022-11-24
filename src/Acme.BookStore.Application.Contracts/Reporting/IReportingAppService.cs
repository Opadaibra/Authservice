using System;
using Volo.Abp.Application.Services;

namespace Acme.BookStore.Reporting;

public interface IReportingAppService:ICrudAppService<ProxyDto, Guid>
{
    
}