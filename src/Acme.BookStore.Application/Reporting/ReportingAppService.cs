using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Acme.BookStore.Authentication;
using Acme.BookStore.Invoices;
using Microsoft.Azure.Cosmos;
using Volo.Abp.Account;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.TenantManagement;
using Volo.Abp.Users;
using IdentityUser = Microsoft.AspNetCore.Identity.IdentityUser;

namespace Acme.BookStore.Reporting
{
    public class ReportingAppService : BookStoreAppService
    {
        private static readonly string EndpointUri = "";
        // This prop represent ConnectionString
        private static readonly string PrimaryKey = "";

        private readonly IRepository<MyUser, Guid> _myUserRepository;
        
        private CosmosClient cosmosClient;
        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;
        // The name of the database and container we will create
        private string _databaseId = "";
        private string _containerId = "";
        public ReportingAppService(IRepository<MyUser, Guid> myUserRepository)
        {
            _myUserRepository = myUserRepository;
        }

        public Task<List<MyUser>> ProxyList()
        {
            var proxyList = _myUserRepository.GetListAsync(u => u.UserType == UserType.Proxy);
            return proxyList;
        }

        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseId);
            Console.WriteLine("Created Database: {0}\n", this.database.Id);
        }
        public async Task TotalMessage()
        { 
                // Create a new instance of the Cosmos Client
                this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
                await this.CreateDatabaseAsync();
                this.container = await this.database.CreateContainerIfNotExistsAsync(_containerId,"/Message");
                var messages = container.GetItemQueryStreamIterator();
              
        }
    }
}
       