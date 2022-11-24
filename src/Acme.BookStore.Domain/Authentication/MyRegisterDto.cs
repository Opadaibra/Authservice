using System;
using Volo.Abp.Account;

namespace Acme.BookStore.Authentication

{
    public class MyRegisterDto:RegisterDto
    {
        public string TenantName { get; set; }
        public bool IsTrial { get; set; }
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
        public string BusinessType { get; set; }
        public string Address { get; set; }
        public SubscriptionPlan SubscriptionPlan { get; set; }
        public bool IsFree { get; set; }
        public DateTime ExpirationDate { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
    }
}