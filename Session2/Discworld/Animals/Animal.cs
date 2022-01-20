namespace Discworld.Animals
{
    public abstract class Animal
    {
        private IEnumerable<Animal> Cellmates => CurrentCell.Animals.Where(x => x != this);

        public Animal(Cell cell, Gender gender)
        {
            if (cell is null) throw new ArgumentNullException(nameof(cell));
            if (gender is null) throw new ArgumentNullException(nameof(gender));

            CurrentCell = cell;
            Gender = gender;
        }

        public Animal(Animal parent, Gender gender) : this(parent.CurrentCell, gender) { }

        public Cell CurrentCell { get; private set; }

        public bool IsDead { get; protected set; }

        public Gender Gender { get; }

        public Position Roam()
        {
            var direction = Random.Shared.Next(0, 4);
            if (direction == 0)
                return CurrentCell.Position.Up();
            if (direction == 1)
                return CurrentCell.Position.Right();
            if (direction == 2)
                return CurrentCell.Position.Down();
            if (direction == 3)
                return CurrentCell.Position.Left();

            throw new NotSupportedException($"Not supported direction {direction}");
        }

        public void Walk(Cell cell)
        {
            CurrentCell.Leave(this);
            CurrentCell = cell;
            CurrentCell.Visit(this);
        }

        public void Feed()
        {
            foreach (var animal in CurrentCell.Animals)
            {
                if (animal == this)
                    continue;

                Eat(animal);
            }
        }

        public Animal? Mate()
        {
            if (Gender.IsMale)
                return null;

            var partner = this.Cellmates.Where(x => x.GetType() == this.GetType() && x.Gender.IsMale != this.Gender.IsMale).FirstOrDefault();
            if (partner == null)
                return null;

            return GiveBirth(Gender.Random());
        }

        public virtual void Die()
        {
            IsDead = true;
        }

        protected abstract void Eat(Animal animal);

        protected abstract Animal GiveBirth(Gender gender);
    }
}
