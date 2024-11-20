using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers.Pruebas
{

    //Este controlador sirve como prueba para entender como funciona la clase TimespanJsonConverter.cs


    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private static readonly List<Task> Tasks = new();

        [HttpPost]
        public IActionResult CreateTask([FromBody] Task newTask)
        {
            newTask.Id = Guid.NewGuid();
            Tasks.Add(newTask);
            return CreatedAtAction(nameof(GetTask), new { id = newTask.Id }, newTask);
        }

        [HttpGet("{id}")]
        public IActionResult GetTask(Guid id)
        {
            var task = Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return NotFound();
            return Ok(task);
        }
    }

    public class Task
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public TimeSpan EstimatedTime { get; set; }
    }

}
