// See https://aka.ms/new-console-template for more information
using System.Globalization;
using System.Text;

//string author = "Mahesh Chand";
//// Convert a C# string to a byte array    
//byte[] bytes = Encoding.ASCII.GetBytes(author);
//foreach (byte b in bytes)
//{
//    Console.WriteLine(b);
//}
//// Convert a byte array to a C# string    
//string str = Encoding.ASCII.GetString(bytes);
//Console.WriteLine(str);
//// Convert one Encoding type to another    
//string authorName = "Here is a unicode characters string. Pi (\u03a0)";
//// Create two different encodings.    
//Encoding ascii = Encoding.ASCII;
//Encoding unicode = Encoding.Unicode;
//// Convert unicode string into a byte array.    
//byte[] bytesInUni = unicode.GetBytes(authorName);
//// Convert unicode to ascii    
//byte[] bytesInAscii = Encoding.Convert(unicode, ascii, bytesInUni);
//// Convert byte[] into a char[]    
//char[] charsAscii = new char[ascii.GetCharCount(bytesInAscii, 0, bytesInAscii.Length)];
//ascii.GetChars(bytesInAscii, 0, bytesInAscii.Length, charsAscii, 0);
//// Convert char[] into a ascii string    
//string asciiString = new string(charsAscii);
//// Print unicode and ascii strings    
//Console.WriteLine($"Author Name: {authorName}");
//Console.WriteLine($"Ascii converted name: {asciiString}");
//Console.ReadKey();

Console.OutputEncoding = System.Text.Encoding.Unicode;

//string[] words = { "Tuesday", "Salı", "Scheisse", "Вторник", "Mardi",
//                         "Τρίτη", "Martes", "יום שלישי",
//                         "الثلاثاء", "วันอังคาร", "Scheiße" };

//// Display array in unsorted order.
//foreach (string word in words)
//    Console.WriteLine(word);

//Console.WriteLine();

//// Create parallel array of words by calling ToUpperInvariant.
//string[] upperWords = new string[words.Length];
//for (int ctr = words.GetLowerBound(0); ctr <= words.GetUpperBound(0); ctr++)
//    upperWords[ctr] = words[ctr].ToUpperInvariant();

//// Sort the words array based on the order of upperWords.
//Array.Sort(upperWords, words, StringComparer.InvariantCulture);

//// Display the sorted array.
//foreach (string word in words)
//    Console.WriteLine(word);

//Console.WriteLine();

//// Sort the words array based on the order of upperWords.
//Array.Sort(upperWords, words);

//// Display the sorted array.
//foreach (string word in words)
//    Console.WriteLine(word);


/// <summary>
/// ///////////
/// </summary>

//String[] cultureNames = { "en-US", "de-DE", "se-SE" };
//String[] strings1 = { "case", "Scheiße", "ä", "ö", "ü", "ä", "Æ", "encyclopædia", };
//String[] strings2 = { "Case", "Scheisse", "ae", "oe", "ue", "Æ", "ae", "encyclopaedia", };
//StringComparison[] comparisons = (StringComparison[])Enum.GetValues(typeof(StringComparison));

//foreach (var cultureName in cultureNames)
//{
//    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureName);
//    Console.WriteLine("Current Culture: {0}", CultureInfo.CurrentCulture.Name);
//    for (int ctr = 0; ctr <= strings1.GetUpperBound(0); ctr++)
//    {
//        foreach (var comparison in comparisons)
//            Console.WriteLine("   {0} = {1} ({2}): {3}", strings1[ctr],
//                              strings2[ctr], comparison,
//                              String.Equals(strings1[ctr], strings2[ctr], comparison));

//        Console.WriteLine();
//    }
//    Console.WriteLine();
//}



/// <summary>
/// ///////////
/// </summary>

var ci = new CultureInfo("de-DE");
Console.WriteLine(ci.CompareInfo.Compare("jörg", "joerg", CompareOptions.IgnoreNonSpace));
Console.WriteLine(String.Compare("jörg", "joerg", true, ci));
Console.WriteLine(String.Compare("straße", "strasse", true, ci));


String[] cultureNames = { "en-US", "se-SE" };
String[] strings1 = { "case",  "encyclopædia",
                            "encyclopædia", "Archæology" };
String[] strings2 = { "Case", "encyclopaedia",
                            "encyclopedia" , "ARCHÆOLOGY" };
StringComparison[] comparisons = (StringComparison[])Enum.GetValues(typeof(StringComparison));

foreach (var cultureName in cultureNames)
{
    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureName);
    Console.WriteLine("Current Culture: {0}", CultureInfo.CurrentCulture.Name);
    for (int ctr = 0; ctr <= strings1.GetUpperBound(0); ctr++)
    {
        foreach (var comparison in comparisons)
            Console.WriteLine("   {0} = {1} ({2}): {3}", strings1[ctr],
                              strings2[ctr], comparison,
                              String.Equals(strings1[ctr], strings2[ctr], comparison));

        Console.WriteLine();
    }
    Console.WriteLine();
}
