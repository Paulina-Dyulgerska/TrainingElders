namespace Session2.Positioning
{
    public class PositionGenerator
    {
        private readonly int maxX;
        private readonly int maxY;
        private readonly Random random;

        public PositionGenerator(int maxX, int maxY)
        {
            this.maxX = maxX;
            this.maxY = maxY;
            this.random = new Random();
        }

        public Position GeneratePosition()
        {
            var positionX = this.random.Next(0, maxX);
            var positionY = this.random.Next(0, maxY);

            return new Position(positionX, positionY);
        }
    }
}
