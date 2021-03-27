using System.Collections.Generic;

namespace PUC.Fio.Model.Inventory
{
    public class Storage
    {
        public List<StorageItem> StorageItems { get; set; } = new List<StorageItem>();
        public string StorageId { get; set; }
        public string AddressableId { get; set; }
        public string Name { get; set; }
        public float WeightLoad { get; set; }
        public float WeightCapacity { get; set; }
        public float VolumeLoad { get; set; }
        public float VolumeCapacity { get; set; }
        public bool FixedStore { get; set; }
        public string Type { get; set; }
    }
}