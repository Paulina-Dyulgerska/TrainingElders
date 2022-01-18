using Session2.Positioning;

namespace Session2.Animals
{
    public class AnimalsGenerator
    {
        private readonly int countOfAnimals;
        private readonly PositionGenerator positionGenerator;
        private readonly Random random;
        private readonly IList<Animal> animals;

        public AnimalsGenerator(int countOfAnimals, PositionGenerator positionGenerator, Random random)
        {
            this.countOfAnimals = countOfAnimals;
            this.positionGenerator = positionGenerator;
            this.random = random;
            this.animals = new List<Animal>();
        }

        public IList<Animal> GenerateCollection()
        {
            for (int i = 0; i < this.countOfAnimals; i++)
            {
                var newPosition = this.positionGenerator.GeneratePosition();

                if (this.random.Next(0, 10000) % 2 == 0)
                {
                    this.animals.Add(new Herbivores(newPosition));
                }
                else
                {
                    this.animals.Add(new Carnivores(newPosition, this.random));
                }
            }

            return this.animals;
        }
    }
}
