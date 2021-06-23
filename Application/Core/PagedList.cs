using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Core
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int count, int PageNumber, int pageSize)
        {
            CurrentPage = PageNumber;
            //Count is the number of items inside the list
            //Page size is the number of items that will be displayed per page
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items);
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int PageNumber, 
            int pageSize)
        {
            //Count items before pagination
            var count = await source.CountAsync();
            var items = await source.Skip((PageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, PageNumber, pageSize);
        }
    }
}