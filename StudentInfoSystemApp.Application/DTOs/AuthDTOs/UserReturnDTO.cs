namespace StudentInfoSystemApp.Application.DTOs.AuthDTOs
{
    public class UserReturnDTO
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
