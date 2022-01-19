using Discworld.Animals;

namespace Discworld
{
    public static class God
    {
        public static World BigBang(WorldDimentions worldDimentions)
        {
            var cells = new List<Cell>();

            for (int i = 0; i < worldDimentions.Width; i++)
            {
                for (int j = 0; j < worldDimentions.Height; j++)
                {
                    var newPosition = new Position { X = i, Y = j };
                    var newCell = new Cell(newPosition);
                    cells.Add(newCell);
                }
            }

            var world = new World(cells);

            return world;
        }
        public static World TheSixthDay(this World world, int count)
        {
            var total = 0;

            foreach (var cell in world.Cells)
            {
                if (total >= count)
                    break;

                switch (Random.Shared.Next(0, 3))
                {
                    case 1:
                        cell.Visit(new Herbivore(cell, Gender.Random()));
                        total++;
                        break;
                    case 2:
                        cell.Visit(new Carnivore(cell, Gender.Random()));
                        total++;
                        break;
                    default:
                        break;
                }
            }

            return world;
        }
    }
}
