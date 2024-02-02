using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using MusicBand_Manager.DAO;
using MusicBand_Manager.Model;

namespace MusicBand_Manager.DAO
{
    public class RepertoireSQLiteDAO
    {
        private MemberSQLiteDAO memberSQLiteDAO;
        public RepertoireSQLiteDAO()
        {
            SQLiteDAO.InitializeDatabase();
            memberSQLiteDAO = new MemberSQLiteDAO();
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
                        SET Title = @Title, Style = @Style, Original_Composer = @OriginalComposer, Lyrics = @Lyrics
                        WHERE Id = @Id;
                    ";

                    command.Parameters.AddWithValue("@Id", repertoireSong.Id);
                    command.Parameters.AddWithValue("@Title", repertoireSong.Title);
                    command.Parameters.AddWithValue("@Style", repertoireSong.Style);
                    command.Parameters.AddWithValue("@OriginalComposer", repertoireSong.OriginalComposer);
                    command.Parameters.AddWithValue("@Lyrics", repertoireSong.Lyrics);

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
                SELECT r.Id, r.Title, r.Style, r.Original_Composer, r.Lyrics,
                       ip.Id, ip.Instrument, ip.Progression, ip.Notes, ip.TutoLink, ip.Member_id
                FROM Repertoire r
                LEFT JOIN InstrumentProgression ip ON ip.Repertoire_Id = r.Id;
            ";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var repertoireSongId = reader.GetInt32(0);

                            // Check if the repertoire song is already in the list
                            var repertoireSong = repertoireSongs.FirstOrDefault(rs => rs.Id == repertoireSongId);

                            // If not, create a new repertoire song object
                            if (repertoireSong == null)
                            {
                                repertoireSong = new RepertoireSong
                                {
                                    Id = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Style = reader.GetString(2),
                                    OriginalComposer = reader.GetString(3),
                                    Lyrics = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    InstrumentProgressions = new List<InstrumentProgression>()
                                };

                                repertoireSongs.Add(repertoireSong);
                            }

                            if (!reader.IsDBNull(5))
                            {
                                // Create the instrument progression object
                                var instrumentProgression = new InstrumentProgression
                                {
                                    Id = reader.GetInt32(5),
                                    Instrument = reader.GetString(6),
                                    Progression = reader.IsDBNull(7) ? null : reader.GetFloat(7),
                                    Notes = reader.IsDBNull(8) ? null : reader.GetString(8),
                                    TutoLink = reader.IsDBNull(9) ? null : reader.GetString(9),
                                    Member = reader.IsDBNull(10) ? null : memberSQLiteDAO.GetMemberById(reader.GetInt32(10))
                                };

                                // Add the instrument progression to the repertoire song
                                repertoireSong.InstrumentProgressions.Add(instrumentProgression);
                            }
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

        public void AddInstrumentProgression(int repertoireSongId, InstrumentProgression instrumentProgression)
        {
            using (var connection = SQLiteDAO.GetConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                INSERT INTO InstrumentProgression (repertoire_id, instrument, progression, notes, tutolink, member_id)
                VALUES (@RepertoireId, @Instrument, @Progression, @Notes, @TutoLink, @MemberId);
            ";

                    command.Parameters.AddWithValue("@RepertoireId", repertoireSongId);
                    command.Parameters.AddWithValue("@Instrument", instrumentProgression.Instrument);
                    command.Parameters.AddWithValue("@Progression", instrumentProgression.Progression);
                    command.Parameters.AddWithValue("@Notes", instrumentProgression.Notes);
                    command.Parameters.AddWithValue("@TutoLink", instrumentProgression.TutoLink);
                    command.Parameters.AddWithValue("@MemberId", instrumentProgression.Member?.Id);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void EditInstrumentProgression(int repertoireSongId, InstrumentProgression instrumentProgression)
        {
            using (var connection = SQLiteDAO.GetConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                UPDATE InstrumentProgression
                SET instrument = @Instrument,
                    progression = @Progression,
                    notes = @Notes,
                    tutolink = @TutoLink,
                    member_id = @MemberId
                WHERE id = @InstrumentProgressionId
                      AND repertoire_id = @RepertoireId;
            ";

                    command.Parameters.AddWithValue("@InstrumentProgressionId", instrumentProgression.Id);
                    command.Parameters.AddWithValue("@RepertoireId", repertoireSongId);
                    command.Parameters.AddWithValue("@Instrument", instrumentProgression.Instrument);
                    command.Parameters.AddWithValue("@Progression", instrumentProgression.Progression);
                    command.Parameters.AddWithValue("@Notes", instrumentProgression.Notes);
                    command.Parameters.AddWithValue("@TutoLink", instrumentProgression.TutoLink);
                    command.Parameters.AddWithValue("@MemberId", instrumentProgression.Member?.Id);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteInstrumentProgression(InstrumentProgression instrumentProgression)
        {
            using (var connection = SQLiteDAO.GetConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                DELETE FROM InstrumentProgression
                WHERE id = @InstrumentProgressionId;
            ";

                    command.Parameters.AddWithValue("@InstrumentProgressionId", instrumentProgression.Id);

                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
