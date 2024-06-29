using LAS.Domain.Models;

namespace LAS.Domain.Services
{
    public interface ITodoItemsService
    {
        /// <summary>
        /// TODOアイテムを取得する
        /// </summary>
        void FindTodoItems();

        /// <summary>
        /// TODOアイテムを追加する
        /// </summary>
        /// <param name="todoItem"></param>
        void Insert(TodoItem todoItem);

        /// <summary>
        /// TODOアイテムを更新する
        /// </summary>
        /// <param name="todoItem"></param>
        void Update(TodoItem todoItem);
    }
}
