using LAS.Domain.Models;
using LAS.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace LAS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoItemsService todoItemsService;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="todoItemsService"></param>
        public TodoItemsController(ITodoItemsService todoItemsService)
        {
            this.todoItemsService = todoItemsService;
        }        

        // Get api/TodoItems
        [HttpGet]
        public ActionResult<IEnumerable<TodoItem>?> Get()
        {
            return this.todoItemsService?.FindTodoItems();            
        }

        // Post api/TodoItems
        [HttpPost]
        public IActionResult Post()
        {
            var todoItem = new TodoItem()
            {
                Title = "Sample",
                Description = "AAA",
                IsComplete = false,
                DueDate = Convert.ToDateTime("2024-06-30"),
                CreatedAt = DateTime.Now
            };

            this.todoItemsService.Insert(todoItem);

            return Ok("ok");
        }

        // Put api/TodoItems
        [HttpPut]
        public IActionResult Put()
        {
            var todoItem = new TodoItem()
            {
                Id = 1,
                Title = "Title",
                Description = "BBB",
                IsComplete = true,
                DueDate = Convert.ToDateTime("2024-07-01"),
                UpdatedAt = DateTime.Now
            };

            this.todoItemsService.Update(todoItem);

            return Ok("ok");
        }

        // Put api/TodoItems/upsert
        [HttpPut]
        [Route("upsert")]
        public IActionResult Upsert()
        {
            var todoItem = new TodoItem()
            {
                Id = 5,
                Title = "newTitle",
                Description = "CCC",
                IsComplete = false,
                DueDate = Convert.ToDateTime("2024-07-01"),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            this.todoItemsService.Upsert(todoItem);

            return Ok("ok");
        }

        // Delete api/TodoItems
        [HttpDelete]
        public IActionResult Delete()
        {
            this.todoItemsService.Delete(5);

            return Ok("ok");
        }
    }
}
