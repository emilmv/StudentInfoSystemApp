using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Helpers.EntityHelpers
{
    public static class DepartmentHelper
    {
        public static async Task<IQueryable<Department>> CreateDepartmentQueryAsync(StudentInfoSystemContext context, string searchInput)
        {
            var query = context.Departments
                .Include(d => d.Instructors)
                .AsQueryable();

            //Search logic
            if (!string.IsNullOrWhiteSpace(searchInput.Trim().ToLower()))
                query = query.Where(d => d.DepartmentName.Trim().ToLower().Contains(searchInput.Trim().ToLower()));

            var results = await query.ToListAsync();

            if (!results.Any())
                throw new CustomException(404, "Nothing found", "No records were found matching the search criteria.");
            return query;
        }

        public static async Task<Department> GetResponseDepartmentAsync(StudentInfoSystemContext context, int id)
        {
            var department = await context
                .Departments
                .Include(d => d.Instructors)
                .FirstOrDefaultAsync(d => d.ID == id);

            if (department is null) throw new CustomException(400, "ID", $"Department with ID of: '{id}' not found in the database.");

            return department;
        }

        public static async Task EnsureDepartmentDoesNotExistAsync(StudentInfoSystemContext context, string departmentName)
        {
            var existingDepartment = await context.Departments.SingleOrDefaultAsync(d => d.DepartmentName.Trim().ToLower() == departmentName.Trim().ToLower());
            if (existingDepartment != null)
                throw new CustomException(400, "DepartmentName", $"A Department with the name of: '{departmentName}' already exists in the database.");
        }

        public static async Task<Department> FindByIdAsync(StudentInfoSystemContext context, int id)
        {
            var existingDepartment = await context.Departments.SingleOrDefaultAsync(a => a.ID == id);
            if (existingDepartment == null) throw new CustomException(400, "ID", $"A department with ID of: '{id}' not found in the database");
            return existingDepartment;
        }

        public static async Task EnsureDepartmentIsNotDuplicateAsync(StudentInfoSystemContext context,int departmentId, string departmentName)
        {
            var duplicateDepartment = await context.Departments.FirstOrDefaultAsync(d => d.DepartmentName.Trim().ToLower().Equals(departmentName.Trim().ToLower()));
            if (duplicateDepartment != null && duplicateDepartment.ID != departmentId) throw new CustomException(400, "Department Name", $"A department with name of: '{departmentName}' already exists in the database");
        }
    }
}
