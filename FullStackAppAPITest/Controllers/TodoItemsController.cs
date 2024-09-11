using FullStackAppAPITest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FullStackAppAPITest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoItemsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public TodoItemsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _dbContext.TodoItems
                .Select(x => x)
                .ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _dbContext.TodoItems.FindAsync(id);

            if (todoItem == null)
                return NotFound();

            return todoItem;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem item) 
        {
            if(id != item.Id)
                return BadRequest();

            var todoItem = await _dbContext.TodoItems.FindAsync(id);

            if (todoItem == null)
                return NotFound();

            todoItem.Name = todoItem.Name;
            todoItem.IsComplite = todoItem.IsComplite;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id)) 
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem item) 
        {
            var todoItem = new TodoItem()
            {
                IsComplite = item.IsComplite,
                Name = item.Name,
            };
            _dbContext.TodoItems.Add(todoItem);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id) 
        {
            var todoItem = await _dbContext.TodoItems.FindAsync(id);
            if(todoItem == null)
                return NotFound();

            _dbContext.TodoItems.Remove(todoItem);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id) 
        {
            return _dbContext.TodoItems.Any(x => x.Id == id);
        }
    }
}
