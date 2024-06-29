using System.Data.SqlClient;

namespace LAS.Infrastructure.SQLServer
{
    public class SQLServerHelper
    {
        /// <summary>
        /// 接続文字列を取得する
        /// </summary>
        /// <param name="server"></param>
        /// <param name="database"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string GetConnectionString(
            string server,
            string database,
            string user,
            string password)
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = server;
            builder.InitialCatalog = database;
            builder.UserID = user;
            builder.Password = password;

            return builder.ConnectionString;
        }

        /// <summary>
        /// Windows認証を使用して接続文字列を取得する
        /// </summary>
        /// <param name="server">サーバー名</param>
        /// <param name="database">データベース名</param>
        /// <returns>接続文字列</returns>
        public static string GetConnectionStringWithWindowsAuth(
            string server,
            string database)
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = server;
            builder.InitialCatalog = database;
            builder.IntegratedSecurity = true;

            return builder.ConnectionString;
        }
    }
}
