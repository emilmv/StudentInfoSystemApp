using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.InstructorDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Extensions;
using StudentInfoSystemApp.Application.Helpers.EntityHelpers;
using StudentInfoSystemApp.Application.Services.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Services.Implementations
{
    public class InstructorService : IInstructorService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;
        private readonly IPaginationService<Instructor> _paginationService;

        public InstructorService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper, IPaginationService<Instructor> paginationService)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
            _paginationService = paginationService;
        }

        public async Task<PaginationListDTO<InstructorReturnDTO>> GetAllAsync(int page = 1, string searchInput = "", int pageSize = 3)
        {
            //Extracting query to not overload requests
            var query = await InstructorHelper.CreateInstructorQueryAsync(_studentInfoSystemContext, searchInput);

            //Applying pagination
            var pagedInstructorDatas = await _paginationService.ApplyPaginationAsync(query, page, pageSize);

            var totalCount = await query.CountAsync();

            return new PaginationListDTO<InstructorReturnDTO>
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
                Objects = _mapper.Map<List<InstructorReturnDTO>>(pagedInstructorDatas)
            };
        }
        public async Task<InstructorReturnDTO> GetByIdAsync(int? id)
        {
            //Validating ID
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");

            //Getting the Instructor
            var responseInstructor = await InstructorHelper.GetResponseInstructorAsync(_studentInfoSystemContext, id.Value);

            //Returning DTO
            return _mapper.Map<InstructorReturnDTO>(responseInstructor);
        }
        public async Task<CreateResponseDTO<InstructorReturnDTO>> CreateAsync(InstructorCreateDTO instructorCreateDTO)
        {
            //Checking if Instructor with same mail address exists in the database
            await InstructorHelper.EnsureInstructorDoesNotExistAsync(_studentInfoSystemContext, instructorCreateDTO.Email);

            //Checking if Department exists in the database
            await InstructorHelper.EnsureDepartmentExistsAsync(_studentInfoSystemContext, instructorCreateDTO.DepartmentID);

            //Mapping DTO to an object
            Instructor instructor = _mapper.Map<Instructor>(instructorCreateDTO);

            //Adding the entity to the database
            await _studentInfoSystemContext.Instructors.AddAsync(instructor);
            await _studentInfoSystemContext.SaveChangesAsync();

            //ResponseDTO
            var responseInstructor = await InstructorHelper.GetResponseInstructorAsync(_studentInfoSystemContext, instructor.ID);

            //Returning the ID of the created entity
            return new CreateResponseDTO<InstructorReturnDTO>
            {
                Response = true,
                CreationDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<InstructorReturnDTO>(responseInstructor)
            };
        }
        public async Task<UpdateResponseDTO<InstructorReturnDTO>> UpdateAsync(int? id, InstructorUpdateDTO instructorUpdateDTO)
        {
            //Checking if ID from body is provided
            if (id is null) throw new CustomException(400, "Instructor ID", "Instructor ID must not be empty");

            //Finding relevant Instructor with ID
            var existingInstructor = await InstructorHelper.GetResponseInstructorAsync(_studentInfoSystemContext, id.Value);

            //Updating DepartmentID
            await InstructorHelper.UpdateDepartmentIdAsync(_studentInfoSystemContext, existingInstructor, instructorUpdateDTO);

            //Updating HireDate
            InstructorHelper.UpdateHireDate(existingInstructor, instructorUpdateDTO);

            //Updating Email
            await InstructorHelper.UpdateEmailAsync(_studentInfoSystemContext, existingInstructor, instructorUpdateDTO);

            //Updating Phone number
            await InstructorHelper.UpdatePhoneNumberAsync(_studentInfoSystemContext, existingInstructor, instructorUpdateDTO);

            //Updating DepartmentID if provided
            if (instructorUpdateDTO.DepartmentID.HasValue && instructorUpdateDTO.DepartmentID != 0)
                existingInstructor.DepartmentID = instructorUpdateDTO.DepartmentID.Value;

            //Updating First name
            existingInstructor.FirstName = string.IsNullOrEmpty(instructorUpdateDTO.FirstName)
                ? existingInstructor.FirstName
                : instructorUpdateDTO.FirstName;

            //Updating Last name
            existingInstructor.LastName = string.IsNullOrEmpty(instructorUpdateDTO.LastName)
                ? existingInstructor.LastName
                : instructorUpdateDTO.LastName;

            //Updating photo
            existingInstructor.Photo = instructorUpdateDTO.PhotoFile == null ? existingInstructor.Photo : instructorUpdateDTO.PhotoFile.Save(existingInstructor.FirstName.FirstCharToUpper(), existingInstructor.LastName.FirstCharToUpper(), Directory.GetCurrentDirectory(), "images");

            // Save changes
            _studentInfoSystemContext.Update(existingInstructor);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Return response DTO
            return new UpdateResponseDTO<InstructorReturnDTO>()
            {
                Response = true,
                UpdateDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<InstructorReturnDTO>(existingInstructor)
            };
        }
        public async Task<bool> DeleteAsync(int? id)
        {
            //Checking if requested ID is mentioned
            if (id is null) throw new CustomException(400, "ID", "ID cannot be empty");

            //Deleting the requested Instructor
            _studentInfoSystemContext.Instructors.Remove(await InstructorHelper.GetInstructorByIdAsync(_studentInfoSystemContext,id.Value));
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning true if delete successful
            return true;
        }
    }
}
