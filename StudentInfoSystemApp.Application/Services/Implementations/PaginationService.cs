using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Services.Interfaces;

namespace StudentInfoSystemApp.Application.Services.Implementations
{
    public class PaginationService<T> : IPaginationService<T>
    {
        public async Task<List<T>> ApplyPaginationAsync(IQueryable<T> query, int page, int pageSize)
        {
            if (page < 1)
                throw new CustomException(400, "Page", "Page must be greater than 0");
            if (pageSize < 1)
                throw new CustomException(400, "Page size", "Page size must be greater than 0");

            return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }
    }
}
