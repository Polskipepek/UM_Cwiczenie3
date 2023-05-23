namespace UM_Cwiczenie3;
internal static class BetterInput {
    public static int GetInputNumber(string title, int min, int max, bool clearAfter = true) {
        int numRow = -1;
        Console.WriteLine(title);
        while (!int.TryParse(Console.ReadLine(), out numRow) || numRow < min || numRow > max) {
            Console.Clear();
            Console.WriteLine("You entered an invalid number");
            Console.Write(title);
        }
        if (clearAfter) Console.Clear();
        return numRow;
    }

    public static int GetInputNumberSameLine(string title, int min, int max, bool clearAfter = true)
    {
        int numRow = -1;
        Console.Write($"{title}: ");
        while (!int.TryParse(Console.ReadLine(), out numRow) || numRow < min || numRow > max)
        {
            Console.Clear();
            Console.WriteLine("You entered an invalid number");
            Console.Write(title);
        }
        if (clearAfter) Console.Clear();
        return numRow;
    }

    public static int GetKeyNumber(string title, int min, int max, bool clearAfter = true) {
        int numRow = -1;
        Console.WriteLine(title);
        while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out numRow) || numRow < min || numRow > max) {
            Console.Clear();
            Console.WriteLine("You entered an invalid number");
            Console.Write(title);
        }
        if (clearAfter) Console.Clear();
        return numRow;
    }

    public static int GetKeyNumberSameLine(string title, int min, int max, bool clearAfter = true) {
        int numRow = -1;
        Console.Write($"{title}: ");
        while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out numRow) || numRow < min || numRow > max) {
            Console.Clear();
            Console.WriteLine("You entered an invalid number");
            Console.Write($"{title}: ");
        }
        if (clearAfter) Console.Clear();
        Console.WriteLine();
        return numRow;
    }

    public static float GetFloat(string title, float min, float max, bool clearAfter = true) {
        float numRow = -1;
        Console.WriteLine(title);
        while (!float.TryParse(Console.ReadLine(), out numRow) || numRow < min || numRow > max) {
            Console.Clear();
            Console.WriteLine("You entered an invalid number");
            Console.Write(title);
        }
        if (clearAfter) Console.Clear();
        return numRow;
    }

    public static float GetFloatSameLine(string title, float min, float max, bool clearAfter = true) {
        float numRow = -1;
        Console.Write($"{title}: ");
        while (!float.TryParse(Console.ReadLine(), out numRow) || numRow < min || numRow > max) {
            Console.Clear();
            Console.WriteLine("You entered an invalid number");
            Console.Write(title);
        }
        if (clearAfter) Console.Clear();
        return numRow;
    }

    public static double GetDoubleSameLine(string title, double min, double max, bool clearAfter = true)
    {
        double numRow = -1;
        Console.Write($"{title}: ");
        while (!double.TryParse(Console.ReadLine(), out numRow) || numRow < min || numRow > max)
        {
            Console.Clear();
            Console.WriteLine("You entered an invalid number");
            Console.Write(title);
        }
        if (clearAfter) Console.Clear();
        return numRow;
    }

    public static string GetReadLine(string title, int minLength = 1) {
        Console.Write($"{title}: ");
        string value = "";
        while (value?.Length < minLength) {
            value = Console.ReadLine();
        }
        return value!;
    }
}
