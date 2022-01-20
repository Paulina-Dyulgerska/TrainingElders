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

        public IEnumerable<Animal> Animals => cells.SelectMany(c => c.Animals).ToList(); // TODO remove the ToList

        // World will not run if there are neither carnivores nor herbivores
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
            foreach (var animal in Animals)
            {
                animal.Feed();
            }

            // Bury the dead animals:
            foreach (var cell in cells)
            {
                cell.Decompose();
            }

            // Deliver babies:
            foreach (var animal in Animals)
            {
                var babyAnimal = animal.Mate();

                if (babyAnimal != null)
                    animal.CurrentCell.Visit(babyAnimal);
            }
        }
    }
}
