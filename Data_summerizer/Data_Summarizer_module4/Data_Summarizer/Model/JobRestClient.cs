using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Data_Summarizer.Model.BalticDataModel;
using Newtonsoft.Json;
using Serilog;

namespace Data_Summarizer.Model
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

            // Variables that start with "SYS_" are system variables and
            // will be set by CAL execution engine during container creation.

            _senderUid = Environment.GetEnvironmentVariable("SYS_MODULE_INSTANCE_UID"); 
            _batchManagerAckUrl = Environment.GetEnvironmentVariable("SYS_BATCH_MANAGER_ACK_ENDPOINT");
            _batchManagerTokenUrl = Environment.GetEnvironmentVariable("SYS_BATCH_MANAGER_TOKEN_ENDPOINT");
        }

        public async Task SendOutputToken(Dictionary<string, string> handle, bool isFinal, string pinName)
        {
            try { 
                var xOutputToken = new XOutputTokenMessage
                {
                    PinName = pinName,
                    SenderUid = _senderUid,
                    Values = JsonConvert.SerializeObject(handle),
                    BaseMsgUid = _baseMsgUid,
                    IsFinal = isFinal
                };

                Log.Information("Output token:\nPinName: " + pinName + "\nSenderUid: " + _senderUid + "\nBaseMsgUid: " + _baseMsgUid + "\nIsFinal: " + isFinal + "\nValues: " + JsonConvert.SerializeObject(handle));
                Console.WriteLine("Output token:\nPinName: " + pinName + "\nSenderUid: " + _senderUid + "\nBaseMsgUid: " + _baseMsgUid + "\nIsFinal: " + isFinal + "\nValues: " + JsonConvert.SerializeObject(handle));

                var serializedXOutputToken = JsonConvert.SerializeObject(xOutputToken);
                var data = new StringContent(serializedXOutputToken, Encoding.UTF8, "application/json");
                await _httpClient.PostAsync(_batchManagerTokenUrl, data);
            }
            catch (Exception e)
            {
                Log.Information("Error JobRestClient outputToken: " + e.Message);
                Console.WriteLine("Error JobRestClient outputToken: " + e.Message);
            }
        }

        public async void SendAckToken(List<string> TokenMsgUids)
        {
            try
            {
                var ackToken = new XAckToken
                {
                    SenderUid = _senderUid,
                    MsgUids = TokenMsgUids,
                    //IsFinal = true,
                    IsFailed = false,
                    Note = null
                };

                Log.Information("Ack token:\nSenderUid: " + _senderUid + "\nMsgUid: " + _baseMsgUid + "\nIsFailed: " + false + "\nNote: " + null);
                Console.WriteLine("Ack token:\nSenderUid: " + _senderUid + "\nMsgUid: " + _baseMsgUid + "\nIsFailed: " + false + "\nNote: " + null);

                var serializedAckToken = JsonConvert.SerializeObject(ackToken);
                var data = new StringContent(serializedAckToken, Encoding.UTF8, "application/json");

                await _httpClient.PostAsync(_batchManagerAckUrl, data);
            }
            catch (Exception e)
            {
                Log.Information("Error JobRestClient accToken: " + e.Message);
                Console.WriteLine("Error JobRestClient accToken: " + e.Message);
            }
        }
    }
}