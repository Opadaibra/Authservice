using Acme.BookStore.Authentication;
using Acme.BookStore.Invoices;
using Acme.BookStore.Reporting;
using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace Acme.BookStore.MongoDB;

[ConnectionStringName("Default")]
public class BookStoreMongoDbContext : AbpMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */
    public IMongoCollection<Invoice> Invoices => Collection<Invoice>();
    public IMongoCollection<Proxy> ProxyLists => Collection<Proxy>();
    public IMongoCollection<MyUser> MyUsers => Collection<MyUser>();
    public IMongoCollection<MyTenant> MyTenants => Collection<MyTenant>();
    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        //modelBuilder.Entity<YourEntity>(b =>
        //{
        //    //...
        //});
    }
}
