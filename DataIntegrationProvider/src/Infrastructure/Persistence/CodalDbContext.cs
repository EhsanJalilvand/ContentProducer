using DataIntegrationProvider.Application.Common.Interfaces;
using DataIntegrationProvider.Domain.ConfigEntities;
using DataIntegrationProvider.Infrastructure.Persistence.Configurations.Config;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DataIntegrationProvider.Infrastructure.Persistence
{
    //ToDo: Find a way to change dt to utc before insert or update
    public class CodalDbContext : DbContext, ICodalDbContext
    {
        private static object _locker = new object();
        public CodalDbContext(
            DbContextOptions<CodalDbContext> options) : base(options)
        {
        }
        public DbSet<CodalResponseData> CodalResponseDatas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CodalResponseDataConfiguration());
            base.OnModelCreating(builder);
        }
        public async Task<bool> BulkInsert<TEntity>(List<TEntity> values) where TEntity : class
        {
            lock (_locker)
            {
                BulkConfig config = new BulkConfig() { BulkCopyTimeout = 30 };
                this.BulkInsert(values, config);
                return true;
            }
        }

    }
  

}
