using System;
using System.Data.SqlClient;

namespace IncreaseAgeStoredProcedure
{
    public class StartUp
    {
        public static void Main()
        {
            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                int id = int.Parse(Console.ReadLine());

                using (connection)
                {
                    var command = new SqlCommand("EXEC usp_GetOlder @Id", connection);
                    command.Parameters.AddWithValue("@Id", id);

                    command.ExecuteNonQuery();

                    command = new SqlCommand("SELECT * FROM Minions WHERE Id = @Id", connection);
                    command.Parameters.AddWithValue("@Id", id);

                    var reader = command.ExecuteReader();

                    using (reader)
                    {
                        reader.Read();

                        Console.WriteLine($"{(string)reader["Name"]} - {(int)reader["Age"]} years old");
                    }
                }
            }
        }
    }
}
