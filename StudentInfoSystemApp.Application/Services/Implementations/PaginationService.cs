using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.Services.Interfaces;

namespace StudentInfoSystemApp.Application.Services.Implementations
{
    public class PaginationService<T> : IPaginationService<T>
    {
        public async Task<List<T>> ApplyPaginationAsync(IQueryable<T> query, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;

            return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }
    }
}
