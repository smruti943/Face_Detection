using System.Collections.Generic;

namespace Emotion.Model.BalticDataModel
{
    public class XAckToken
    {
        public List<string> MsgUids { get; set; }
        public string SenderUid { get; set; }
        public bool IsFinal { get; set; }
        public string Note { get; set; }
    }
}