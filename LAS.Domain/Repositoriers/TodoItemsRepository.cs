using LAS.Domain.Models;
using LAS.Infrastructure.SQLServer;
using System.Data;
using System.Data.SqlClient;

namespace LAS.Domain.Repositoriers
{
    public class TodoItemsRepository : ITodoItemsRepository
    {
        public List<TodoItems> FindWithDataTable()
        {
            var list = new List<TodoItems>();
            var query = "SELECT * FROM TodoItems";

            var dataTable = new DataTable();
            using (var connection = new SqlConnection(
                SQLServerHelper.GetConnectionStringWithWindowsAuth("(localdb)\\MSSQLLocalDB","LAS")))
            using (var adapter = new SqlDataAdapter(query, connection))
            {
                adapter.Fill(dataTable);

                list = dataTable.AsEnumerable().Select(row =>
                    new TodoItems
                    {
                        Id = row.Field<long>("Id"),
                        Title = row.Field<string>("Title"),
                        Description = row.Field<string>("Description"),
                        IsComplete = row.Field<bool>("IsComplete"),
                        DueDate = row.Field<DateTime?>("DueDate"),
                        CreatedAt = row.Field<DateTime>("CreatedAt"),
                        UpdatedAt = row.Field<DateTime?>("UpdatedAt"),
                        DeletedAt = row.Field<DateTime?>("DeletedAt")
                    }).ToList();
            }

            return list;
        }
    }
}
