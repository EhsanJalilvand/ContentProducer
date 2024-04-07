using DataIntegrationProvider.Application.Common.Interfaces;
using DataIntegrationProvider.Domain.ConfigEntities;
using DataIntegrationProvider.Infrastructure.Persistence.Configurations.Config;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DataIntegrationProvider.Infrastructure.Persistence
{
    //ToDo: Find a way to change dt to utc before insert or update
    public class ConfigDbContext : DbContext, IConfigDbContext
    {
        public ConfigDbContext(
            DbContextOptions<ConfigDbContext> options) : base(options)
        {
        }
        public DbSet<ServiceInfoCategory> ServiceInfoCategories { get; set; }
        public DbSet<ServiceInfo> ServiceInfoS { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ServiceInfoCategoryConfiguration());
            builder.ApplyConfiguration(new ServiceInfoConfiguration());

            base.OnModelCreating(builder);
        }

    }
  

}
