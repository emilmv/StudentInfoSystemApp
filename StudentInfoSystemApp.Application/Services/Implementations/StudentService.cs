using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;
using StudentInfoSystemApp.Application.DTOs.StudentDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Extensions;
using StudentInfoSystemApp.Application.Helpers.EntityHelpers;
using StudentInfoSystemApp.Application.Services.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;
using System.Globalization;

namespace StudentInfoSystemApp.Application.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;
        private readonly IPaginationService<Student> _paginationService;
        public StudentService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper, IPaginationService<Student> paginationService)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
            _paginationService = paginationService;
        }

        public async Task<PaginationListDTO<StudentReturnDTO>> GetAllAsync(int page = 1, string searchInput = "", int pageSize = 3)
        {
            //Creating the query based on searchInput
            var query = await StudentHelper.CreateStudentQueryAsync(_studentInfoSystemContext,searchInput);

            //Applying Pagination logic
            var paginatedStudents = await _paginationService.ApplyPaginationAsync(query, page, pageSize);

            //Total count of students based on searchInput
            var totalCount = await query.CountAsync();

            //Returning PaginationDTO
            return new PaginationListDTO<StudentReturnDTO>
            {
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize=pageSize,
                Objects = _mapper.Map<List<StudentReturnDTO>>(paginatedStudents)
            };
        }
        public async Task<StudentReturnDTO> GetByIdAsync(int? id)
        {
            //Validating if ID from body is provided
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");

            //Finding student with relevant ID
            var responseStudent = await StudentHelper.GetResponseStudentAsync(_studentInfoSystemContext,id.Value);

            //Returning DTO
            return _mapper.Map<StudentReturnDTO>(responseStudent);
        }
        public async Task<CreateResponseDTO<StudentReturnDTO>> CreateAsync(StudentCreateDTO studentCreateDTO)
        {
            //Validating Program
            await StudentHelper.ValidateProgramAsync(_studentInfoSystemContext,studentCreateDTO.ProgramID);

            //Checking if Student is already registered by the same email
            await StudentHelper.ValidateAvailableEmailAsync(_studentInfoSystemContext, studentCreateDTO.Email);

            //Phone number
            await StudentHelper.CheckAvailablePhoneNumberAsync(_studentInfoSystemContext,studentCreateDTO.PhoneNumber);

            //Mapping DTO to an object
            Student student = _mapper.Map<Student>(studentCreateDTO);

            //Adding the entity to the database
            await _studentInfoSystemContext.Students.AddAsync(student);
            await _studentInfoSystemContext.SaveChangesAsync();

            //responseDTO
            var responseStudent=await StudentHelper.GetResponseStudentAsync(_studentInfoSystemContext,student.ID);

            //Returning ResponseDTO of the created entity
            return new CreateResponseDTO<StudentReturnDTO>()
            {
                Response = true,
                CreationDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<StudentReturnDTO>(responseStudent)
            };
        }
        public async Task<bool> DeleteAsync(int? id)
        {
            //Checking if requested ID is mentioned
            if (id is null) throw new CustomException(400, "ID", "ID cannot be empty");

            //Checking if a Student with requested ID exists in the database

            //Deleting the requested attendance
            _studentInfoSystemContext.Students.Remove(await StudentHelper.GetExistingStudentAsync(_studentInfoSystemContext,id.Value));
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning true if delete successful
            return true;
        }
        public async Task<UpdateResponseDTO<StudentReturnDTO>> UpdateAsync(int? id, StudentUpdateDTO studentUpdateDTO)
        {
            // Check if ID from body is provided
            if (id is null) throw new CustomException(400, "Student ID", "Student ID must not be empty");

            // Finding the Student
            var existingStudent = await StudentHelper.GetResponseStudentAsync(_studentInfoSystemContext,id.Value);

            //Updating fields that require logic
            await StudentHelper.ValidateProgramUpdateAsync(_studentInfoSystemContext,existingStudent, studentUpdateDTO);
            StudentHelper.UpdateDateOfBirth(existingStudent, studentUpdateDTO);
            StudentHelper.UpdateEnrollmentDate(existingStudent, studentUpdateDTO);
            await StudentHelper.ValidateEmailUpdateAsync(_studentInfoSystemContext,existingStudent, studentUpdateDTO);
            await StudentHelper.ValidatePhoneNumberUpdateAsync(_studentInfoSystemContext,existingStudent, studentUpdateDTO);

            //Updating the other fields that don't require logic
            existingStudent.FirstName = string.IsNullOrEmpty(studentUpdateDTO.FirstName)
                ? existingStudent.FirstName
                : studentUpdateDTO.FirstName.FirstCharToUpper();

            existingStudent.LastName = string.IsNullOrEmpty(studentUpdateDTO.LastName)
                ? existingStudent.LastName
                : studentUpdateDTO.LastName.FirstCharToUpper();

            existingStudent.Gender = string.IsNullOrEmpty(studentUpdateDTO.Gender)
                ? existingStudent.Gender
                : studentUpdateDTO.Gender.FirstCharToUpper();

            existingStudent.Address = string.IsNullOrEmpty(studentUpdateDTO.Address)
                ? existingStudent.Address
                : studentUpdateDTO.Address.FirstCharToUpper();

            existingStudent.Status = string.IsNullOrEmpty(studentUpdateDTO.Status)
                ? existingStudent.Status
                : studentUpdateDTO.Status.FirstCharToUpper();

            if (studentUpdateDTO.PhotoFile != null)
                existingStudent.Photo = studentUpdateDTO.PhotoFile.Save(existingStudent.FirstName.FirstCharToUpper(), existingStudent.LastName.FirstCharToUpper(), Directory.GetCurrentDirectory(), "images");

            // Save changes
            _studentInfoSystemContext.Update(existingStudent);
            await _studentInfoSystemContext.SaveChangesAsync();

            // Return response DTO
            return new UpdateResponseDTO<StudentReturnDTO>()
            {
                Response = true,
                UpdateDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<StudentReturnDTO>(existingStudent)
            };
        }
    }
}
