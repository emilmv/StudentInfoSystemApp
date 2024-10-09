﻿using StudentInfoSystemApp.Application.DTOs.CourseDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;

namespace StudentInfoSystemApp.Application.Services.Interfaces
{
    public interface ICourseService
    {
        Task<PaginationListDTO<CourseReturnDTO>> GetAllAsync(int page = 1, string searchInput = "");
        Task<CourseReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(CourseCreateDTO courseCreateDTO);
        Task<bool> DeleteAsync(int? id);
    }
}