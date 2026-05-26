using CodePulse.API.Data;
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
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateCategories([FromBody] CreateCategoryRequestDTO req)
        {
            //Map DTO to Domain
            var category = new Category
            {
                Name = req.Name,
                UrlHandle = req.UrlHandle
            };

            await categoryRepository.CreateCategory(category);

            //Map Domain to DTO
            var response = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle
            };
            return Ok(response);

        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories([FromQuery] string? query, [FromQuery] string? sortBy, [FromQuery] string? sortDirection, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var categories = await categoryRepository.GetAllCategories(query, sortBy, sortDirection, pageNumber, pageSize);

            //Map Domain to DTO
            var response = new List<CategoryDTO>();

            foreach (var category in categories)
            {
                response.Add(new CategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    UrlHandle = category.UrlHandle
                });
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] Guid id)
        {
            var existingCategory = await categoryRepository.GetCategoryById(id);
            if (existingCategory == null)
            {
                return NotFound();
            }
            //Map Domain to DTO
            var response = new CategoryDTO
            {
                Id = existingCategory.Id,
                Name = existingCategory.Name,
                UrlHandle = existingCategory.UrlHandle
            };
            return Ok(response);
        }

        [HttpGet]
        [Route("count")]
        public async Task<IActionResult> GetCategoryCount()
        {
            var count = await categoryRepository.GetCategoryCount();
            return Ok(count);
        }

        [HttpPut]
        [Route("{id:guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> EditCategory([FromRoute] Guid id, [FromBody] UpdateCategoryRequestDTO req)
        {
            //Map DTO to Domain
            var category = new Category
            {
                Id = id,
                Name = req.Name,
                UrlHandle = req.UrlHandle,
            };

            category = await categoryRepository.EditCategory(category);

            if (category == null)
            {
                return NotFound();
            }

            //Map Domain to DTO
            var response = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle,
            };
            return Ok(response);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
        {
            var category = await categoryRepository.DeleteCategory(id);

            if (category == null)
            {
                return NotFound();
            }

            //Map Domain to DTO
            var response = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle,
            };
            return Ok(response);
        }

    }
}
