namespace StudentInfoSystemApp.Application.DTOs.ResponseDTOs
{
    public class CreateResponseDTO<T>
    {
        public bool Response { get; set; }
        public string CreationDate { get; set; }
        public T Objects { get; set; }
    }
}
