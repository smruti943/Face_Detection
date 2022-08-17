using System.Collections.Generic;

namespace Emotion.Model
{
    public class PinMetadata
    {
        public string PinName { get; set; }
        public string PinType { get; set; }
        public string AccessType { get; set; }
        public string DataMultiplicity { get; set; }
        public string TokenMultiplicity { get; set; }
        public Dictionary<string, string> AccessCredential { get; set; }
    }
}
