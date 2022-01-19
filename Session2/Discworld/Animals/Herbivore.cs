namespace Discworld.Animals
{
    public class Herbivore : Animal
    {
        public Herbivore(Cell cell, Gender gender) : base(cell, gender) { }

        protected override void Eat(Animal animal) { }
    }
}
