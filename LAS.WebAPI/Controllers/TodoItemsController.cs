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

        // Get api/TodoItems/get
        [HttpGet]
        [Route("get")]
        public IActionResult Get()
        {
            this.todoItemsService.FindTodoItems();

            return Ok("ok");
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
    }
}
