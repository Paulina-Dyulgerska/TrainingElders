// See https://aka.ms/new-console-template for more information
using As;

Console.WriteLine("Hello, World!");

var gg0 = new Test { Asd = 12 };
var gg1 = new Test { Asd = 13 };
var gg2 = new Test { Asd = 14 };
var gList = new List<Parent> { gg0, gg1, gg2 };

foreach (var item in gList)
{
    var t = item as Test; // if ggo, gg1 or gg2 are not created as Test, but as something else, t will be null
    var a = t.Asd;
    var b = t.MyProperty;
}

Console.ReadLine();
