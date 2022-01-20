namespace Discworld.Animals
{
    public class Herbivore : Animal
    {
        public Herbivore(Cell cell, Gender gender) : base(cell, gender) { }

        public Herbivore(Animal parent, Gender gender) : base(parent, gender) { }

        protected override void Eat(Animal animal) { }

        protected override Animal GiveBirth(Gender gender)
        {
            var baby = new Herbivore(this, gender);
            baby.Walk(CurrentCell);

            return baby;
        }
    }
}
