using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = ("Administrador"))]
        public IActionResult Get()
        {
            string response = "Exitos";
            return Ok(response);
        }
    }
}
