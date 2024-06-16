using Microsoft.AspNetCore.Http;

namespace PizzaSales.Models
{
    public class FileUploadModel
    {
        public IFormFile File { get; set; }
    }
}
