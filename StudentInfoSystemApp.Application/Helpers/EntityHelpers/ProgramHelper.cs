using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Helpers.EntityHelpers
{
    public static class ProgramHelper
    {
        public static async Task<Program> GetResponseProgramAsync(StudentInfoSystemContext context, int id)
        {
            var program = await context
                .Programs
                .Include(p => p.Students)
                .Include(p => p.Courses)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (program == null)
                throw new CustomException(400, "ID", $"A program with ID of: '{id}' not found in the database.");

            return program;
        }

        public static async Task<IQueryable<Program>> CreateProgramQueryAsync(StudentInfoSystemContext context, string searchInput)
        {
            var query = context
                .Programs
                .Include(p => p.Students)
                .Include(p => p.Courses)
                .AsQueryable();

            // If search input is provided, apply search filter
            if (!string.IsNullOrWhiteSpace(searchInput))
            {
                var lowerSearchInput = searchInput.ToLower();

                query = query.Where(p =>
                    p.RequiredCredits.ToString() == lowerSearchInput ||
                    (p.ProgramName != null && p.ProgramName.ToLower().Contains(lowerSearchInput)) ||
                    (p.Description != null && p.Description.ToLower().Contains(lowerSearchInput)) ||
                    (p.Students != null && p.Students.Any(s =>
                        (s.FirstName != null && s.FirstName.ToLower().Contains(lowerSearchInput)) ||
                        (s.LastName != null && s.LastName.ToLower().Contains(lowerSearchInput)) ||
                        (((s.FirstName ?? string.Empty) + " " + (s.LastName ?? string.Empty)).ToLower().Contains(lowerSearchInput))
                    ))
                );
            }
            var results = await query.ToListAsync();

            if (!results.Any())
                throw new CustomException(404, "Nothing found", "No records were found matching the search criteria.");

            return query;
        }

        public static async Task EnsureProgramDoesNotExistAsync(StudentInfoSystemContext context, string programName)
        {
            var existingProgram = await context.Programs
                .SingleOrDefaultAsync(p => p.ProgramName.Trim().ToLower() == programName.Trim().ToLower());

            if (existingProgram != null)
                throw new CustomException(400, "Program Name", $"A Program with name of: '{programName}' already exists in the database");
        }

        public static async Task<Program> GetProgramByIdAsync(StudentInfoSystemContext context, int id)
        {
            var program = await context.Programs
                .FirstOrDefaultAsync(p => p.ID == id);

            if (program == null)
                throw new CustomException(400, "Program ID", $"A Program with ID of: '{id}' not found in the database");

            return program;
        }

        public static async Task ValidateDuplicateProgramNameAsync(StudentInfoSystemContext context, Program existingProgram, string newProgramName)
        {
            if (string.IsNullOrWhiteSpace(newProgramName)) return;

            var duplicateProgram = await context.Programs
                .FirstOrDefaultAsync(d => d.ProgramName.Trim().ToLower() == newProgramName.Trim().ToLower());

            if (duplicateProgram != null && duplicateProgram != existingProgram)
                throw new CustomException(400, "Program Name", $"A Program with name of: '{newProgramName}' already exists in the database.");
        }
    }
}
