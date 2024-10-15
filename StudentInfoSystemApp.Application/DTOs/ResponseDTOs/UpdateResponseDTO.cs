namespace StudentInfoSystemApp.Application.DTOs.ResponseDTOs
{
    public class UpdateResponseDTO<T>
    {
        public bool Response { get; set; }
        public string? UpdateDate {  get; set; }
        public T? Objects { get; set; }
    }
}
