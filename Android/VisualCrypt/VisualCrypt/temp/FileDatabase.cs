using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using VisualCrypt.Applications.Models;
using System.Data;

namespace VisualCrypt.temp
{
	static class FileDatabase
    {
        static object _lockObject = new object();

        static FileReference FromReader(SqliteDataReader r)
        {
            var t = new FileReference();
            t.ID = Convert.ToInt32(r["_id"]);
            t.ShortFilename = r["ShortFilenamee"].ToString();
            t.PathAndFileName = r["PathAndFileName"].ToString();
            t.ModifiedDate = (r["ModifiedDate"]).ToString();
            return t;
        }

        public static List<FileReference> GetItems()
        {
            var tl = new List<FileReference>();

            lock (_lockObject)
            {
                using (var contents = ConnectionManager.Connection.CreateCommand())
                {
                    contents.CommandText = "SELECT [_id], [ShortFilename], [PathAndFileName], [ModifiedDate] from [Items]";
                    var r = contents.ExecuteReader();
                    while (r.Read())
                    {
                        tl.Add(FromReader(r));
                    }
                }
            }
            return tl;
        }

        public static FileReference GetItem(int id)
        {
            var t = new FileReference();
            lock (_lockObject)
            {
                
                using (var command = ConnectionManager.Connection.CreateCommand())
                {
                    command.CommandText = "SELECT [_id], [ShortFilename], [PathAndFileName], [ModifiedDate] from [Items] WHERE [_id] = ?";
                    command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = id });
                    var r = command.ExecuteReader();
                    while (r.Read())
                    {
                        t = FromReader(r);
                        break;
                    }
                }
               
            }
            return t;
        }

        public static int SaveItem(FileReference item)
        {
            int r;
            lock (_lockObject)
            {
                if (item.ID != 0)
                {
                    using (var command = ConnectionManager.Connection.CreateCommand())
                    {
                        command.CommandText = "UPDATE [Items] SET [ShortFilename] = ?, [PathAndFileName] = ?, [ModifiedDate] = ? WHERE [_id] = ?;";
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.ShortFilename });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.PathAndFileName });
                        command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = item.ModifiedDate });
                        command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = item.ID });
                        r = command.ExecuteNonQuery();
                    }
                    return r;
                }
                else
                {
                    using (var command = ConnectionManager.Connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO [Items] ([ShortFilename], [PathAndFileName], [ModifiedDate]) VALUES (? ,?, ?)";
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.ShortFilename });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.PathAndFileName });
                        command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = item.ModifiedDate });
                        r = command.ExecuteNonQuery();
                    }
                    return r;
                }

            }
        }

        public static int DeleteItem(int id)
        {
            lock (_lockObject)
            {
                int r;
                using (var command = ConnectionManager.Connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM [Items] WHERE [_id] = ?;";
                    command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = id });
                    r = command.ExecuteNonQuery();
                }
                return r;
            }
        }
    }
}