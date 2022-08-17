using System.Collections.Generic;

namespace PointFindingModule.Model
{
    public class PinMetadata
    {
        public string PinName { get; set; }
        public string PinType { get; set; }
        public string AccessType { get; set; }
        public Dictionary<string, string> AccessCredential { get; set; }
    }
}
