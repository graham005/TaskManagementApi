using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Data_Models;
using Task = TaskManagementApi.Data_Models.Task;

[Authorize]
[ApiController]
[Route("api/tasks")]
public class TaskController : ControllerBase
{
    
    private readonly AppDBContext _dbContext;
    private readonly ILogger<TaskController> _logger;

    public TaskController(AppDBContext dbContext,ILogger<TaskController> logger)
    { 
        _dbContext = dbContext;
        _logger = logger;
    }
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    public IActionResult GetTasks(
        [FromQuery] string status,
        [FromQuery] int? priority,
        [FromQuery] DateTime? dueDate,
        [FromQuery]int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var tasks = _dbContext.Tasks.AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            tasks = tasks.Where(t => t.Status == status);
        }

        if (priority.HasValue)
        {
            tasks = tasks.Where(t => t.Priority == priority.Value);
        }

        if (dueDate.HasValue)
        {
            tasks = tasks.Where(t => t.DueDate.Date == dueDate.Value.Date);
        }
        var skip = (page - 1) * pageSize;


        tasks = tasks.Skip(skip).Take(pageSize);

        var result = tasks.ToList();
        return Ok(result);
    }
    [Authorize]
    [HttpGet("{id}")]
    public IActionResult GetTask(int id)
    {
        var task = _dbContext.Tasks.Find(id);

        if (task == null)
        {
            return NotFound();
        }

        return Ok(task);
    }
    [Authorize]
    [HttpPost]
    public IActionResult CreateTask([FromBody] Task task)
    {
        if (task == null)
        {
            return BadRequest("Task object is null"); 
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); 
        }
        _dbContext.Tasks.Add(task);
        _dbContext.SaveChanges();

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    [Authorize]
    [HttpPut("{id}")]
    public IActionResult UpdateTask(int id, [FromBody] Task updatedTask)
    {
        if (updatedTask == null)
        {
            return BadRequest("Task object is null");
        }
        var existingTask = _dbContext.Tasks.Find(id);

        if (existingTask == null)
        {
            return NotFound();
        }

        existingTask.Title = updatedTask.Title;
        existingTask.Description = updatedTask.Description;
        existingTask.DueDate = updatedTask.DueDate;
        existingTask.Priority = updatedTask.Priority;
        existingTask.Status = updatedTask.Status;

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _dbContext.SaveChanges();

        return Ok(existingTask);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult DeleteTask(int id)
    {
        var task = _dbContext.Tasks.Find(id);

        if (task == null)
        {
            return NotFound();
        }

        _dbContext.Tasks.Remove(task);
        _dbContext.SaveChanges();

        return NoContent();
    }


}
