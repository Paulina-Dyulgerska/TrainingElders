using Session2.Positioning;

namespace Session2.Animals
{
    public abstract class Animal
    {
        private Position position;

        public Animal(Position position)
        {
            this.position = position;
        }

        public Position Position
        {
            get
            {
                return this.position;
            }

            private set { }
        }

        public bool IsDead { get; private set; }

        //public void ChangePosition(Position newPosition)
        //{
        //    this.position = newPosition;
        //}

        public void Die()
        {
            this.IsDead = true;
        }

        public abstract string Eat(ICollection<Animal> animals);
    }
}
