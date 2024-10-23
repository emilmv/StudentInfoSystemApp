using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ProgramDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Helpers.EntityHelpers;
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
            var query =await ProgramHelper.CreateProgramQueryAsync(_studentInfoSystemContext,searchInput);

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
            var responseProgram = await ProgramHelper.GetResponseProgramAsync(_studentInfoSystemContext,id.Value);

            //Returning DTO
            return _mapper.Map<ProgramReturnDTO>(responseProgram);
        }
        public async Task<CreateResponseDTO<ProgramReturnDTO>> CreateAsync(ProgramCreateDTO programCreateDTO)
        {
            //Checking if program exists in the database
            await ProgramHelper.EnsureProgramDoesNotExistAsync(_studentInfoSystemContext,programCreateDTO.ProgramName);

            //Mapping DTO to an object
            Program program = _mapper.Map<Program>(programCreateDTO);

            //Adding the entity to the database
            await _studentInfoSystemContext.Programs.AddAsync(program);
            await _studentInfoSystemContext.SaveChangesAsync();

            //ResponseDTO
            var responseProgram = await ProgramHelper.GetResponseProgramAsync(_studentInfoSystemContext, program.ID);
            //Returning ResponseDTO
            return new CreateResponseDTO<ProgramReturnDTO>()
            {
                Response = true,
                CreationDate=DateTime.Now.ToShortDateString(),
                Objects=_mapper.Map<ProgramReturnDTO>(responseProgram)
            };
        }
        public async Task<bool> DeleteAsync(int? id)
        {
            //Checking if requested ID is mentioned
            if (id is null) throw new CustomException(400, "ID", "ID cannot be empty");
            
            //Deleting the requested attendance
            _studentInfoSystemContext.Programs.Remove(await ProgramHelper.GetProgramByIdAsync(_studentInfoSystemContext,id.Value));
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning true if delete successful
            return true;
        }
        public async Task<UpdateResponseDTO<ProgramReturnDTO>> UpdateAsync(int? id, ProgramUpdateDTO programUpdateDTO)
        {
            //Checking if ID from body is provided
            if (id is null) throw new CustomException(400, "Program ID", "Program ID must not be empty");

            var existingProgram = await ProgramHelper.GetResponseProgramAsync(_studentInfoSystemContext,id.Value);

            await ProgramHelper.ValidateDuplicateProgramNameAsync(_studentInfoSystemContext,existingProgram, programUpdateDTO.ProgramName);

            //Updating fields
            if (!string.IsNullOrEmpty(programUpdateDTO.ProgramName))
                existingProgram.ProgramName = programUpdateDTO.ProgramName.FirstCharToUpper(); //Program Name if provided

            if(programUpdateDTO.RequiredCredits.HasValue&&programUpdateDTO.RequiredCredits!=0)
                existingProgram.RequiredCredits = programUpdateDTO.RequiredCredits.Value;

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
    }
}
