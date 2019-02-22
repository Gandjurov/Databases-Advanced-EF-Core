using System;
using System.Data.SqlClient;

namespace MinionNames
{
    public class StartUp
    {
        public static void Main()
        {
            int id = int.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string villainNameQuery = $"SELECT Name FROM Villains WHERE Id = @id";

                using (SqlCommand command = new SqlCommand(villainNameQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    string villainName = (string)command.ExecuteScalar();

                    if (villainName == null)
                    {
                        Console.WriteLine("No villain with ID 10 exists in the database.");
                    }

                    Console.WriteLine($"Villain: {villainName}");
                }

                string minionsQuery = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = @Id
                                ORDER BY m.Name";

                using (SqlCommand command = new SqlCommand(minionsQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int rowNumber = (int)reader[0];
                            string name = (string)reader[1];
                            int age = (int)reader[2];

                            Console.WriteLine($"{rowNumber}. {name} {age}");

                            if (reader.HasRows)
                            {
                                Console.WriteLine("(no minions)");
                            }
                        }
                    }
                }
                

            }
        }
    }
}
