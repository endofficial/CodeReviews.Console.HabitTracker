using Microsoft.Data.Sqlite;
using System.Globalization;

namespace Habit_tracker;

public class DatabaseManager
{
    static string connectionString = @"Data Source=habit-tracker.db";

    public static void Register()
    {
        Console.Clear();
        string nameHabit = InputInsert.GetNewHabitInput(Console.In);

        if (nameHabit == "0") return;

        string unitOfMeasure = InputInsert.GetNewUnitOfMeasureInput(Console.In);

        if (unitOfMeasure == "0") return;

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText =
                $"INSERT INTO Register_Habit(name_habit, unit_of_measurement) VALUES('{nameHabit}', '{unitOfMeasure}')";
            tableCmd.ExecuteNonQuery(); 
            connection.Close();
        }
    }

    public static void Insert()
    {
        Console.Clear();

        GetAllHabit();

        int chooseHabit = InputInsert.GetHabitInput("CHOOSE A YOUR HABIT THAT YOU WOULD LIKE TO ADD. TYPE 0 TO RETURN TO MAIN MENU.");
        if (chooseHabit == 0) return;

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText =
                $"SELECT EXISTS (SELECT 1 FROM Register_Habit WHERE name_habit = '{chooseHabit}'";
        }

        string date = InputInsert.GetDateInput(Console.In);

        if (int.TryParse(date, out int insertDate))
        {
            if (insertDate == 0) return;
        }

        int quantity = InputInsert.GetNumberInput("\nPLEAE ENTER HOW MANY TIMES YOU HAVE PERFORMED THE HABIT. TYPE 0 TO RETURN TO MAIN MENU.", Console.In);

        if (quantity == 0) return;

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText =
                $"INSERT INTO habit(date, quantity, HabitId) VALUES('{date}', {quantity}, {chooseHabit})";
            tableCmd.ExecuteNonQuery();
            connection.Close();
        }
    }

    public static void GetAllHabit()
    {
        Console.Clear();
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = "SELECT * FROM Register_Habit";

            List<RegisterHabit> tableData = new List<RegisterHabit>();

            SqliteDataReader reader = tableCmd.ExecuteReader(); // Execute the query and get a reader; ExecuteReader is used for SELECT statements

            if (reader.HasRows) // Check if there are any rows
            {
                // Read each row
                while (reader.Read())
                {
                    tableData.Add(
                    new RegisterHabit
                    {
                        Id = reader.GetInt32(0), // Get the value of the first column (Id)
                        NameHabit = reader.GetString(1), // Get the value of the second column (NameHabit)
                        UnitOfMeasurement = reader.GetString(2) // Get the value of the third column (UnitOfMeasurement)
                    });
                }
            }
            else
            {
                Console.WriteLine("No rows found.");
            }

            connection.Close();

            WriteLine("-------------------------------------------------------");
            foreach (var db in tableData)
            {
                WriteLine($"ID: {db.Id} - {db.NameHabit} - UNIT OF MEASURE: {db.UnitOfMeasurement}");
            }
            WriteLine("-------------------------------------------------------");
        }
    }

    public static void GetAllrecords()
    {
        Console.Clear();
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();

            // SQL query to select all records
            // h => habit table alias; r => register_habit table alias
            tableCmd.CommandText = @"
            SELECT
            h.Id, 
            h.Date,
            h.Quantity,
            r.Name_Habit
            FROM habit h INNER JOIN register_habit r ON h.HabitId = r.Id"; 

            List<RegisterHabit> tableData = new List<RegisterHabit>();

            SqliteDataReader reader = tableCmd.ExecuteReader(); // Execute the query and get a reader; ExecuteReader is used for SELECT statements

            if (reader.HasRows) // Check if there are any rows
            {
                // Read each row
                while (reader.Read())
                {
                    tableData.Add(
                    new RegisterHabit
                    {
                        Id = reader.GetInt32(0), // Get the value of the first column (Id)
                        Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo("en-US"), DateTimeStyles.None), // Get the value of the second column (Date)
                        Quantity = reader.GetInt32(2), // Get the value of the third column (Quantity)
                        NameHabit = reader.GetString(3) // Get the value of the fourth column (NameHabit)
                    });
                }
            }
            else
            {
                Console.WriteLine("No rows found.");
            }

            connection.Close();

            WriteLine("-------------------------------------------------------");
            foreach (var db in tableData)
            {
                WriteLine($"ID: {db.Id} - {db.Date.ToString("dd-MM-yy")} - QUANTITY: {db.Quantity} - NAME HABIT: {db.NameHabit}");
            }
            WriteLine("-------------------------------------------------------");
        }
    }

    public static void Update()
    {
        GetAllrecords();
        bool continueUpdate = true;

        while (continueUpdate)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = "SELECT * FROM habit"; // SQL query to select all records
                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (!reader.HasRows) // Check if there are any rows
                {
                    WriteLine("\nNO RECORDS TO UPDATE.");
                    connection.Close();
                    continueUpdate = false;
                }

                else
                {
                    var recordId = InputInsert.GetNumberInput("\nPLEASE ENTER THE ID OF THE RECORD YOU WANT TO UPDATE. TYPE 0 TO RETURN TO MAIN MENU.", Console.In);
                    if (recordId == 0)
                    {
                        connection.Close();
                        return;
                    } 
                    else
                    {
                        tableCmd = connection.CreateCommand();
                        tableCmd.CommandText = $"SELECT EXISTS (SELECT 1 FROM habit WHERE Id = {recordId})";

                        var checkQuery = Convert.ToInt32(tableCmd.ExecuteScalar()); // ExecuteScalar is used to get a single value, it used to check if the record exists
                        if (checkQuery == 0)
                        {
                            WriteLine("\nRECORD NOT FOUND.");
                            connection.Close();
                            Update();
                        }

                        WriteLine("\nTYPE 1 IF YOU WANT UPDATE A NAME HABIT. TYPE 2 TO CHANGE DATE. TYPE 0 TO RETURN TO MAIN MENU.");
                        string? updateInput = ReadLine();

                        while (!int.TryParse(updateInput, out _) || Convert.ToInt32(updateInput) < 0)
                        {
                            WriteLine("\nINVALID INPUT. PLEASE TRY AGAIN.");
                            connection.Close();
                            updateInput = ReadLine();
                        }

                        if (updateInput == "0")
                        {
                            connection.Close();
                            return;
                        } 

                        if (updateInput == "1")
                        {
                            WriteLine("\nUPDATING A NAME HABIT. TYPE '0' TO RETURN TO MAIN MENU.");
                            GetAllHabit();

                            int recordHabitId = InputInsert.GetNumberInput("\nPLEASE ENTER THE ID OF THE HABIT YOU WANT TO UPDATE. TYPE 0 TO RETURN TO MAIN MENU.", Console.In);
                            if (recordHabitId == 0)
                            {
                                connection.Close();
                                return;
                            }

                            tableCmd = connection.CreateCommand();
                            tableCmd.CommandText = $"SELECT EXISTS (SELECT 1 FROM Register_Habit WHERE Id = {recordHabitId})";

                            var checkHabitQuery = Convert.ToInt32(tableCmd.ExecuteScalar());
                            if (checkHabitQuery == 0)
                            {
                                WriteLine("\nHABIT NOT FOUND.");
                                connection.Close();
                                Update();
                            }

                            string newNameHabit = InputInsert.GetNewHabitInput(Console.In);
                            if (newNameHabit == "0")
                            {
                                connection.Close();
                                return;
                            }

                            string newUnitOfMeasure = InputInsert.GetNewUnitOfMeasureInput(Console.In);
                            if (newUnitOfMeasure == "0")
                            {
                                connection.Close();
                                return;
                            }

                            tableCmd.CommandText =
                                $"UPDATE Register_Habit SET name_habit = '{newNameHabit}', unit_of_measurement = '{newUnitOfMeasure}' WHERE Id = {recordHabitId}";
                            tableCmd.ExecuteNonQuery();
                            connection.Close();
                            continueUpdate = false;
                        }

                        if (updateInput == "2")
                        {
                            string UpDate = InputInsert.GetDateInput();

                            if (int.TryParse(UpDate, out int UpMenu))
                            {
                                if (UpMenu == 0)
                                {
                                    connection.Close();
                                    return;
                                }
                            }

                            int UpQuantity = InputInsert.GetNumberInput("\nPLEAE ENTER HOW MANY TIMES YOU HAVE PERFORMED THE HABIT. TYPE 0 TO RETURN TO MAIN MENU.");

                            if (UpQuantity == 0)
                            {
                                connection.Close();
                                return;
                            }

                            tableCmd.CommandText = $"UPDATE habit SET Date = '{UpDate}', Quantity = {UpQuantity} WHERE Id = {recordId}";
                            tableCmd.ExecuteNonQuery();
                            connection.Close();
                            continueUpdate = false;
                        }
                    }
                }
            }
        }
    }

    public static void Delete()
    {
        Console.Clear();
        GetAllrecords();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();

            tableCmd.CommandText = "SELECT * FROM habit"; // SQL query to select all records

            SqliteDataReader reader = tableCmd.ExecuteReader();

            if (!reader.HasRows) // Check if there are any rows
            {
                WriteLine("\nNO RECORDS TO DELETE.");
                connection.Close();
                return;
            }
            else 
            {
                WriteLine("\nTYPE 'D' TO DELETE ALL YOUR HABITS. PRESS 'R' TO DELETE THE NAME HABIT. PRESS 'C' TO SELECT THE RECORD. PRESS 0 TO RETURN TO MAIN MENU.");

                string? delInput = ReadLine()?.ToUpper();
                bool inputValid = false;

                do
                {
                    if (Int32.TryParse(delInput, out int numberInput))
                    {
                        if (numberInput == 0)
                        {
                            connection.Close();
                            return;
                        } 
                    }

                    if ((delInput == "D"))
                    {
                        tableCmd = connection.CreateCommand();
                        tableCmd.CommandText = $"DELETE FROM habit";
                        tableCmd.ExecuteNonQuery();
                        connection.Close();
                        inputValid = true;

                    }
                    else if (delInput == "R")
                    {
                        Console.Clear();
                        GetAllHabit();

                        tableCmd = connection.CreateCommand();
                        tableCmd.CommandText = $"SELECT * FROM Register_Habit";

                        SqliteDataReader readerHabit = tableCmd.ExecuteReader();

                        if (!reader.HasRows)
                        {
                            WriteLine("\nNO HABITS TO DELETE.");
                            connection.Close();
                            return;
                        }

                        else
                        {
                            WriteLine("TYPE 'D' TO DELETE ALL HABIT NAMES. TYPE 'C' TO SELECT THE RECROD. TYPE 0 TO RETURN TO MAIN MENU. ");
                            string? delHabitInput = ReadLine()?.ToUpper();

                            if (int.TryParse(delHabitInput, out int habitInput))
                            {
                                if (habitInput == 0)
                                {
                                    connection.Close();
                                    return;
                                }
                            }

                            if (delHabitInput == "D")
                            {
                                tableCmd = connection.CreateCommand();
                                tableCmd.CommandText = $"DELETE FROM Register_Habit";
                                tableCmd.ExecuteNonQuery();
                                connection.Close();
                                inputValid = true;
                            }
                            else if (delHabitInput == "C")
                            {
                                int recordHabitId = InputInsert.GetNumberInput("\nPLEASE ENTER THE ID OF THE RECORD THAT YOU WANT TO DELETE. TYOE 0 TO RETURN TO MAIN MENU.");
                                if (recordHabitId == 0)
                                {
                                    connection.Close();
                                    return;
                                }

                                tableCmd = connection.CreateCommand();
                                tableCmd.CommandText = $"DELETE FROM Register_Habit WHERE Id = {recordHabitId}";
                                tableCmd.ExecuteNonQuery();
                                connection.Close();
                                inputValid = true;
                            }
                        }
                    }
                    else if (delInput == "C")
                    {
                        int recordId = InputInsert.GetNumberInput("" +
                        "\nPLEASE ENTER THE ID OF THE RECORD YOU WANT TO DELETE. TYPE 0 TO RETURN TO MAIN MENU.");

                        if (recordId == 0)
                        {
                            connection.Close();
                            return;
                        } 

                        tableCmd = connection.CreateCommand();
                        tableCmd.CommandText = $"DELETE FROM habit WHERE Id = {recordId}";
                        tableCmd.ExecuteNonQuery(); // ExecuteNonQuery is used for DELETE statements
                        connection.Close();
                        inputValid = true;
                    }
                    else
                    {
                        WriteLine("\nINVALID INPUT. PLEASE TRY AGAIN.");
                        delInput = ReadLine()?.ToUpper();
                    }
                } while (!inputValid);

            }
        }
    }
}