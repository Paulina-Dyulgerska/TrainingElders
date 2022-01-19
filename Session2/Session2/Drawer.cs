using Discworld;
using Discworld.Animals;
using System.Text;

namespace Session2
{
    public static class Drawer
    {
        public static string Draw(World world)
        {
            var herbivoresCount = world.Cells.SelectMany(x => x.Animals).Count(x => x.GetType() == typeof(Herbivore));
            var carnivoresCount = world.Cells.SelectMany(x => x.Animals).Count(x => x.GetType() == typeof(Carnivore));
            var worldWidth = world.Cells.Select(x=>x.Position.Y).Max();

            var stringBuilder = new StringBuilder();
            var countWidth = 0;

            foreach (var cell in world.Cells)
            {
                if (cell.Animals.Any())
                {
                    foreach (var animal in cell.Animals)
                    {
                        stringBuilder.Append($"{animal.GetDrawing()}");

                        if (cell.Animals.Count() > 1)
                        {
                            stringBuilder.Append('|');
                        }
                        else
                        {
                            stringBuilder.Append(' ');
                        }
                    }
                }
                else
                {
                    stringBuilder.Append($"- ");
                }

                countWidth++;
                if (countWidth == worldWidth)
                {
                    stringBuilder.AppendLine();
                    countWidth = 0;
                }
            }
            stringBuilder.AppendLine($"-----World has {herbivoresCount} herbivores and {carnivoresCount} carnivores-----");

            return stringBuilder.ToString();
        }
    }
}
