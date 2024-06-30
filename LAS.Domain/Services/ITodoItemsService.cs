using LAS.Domain.Models;

namespace LAS.Domain.Services
{
    public interface ITodoItemsService
    {
        /// <summary>
        /// TODOアイテムを取得する
        /// </summary>
        List<TodoItem> FindTodoItems();

        /// <summary>
        /// TODOアイテムを追加する
        /// </summary>
        /// <param name="todoItem"></param>
        void Insert(TodoItem todoItem);

        /// <summary>
        /// TODOアイテムを更新する
        /// </summary>
        /// <param name="todoItem"></param>
        int Update(TodoItem todoItem);

        /// <summary>
        /// TODOアイテムを追加または更新する
        /// </summary>
        /// <param name="todoItem"></param>
        void Upsert(TodoItem todoItem);

        /// <summary>
        /// TODOアイテムを削除する
        /// </summary>
        /// <param name="id"></param>
        void Delete(long id);
    }
}
