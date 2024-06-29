using LAS.Domain.Models;
using LAS.Domain.Repositoriers;

namespace LAS.Domain.Services
{
    public class TodoItemsService : ITodoItemsService
    {
        private readonly ITodoItemsRepository todoItemsRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="todoItemsRepository"></param>
        public TodoItemsService(ITodoItemsRepository todoItemsRepository)
        {
            this.todoItemsRepository = todoItemsRepository;
        }

        public void FindTodoItems()
        {
            var list = this.todoItemsRepository.FindWithSqlDataReader();
        }

        public void Insert(TodoItem todoItem)
        {
            this.todoItemsRepository.Insert(todoItem);
        }

        public void Update(TodoItem todoItem)
        {
            this.todoItemsRepository.Update(todoItem);
        }
    }
}
