using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAS.Domain.Services
{
    public interface ITodoItemsService
    {
        /// <summary>
        /// TODOアイテムを取得する
        /// </summary>
        void FindTodoItems();
    }
}
