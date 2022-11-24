using System;
using Volo.Abp.Identity;

namespace Acme.BookStore.Authentication;

public class MyIdentityUser:IdentityUserDto
{
    public string Name { get; set; }
    public UserType UserType { get; set; }
    public string Password { get; set; }
    public bool Statues { get; set; }
    public DateTime LastSeen { get; set; }
    public string LastMessage { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; }
}