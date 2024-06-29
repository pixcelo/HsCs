using LAS.Domain.Models;

namespace LAS.Domain.Repositoriers
{
    public interface ITodoItemsRepository
    {
        /// <summary>
        /// TODOアイテムを取得する (SqlDataReaderを使用)
        /// </summary>
        /// <returns></returns>
        List<TodoItem> FindWithSqlDataReader();

        /// <summary>
        /// TODOアイテムを取得する (データテーブルを使用)
        /// </summary>
        /// <returns></returns>
        List<TodoItem> FindWithDataTable();

        /// <summary>
        /// TODOアイテムを追加する
        /// </summary>
        /// <param name="todoItems"></param>
        void Insert(TodoItem todoItems);

        /// <summary>
        /// TODOアイテムを更新する
        /// </summary>
        /// <param name="todoItems"></param>
        void Update(TodoItem todoItems);
    }
}