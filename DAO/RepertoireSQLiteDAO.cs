using System;
using System.Collections.Generic;
using System.Data.SQLite;
using MusicBand_Manager.DAO;
using MusicBand_Manager.Model;

namespace MusicBand_Manager.DAO
{
    public class RepertoireSQLiteDAO
    {
        public RepertoireSQLiteDAO()
        {
            SQLiteDAO.InitializeDatabase();
        }

        public int AddRepertoireSong(RepertoireSong repertoireSong)
        {
            int insertedId = 0;

            using (var connection = SQLiteDAO.GetConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO Repertoire (Title, Style, Original_Composer)
                        VALUES (@Title, @Style, @OriginalComposer);
                        SELECT last_insert_rowid();
                    ";

                    command.Parameters.AddWithValue("@Title", repertoireSong.Title);
                    command.Parameters.AddWithValue("@Style", repertoireSong.Style);
                    command.Parameters.AddWithValue("@OriginalComposer", repertoireSong.OriginalComposer);

                    insertedId = Convert.ToInt32(command.ExecuteScalar());
                }
            }

            return insertedId;
        }

        public void UpdateRepertoireSong(RepertoireSong repertoireSong)
        {
            using (var connection = SQLiteDAO.GetConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        UPDATE Repertoire
                        SET Title = @Title, Style = @Style, Original_Composer = @OriginalComposer
                        WHERE Id = @Id;
                    ";

                    command.Parameters.AddWithValue("@Id", repertoireSong.Id);
                    command.Parameters.AddWithValue("@Title", repertoireSong.Title);
                    command.Parameters.AddWithValue("@Style", repertoireSong.Style);
                    command.Parameters.AddWithValue("@OriginalComposer", repertoireSong.OriginalComposer);

                    command.ExecuteNonQuery();
                }
            }
        }

        public List<RepertoireSong> GetAllRepertoireSongs()
        {
            var repertoireSongs = new List<RepertoireSong>();

            using (var connection = SQLiteDAO.GetConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT Id, Title, Style, Original_Composer
                        FROM Repertoire;
                    ";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var repertoireSong = new RepertoireSong
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Style = reader.GetString(2),
                                OriginalComposer = reader.GetString(3)
                            };

                            repertoireSongs.Add(repertoireSong);
                        }
                    }
                }
            }

            return repertoireSongs;
        }

        public void DeleteRepertoireSong(RepertoireSong repertoireSong)
        {
            using (var connection = SQLiteDAO.GetConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        DELETE FROM Repertoire
                        WHERE Id = @Id;
                    ";

                    command.Parameters.AddWithValue("@Id", repertoireSong.Id);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
