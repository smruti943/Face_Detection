using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using PointFindingModule.Model.BalticDataModel;
using Newtonsoft.Json;
using Serilog;
using System.Threading.Tasks;

namespace PointFindingModule.Model
{
    public class JobRestClient
    {
        private HttpClient _httpClient;
        private string _batchManagerAckUrl;
        private string _batchManagerTokenUrl;
        private string _senderUid;
        private string _baseMsgUid;

        public JobRestClient(string baseMsgUid)
        {
            _baseMsgUid = baseMsgUid;
            _httpClient = new HttpClient();

            //Variables that start with "SYS_" are system variables and
            // will be set by CAL execution engine during container creation.

            _senderUid = Environment.GetEnvironmentVariable("SYS_MODULE_INSTANCE_UID");
            _batchManagerAckUrl = Environment.GetEnvironmentVariable("SYS_BATCH_MANAGER_ACK_ENDPOINT");
            _batchManagerTokenUrl = Environment.GetEnvironmentVariable("SYS_BATCH_MANAGER_TOKEN_ENDPOINT");
            //_senderUid = "1";
            //_batchManagerAckUrl = "49157";
            //_batchManagerTokenUrl = "49157";
        }

        public async Task SendOutputToken(Dictionary<string, string> handle, bool isFinal, string pinName)
        {
            var xOutputToken = new XOutputTokenMessage
            {
                PinName = "Output",
                SenderUid = _senderUid,
                Values = JsonConvert.SerializeObject(handle),
                BaseMsgUid = _baseMsgUid,
                IsFinal = isFinal
            };
            var serializedXOutputToken = JsonConvert.SerializeObject(xOutputToken);
            var data = new StringContent(serializedXOutputToken, Encoding.UTF8, "application/json");
            await _httpClient.PostAsync(_batchManagerTokenUrl, data);

            Console.WriteLine("Output token:\nPinName: " + "Output" + "\nSenderUid: " + _senderUid + "\nBaseMsgUid: " + _baseMsgUid + "\nIsFinal: " + isFinal + "\nValues: " + JsonConvert.SerializeObject(handle));
            Log.Information("Output token:\nPinName: " + "Output" + "\nSenderUid: " + _senderUid + "\nBaseMsgUid: " + _baseMsgUid + "\nIsFinal: " + isFinal + "\nValues: " + JsonConvert.SerializeObject(handle));
        }

        public async void SendAckToken(List<string> TokenMsgUids, string note, bool IsFailed)
        {
            var ackToken = new XAckToken
            {
                SenderUid = _senderUid,
                MsgUids = TokenMsgUids,
                IsFailed = IsFailed,
                Note = note
            };

            var serializedAckToken = JsonConvert.SerializeObject(ackToken);
            var data = new StringContent(serializedAckToken, Encoding.UTF8, "application/json");

            await _httpClient.PostAsync(_batchManagerAckUrl, data);

            Console.WriteLine("Ack token:\nSenderUid: " + _senderUid + "\nMsgUid: " + TokenMsgUids + "\nIsFailed: " + IsFailed + "\nNote: " + note);
            Log.Information("Ack token:\nSenderUid: " + _senderUid + "\nMsgUid: " + TokenMsgUids + "\nIsFailed: " + IsFailed + "\nNote: " + note);
        }
    }
}