using Session2.Animals;
using Session2.Positioning;

namespace Session2
{
    class Program
    {
        static void Main(string[] args)
        {
            //var sizes = Console.ReadLine().Split(",").Select(int.Parse).ToArray();
            //var sizeX = sizes[0];
            //var sizeY = sizes[1];
            //var countOfAnimals = int.Parse(Console.ReadLine());

            var sizeX = 4;
            var sizeY = 3;
            var countOfAnimals = 4;

            var random = new Random();
            var positionGenerator = new PositionGenerator(sizeX, sizeY);
            var animalsGenerator = new AnimalsGenerator(countOfAnimals, positionGenerator, random);
            var animals = animalsGenerator.GenerateCollection();

            var world = new World(sizeX, sizeY, animals, positionGenerator);

            Console.WriteLine(world.ToString());

            Console.WriteLine("------------------");

            //while (true)
            //{
            //    var countHerbivoresLeft = world.MakeATurn();

            //    Console.WriteLine(world);

            //    Console.WriteLine("------------------");

            //    if (countHerbivoresLeft == 0)
            //    {
            //        break;
            //    }
            //}
        }
    }
}