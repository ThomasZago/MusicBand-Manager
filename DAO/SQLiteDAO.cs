using System.IO;
using System.Data.SQLite;

namespace MusicBand_Manager.DAO
{
    public static class SQLiteDAO
    {
        private const string DatabaseFolderPath = "Database";
        private const string DatabaseFileName = "MusicManagerDB.sqlite";
        private const string MigrationScriptFileName = "MigrationScript.sql";
        private const string DatabasePath = DatabaseFolderPath + "\\" + DatabaseFileName;

        public static void InitializeDatabase()
        {
            if (!Directory.Exists(DatabaseFolderPath))
                Directory.CreateDirectory(DatabaseFolderPath);

            if (!File.Exists(DatabasePath))
            {
                SQLiteConnection.CreateFile(DatabasePath);

                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        string migrationScript = File.ReadAllText(MigrationScriptFileName);

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = migrationScript;
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }
            }
        }

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection($"Data Source={DatabasePath}");
        }
    }
}
