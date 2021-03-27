using System.Collections.Generic;

namespace PUC.Fio.Model.Buildings
{
    public class Building
    {
        private sealed class TickerEqualityComparer : IEqualityComparer<Building>
        {
            public bool Equals(Building x, Building y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Ticker == y.Ticker;
            }

            public int GetHashCode(Building obj)
            {
                return (obj.Ticker != null ? obj.Ticker.GetHashCode() : 0);
            }
        }

        public static IEqualityComparer<Building> TickerComparer { get; } = new TickerEqualityComparer();

        public List<BuildingCost> BuildingCosts { get; set; } = new List<BuildingCost>();
        public List<BuildingRecipe> Recipes { get; set; } = new List<BuildingRecipe>();

        public string Name { get; set; }
        public string Ticker { get; set; }
        public string Expertise { get; set; }
        public string Pioneers { get; set; }
        public string Settlers { get; set; }
        public string Technicians { get; set; }
        public string Engineers { get; set; }
        public string Scientists { get; set; }
        public string AreaCost { get; set; }
        public string UserNameSubmitted { get; set; }
        public string Timestamp { get; set; }
    }
}