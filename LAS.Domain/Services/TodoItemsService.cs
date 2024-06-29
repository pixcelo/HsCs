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

        /// <summary>
        /// TODOアイテムを取得する
        /// </summary>
        public void FindTodoItems()
        {
            var list = this.todoItemsRepository.FindWithSqlDataReader();
        }
    }
}
