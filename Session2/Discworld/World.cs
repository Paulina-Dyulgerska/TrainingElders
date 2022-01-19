using Discworld.Animals;

namespace Discworld
{
    public class World
    {
        private readonly IEnumerable<Cell> cells;

        public World(IEnumerable<Cell> cells)
        {
            this.cells = cells ?? throw new ArgumentNullException(nameof(cells));
        }

        public IEnumerable<Cell> Cells => cells;

        public IEnumerable<Animal> Animals => cells.SelectMany(c => c.Animals).ToList();

        // World will not run if there neither carnivores nor herbivores
        public bool CanRun => Animals.Any(x => x.GetType() == typeof(Herbivore)) && Animals.Any(x => x.GetType() == typeof(Carnivore));

        public void Run()
        {
            // Move animals onto new cells:
            foreach (var animal in Animals)
            {
                Cell? nextCell = null;
                do
                {
                    var next = animal.Roam();
                    nextCell = cells.FirstOrDefault(x => x.Position == next);
                } while (nextCell is null);

                animal.Walk(nextCell);
            }

            // Feed animals:
            foreach (var animal in Animals.Where(x=>x.IAmFood)) // Carnivores eat eachother if not check for IAmFood
            {
                animal.Feed();
            }

            // Bury the dead animals:
            //var deadAnimals = cells.Where(x => x.Animals.Any(x => x.IsDead)).SelectMany(x=>x.Animals.Where(x=>x.IsDead));
            var cellsWithDeadAnimal = cells.Where(x => x.Animals.Any(x => x.IsDead));
            foreach (var cell in cellsWithDeadAnimal)
            {
                var deadAnimal = cell.Animals.Where(x => x.IsDead).FirstOrDefault();
                if (deadAnimal != null)
                    cell.Leave(deadAnimal);
            }

            // Deliver babies:
            foreach (var animal in Animals)
            {
                animal.FindPartner(); // TODO
            }
        }

        //public string GetDrawing()
        //{
        //}
    }
}
