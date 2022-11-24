using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp.Account;
using Volo.Abp.Account.Web.Areas.Account.Controllers.Models;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Settings;
using Volo.Abp.TenantManagement;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectExtending;
using AccountController = Volo.Abp.Account.Web.Areas.Account.Controllers.AccountController;
using IdentityUser = Volo.Abp.Identity.IdentityUser;
using UserLoginInfo = Volo.Abp.Account.Web.Areas.Account.Controllers.Models.UserLoginInfo;

namespace Acme.BookStore.Authentication;

public class RegisterAppService:BookStoreAppService
{
    private readonly ITenantManager _tenantManger;
    private readonly ITenantRepository _tenantRepository;
     private readonly IDistributedEventBus _distributedEventBus;
    private readonly AbpSignInManager _signInManager;
    private readonly ISettingProvider _settingProvider;
    private readonly IdentitySecurityLogManager _identitySecurityLogManager;
    private readonly IRepository<MyTenant,Guid> _myTenantRepository;
    private readonly IRepository<MyUser, Guid> _myUserRepository;
    private readonly IIdentityUserRepository _userRepository;
    private readonly IDataFilter _dataFilter;
    private  IdentityUserManager UserManager { get; }
    private IIdentityUserAppService _identityUserAppService;
    private readonly IDataSeeder _dataSeeder;
    private IOptions<IdentityOptions> _options;
    public RegisterAppService(ITenantManager tenantManger, ITenantRepository tenantRepository, 
        IDataSeeder dataSeeder, IRepository<MyTenant,Guid> myTenantRepository, 
        AbpSignInManager signInManager, ISettingProvider settingProvider,
        IdentitySecurityLogManager identitySecurityLogManager, 
        IdentityUserManager userManager, 
        IOptions<IdentityOptions> identityOptions,
        IIdentityUserRepository userRepository, 
        IDistributedEventBus distributedEventBus, 
        IRepository<MyUser, Guid> myUserRepository, IDataFilter dataFilter, IIdentityUserAppService identityUserAppService) 
    {
        _tenantManger = tenantManger;
        _tenantRepository = tenantRepository;
        _dataSeeder = dataSeeder;
        // _distributedEventBus = distributedEventBus;
        _myTenantRepository = myTenantRepository;
        _signInManager = signInManager;
        _settingProvider = settingProvider;
        UserManager = userManager;
        _identitySecurityLogManager = identitySecurityLogManager;
        _options = identityOptions;
        _userRepository = userRepository;
        _distributedEventBus = distributedEventBus;
        _myUserRepository = myUserRepository;
        _dataFilter = dataFilter;
        _identityUserAppService = identityUserAppService;
    }

    public async Task<TenantDto> Register(MyRegisterDto input)
    {
        var tenant = await _tenantManger.CreateAsync(input.TenantName);
        await _tenantRepository.InsertAsync(tenant);
        tenant.MapExtraPropertiesTo(tenant);
        //input.setTenantId(tenant.Id);
        var myTenant = new MyTenant()
          {
              TenantId = tenant.Id,
              Name = input.TenantName,  
              CompanyName = input.CompanyName,
              PhoneNumber = input.PhoneNumber,
              BusinessType = input.BusinessType,
              Address= input.Address,
              IsFree = input.IsFree,
              ExpirationDate = input.ExpirationDate,
              SubscriptionPlan = input.SubscriptionPlan,
          };
        await _myTenantRepository.InsertAsync(myTenant);
        await CurrentUnitOfWork.SaveChangesAsync();
        /*await _distributedEventBus.PublishAsync(
            new TenantCreatedEto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Properties =
                {
                    { "AdminEmail", input.EmailAddress },
                    { "AdminPassword", input.Password },
                }

            });*/
        
       
       // await _myUserRepository.InsertAsync(myUser);
      using (CurrentTenant.Change(tenant.Id, tenant.Name))
      {
            await _dataSeeder.SeedAsync(
                new DataSeedContext(tenant.Id)
                    .WithProperty("AdminEmail", input.EmailAddress)
                    .WithProperty("AdminPassword", input.Password)
            );
        }
      
    //  Console.WriteLine(CurrentTenant.GetId());
      MyUser myUser = new MyUser()
      {
          UserName = input.UserName,
          IdentityUserId = CurrentTenant.Id,
          SubscriptionType = SubscriptionType.SubscriptionAdmin,
          
      };
      return ObjectMapper.Map<Tenant,TenantDto>(tenant);
    }

