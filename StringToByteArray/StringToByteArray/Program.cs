// See https://aka.ms/new-console-template for more information
using System.Text;

Console.WriteLine("Hello, World!");


var str = "00 00 00 20 66 74 79 70 69 73 6F 36 00 00 00 01 6D 70 34 32 69 73 6F 36 61 76 63 31 69 73 6F 6D";
var arr = StringToByteArray(str);
Console.WriteLine(string.Join(", ", arr));
Console.WriteLine(arr.Length);
Console.WriteLine(Encoding.Unicode.GetString(arr));

static byte[] StringToByteArray(string hex)
{
    hex = hex.Replace(" ", "");
    int NumberChars = hex.Length;
    byte[] bytes = new byte[NumberChars / 2];
    for (int i = 0; i < NumberChars; i += 2)
        bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
    return bytes;
}

