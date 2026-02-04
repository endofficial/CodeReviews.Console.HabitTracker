using Microsoft.Data.Sqlite;
using System.Collections.Specialized;

namespace Habit_tracker
{
    internal class Program
    {
        static string connectionString = @"Data Source=Habit-tracker.db";
        static void Main(string[] args)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Habit (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Date TEXT,
                Quantity INTEGER,
                HabitId INTEGER);

                CREATE TABLE IF NOT EXISTS Register_Habit (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name_Habit TEXT,
                Unit_Of_Measurement TEXT)";

                tableCmd.ExecuteNonQuery();
                connection.Close();

                #region To register some habits and some records for testing purposes

                (string Col1, string Col2)[] arrayDb = new (string, string)[]
                    { ("DRINKING COFFEE", "NUMBER OF CUPS"),
                      ("READING BOOKS", "NUMBER OF PAGES"),
                      ("RUNNING", "KILOMETERS"),
                      ("MEDITATION", "MINUTES"),
                      ("WATER INTAKE", "LITERS") };

                connection.Open();

                // check if the Register_habit table is empty
                string checkQuery = "SELECT COUNT(*) FROM Register_Habit";
                long count = 0;

                using (var checkCommand = new SqliteCommand(checkQuery, connection))
                {
                    // ExecuteScalar can be null or return DBNull, so we need to handle that
                    object? result = checkCommand.ExecuteScalar();
                    count = (result != null && result != DBNull.Value) ? Convert.ToInt64(result) : 0;
                }

                if (count == 0)
                {
                    foreach (var dbArray in arrayDb)
                    {
                        string query =
                        $"INSERT INTO Register_Habit (Name_Habit, Unit_Of_Measurement) VALUES(@val1, @val2)";

                        using (var command = new SqliteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@val1", dbArray.Col1); // Name_Habit
                            command.Parameters.AddWithValue("@val2", dbArray.Col2); // Unit_Of_Measurement

                            command.ExecuteNonQuery();
                        }
                    }
                }

                string checkQueryHabit = "SELECT COUNT(*) FROM Habit";
                long countHabit = 0;

                using (var checkCommand = new SqliteCommand(checkQueryHabit, connection))
                {
                    object? result = checkCommand.ExecuteScalar();
                    countHabit = (result != null && result != DBNull.Value) ? Convert.ToInt64(result) : 0;
                }

                if (countHabit == 0)
                {
                    // I take all the habits id from the Register_Habit table
                    List<int> habitsId = new List<int>();
                    connection.Open();
                    tableCmd = connection.CreateCommand();
                    tableCmd.CommandText = "SELECT Id FROM Register_Habit";

                    SqliteDataReader reader = tableCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        habitsId.Add(reader.GetInt32(0)); // Assuming the Id is in the first column
                    }

                    Random random = new Random();

                    foreach (int id in habitsId)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            string randomDate = DateTime.Now.AddDays(-random.Next(0, 31)).ToString("dd-MM-yy"); // Random date within the last year

                            int randomQuantity = random.Next(1, 11); // Random quantity between 1 and 100

                            tableCmd = connection.CreateCommand();
                            tableCmd.CommandText = $"INSERT INTO Habit (Date, Quantity, HabitId) VALUES('{randomDate}', {randomQuantity}, {id})";
                            tableCmd.ExecuteNonQuery();
                        }
                    }
                } 
                #endregion
            }

            Menu.GetUserInput();
        }
    }
}
