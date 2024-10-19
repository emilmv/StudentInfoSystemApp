using Microsoft.AspNetCore.Http;

namespace StudentInfoSystemApp.Application.Helpers.StudentHelpers
{
    public static class ValidationHelper
    {
        public static bool BeAtLeast16YearsOld(this DateTime dateOfBirth)
        {
            var minAllowedDate = DateTime.Now.AddYears(-16);
            return dateOfBirth <= minAllowedDate;
        }
        public static bool BeValidGender(this string gender)
        {
            return string.Equals(gender, "Male", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(gender, "Female", StringComparison.OrdinalIgnoreCase);
        }
        public static bool BeValidStatus(this string status)
        {
            return string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(status, "Inactive", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(status, "Graduated", StringComparison.OrdinalIgnoreCase);
        }
        public static bool BeValidFile(this IFormFile file)
        {
            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();
            return validExtensions.Contains(fileExtension);
        }
    }
}
