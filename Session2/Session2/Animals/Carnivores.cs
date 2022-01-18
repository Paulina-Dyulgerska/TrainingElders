using Session2.Positioning;

namespace Session2.Animals
{
    public class Carnivores : Animal
    {
        private readonly Random random;

        public Carnivores(Position position, Random random) : base(position)
        {
            this.random = random;
        }

        public override string Eat(ICollection<Animal> animals)
        {
            //foreach (var animal in animals)
            //{
            //    var shallAnimalBeEaten = random.Next(0, 100) <= 60;

            //    if (animal.GetType() == typeof(Herbivores) && shallAnimalBeEaten)
            //    {
            //        animal.Die();
            //        return "I ate a Herbivores!";
            //    }
            //}

            return "I ate nothing!";
        }
    }
}
