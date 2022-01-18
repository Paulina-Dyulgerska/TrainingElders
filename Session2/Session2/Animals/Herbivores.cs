using Session2.Positioning;

namespace Session2.Animals
{
    public class Herbivores : Animal
    {
        public Herbivores(Position position) : base(position)
        {
        }

        public override string Eat(ICollection<Animal> animals)
        {
            return "I ate a cabbage!";
        }
    }
}
