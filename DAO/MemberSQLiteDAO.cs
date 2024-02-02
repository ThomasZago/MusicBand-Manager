using MusicBand_Manager.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBand_Manager.DAO
{
    public class MemberSQLiteDAO
    {
        private static Dictionary<int, Member> memberCache { get; set; }

        public MemberSQLiteDAO()
        {
            SQLiteDAO.InitializeDatabase();
            memberCache = new Dictionary<int, Member>();
        }

        public int AddMember(Member member)
        {
            int insertedId = 0;

            using (var connection = SQLiteDAO.GetConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO Member (FullName, Arrival_Date)
                        VALUES (@FullName, @ArrivalDate);
                        SELECT last_insert_rowid();
                    ";

                    command.Parameters.AddWithValue("@FullName", member.FullName);
                    command.Parameters.AddWithValue("@ArrivalDate", member.ArrivalDate);

                    insertedId = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            member.Id = insertedId;
            return SearchOrUpdateCacheMember(member).Id;
        }

        public void UpdateMember(Member member)
        {
            using (var connection = SQLiteDAO.GetConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        UPDATE Member
                        SET FullName = @FullName, Arrival_Date = @ArrivalDate, Departure_Date = @DepartureDate
                        WHERE Id = @Id;
                    ";

                    command.Parameters.AddWithValue("@Id", member.Id);
                    command.Parameters.AddWithValue("@FullName", member.FullName);
                    command.Parameters.AddWithValue("@ArrivalDate", member.ArrivalDate);
                    command.Parameters.AddWithValue("@DepartureDate", member.DepartureDate);

                    command.ExecuteNonQuery();
                    SearchOrUpdateCacheMember(member);
                }
            }
        }

        private Member SearchOrUpdateCacheMember(Member member)
        {
            if (memberCache.ContainsKey(member.Id)) 
            {
                memberCache[member.Id].FullName = member.FullName;
                memberCache[member.Id].ArrivalDate = member.ArrivalDate;
                memberCache[member.Id].DepartureDate = member.DepartureDate;
            }
            else
            {
                memberCache.Add(member.Id, member);
            }
            return memberCache[member.Id];

        }

        public List<Member> GetAllMembers()
        {
            var members = new List<Member>();

            using (var connection = SQLiteDAO.GetConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT Id, FullName, Arrival_Date, Departure_Date
                        FROM Member;
                    ";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var member = new Member
                            {
                                Id = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                ArrivalDate = reader.GetDateTime(2),
                                DepartureDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3)
                            };

                            members.Add(SearchOrUpdateCacheMember(member));
                        }
                    }
                }
            }

            return members;
        }

        public bool DeleteMember(Member member)
        {
            using (var connection = SQLiteDAO.GetConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    // Check if the member ID is used in the InstrumentProgression or EquipmentInventory table
                    command.CommandText = @"
                SELECT COUNT(*) FROM InstrumentProgression WHERE member_id = @Id;
                SELECT COUNT(*) FROM EquipmentInventory WHERE member_id = @Id;
            ";

                    command.Parameters.AddWithValue("@Id", member.Id);

                    int instrumentProgressionCount = 0;
                    int equipmentInventoryCount = 0;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            instrumentProgressionCount = reader.GetInt32(0);
                        }

                        reader.NextResult();

                        if (reader.Read())
                        {
                            equipmentInventoryCount = reader.GetInt32(0);
                        }
                    }

                    // Check if the member ID is used in any other table
                    if (instrumentProgressionCount > 0 || equipmentInventoryCount > 0)
                    {
                        // Member ID is used as a foreign key, cannot delete the member
                        return false;
                    }

                    // Delete the member from the Member table
                    command.CommandText = @"
                DELETE FROM Member
                WHERE Id = @Id;
            ";

                    command.ExecuteNonQuery();
                    memberCache.Remove(member.Id);
                }
            }

            return true; // Member deleted successfully
        }

        public Member GetMemberById(int memberId)
        {
            if (memberCache.TryGetValue(memberId, out var cachedMember))
            {
                return cachedMember;
            }

            Member member = null;

            using (var connection = SQLiteDAO.GetConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                SELECT Id, FullName, Arrival_Date, Departure_Date
                FROM Member
                WHERE Id = @Id;
            ";

                    command.Parameters.AddWithValue("@Id", memberId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            member = new Member
                            {
                                Id = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                ArrivalDate = reader.GetDateTime(2),
                                DepartureDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3)
                            };
                        }
                    }
                }
            }

            return SearchOrUpdateCacheMember(member);
        }
    }
}
