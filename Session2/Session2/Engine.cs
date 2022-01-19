using Discworld;
using Session2.IO;

namespace Session2
{
    public class Engine
    {
        private readonly IReader reader;
        private readonly IWriter writer;

        public Engine(IReader reader, IWriter writer)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            this.reader = reader;
            this.writer = writer;
        }

        public void Run()
        {
            //var sizes = reader.ReadLine().Split(",").Select(int.Parse).ToArray();
            //var sizeX = sizes[0];
            //var sizeY = sizes[1];
            //var countOfAnimals = int.Parse(reader.ReadLine());

            var worldDimentions = new WorldDimentions { Width = 4, Height = 5 };
            var world = God.BigBang(worldDimentions).TheSixthDay(12);
            var worldDrawing = Drawer.Draw(world);
            writer.WriteLine("After The Big Bang");
            writer.WriteLine(worldDrawing);

            while (world.CanRun)
            {
                //Thread.Sleep(1000);
                //writer.Clear();
                world.Run();
                worldDrawing = Drawer.Draw(world);
                writer.WriteLine(worldDrawing);
            }
        }
    }
}
