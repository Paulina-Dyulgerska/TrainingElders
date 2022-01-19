using Session2.IO;

namespace Session2
{
    class Program
    {
        static void Main(string[] args)
        {
            var writer = new ConsoleWriter();
            var reader = new ConsoleReader();
            var engine = new Engine(reader, writer);
            engine.Run();
        }
    }
}