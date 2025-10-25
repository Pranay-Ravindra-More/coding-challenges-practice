// See https://aka.ms/new-console-template for more information
using Microsoft.VisualBasic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;


if (Console.IsInputRedirected)
{
    int readChar;
    List<byte> bytesFromConsole = new List<byte>();


    while ((readChar = Console.Read()) >= 0)
    {
        bytesFromConsole.Add((byte)readChar);
    }

    WcFromConsoleInput(args, bytesFromConsole);
}
else if (args.Length == 2)
{
    switch (args[0])
    {
        case "-c":
            Console.WriteLine($"{GetNumberOfBytes(args[1])} {args[1]}");
            break;
        case "-l":
            Console.WriteLine($"{GetNumberOfLines(args[1])} {args[1]}");
            break;
        case "-w":
            Console.WriteLine($"{GetNumberOfWords(args[1])} {args[1]}");
            break;
        case "-m":
            Console.WriteLine($"{GetNumberOfCharacters(args[1])} {args[1]}");
            break;
    }
}
else if (args.Length == 1)
{
    Console.WriteLine($"{GetNumberOfLines(args[0])}\t{GetNumberOfWords(args[0])}\t{GetNumberOfBytes(args[0])} {args[0]}");
}




static int GetNumberOfBytes(string file)
{
    return File.ReadAllBytes(file).Length;
}

static int GetNumberOfLines(string file)
{
    return File.ReadAllLines(file).Length;
}

static int GetNumberOfWords(string file)
{
    string contents = File.ReadAllText(file);
    return contents.Split(new char[] {' ','\n','\r','\t'}, StringSplitOptions.RemoveEmptyEntries).Length;
}

static int GetNumberOfCharacters(string file)
{
    return File.ReadAllText(file).Length;
}


static void WcFromConsoleInput(string[] args, List<byte> byteFromConsole)
{
    if(byteFromConsole.Count > 0)
    {
        string fullString = System.Text.Encoding.Default.GetString(byteFromConsole.ToArray());

        if(args.Length == 0)
        {
            var numberOfLines = fullString.Split('\n').Length;
            var numberOfWords = fullString.Split(new char[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
            Console.WriteLine($"{numberOfLines}\t{numberOfWords}\t{byteFromConsole.Count}");
        }
        else
        {
            switch(args[0])
            {
                case "-c":
                    Console.WriteLine($"{byteFromConsole.Count}");
                    break;
                case "-l":
                    var numberOfLines = fullString.Split('\n').Length;
                    Console.WriteLine($"{numberOfLines}");
                    break;
                case "-w":
                    var numberOfWords = fullString.Split(new char[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
                    Console.WriteLine($"{numberOfWords}");
                    break;
                case "-m":
                    Console.WriteLine($"{fullString.Length}");
                    break;
            }
          
        }

    }
}
