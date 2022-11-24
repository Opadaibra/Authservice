using System;
using Volo.Abp.Application.Services;

namespace Acme.BookStore.Authentication;

public interface IUserAppService:ICrudAppService<MyUserDto,Guid>
{
    
}