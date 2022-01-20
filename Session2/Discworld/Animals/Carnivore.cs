namespace Discworld.Animals
{
    public class Carnivore : Animal
    {
        public Carnivore(Cell cell, Gender gender) : base(cell, gender) { }

        public Carnivore(Animal parent, Gender gender) : base(parent, gender) { }

        protected override void Eat(Animal animal)
        {
            var toughLuck = Random.Shared.Next(0, 100) <= 60;
            if (toughLuck)
            {
                animal.Die();
            }
        }

        public override void Die() { }

        protected override Animal GiveBirth(Gender gender)
        {
            var baby = new Carnivore(this, gender);
            return baby;
        }
    }
}
