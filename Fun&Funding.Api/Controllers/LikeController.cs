using Fun_Funding.Application.IRepository;
using Fun_Funding.Domain.Entity.NoSqlEntities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Fun_Funding.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly ILikeRepository _likeRepository;

        public LikeController(ILikeRepository likeRepository)
        {
            _likeRepository = likeRepository;
        }

        // GET: api/like
        [HttpGet]
        public ActionResult<IEnumerable<Like>> Get()
        {
            var likes = _likeRepository.GetAll(); // Get all likes from the repository
            return Ok(likes);
        }

        // GET api/like/{id}
        [HttpGet("{id}")]
        public ActionResult<Like> Get(Guid id)
        {
            var like = _likeRepository.Get(l => l.Id == id); // Get like by ID
            if (like == null)
            {
                return NotFound(); // Return 404 if not found
            }
            return Ok(like);
        }

        // POST api/like
        [HttpPost]
        public ActionResult<Like> Post([FromBody] Like like)
        {
            if (like == null)
            {
                return BadRequest("Like object is null."); // Return 400 if the like object is null
            }
            like.Id = Guid.NewGuid(); // Generate a new ID for the like
            like.CreateDate = DateTime.UtcNow; // Set the creation date
            _likeRepository.Create(like); // Create a new like in the repository

            return CreatedAtAction(nameof(Get), new { id = like.Id }, like); // Return 201 with the created like
        }

        // PUT api/like/{id}
        [HttpPut("{id}")]
        public ActionResult Put(Guid id, [FromBody] Like like)
        {
            if (like == null || like.Id != id)
            {
                return BadRequest("Like object is null or ID mismatch."); // Return 400 if the object is null or ID doesn't match
            }

            var existingLike = _likeRepository.Get(l => l.Id == id); // Check if like exists
            if (existingLike == null)
            {
                return NotFound(); // Return 404 if not found
            }

            _likeRepository.Update(l => l.Id == id, like); // Update the like in the repository
            return NoContent(); // Return 204 No Content
        }

        // DELETE api/like/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var existingLike = _likeRepository.Get(l => l.Id == id); // Check if like exists
            if (existingLike == null)
            {
                return NotFound(); // Return 404 if not found
            }

            _likeRepository.Remove(l => l.Id == id); // Remove the like from the repository
            return NoContent(); // Return 204 No Content
        }
    }
}
