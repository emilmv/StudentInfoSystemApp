using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.ProgramDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Implementations
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

        public async Task<ProgramListDTO> GetAllAsync(int page = 1, string searchInput = "")
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
                    (p.RequiredCredits.ToString()==searchInput)||
                    (p.ProgramName != null && p.ProgramName.ToLower().Contains(searchInput)) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchInput)) ||
                    p.Students.Any(s =>
                    (s.FirstName != null && s.FirstName.ToLower().Contains(searchInput)) ||
                    (s.LastName != null && s.LastName.ToLower().Contains(searchInput)) ||
                        ((s.FirstName ?? string.Empty) + " " + (s.LastName ?? string.Empty)).ToLower().Contains(searchInput)
                    )
                );
            }

            var datas = await query
               .Skip((page - 1) * 2)
               .Take(2)
               .ToListAsync();

            var totalCount = await query.CountAsync();

            ProgramListDTO programListDTO = new();
            programListDTO.TotalCount = totalCount;
            programListDTO.CurrentPage = page;
            programListDTO.Programs = _mapper.Map<List<ProgramReturnDTO>>(datas);

            return programListDTO;
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
    }
}
