namespace Discworld
{
    public record Gender
    {
        public bool IsMale { get; init; }

        public static readonly Gender Male = new Gender { IsMale = true };
        public static readonly Gender Female = new Gender { IsMale = false };
        public static Gender Random()
        {
            var gender = System.Random.Shared.Next(0, 2);
            if (gender == 0)
                return Male;

            return Female;  
        }
    }
}
