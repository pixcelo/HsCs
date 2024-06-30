using LAS.Domain.Models;
using LAS.Domain.Repositoriers;
using LAS.Domain.Services;
using Moq;

namespace LAS.Test.Services
{
    /// <summary>
    /// TODOアイテムサービステスト
    /// </summary>
    public class TodoItemsServiceTest
    {
        /// <summary>
        /// FindTodoItemsの正常系テスト
        /// </summary>
        [Fact]
        public void OkFindTodoItemsTest()
        {
            // Arrange
            var todoItemRepository = new Mock<ITodoItemsRepository>();
            todoItemRepository
                .Setup(x => x.FindWithDapper())
                .Returns(new List<TodoItem> { new TodoItem() });            
            var todoItemsService = new TodoItemsService(todoItemRepository.Object);            

            // Act
            var list = todoItemsService.FindTodoItems();

            // Assert
            Assert.True(list.Count == 1);
        }

        /// <summary>
        /// Insretの正常系テスト
        /// </summary>
        [Fact]
        public void OkInsertTest()
        {
            // Arrange
            var todoItemRepository = new Mock<ITodoItemsRepository>();
            todoItemRepository
                .Setup(x => x.InsertWithDapper(It.IsAny<TodoItem>()));
            var todoItemsService = new TodoItemsService(todoItemRepository.Object);

            // Act
            todoItemsService.Insert(new TodoItem());

            // Assert
            todoItemRepository.Verify(x => x.InsertWithDapper(It.IsAny<TodoItem>()), Times.Once);
        }
    }
}
