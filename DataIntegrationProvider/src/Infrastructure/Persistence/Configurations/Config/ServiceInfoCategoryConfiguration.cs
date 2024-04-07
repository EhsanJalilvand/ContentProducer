using DataIntegrationProvider.Domain.ConfigEntities;
using DataIntegrationProvider.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Share.Domain.Extensions;
using System;
using System.Linq;

namespace DataIntegrationProvider.Infrastructure.Persistence.Configurations.Config
{
    public class ServiceInfoCategoryConfiguration : IEntityTypeConfiguration<ServiceInfoCategory>
    {
        public void Configure(EntityTypeBuilder<ServiceInfoCategory> builder)
        {
            builder.ToTable("ServiceInfoCategory");

            builder.HasKey(a => a.ServiceInfoCategoryId);
            builder.Property(a => a.ServiceInfoCategoryId).IsRequired().ValueGeneratedNever();

            builder.Property(e => e.ServiceInfoCategoryName).IsRequired();
            builder.Property(e => e.APIKey).HasMaxLength(2000);
            builder.HasData(
        Enum.GetValues(typeof(ServiceInfoCategoryId))
            .Cast<ServiceInfoCategoryId>()
            .Select(e => new ServiceInfoCategory()
            {
                ServiceInfoCategoryId=e,
                ServiceInfoCategoryName = e.GetDisplayName(),
            })
        );
        }
    }
}
