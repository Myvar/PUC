using System.Collections.Generic;

namespace PUC.Fio.Model.Planets
{
    public class Planet
    {
        public List<Resource> Resources { get; set; } = new List<Resource>();
        public List<BuildRequirement> BuildRequirements { get; set; } = new List<BuildRequirement>();
        public string PlanetId { get; set; }
        public string PlanetNaturalId { get; set; }
        public string PlanetName { get; set; }
        public string SystemId { get; set; }
        public float Gravity { get; set; }
        public float MagneticField { get; set; }
        public float Mass { get; set; }
        public float MassEarth { get; set; }
        public float Pressure { get; set; }
        public float Radiation { get; set; }
        public float Radius { get; set; }
        public float Sunlight { get; set; }
        public bool Surface { get; set; }
        public float Temperature { get; set; }
        public float Fertility { get; set; }
    }
}