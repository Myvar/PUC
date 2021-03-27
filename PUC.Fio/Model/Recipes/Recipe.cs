using System.Collections.Generic;

namespace PUC.Fio.Model.Recipes
{
    public class Recipe
    {
        public string BuildingTicker { get; set; }
        public string RecipeName { get; set; }

        public List<TickerAmountPair> Inputs { get; set; } = new List<TickerAmountPair>();
        public List<TickerAmountPair> Outputs { get; set; } = new List<TickerAmountPair>();
        public int TimeMs { get; set; }
    }
}