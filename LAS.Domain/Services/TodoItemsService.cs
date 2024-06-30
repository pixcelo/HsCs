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

        public int Update(TodoItem todoItem)
        {
            return this.todoItemsRepository.Update(todoItem);
        }

        public void Upsert(TodoItem todoItem)
        {
            this.todoItemsRepository.Upsert(todoItem);
        }

        public void Delete(long id)
        {
            this.todoItemsRepository.Delete(id);
        }
    }
}
