using DataIntegrationProvider.Domain.ConfigEntities;
using DataIntegrationProvider.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;

namespace DataIntegrationProvider.Infrastructure.Persistence.Configurations.Config
{
    public class ServiceInfoConfiguration : IEntityTypeConfiguration<ServiceInfo>
    {
        public void Configure(EntityTypeBuilder<ServiceInfo> builder)
        {
            builder.ToTable("ServiceInfo");

            builder.HasKey(a => a.ServiceInfoId);
            builder.Property(a => a.ServiceInfoId).IsRequired().ValueGeneratedOnAdd();

            builder.Property(e => e.ServiceInfoCategoryId).IsRequired();
            builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            builder.Property(e => e.ServiceInfoTypeName).HasMaxLength(100).IsRequired();
            builder.Property(e => e.CanDelete).IsRequired().HasDefaultValue(false);
            builder.Property(e => e.RunInHoliday).IsRequired().HasDefaultValue(false);

            builder.Property(e => e.InstanceName).HasMaxLength(20);






            builder.Property(e => e.UrlPath).HasMaxLength(500).IsRequired();
            builder.Property(e => e.StartTime).HasColumnType(nameof(System.Data.SqlDbType.Time)).HasDefaultValueSql("getdate()").IsRequired();
            builder.Property(e => e.StopTime).HasColumnType(nameof(System.Data.SqlDbType.Time));
            builder.Property(e => e.Interval).HasDefaultValue(300).IsRequired();
            builder.Property(e => e.ResponseData);
            builder.Property(e => e.ResponseKey);
            builder.Property(e => e.KeyParameter);
            builder.Property(e => e.FilterParameter);
            builder.Property(e => e.ParentRef);
            builder.HasOne(a => a.Parent).WithMany(b => b.Childs).HasForeignKey(c => c.ParentRef).OnDelete(DeleteBehavior.Restrict);





            builder.HasOne(a => a.ServiceInfoCategory).WithMany(b => b.ServiceInfos).HasForeignKey(c => c.ServiceInfoCategoryId).OnDelete(DeleteBehavior.Restrict);


      
        }
    }
}
