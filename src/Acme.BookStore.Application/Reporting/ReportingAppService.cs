using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acme.BookStore.Authentication;
using Volo.Abp.Account;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.TenantManagement;
using Volo.Abp.Users;
using IdentityUser = Microsoft.AspNetCore.Identity.IdentityUser;

namespace Acme.BookStore.Reporting
{
    public class ReportingAppService : CrudAppService<Proxy, ProxyDto, Guid, IReportingAppService>
    {
        private readonly IIdentityUserRepository _userRepository;
        private readonly IdentityUserManager _identityUserManager;
        public ReportingAppService(IRepository<Proxy, Guid> repository, IIdentityUserRepository userRepository, IdentityUserManager identityUserManager) : base(repository)
        {
            _userRepository = userRepository;
            _identityUserManager = identityUserManager;
        }

        public async Task<List<Proxy>> GetListOfProxy(Guid tenantid)
        {
            List<Proxy> proxyList = new List<Proxy>();
         //    = await _userRepository.GetListAsync(u => u.creatorid == tenantid);
            var listOfProxy =  _identityUserManager.Users.Where(u => u.CreatorId == tenantid);
            foreach (var proxy in listOfProxy)
            {
                Proxy temp = new Proxy()
                {
                    LastActiveTime = (DateTime)proxy.GetProperty("last-seen"),
                    Status = (bool)proxy.GetProperty("statues"),
                    LastMessage = (string)proxy.GetProperty("last-message"),
                    AssignedNumbers = (string[])proxy.GetProperty("numbers")
                };
                proxyList.Add(await Repository.GetAsync(p => p.Id == proxy.Id));
            }
            //var users = await _identityUserManager.Users();
            
            Console.WriteLine(proxyList);
            return proxyList;
        }


    }
}
