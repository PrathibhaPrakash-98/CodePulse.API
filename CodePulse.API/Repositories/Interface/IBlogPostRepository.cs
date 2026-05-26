using CodePulse.API.Models.Domain;

namespace CodePulse.API.Repositories.Interface
{
    public interface IBlogPostRepository
    {
        Task<BlogPost> CreateBlogPost(BlogPost blogPost);

        Task<IEnumerable<BlogPost>> GetAllBlogPost();

        Task<BlogPost?> GetBlogPostID(Guid id);

        Task<BlogPost?> GetBlogPostByURLHandle(string url);

        Task<BlogPost?> UpdateBlogPostbyId(BlogPost blogPost);

        Task<BlogPost?> DeleteBlogPostById(Guid id);
    }
}
