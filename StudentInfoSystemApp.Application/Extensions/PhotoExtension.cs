using Microsoft.AspNetCore.Http;

namespace StudentInfoSystemApp.Application.Extensions
{
    public static class PhotoExtension
    {
        public static string Save(this IFormFile photo,string firstName,string lastName, string root, string folder)
        {
            string newPhotoName = $"{firstName}_{lastName}" + Path.GetExtension(photo.FileName);
            string path = Path.Combine(root, "wwwroot", folder, newPhotoName);
            using FileStream fs = new FileStream(path, FileMode.Create);
            photo.CopyTo(fs);
            return newPhotoName;
        }
    }
}
