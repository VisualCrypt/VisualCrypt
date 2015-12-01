using Mono.Data.Sqlite;
using System.IO;

namespace VisualCrypt.temp
{
    /// <summary>
    /// Opens a SqliteConnection and never closes it.
    /// http://touchlabblog.tumblr.com/post/24474750219/single-sqlite-connection
    /// </summary>
    static class ConnectionManager
    {
        static SqliteConnection _connection;

        static ConnectionManager()
        {
            if (!File.Exists(GetDataBaseFilePath()))
                CreateDatabase();
        }

        public static SqliteConnection Connection
        {
            get
            {
                if (_connection != null)
                    return _connection;
                var connectionString = ("Data Source=" + GetDataBaseFilePath());
                _connection = new SqliteConnection(connectionString);
                _connection.Open();
                return _connection;
            }
        }

        static void CreateDatabase()
        {
                var ddlCommands = new[] {
                    "DROP TABLE IF EXISTS [Items];",
                    "CREATE TABLE [Items] (_id INTEGER PRIMARY KEY ASC, ShortFilename NTEXT, PathAndFileName NTEXT, ModifiedDate NTEXT);"
                };
                foreach (var ddlCommand in ddlCommands)
                {
                    using (var command = Connection.CreateCommand())
                    {
                        command.CommandText = ddlCommand;
                        var i = command.ExecuteNonQuery();
                    }
                }
            }
        
        static string GetDataBaseFilePath()
        {

            var sqliteFilename = "TaskDatabase.db3";
#if NETFX_CORE
				var path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sqliteFilename);
#else

#if SILVERLIGHT
				// Windows Phone expects a local path, not absolute
				var path = sqliteFilename;
#else

#if __ANDROID__
            // Just use whatever directory SpecialFolder.Personal returns
            string libraryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); ;
#else
				// we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
				// (they don't want non-user-generated data in Documents)
				string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
				string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
#endif
            var path = System.IO.Path.Combine(libraryPath, sqliteFilename);
#endif

#endif
            return path;
        }
    }
}