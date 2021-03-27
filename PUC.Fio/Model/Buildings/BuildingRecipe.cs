using System.Collections.Generic;

namespace PUC.Fio.Model.Buildings
{
    public class BuildingRecipe
    {
        public List<BuildingCost> Inputs { get; set; } = new List<BuildingCost>();
        public List<BuildingCost> Outputs { get; set; } = new List<BuildingCost>();
        public int DurationMs { get; set; }
        public string RecipeName { get; set; }
    }
}