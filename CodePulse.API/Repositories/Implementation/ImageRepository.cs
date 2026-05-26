using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.API.Repositories.Implementation
{
    public class ImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ApplicationDbContext dbContext;

        public ImageRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = dbContext;
        }

        public async Task<BlogImage> UploadBlogImage(IFormFile file, BlogImage blogimage)
        {
            //upload to your storage solution here
            var localpath = Path.Combine(webHostEnvironment.ContentRootPath, "Images", $"{blogimage.FileName}{blogimage.FileExtension}");
            using var stream = new FileStream(localpath, FileMode.Create);
            await file.CopyToAsync(stream);

            // Set the URL for the image
            var httpRequest = httpContextAccessor.HttpContext?.Request;
            var urlpath = $"{httpRequest?.Scheme}://{httpRequest?.Host}/Images/{blogimage.FileName}{blogimage.FileExtension}";

            blogimage.Url = urlpath;


            await dbContext.BlogImages.AddAsync(blogimage);
            await dbContext.SaveChangesAsync();
            return blogimage;
        }

        public async Task<IEnumerable<BlogImage>> GetBlogImage()
        {
            return await dbContext.BlogImages.ToListAsync();
        }
    }
}
