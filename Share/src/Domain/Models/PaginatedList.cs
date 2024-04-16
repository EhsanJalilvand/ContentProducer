using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedDomain.Models
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            Items = items;
        }

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
   
            if (pageNumber <= 0)
                pageNumber = 1;
            if (pageSize <= 0)
                pageSize = 10;
            var count =  source.Count();
            var totalPageCount = (int)Math.Ceiling(count / (double)pageSize);
            if (pageNumber > totalPageCount) pageNumber = totalPageCount;
            if (pageNumber <= 0) pageNumber = 1;
            var items =  source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageNumber, pageSize);
        }
    }
}

