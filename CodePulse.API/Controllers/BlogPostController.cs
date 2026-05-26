using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Implementation;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostController : ControllerBase
    {
        private readonly IBlogPostRepository blogPostRepository;
        private readonly ICategoryRepository categoryRepository;

        public BlogPostController(IBlogPostRepository blogPostRepository, ICategoryRepository categoryRepository)
        {
            this.blogPostRepository = blogPostRepository;
            this.categoryRepository = categoryRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostRequestDTO request)
        {
            //Map DTO to Domain
            var blogpost = new BlogPost
            {
                Title = request.Title,
                ShortDescription = request.ShortDescription,
                Content = request.Content,
                FeaturedImageURL = request.FeaturedImageURL,
                URLHandle = request.URLHandle,
                PublishedDate = request.PublishedDate,
                Author = request.Author,
                IsVisible = request.IsVisible,
                Categories = new List<Category>()
            };

            foreach (var categoryguid in request.Categories)
            {
                var existingCategory = await categoryRepository.GetCategoryById(categoryguid);
                if (existingCategory != null)
                {
                    blogpost.Categories.Add(existingCategory);
                }
            }

            blogpost = await blogPostRepository.CreateBlogPost(blogpost);

            //Map Domain to DTO
            var response = new BlogPostDTO
            {
                Id = blogpost.Id,
                Author = blogpost.Author,
                Title = blogpost.Title,
                ShortDescription = blogpost.ShortDescription,
                Content = blogpost.Content,
                FeaturedImageURL = blogpost.FeaturedImageURL,
                URLHandle = blogpost.URLHandle,
                PublishedDate = blogpost.PublishedDate,
                IsVisible = blogpost.IsVisible,
                Categories = blogpost.Categories.Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    UrlHandle = c.UrlHandle,
                }).ToList()
            };
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBlogPost()
        {
            var blogPosts = await blogPostRepository.GetAllBlogPost();

            //Map Domain to DTO
            var response = new List<BlogPostDTO>();
            foreach (var blogPost in blogPosts)
            {
                response.Add(new BlogPostDTO
                {
                    Id = blogPost.Id,
                    Title = blogPost.Title,
                    ShortDescription = blogPost.ShortDescription,
                    Content = blogPost.Content,
                    FeaturedImageURL = blogPost.FeaturedImageURL,
                    URLHandle = blogPost.URLHandle,
                    PublishedDate = blogPost.PublishedDate,
                    Author = blogPost.Author,
                    IsVisible = blogPost.IsVisible,
                    Categories = blogPost.Categories.Select(c => new CategoryDTO
                    {
                        Id = c.Id,
                        Name = c.Name,
                        UrlHandle = c.UrlHandle,
                    }).ToList()
                });
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetBlogPostByID([FromRoute] Guid id)
        {
            var blogPost = await blogPostRepository.GetBlogPostID(id);
            if (blogPost == null)
            {
                return NotFound();
            }
            var response = new BlogPostDTO
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                ShortDescription = blogPost.ShortDescription,
                Content = blogPost.Content,
                FeaturedImageURL = blogPost.FeaturedImageURL,
                URLHandle = blogPost.URLHandle,
                PublishedDate = blogPost.PublishedDate,
                Author = blogPost.Author,
                IsVisible = blogPost.IsVisible,
                Categories = blogPost.Categories.Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    UrlHandle = c.UrlHandle,
                }).ToList()
            };
            return Ok(response);

        }

        [HttpGet]
        [Route("{url}")]
        public async Task<IActionResult> GetBlogPostByURLHandle([FromRoute] string url)
        {
            var blogPost = await blogPostRepository.GetBlogPostByURLHandle(url);
            if (blogPost == null)
            {
                return NotFound();
            }
            var response = new BlogPostDTO
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                ShortDescription = blogPost.ShortDescription,
                Content = blogPost.Content,
                FeaturedImageURL = blogPost.FeaturedImageURL,
                URLHandle = blogPost.URLHandle,
                PublishedDate = blogPost.PublishedDate,
                Author = blogPost.Author,
                IsVisible = blogPost.IsVisible,
                Categories = blogPost.Categories.Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    UrlHandle = c.UrlHandle,
                }).ToList()
            };
            return Ok(response);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> UpdateBlogPostById([FromRoute] Guid id, [FromBody] UpdateBlogPostRequestDTO req)
        {
            //Map DTO to Domain
            var blogPost = new BlogPost
            {
                Id = id,
                Title = req.Title,
                ShortDescription = req.ShortDescription,
                Content = req.Content,
                FeaturedImageURL = req.FeaturedImageURL,
                URLHandle = req.URLHandle,
                PublishedDate = req.PublishedDate,
                Author = req.Author,
                IsVisible = req.IsVisible,
                Categories = new List<Category>()
            };

            foreach (var category in req.Categories)
            {
                var existingCategory = await categoryRepository.GetCategoryById(category);
                if (existingCategory != null)
                {
                    blogPost.Categories.Add(existingCategory);
                }
            }

            var updatedBlogPost = await blogPostRepository.UpdateBlogPostbyId(blogPost);
            if (updatedBlogPost == null)
            {
                return NotFound();
            }
            //Map Domain to DTO

            var response = new BlogPostDTO
            {

                Id = updatedBlogPost.Id,
                Author = updatedBlogPost.Author,
                Title = updatedBlogPost.Title,
                ShortDescription = updatedBlogPost.ShortDescription,
                Content = updatedBlogPost.Content,
                FeaturedImageURL = updatedBlogPost.FeaturedImageURL,
                URLHandle = updatedBlogPost.URLHandle,
                PublishedDate = updatedBlogPost.PublishedDate,
                IsVisible = updatedBlogPost.IsVisible,
                Categories = updatedBlogPost.Categories.Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    UrlHandle = c.UrlHandle,
                }).ToList()
            };
            return Ok(response);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteBlogPostById([FromRoute] Guid id)
        {
            var blogPost = await blogPostRepository.DeleteBlogPostById(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            //Map Domain to DTO
            var response = new BlogPostDTO
            {
                Id = id,
                Title = blogPost.Title,
                ShortDescription = blogPost.ShortDescription,
                Content = blogPost.Content,
                FeaturedImageURL = blogPost.FeaturedImageURL,
                URLHandle = blogPost.URLHandle,
                PublishedDate = blogPost.PublishedDate,
                Author = blogPost.Author,
                IsVisible = blogPost.IsVisible,
            };
            await blogPostRepository.DeleteBlogPostById(id);
            return NoContent();

        }
    }
}
