namespace Discworld.Animals
{
    public abstract class Animal
    {
        public Animal(Cell cell, Gender gender)
        {
            if (cell is null) throw new ArgumentNullException(nameof(cell));
            if (gender is null) throw new ArgumentNullException(nameof(gender));

            cell.Visit(this);
            CurrentCell = cell;
            Gender = gender;
        }

        public Cell CurrentCell { get; protected set; }

        public bool IsDead { get; private set; }

        public bool IAmFood { get; protected set; }

        public Gender Gender { get; protected set; }

        private IEnumerable<Animal> Cellmates => CurrentCell.Animals.Where(x => x != this);

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
                // Do not eat yourself
                if (animal == this)
                    continue;

                Eat(animal);
            }
        }

        public void FindPartner()
        {
            var partner = this.Cellmates.Where(x => x.GetType() == this.GetType() && x.Gender.IsMale != x.Gender.IsMale).FirstOrDefault();
            if (partner == null) return;


        }

        public void Die()
        {
            //CurrentCell.Leave(this);
            IsDead = true;
        }

        public char GetDrawing()
        {
            return GetType().Name[0];
        }

        protected abstract void Eat(Animal animal);
    }
}
