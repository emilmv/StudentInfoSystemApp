﻿namespace StudentInfoSystemApp.Application.DTOs.ScheduleDTOs
{
    public class ScheduleListDTO
    {
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => (CurrentPage * 2) < TotalCount;
        public List<ScheduleReturnDTO>? Schedules { get; set; }
    }
}
