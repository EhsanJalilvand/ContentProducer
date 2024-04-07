using DataIntegrationProvider.Domain.ConfigEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;

namespace DataIntegrationProvider.Infrastructure.Persistence.Configurations.Config
{
    public class CodalResponseDataConfiguration : IEntityTypeConfiguration<CodalResponseData>
    {
        public void Configure(EntityTypeBuilder<CodalResponseData> builder)
        {
            builder.ToTable("CodalResponseData");
            builder.HasKey(a => a.CodalResponseDataId);

            builder.Property(a=>a.CodalResponseDataId).IsRequired().ValueGeneratedOnAdd();
            builder.Property(a => a.Body).IsRequired();
            builder.Property(a => a.BodyHash).IsRequired();
            builder.Property(a => a.InsertTime).IsRequired();
            builder.Property(a => a.IsDeleted).IsRequired();
            builder.Property(a => a.ResponseKey);
            builder.Property(a => a.ParentRef);
            builder.HasOne(a=>a.Parent).WithMany(b=>b.Childs).HasForeignKey(c=>c.ParentRef).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

