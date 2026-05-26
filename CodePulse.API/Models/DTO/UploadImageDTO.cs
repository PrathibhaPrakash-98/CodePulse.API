namespace CodePulse.API.Models.DTO
{
    public class UploadImageDTO
    {
        public IFormFile File { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
    }
}
