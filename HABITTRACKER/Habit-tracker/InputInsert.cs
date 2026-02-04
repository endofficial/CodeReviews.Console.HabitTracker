using System.Globalization;
using System.Reflection.Metadata.Ecma335;

namespace Habit_tracker;

public class InputInsert
{
    public static string GetNewHabitInput(TextReader? reader = null)
    {
        reader ??= Console.In; 

        WriteLine("\nPLEASE ENTER THE NAME OF THE HABIT YOU WANT TO REGISTER. TYPE 0 TO RETURN TO MAIN MENU.");
        string? userInputNewHabit = reader?.ReadLine()?.Trim();
        if (userInputNewHabit == "0") return "0";

        while (int.TryParse(userInputNewHabit, out _) || string.IsNullOrEmpty(userInputNewHabit))
        {
            WriteLine("\nINVALID INPUT. PLEASE ENTER A VALID HABIT NAME. TYPE 0 TO RETURN TO MAIN MENU.");
            userInputNewHabit = reader?.ReadLine()?.Trim();
            if (userInputNewHabit == "0") return "0";
        }

        return userInputNewHabit!; // the ! operator is used to indicate that userInputDate is not null here
    }

    public static string GetNewUnitOfMeasureInput(TextReader? reader = null)
    {
        reader ??= Console.In;

        WriteLine("\nPLEASE ENTER THE UNIT OF MEASUREMENT FOR THE HABIT YOU WANT TO REGISTER. TYPE 0 TO RETURN TO MAIN MENU.");
        string? userInputNewUnitOfMeasure = reader?.ReadLine()?.Trim();

        if (userInputNewUnitOfMeasure == "0") return "0";

        while (int.TryParse(userInputNewUnitOfMeasure, out _) || string.IsNullOrEmpty(userInputNewUnitOfMeasure))
        {
            WriteLine("\nINVALID INPUT. PLEASE ENTER A VALID UNIT OF MEASUREMENT. TYPE 0 TO RETURN TO MAIN MENU.");
            userInputNewUnitOfMeasure = reader?.ReadLine()?.Trim();
            if (userInputNewUnitOfMeasure == "0") return "0";
        }

        return userInputNewUnitOfMeasure!;
    }

    public static int GetHabitInput(string message, TextReader? reader = null)
    {
        reader ??= Console.In;

        WriteLine(message);
        string? userInputHabit = reader?.ReadLine()?.Trim();

        if (userInputHabit == "0") return 0;

        while (!Int32.TryParse(userInputHabit, out _) || Convert.ToInt32(userInputHabit) < 0)
        {
            WriteLine("\nINVALID INPUT. PLEASE ENTER A VALID HABIT NAME. TYPE 0 TO RETURN TO MAIN MENU.");
            userInputHabit = reader?.ReadLine()?.Trim();
            if (userInputHabit == "0") return 0;
        }
        int finalInputHabit = Convert.ToInt32(userInputHabit);
        return finalInputHabit!;
    }

    public static string GetDateInput(TextReader? reader = null)
    {
        reader ??= Console.In; // if reader is null, assign Console.In to it

        WriteLine("\nPLEASE ENTER DATE (dd-MM-yy). TYPE 0 TO RETURN TO MAIN MENU. ");
        string? userInputDate = reader?.ReadLine()?.Trim();
        if (userInputDate == "0") return "0";

        while (!DateTime.TryParseExact(userInputDate, "dd-MM-yy", new CultureInfo("it-IT"),
              DateTimeStyles.None, out _))
        {
            WriteLine("\nINVALID DATE FORMAT. PLEASE ENTER DATE IN FORMAT (dd-MM-yy). TYPE 0 TO RETURN TO MAIN MENU.");
            userInputDate = reader?.ReadLine()?.Trim();
            if (userInputDate == "0") return "0";
        }
        return userInputDate!; 
    }

    public static int GetNumberInput(string message, TextReader? reader = null)
    {
        reader ??= Console.In;

        WriteLine(message);
        string? userInputNumber = reader?.ReadLine()?.Trim();
        if (userInputNumber == "0") return 0;

        while (!Int32.TryParse(userInputNumber, out _) || Convert.ToInt32(userInputNumber) < 0)
        {
            WriteLine("\nINVALID NUMBER. PLEASE ENTER A VALID NUMBER. TYPE 0 TO RETURN TO MAIN MENU.");
            userInputNumber = reader?.ReadLine()?.Trim();
            if (userInputNumber == "0") return 0;
        }

        int finalInputNumber = Convert.ToInt32(userInputNumber);
        return finalInputNumber!;
    }
}

public class RegisterHabit
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int Quantity { get; set; }
    public string NameHabit { get; set; }
    public string UnitOfMeasurement { get; set; }
}