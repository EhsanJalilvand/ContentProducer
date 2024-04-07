using DataIntegrationProvider.Domain.ConfigEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataIntegrationProvider.Application.Common.Interfaces
{
    public interface ICodalDbContext : IDisposable
    {
        public DbSet<CodalResponseData> CodalResponseDatas { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        void Dispose();
    }
}
