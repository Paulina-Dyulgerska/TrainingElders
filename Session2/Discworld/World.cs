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

        public IEnumerable<Animal> Animals => cells.SelectMany(c => c.Animals);

        // World will not run if there are neither carnivores nor herbivores
        public bool CanRun => Animals.Any(x => x.GetType() == typeof(Herbivore)) && Animals.Any(x => x.GetType() == typeof(Carnivore));

        public void Run()
        {
            var movements = new List<(Animal animal, Cell cell)>();
            foreach (var animal in Animals)
            {
                Cell? nextCell = null;
                do
                {
                    var next = animal.Roam();
                    nextCell = cells.FirstOrDefault(x => x.Position == next);
                } while (nextCell is null);

                movements.Add((animal, nextCell));
            }

            foreach (var movement in movements)
            {
                movement.animal.Walk(movement.cell);
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
            var babies = new List<Animal>();
            foreach (var animal in Animals)
            {
                var baby = animal.Mate();
                if (baby == null)
                    continue;

                babies.Add(baby);
            }

            foreach (var baby in babies)
            {
                baby.CurrentCell.Visit(baby);
            }
        }
    }
}
