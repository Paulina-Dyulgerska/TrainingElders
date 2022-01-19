namespace Discworld.Animals
{
    public class Carnivore : Animal
    {
        public Carnivore(Cell cell, Gender gender) : base(cell, gender) 
        {
            IAmFood = true;
        }

        protected override void Eat(Animal animal)
        {
            var toughLuck = Random.Shared.Next(0, 100) <= 60;
            if (toughLuck)
            {
                animal.Die();
            }
        }
    }
}
