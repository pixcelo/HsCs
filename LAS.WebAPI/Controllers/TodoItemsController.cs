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
    }
}
