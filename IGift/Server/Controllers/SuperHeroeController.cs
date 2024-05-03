using IGift.Server.Data;
using IGift.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        [HttpGet]
        public async Task<ActionResult<List<SuperHeroe>>> GetAll()
        {
            var heroes = new List<SuperHeroe>()
            {
                new SuperHeroe() { Id = 1,Name="Spiderman",FirstName="Peter",LastName="Parker",Place="New York City"}
            };
            return Ok(heroes);
        }
    }
}
