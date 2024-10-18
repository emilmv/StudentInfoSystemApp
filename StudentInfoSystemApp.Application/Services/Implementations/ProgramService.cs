using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        private readonly IPaginationService<Program> _paginationService;
        public ProgramService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper, IPaginationService<Program> paginationService)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
            _paginationService = paginationService;
        }

        public async Task<PaginationListDTO<ProgramReturnDTO>> GetAllAsync(int page = 1, string searchInput = "", int pageSize = 3)
        {
            //Extracting query to not overload requests
            var query =CreateProgramQuery(searchInput);

            //Applying Pagination logic
            var paginatedPrograms = await _paginationService.ApplyPaginationAsync(query, page, pageSize);

            var totalCount = await query.CountAsync();

            return new PaginationListDTO<ProgramReturnDTO>
            {
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize=pageSize,
                Objects = _mapper.Map<List<ProgramReturnDTO>>(paginatedPrograms)
            };
        }
        public async Task<ProgramReturnDTO> GetByIdAsync(int? id)
        {
            //Validating ID
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");

            //Getting the program from the database
            var program = await FindProgramByIdAsync(id.Value);

            //Not found exception
            if (program is null) throw new CustomException(400, "ID", $"Program with ID of:'{id}'not found in the database");
           
            //Returning DTO
            return _mapper.Map<ProgramReturnDTO>(program);
        }
        public async Task<CreateResponseDTO<ProgramReturnDTO>> CreateAsync(ProgramCreateDTO programCreateDTO)
        {
            //Checking if program exists in the database
            await ValidateProgramDoesNotExistAsync(programCreateDTO.ProgramName);

            //Mapping DTO to an object
            Program program = _mapper.Map<Program>(programCreateDTO);

            //Adding the entity to the database
            await _studentInfoSystemContext.Programs.AddAsync(program);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning ResponseDTO
            return new CreateResponseDTO<ProgramReturnDTO>()
            {
                Response = true,
                CreationDate=DateTime.Now.ToShortDateString(),
                Objects=_mapper.Map<ProgramReturnDTO>(program)
            };
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
            //Checking if ID from body is provided
            if (id is null) throw new CustomException(400, "Program ID", "Program ID must not be empty");

            var existingProgram = await GetProgramByIdAsync(id.Value);

            await ValidateDuplicateProgramNameAsync(existingProgram, programUpdateDTO.ProgramName);

            //Updating fields
            if (!string.IsNullOrEmpty(programUpdateDTO.ProgramName))
                existingProgram.ProgramName = programUpdateDTO.ProgramName.FirstCharToUpper(); //Program Name if provided

            existingProgram.RequiredCredits = programUpdateDTO.RequiredCredits==0||programUpdateDTO.RequiredCredits is null
            ? existingProgram.RequiredCredits
            : programUpdateDTO.RequiredCredits.GetValueOrDefault(); //Required Credits if changed

            existingProgram.Description = string.IsNullOrEmpty(programUpdateDTO.Description)
            ? existingProgram.Description
            : programUpdateDTO.Description.FirstCharToUpper(); //Description if provided

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


        //Private methods
        private IQueryable<Program> CreateProgramQuery(string searchInput)
        {
            var query = _studentInfoSystemContext
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

            return query;
        }
        private async Task<Program?>FindProgramByIdAsync(int id)
        {
            return await _studentInfoSystemContext
                .Programs
                .Include(p => p.Students)
                .Include(p => p.Courses)
                .FirstOrDefaultAsync(d => d.ID == id);
        }
        private async Task ValidateProgramDoesNotExistAsync(string programName)
        {
            var existingProgram = await _studentInfoSystemContext.Programs
                .SingleOrDefaultAsync(p => p.ProgramName.Trim().ToLower() == programName.Trim().ToLower());

            if (existingProgram != null)
            {
                throw new CustomException(400, "Program Name", $"A Program with name of: '{programName}' already exists in the database");
            }
        }
        private async Task<Program> GetProgramByIdAsync(int id)
        {
            var program = await _studentInfoSystemContext.Programs
                .Include(p => p.Students)
                .Include(p => p.Courses)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (program == null)
                throw new CustomException(400, "Program ID", $"A Program with ID of: '{id}' not found in the database");

            return program;
        }
        private async Task ValidateDuplicateProgramNameAsync(Program existingProgram, string newProgramName)
        {
            if (string.IsNullOrWhiteSpace(newProgramName)) return;

            var duplicateProgram = await _studentInfoSystemContext.Programs
                .FirstOrDefaultAsync(d => d.ProgramName.Trim().ToLower() == newProgramName.Trim().ToLower());

            if (duplicateProgram != null && duplicateProgram != existingProgram)
                throw new CustomException(400, "Program Name", $"A Program with name of: '{newProgramName}' already exists in the database.");
        }
    }
}
