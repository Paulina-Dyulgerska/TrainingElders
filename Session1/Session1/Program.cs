// See https://aka.ms/new-console-template for more information

using System.Text;

Console.WriteLine("The entered text would be displayed in the console window in reversed order and if there are two identical characters next to each other, they will be replaced with a single character with the same value.");

var input = Console.ReadLine();

var stringBuilder = new StringBuilder();

if (string.IsNullOrWhiteSpace(input) == false)
{
    for (int i = input.Length - 1; i >= 0; i--)
    {
        if (i > 0 && input[i - 1] == input[i])
        {
            continue;
        }
        stringBuilder.Append(input[i]);
    }
}

Console.WriteLine(stringBuilder.ToString());
