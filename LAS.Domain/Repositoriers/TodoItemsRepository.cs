using LAS.Domain.Models;
using LAS.Infrastructure.SQLServer;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Serialization;

namespace LAS.Domain.Repositoriers
{
    public class TodoItemsRepository : ITodoItemsRepository
    {
        public List<TodoItem> FindWithSqlDataReader()
        {
            var list = new List<TodoItem>();
            var query = "SELECT * FROM TodoItems";

            using (var connection = new SqlConnection(
                SQLServerHelper.GetConnectionStringWithWindowsAuth("(localdb)\\MSSQLLocalDB","LAS")))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new TodoItem
                        {
                            // インデックスで指定
                            //Id = reader.GetInt64(0),
                            //Title = reader.GetString(1),
                            //Description = reader.GetString(2),
                            //IsComplete = reader.GetBoolean(3),
                            //DueDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                            //CreatedAt = reader.GetDateTime(5),
                            //UpdatedAt = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                            //DeletedAt = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7)

                            // 項目名で指定
                            Id = reader.GetInt64("Id"),
                            Title = reader.GetString("Title"),
                            Description = reader.GetString("Description"),
                            IsComplete = reader.GetBoolean("IsComplete"),
                            DueDate = reader.IsDBNull(reader.GetOrdinal("DueDate")) ? (DateTime?)null : reader.GetDateTime("DueDate"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime("UpdatedAt"),
                            DeletedAt = reader.IsDBNull(reader.GetOrdinal("DeletedAt")) ? (DateTime?)null : reader.GetDateTime("DeletedAt")
                        });
                    }
                }
            }

            return list;
        }

        public List<TodoItem> FindWithDataTable()
        {
            var list = new List<TodoItem>();
            var query = "SELECT * FROM TodoItems";

            var dataTable = new DataTable();
            using (var connection = new SqlConnection(
                SQLServerHelper.GetConnectionStringWithWindowsAuth("(localdb)\\MSSQLLocalDB","LAS")))
            using (var adapter = new SqlDataAdapter(query, connection))
            {
                adapter.Fill(dataTable);

                list = dataTable.AsEnumerable().Select(row =>
                    new TodoItem
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

        public void Insert(TodoItem todoItem)
        {
            var query = @"
INSERT INTO TodoItems (
    Title,
    Description,
    IsComplete,
    DueDate,
    CreatedAt
)
VALUES (
    @Title,
    @Description,
    @IsComplete,
    @DueDate,
    @CreatedAt
)";

            using (var connection = new SqlConnection(
                SQLServerHelper.GetConnectionStringWithWindowsAuth("(localdb)\\MSSQLLocalDB","LAS")))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Title", todoItem.Title);
                command.Parameters.AddWithValue("@Description", todoItem.Description);
                command.Parameters.AddWithValue("@IsComplete", todoItem.IsComplete);
                command.Parameters.AddWithValue("@DueDate", todoItem.DueDate);
                command.Parameters.AddWithValue("@CreatedAt", todoItem.CreatedAt);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }


        public int Update(TodoItem todoItem)
        {
            var query = @"
UPDATE TodoItems
SET
Title = @Title,
Description = @Description,
IsComplete = @IsComplete,
DueDate = @DueDate,
UpdatedAt = @UpdatedAt
WHERE Id = @Id
";

            using (var connection = new SqlConnection(
                SQLServerHelper.GetConnectionStringWithWindowsAuth("(localdb)\\MSSQLLocalDB","LAS")))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", todoItem.Id);
                command.Parameters.AddWithValue("@Title", todoItem.Title);
                command.Parameters.AddWithValue("@Description", todoItem.Description);
                command.Parameters.AddWithValue("@IsComplete", todoItem.IsComplete);
                command.Parameters.AddWithValue("@DueDate", todoItem.DueDate);
                command.Parameters.AddWithValue("@UpdatedAt", todoItem.UpdatedAt);

                connection.Open();
                var updateCount = command.ExecuteNonQuery();

                return updateCount;
            }
        }

        public void Upsert(TodoItem todoItem)
        {
            var updateCount = this.Update(todoItem);   
            if (updateCount == 0)
            {
                this.Insert(todoItem);
            }            
        }

//        public void Upsert(TodoItem todoItems)
//        {
//            var query = @"
//MERGE INTO TodoItems as target
//USING (values (
//@Id,
//@Title,
//@Description,
//@IsComplete,
//@DueDate,
//@CreatedAt,
//@UpdatedAt,
//@DeletedAt
//)) as source (
//Id,
//Title,
//Description,
//IsComplete,
//DueDate,
//CreatedAt,
//UpdatedAt,
//DeletedAt
//)
//ON target.Id = source.Id
//WHEN Matched THEN
//    Update Set
//        Title = source.Title,
//        Description = source.Description,
//        IsComplete = source.IsComplete,
//        DueDate = source.DueDate,
//        UpdatedAt = source.UpdatedAt,
//        DeletedAt = source.DeletedAt
//WHEN Not Matched THEN
//    Insert (
//        Title,
//        Description,
//        IsComplete,
//        DueDate,
//        CreatedAt,
//        UpdatedAt,
//        DeletedAt
//    )
//    Values (
//        source.Title,
//        source.Description,
//        source.IsComplete,
//        source.DueDate,
//        source.CreatedAt,
//        source.UpdatedAt,
//        source.DeletedAt
//    );
//";

//            using (var connection = new SqlConnection(
//                SQLServerHelper.GetConnectionStringWithWindowsAuth("(localdb)\\MSSQLLocalDB","LAS")))
//            using (var command = new SqlCommand(query, connection))
//            {
//                command.Parameters.AddWithValue("@Id", todoItems.Id);
//                command.Parameters.AddWithValue("@Title", todoItems.Title);
//                command.Parameters.AddWithValue("@Description", todoItems.Description);
//                command.Parameters.AddWithValue("@IsComplete", todoItems.IsComplete);
//                command.Parameters.AddWithValue("@DueDate", todoItems.DueDate);
//                command.Parameters.AddWithValue("@CreatedAt", todoItems.CreatedAt);
//                command.Parameters.AddWithValue("@UpdatedAt", todoItems.UpdatedAt);
//                command.Parameters.AddWithValue("@DeletedAt", todoItems.DeletedAt);

//                connection.Open();
//                command.ExecuteNonQuery();
//            }
//        }

        public void Delete(long id)
        {
            var query = @"
DELETE FROM TodoItems
WHERE Id = @Id
";

            using (var connection = new SqlConnection(
                SQLServerHelper.GetConnectionStringWithWindowsAuth("(localdb)\\MSSQLLocalDB","LAS")))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

    }
}
