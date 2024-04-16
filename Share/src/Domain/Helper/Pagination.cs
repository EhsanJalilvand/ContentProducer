using System;
using System.Linq;
using System.Threading.Tasks;
using SharedDomain.Models;

namespace System
{
    public static class Pagination
    {
        public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, int pageNumber, int pageSize)
        => PaginatedList<TDestination>.CreateAsync(queryable, pageNumber, pageSize);
    }
}

