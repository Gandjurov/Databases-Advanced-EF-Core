using System;
using System.Data.SqlClient;

namespace RemoveVillain
{
    public class StartUp
    {
        public static void Main()
        {
            int id = int.Parse(Console.ReadLine());
            string villainName;

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string villainQuery = "SELECT Name FROM Villains WHERE Id = @villainId";

                using (SqlCommand command = new SqlCommand(villainQuery, connection))
                {
                    command.Parameters.AddWithValue("@villainId", id);
                    villainName = (string)command.ExecuteScalar();

                    if (villainName == null)
                    {
                        Console.WriteLine("No such villain was found");
                        return;
                    }
                }

                int affectedRows = DeleteMinionVillainById(connection, id);

                DeleteVillainById(connection, id);

                Console.WriteLine($"{villainName} was deleted.");
                Console.WriteLine($"{affectedRows} minions were released.");
            }
        }

        private static void DeleteVillainById(SqlConnection connection, int id)
        {
            string deleteVillainQuery = @"DELETE FROM Villains WHERE Id = @villainId";

            using (SqlCommand command = new SqlCommand(deleteVillainQuery, connection))
            {
                command.Parameters.AddWithValue("@villainId", id);
                command.ExecuteNonQuery();

            }
        }

        private static int DeleteMinionVillainById(SqlConnection connection, int id)
        {
            string deleteVillainQuery = @"DELETE FROM MinionsVillains WHERE VillainId = @villainId";

            using (SqlCommand command = new SqlCommand(deleteVillainQuery, connection))
            {
                command.Parameters.AddWithValue("@villainId", id);
                return command.ExecuteNonQuery();
                
            }
        }
    }
}
