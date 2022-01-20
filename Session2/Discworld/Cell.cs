using Discworld.Animals;

namespace Discworld
{
    public class Cell
    {
        private readonly List<Animal> animals;

        public Cell(Position position)
        {
            Position = position ?? throw new ArgumentNullException(nameof(position));
            animals = new List<Animal>();
        }

        public Position Position { get; }

        public IEnumerable<Animal> Animals => animals.AsReadOnly();

        public void Visit(Animal animal)
        {
            if (Animals.Contains(animal))
                return;

            animals.Add(animal);
        }

        public void Leave(Animal animal)
        {
            if (Animals.Contains(animal) == false)
                return;

            animals.Remove(animal);
        }

        public void Decompose()
        {
            animals.RemoveAll(a => a.IsDead);
        }

    }
}
