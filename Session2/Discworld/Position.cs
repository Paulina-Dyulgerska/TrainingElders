namespace Discworld
{
    public record Position
    {
        public int X { get; init; }

        public int Y { get; init; }

        public Position Up()
        {
            return this with { Y = Y - 1 };
        }

        public Position Down()
        {
            return this with { Y = Y + 1 };
        }

        public Position Left()
        {
            return this with { X = X - 1 };
        }

        public Position Right()
        {
            return this with { X = X + 1 };
        }
    }
}
