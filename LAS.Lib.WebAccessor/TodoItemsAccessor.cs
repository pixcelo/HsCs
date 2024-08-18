using LAS.Domain.Models;

namespace LAS.Lib.WebAccessor
{
    public sealed class TodoItemsAccessor
    {
        /// <summary>
        /// TODOアイテムを取得する
        /// </summary>
        /// <returns></returns>
        public static Task<IEnumerable<TodoItem>?> GetTodoItemsAsync()
        {
            return WebAccessor.SendRequestAsync<IEnumerable<TodoItem>?>("api/TodoItems", HttpMethod.Get);
        }

        public static Task<TodoItem?> CreateTodoItemAsync(TodoItem item)
        {
            return WebAccessor.SendRequestAsync<TodoItem>("api/TodoItems", HttpMethod.Post, item);
        }

        public static Task<TodoItem?> UpdateTodoItemAsync(TodoItem item)
        {
            return WebAccessor.SendRequestAsync<TodoItem>($"api/TodoItems/{item.Id}", HttpMethod.Put, item);
        }

        public static Task<bool> DeleteTodoItemAsync(int id)
        {
            return WebAccessor.SendRequestAsync<bool>($"api/TodoItems/{id}", HttpMethod.Delete);
        }
    }
}