    public async Task AddNewUser(MyIdentityUser input)
    {
        IdentityUserController userController = new IdentityUserController(_identityUserAppService);
        IdentityUserCreateDto tempuser = new IdentityUserCreateDto()
        {
            UserName = input.UserName,
            Email = input.Email,
            Password = input.Password,
            Name = input.Name,
            Surname = input.Surname,
            PhoneNumber = input.PhoneNumber,
            IsActive = input.IsActive,
            LockoutEnabled = input.LockoutEnabled,
            
        };
        var identityUser = await userController.CreateAsync(tempuser);
     
        MyUser myUser = new MyUser()
        {
            IdentityUserId = identityUser.Id,
            UserName = identityUser.UserName,
            Type = input.Type,
            CreatorId = input.CreatorId,
            SubscriptionType = input.SubscriptionType,
        };
       await _myUserRepository.InsertAsync(myUser);
       
    }
  
    public async  Task<SubscriptionType> Login(UserLoginInfo loginInfo)
    {
        AccountController controller = new AccountController(_signInManager, UserManager, _settingProvider, _identitySecurityLogManager, _options);
        Console.WriteLine(loginInfo.UserNameOrEmailAddress);
        var loginResult =   await controller.Login(loginInfo);
        SubscriptionType type = SubscriptionType.User;

        if (loginResult.Result == LoginResultType.Success)
        {
            /*IQueryable<MyUser> queryable = await _myUserRepository.GetQueryableAsync();
           
            //Create a query
            var query = from myUser in queryable
                where myUser.Id == CurrentUser.Id
                select myUser.SubscriptionType;
                    
            var currentUser = query.ToList().FirstOrDefault();
            Console.WriteLine(currentUser);*/
            var currentUser = await UserManager.FindByNameAsync(loginInfo.UserNameOrEmailAddress);
            
            if (currentUser == null)
            {
                Console.WriteLine("hello");
                currentUser = await UserManager.FindByEmailAsync(loginInfo.UserNameOrEmailAddress);
            }
            Console.WriteLine(currentUser.Id);
            Guid currentUserid = currentUser.Id;
            
            var user = await _myUserRepository.GetAsync(u => u.IdentityUserId == currentUser.Id);
            type = user.SubscriptionType;
            Console.WriteLine(user.SubscriptionType);

        }
        
        return type;

        // return user.GetProperty<SubscriptionType>("SubscriptionType");
    }

    public async Task RevokeUser(Guid id)
    {
        IdentityUserController userController = new IdentityUserController(_identityUserAppService);
        await userController.DeleteAsync(id);
        
       
      var user = await _myUserRepository.GetAsync(u => u.IdentityUserId == id);
 
    await _myUserRepository.DeleteAsync(user.Id);
    }
    public async Task EditUser(MyIdentityUser input)
    {
        IdentityUserController userController = new IdentityUserController(_identityUserAppService);
        IdentityUserUpdateDto tempuser = new IdentityUserUpdateDto()
        {
            UserName = input.UserName,
            Email = input.Email,
            Password = input.Password,
            Name = input.Name,
            Surname = input.Surname,
            PhoneNumber = input.PhoneNumber,
            IsActive = input.IsActive,
            LockoutEnabled = input.LockoutEnabled,
            
        };
        var identityUser = await userController.UpdateAsync(input.Id,tempuser);
     
        MyUser myUser = new MyUser()
        {
            IdentityUserId = identityUser.Id,
            UserName = identityUser.UserName,
            Type = input.Type,
            CreatorId = input.CreatorId,
            SubscriptionType = input.SubscriptionType,
        };
        await _myUserRepository.UpdateAsync(myUser);
       
    }

}
    