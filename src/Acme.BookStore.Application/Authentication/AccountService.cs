
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Account;
using Volo.Abp.Account.Emailing;
using Volo.Abp.Data;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectExtending;
using Volo.Abp.TenantManagement;

using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Settings;
using AccountController = Volo.Abp.Account.Web.Areas.Account.Controllers.AccountController;
using UserLoginInfo = Volo.Abp.Account.Web.Areas.Account.Controllers.Models.UserLoginInfo;
using Volo.Abp.Account.Web.Areas.Account.Controllers.Models;
namespace Acme.BookStore.Authentication
{
    public class AccountService : AccountAppService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ITenantManager _tenantManger;
        private readonly IdentityUserManager _identityUserManager;
        private readonly IDataSeeder _dataSeeder;
        private readonly IDistributedEventBus _distributedEventBus;
        private readonly AbpSignInManager _signInManager;
        private readonly ISettingProvider _settingProvider;
        private readonly IdentitySecurityLogManager _identitySecurityLogManager;
        public AccountService(ITenantRepository tenantRepository,
            IdentityUserManager userManager, IIdentityRoleRepository roleRepository,
            IAccountEmailer accountEmailer, IdentitySecurityLogManager identitySecurityLogManager,
            IOptions<IdentityOptions> identityOptions, ITenantManager tenantManger, IDataSeeder dataSeeder,
            IDistributedEventBus distributedEventBus, AbpSignInManager signInManager, ISettingProvider settingProvider
            ) : base(userManager, roleRepository, accountEmailer,
            identitySecurityLogManager, identityOptions)
        {
            _tenantRepository = tenantRepository;
            _tenantManger = tenantManger;
            _identityUserManager = userManager;
            _dataSeeder = dataSeeder;
            _distributedEventBus = distributedEventBus;
            _signInManager = signInManager;
            _settingProvider = settingProvider;
            _identitySecurityLogManager = identitySecurityLogManager;
           
        }

        public async Task<TenantDto> RegisterNewUser(MyRegisterDto input)
        {
            var tenant = await _tenantManger.CreateAsync(input.TenantName);
            input.MapExtraPropertiesTo(tenant);
            tenant.SetProperty("CompanyName", input.CompanyName);
            tenant.SetProperty("BusinessType", input.BusinessType);
            tenant.SetProperty("PhoneNumber", input.PhoneNumber);
            tenant.SetProperty("IsFree", input.IsFree);
            tenant.SetProperty("ExpirationDate", input.ExpirationDate);
            tenant.SetProperty("SubscriptionPlan", input.SubscriptionPlan);
            tenant.SetProperty("Address", input.Address);
            await _tenantRepository.InsertAsync(tenant);
            await CurrentUnitOfWork.SaveChangesAsync();
            /*await _distributedEventBus.PublishAsync(
                new TenantCreatedEto
                {
                    Id = tenant.Id,
                    Name = tenant.Name,
                    Properties =
                    {
                        { "AdminEmail", input.EmailAddress },
                        { "AdminPassword", input.Password }
                    }
                });*/
            using (CurrentTenant.Change(tenant.Id, tenant.Name))
            {
                //TODO: Handle database creation?
                // TODO: Seeder might be triggered via event handler.
                await _dataSeeder.SeedAsync(
                    new DataSeedContext(tenant.Id)
                        .WithProperty("AdminEmail", input.EmailAddress)
                        .WithProperty("AdminPassword", input.Password)

                );

            }
            // tenant.SetProperty("TenantName", input.TenantName);
          
          return  ObjectMapper.Map<Tenant, TenantDto>(tenant);

        }
        public async  Task<SubscriptionType> Login(UserLoginInfo loginInfo)
        {
            AccountController controller = new AccountController(_signInManager, _identityUserManager, _settingProvider,
                _identitySecurityLogManager, IdentityOptions);
            Console.WriteLine(loginInfo.UserNameOrEmailAddress);
            var loginResult = await controller.Login(loginInfo);
            Console.WriteLine(loginResult.Result);

//            Console.WriteLine(email);
            /*if (loginResult.Result == LoginResultType.Success)
            {
                var user = await GetUserByEmailAsync(loginInfo.UserNameOrEmailAddress);
                return user.GetProperty<SubscriptionType>("SubscriptionType");
            }*/

            return (SubscriptionType.User);

            // return user.GetProperty<SubscriptionType>("SubscriptionType");
        }

        public async Task logout()
        {
            AccountController controller = new AccountController(_signInManager, _identityUserManager, _settingProvider,
                _identitySecurityLogManager, IdentityOptions);
           await controller.Logout();

        }
    
        public Task<PaymentMethode> PaymentMethode(PaymentMethode paymentMethode)
        {
            return Task.FromResult(paymentMethode);
        }

        public Task PayBills(Guid id, string cardNumber, PaymentMethode paymentMethode)
        {
            Validator validator = new Validator();
            if (paymentMethode == Authentication.PaymentMethode.CreditCard)
            {
                if (validator.ValidateCardNumber(cardNumber))
                {
                        
                }
                else
                {
                    throw  new Exception("This card is not valid");
                }
            }

            if (paymentMethode == Authentication.PaymentMethode.DebiCard)
            {

                if (validator.ValidateCardNumber(cardNumber))
                {
                    
                }
                else
                {
                    throw  new Exception("This card is not valid");
                }
            }

            if (paymentMethode == Authentication.PaymentMethode.PayBal)
            {
                if (validator.ValidateCardNumber(cardNumber))
                {
                    
                }
                else
                {
                    throw  new Exception("This card is not valid");
                }

            }

            return Task.CompletedTask;
        }
        public Task<string> GetCardNumber(string cardNumber)
        {
            return Task.FromResult(cardNumber);
        }
        //public Task<>
        public async Task DownloadInvoices(Guid id)
        {
     
          //jsonobj<==Find invoice by ID
         //convert json to xml or excel file 
         //download invoice
        }
        public async Task ViewInvoices(Guid id)
        {
          
           
            //return invoice
        }

        public async Task EnableTwoFactor(Guid id)
        {
            var user = await UserManager.GetByIdAsync(id);
            if (user != null)
            {
                await UserManager.SetTwoFactorEnabledAsync(user,true);
            }

        }

        public async Task DisableTwoFactor(Guid id)
        {
            var user = await UserManager.GetByIdAsync(id);
            if (user != null)
            {
                await UserManager.SetTwoFactorEnabledAsync(user,false);
            }
        }
    }

}
