using Fun_Funding.Domain.Entity;
using Fun_Funding.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controller
{
    [Route("api/catagories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly MyDbContext dbContext;

        public CategoryController(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpPost]
        public IActionResult Create()
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                Name = "FigureWibuLo",

            };

            dbContext.Add(category);
            dbContext.SaveChanges();

            return Ok(category);
        }
        [HttpDelete]
        public IActionResult Delete()
        {
            var nCate = dbContext.Category.FirstOrDefault(x => x.Name.Equals("FigureWibuLo"));

            if (nCate == null)
            {
                return NotFound("Category not found");
            }

            // Trigger the soft delete by calling Remove
            dbContext.Remove(nCate); // This will be intercepted by the SoftDeleteInterceptor
            dbContext.SaveChanges(); // Interceptor will change the state to Modified and set IsDeleted

            return Ok(nCate);
        }


    }
}
