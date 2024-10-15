using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.DepartmentDTOs;
using StudentInfoSystemApp.Application.DTOs.InstructorDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ProgramDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Services.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Services.Implementations
{
    public class ProgramService : IProgramService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;
        public ProgramService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
        }

        public async Task<PaginationListDTO<ProgramReturnDTO>> GetAllAsync(int page = 1, string searchInput = "")
        {
            //Extracting query to not overload requests
            var query = _studentInfoSystemContext
                .Programs
                .Include(p => p.Students)
                .Include(p => p.Courses)
                .AsQueryable();

            //Search logic
            if (!string.IsNullOrWhiteSpace(searchInput))
            {
                searchInput = searchInput.ToLower();

                query = query.Where(p =>
                    p.RequiredCredits.ToString() == searchInput ||
                    p.ProgramName != null && p.ProgramName.ToLower().Contains(searchInput) ||
                    p.Description != null && p.Description.ToLower().Contains(searchInput) ||
                    p.Students.Any(s =>
                    s.FirstName != null && s.FirstName.ToLower().Contains(searchInput) ||
                    s.LastName != null && s.LastName.ToLower().Contains(searchInput) ||
                        ((s.FirstName ?? string.Empty) + " " + (s.LastName ?? string.Empty)).ToLower().Contains(searchInput)
                    )
                );
            }

            var datas = await query
               .Skip((page - 1) * 2)
               .Take(2)
               .ToListAsync();

            var totalCount = await query.CountAsync();

            return new PaginationListDTO<ProgramReturnDTO>
            {
                TotalCount = totalCount,
                CurrentPage = page,
                Objects = _mapper.Map<List<ProgramReturnDTO>>(datas)
            };
        }
        public async Task<ProgramReturnDTO> GetByIdAsync(int? id)
        {
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");
            var program = await _studentInfoSystemContext
                .Programs
                .Include(p => p.Students)
                .Include(p => p.Courses)
                .FirstOrDefaultAsync(d => d.ID == id);
            if (program is null) throw new CustomException(400, "ID", $"Program with ID of:'{id}'not found in the database");
            return _mapper.Map<ProgramReturnDTO>(program);
        }
        public async Task<int> CreateAsync(ProgramCreateDTO programCreateDTO)
        {
            //Checking if program exists in the database
            var existingProgram = await _studentInfoSystemContext.Programs.SingleOrDefaultAsync(p => p.ProgramName.Trim().ToLower() == programCreateDTO.ProgramName.Trim().ToLower());
            if (existingProgram != null) throw new CustomException("Program Name", $"A Program with name of: '{programCreateDTO.ProgramName}' already exists in the database");

            //Mapping DTO to an object
            Program program = _mapper.Map<Program>(programCreateDTO);

            //Adding the entity to the database
            await _studentInfoSystemContext.Programs.AddAsync(program);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning the ID of the created entity
            return program.ID;
        }
        public async Task<bool> DeleteAsync(int? id)
        {
            //Checking if requested ID is mentioned
            if (id is null) throw new CustomException(400, "ID", "ID cannot be empty");

            //Checking if a Program with requested ID exists in the database
            var existingProgram = _studentInfoSystemContext.Programs.SingleOrDefault(a => a.ID == id);
            if (existingProgram == null) throw new CustomException(400, "ID", $"A program with ID of: '{id}' not found in the database");

            //Deleting the requested attendance
            _studentInfoSystemContext.Programs.Remove(existingProgram);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning true if delete successful
            return true;
        }
        public async Task<UpdateResponseDTO<ProgramReturnDTO>> UpdateAsync(int? id, ProgramUpdateDTO programUpdateDTO)
        {
            //extracting query
            var query = _studentInfoSystemContext
                .Programs
                .Include(p => p.Students)
                .Include(p => p.Courses)
                .AsQueryable();

            //Checking if ID from body is provided
            if (id is null) throw new CustomException(400, "Program ID", "Program ID must not be empty");

            //Finding relevant Program with ID
            var existingProgram = await query.FirstOrDefaultAsync(p => p.ID == id);
            if (existingProgram == null) throw new CustomException(400, "Program ID", $"A Program with ID of: '{id}' not found in the database");

            if (!string.IsNullOrEmpty(programUpdateDTO.ProgramName))
            {
                //avoiding duplicate ProgramName
                var duplicateProgram = await _studentInfoSystemContext.Programs.FirstOrDefaultAsync(d => d.ProgramName.Trim().ToLower().Equals(programUpdateDTO.ProgramName.Trim().ToLower()));
                if(duplicateProgram !=null&&duplicateProgram!=existingProgram) throw new CustomException(400, "Program Name", $"A Program with name of: '{programUpdateDTO.ProgramName}' already exists in the database.");
                existingProgram.ProgramName = programUpdateDTO.ProgramName.FirstCharToUpper();
            }

            //Updating
            existingProgram.RequiredCredits = programUpdateDTO.RequiredCredits==0||programUpdateDTO.RequiredCredits is null
            ? existingProgram.RequiredCredits
            : programUpdateDTO.RequiredCredits.GetValueOrDefault();

            existingProgram.Description = string.IsNullOrEmpty(programUpdateDTO.Description)
            ? existingProgram.Description
            : programUpdateDTO.Description.FirstCharToUpper();

            //Save changes
            _studentInfoSystemContext.Update(existingProgram);
            await _studentInfoSystemContext.SaveChangesAsync();

            return new UpdateResponseDTO<ProgramReturnDTO>()
            {
                Response = true,
                UpdateDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<ProgramReturnDTO>(existingProgram)
            };
        }
    }
}
