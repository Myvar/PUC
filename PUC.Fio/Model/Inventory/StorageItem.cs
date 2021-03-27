namespace PUC.Fio.Model.Inventory
{
    public class StorageItem
    {
        public string MaterialId { get; set; }
        public string MaterialName { get; set; }
        public string MaterialTicker { get; set; }
        public string MaterialCategory { get; set; }
        public string Type { get; set; }
        public float MaterialWeight { get; set; }
        public float MaterialVolume { get; set; }
        public float MaterialAmount { get; set; }
        public float TotalWeight { get; set; }
        public float TotalVolume { get; set; }
    }
}