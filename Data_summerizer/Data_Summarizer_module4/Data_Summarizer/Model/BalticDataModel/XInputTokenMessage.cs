using System.Collections.Generic;

namespace Data_Summarizer.Model.BalticDataModel
{
    public class XInputTokenMessage
    {
        public string MsgUid { get; set; }
        public string PinName { get; set; }
        public string Values { get; set; }
        public string AccessType { get; set; }
        public IEnumerable<XSeqToken> TokenSeqStack { get; set; }
    }
}