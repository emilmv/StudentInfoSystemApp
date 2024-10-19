using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;

namespace StudentInfoSystemApp.Application.Services.Interfaces
{
    public interface IPaginationService<T>
    {
        Task<List<T>> ApplyPaginationAsync(IQueryable<T> query, int page, int pageSize);
    }
}
