using System.Collections.Generic;

namespace Data_Summarizer.Model.BalticDataModel
{
    public class XAckToken
    {
        public List<string> MsgUids { get; set; }
        public string SenderUid { get; set; }
        public bool IsFinal { get; set; }
        public bool IsFailed { get; set; }
        public string Note { get; set; }
    }
}