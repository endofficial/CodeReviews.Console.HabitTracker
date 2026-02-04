namespace Habit_tracker;

public class Menu
{
    public static void GetUserInput()
    {
        bool closeApp = false;

        while (!closeApp)
        {
            WriteLine("\nWELCOME IN YOUR HABIT TRACKER!");
            WriteLine("\nWHAT WOULD YOU LIKE TO DO?");
            WriteLine("\nTYPE 0 TO CLOSE APP");
            WriteLine("TYPE 1 TO A REGISTER A NEW HABIT");
            WriteLine("TYPE 2 TO ADD A YOUR HABIT");
            WriteLine("TYPE 3 TO UPDATE RECORD");
            WriteLine("TYPE 4 TO DELETE RECORD");
            WriteLine("TYPE 5 TO SEE ALL RECORDS");

            string? userInput = ReadLine();

            while (!Int32.TryParse(userInput, out _) || Convert.ToInt32(userInput) < 0 || Convert.ToInt32(userInput) > 5)
            {
                WriteLine("\nINVALID INPUT, PLEASE TRY AGAIN:");
                userInput = ReadLine();
            }

            switch (userInput)
            {
                case "0":
                    closeApp = true;
                    break;
                case "1":
                    DatabaseManager.Register();
                    break;
                case "2":
                    DatabaseManager.Insert();
                    break;
                case"3":
                    DatabaseManager.Update();
                    break;
                case "4":
                    DatabaseManager.Delete();
                    break;
                case "5":
                    DatabaseManager.GetAllrecords();
                    break;
            }
        }
    }
}