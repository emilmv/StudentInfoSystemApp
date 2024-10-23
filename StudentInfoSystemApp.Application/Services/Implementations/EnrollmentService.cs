using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Helpers.EntityHelpers;
using StudentInfoSystemApp.Application.Services.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Services.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;
        private readonly IPaginationService<Enrollment> _paginationService;
        public EnrollmentService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper, IPaginationService<Enrollment> paginationService)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
            _paginationService = paginationService;
        }
        
        
        public async Task<PaginationListDTO<EnrollmentReturnDTO>> GetAllAsync(int page = 1, string searchInput = "", int pageSize = 3)
        {
            //Extracting query to not overload requests
            var query = await EnrollmentHelper.CreateEnrollmentQueryAsync(_studentInfoSystemContext, searchInput);

            //Applying pagination
            var pagedEnrollmentDatas = await _paginationService.ApplyPaginationAsync(query, page, pageSize);

            var totalCount = await query.CountAsync();

            return new PaginationListDTO<EnrollmentReturnDTO>
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
                Objects = _mapper.Map<List<EnrollmentReturnDTO>>(pagedEnrollmentDatas)
            };
        }
        public async Task<EnrollmentReturnDTO> GetByIdAsync(int? id)
        {
            //Validating ID
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");

            //Getting Enrollment
            var responseEnrollment = await EnrollmentHelper.GetResponseEnrollmentAsync(_studentInfoSystemContext, id.Value);

            //Returning DTO
            return _mapper.Map<EnrollmentReturnDTO>(responseEnrollment);
        }
        public async Task<CreateResponseDTO<EnrollmentReturnDTO>> CreateAsync(EnrollmentCreateDTO enrollmentCreateDTO)
        {
            //Checking if Enrollment is duplicate
            await EnrollmentHelper.EnsureEnrollmentDoesNotExistAsync(_studentInfoSystemContext, enrollmentCreateDTO.EnrollmentDate, enrollmentCreateDTO.StudentID);

            //Checking if Student exists
            await EnrollmentHelper.EnsureStudentExistsAsync(_studentInfoSystemContext, enrollmentCreateDTO.StudentID);

            //Checking if Course exists
            await EnrollmentHelper.EnsureCourseExistsAsync(_studentInfoSystemContext, enrollmentCreateDTO.CourseID);

            //Mapping DTO to an object
            Enrollment enrollment = _mapper.Map<Enrollment>(enrollmentCreateDTO);

            //Adding the entity to the database
            await _studentInfoSystemContext.Enrollments.AddAsync(enrollment);
            await _studentInfoSystemContext.SaveChangesAsync();

            //ResponseDTO
            var responseEnrollment = await EnrollmentHelper.GetResponseEnrollmentAsync(_studentInfoSystemContext, enrollment.ID);

            //Returning the ID of the created entity
            return new CreateResponseDTO<EnrollmentReturnDTO>()
            {
                Response = true,
                CreationDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<EnrollmentReturnDTO>(responseEnrollment)
            };
        }
        public async Task<bool> DeleteAsync(int? id)
        {
            //Checking if requested ID is mentioned
            if (id is null) throw new CustomException(400, "ID", "ID cannot be empty");

            //Checking if an Enrollment with requested ID exists in the database
            var existingEnrollment = _studentInfoSystemContext.Enrollments.SingleOrDefault(a => a.ID == id);
            if (existingEnrollment == null) throw new CustomException(400, "ID", $"An enrollment with ID of: '{id}' not found in the database");

            //Deleting the requested attendance
            _studentInfoSystemContext.Enrollments.Remove(await EnrollmentHelper.GetEnrollmentByIdAsync(_studentInfoSystemContext, id.Value));
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning true if delete successful
            return true;
        }
        public async Task<UpdateResponseDTO<EnrollmentReturnDTO>> UpdateAsync(int? id, EnrollmentUpdateDTO enrollmentUpdateDTO)
        {
            //Checking if ID from body is provided
            if (id is null) throw new CustomException(400, "Enrollment ID", "Enrollment ID must not be empty");

            //Finding relevant Enrollment with ID
            var existingEnrollment = await EnrollmentHelper.GetResponseEnrollmentAsync(_studentInfoSystemContext, id.Value);

            //Updating StudentID
            await EnrollmentHelper.UpdateStudentIdAsync(_studentInfoSystemContext, existingEnrollment, enrollmentUpdateDTO);

            //Updating CourseID
            await EnrollmentHelper.UpdateCourseIdAsync(_studentInfoSystemContext, existingEnrollment, enrollmentUpdateDTO);

            //Updating EnrollmentDate
            EnrollmentHelper.UpdateEnrollmentDate(existingEnrollment, enrollmentUpdateDTO);

            //Checking for duplication
            await EnrollmentHelper.CheckForDuplicationAsync(_studentInfoSystemContext, existingEnrollment.EnrollmentDate, existingEnrollment.StudentID, existingEnrollment.CourseID, existingEnrollment.ID);

            //Changing other fields validated from AbstractValidator
            existingEnrollment.Grade = enrollmentUpdateDTO.Grade ?? existingEnrollment.Grade;

            existingEnrollment.Semester = !string.IsNullOrWhiteSpace(enrollmentUpdateDTO.Semester) ? enrollmentUpdateDTO.Semester.FirstCharToUpper() : existingEnrollment.Semester;

            if (enrollmentUpdateDTO.StudentID.HasValue && enrollmentUpdateDTO.StudentID != 0)
                existingEnrollment.StudentID = enrollmentUpdateDTO.StudentID.Value;

            if (enrollmentUpdateDTO.CourseID.HasValue && enrollmentUpdateDTO.CourseID != 0)
                existingEnrollment.CourseID = enrollmentUpdateDTO.CourseID.Value;

            // Save changes
            _studentInfoSystemContext.Update(existingEnrollment);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Return response DTO
            return new UpdateResponseDTO<EnrollmentReturnDTO>()
            {
                Response = true,
                UpdateDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<EnrollmentReturnDTO>(existingEnrollment)
            };
        }
    }
}
