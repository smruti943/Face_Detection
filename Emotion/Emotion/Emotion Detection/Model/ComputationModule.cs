using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Emotion.Model.BalticDataModel;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Serilog;

namespace Emotion.Model
{
    public static class ComputationModule
    {
        public static JobStatus JobStatus = new JobStatus
        {
            JobInstanceUid = Environment.GetEnvironmentVariable("SYS_MODULE_INSTANCE_UID"),
            //JobInstanceUid = Environment.GetEnvironmentVariable("1"),
            JobProgress = -1,
            Status = ComputationStatus.Idle
        };

        public static void DataProcessing1(XInputTokenMessage token)
        {
            try
            {
                Log.Information("Data processing started");
                Console.WriteLine("Data processing started");

                JobStatus.Status = ComputationStatus.Working;
                JobStatus.JobProgress = 0;

                //var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(token.Values);

                // token końcowy potwierdzający
                // ######################################################################
                // # PUT YOUR CODE HERE                                                 #
                // ######################################################################
                // # 1) Use pin_input from config and value variable to read data       #
                // # 2) Preform operation on data                                       #
                // # 3) Send output data to given output,                               #
                // #    use pin_output from config to get credentials to output source  #
                // ######################################################################

                var pinConfig = System.IO.File.ReadAllText(Environment.GetEnvironmentVariable("SYS_PIN_CONFIG_FILE_PATH"));
                //var pinConfig = System.IO.File.ReadAllText("pin_Examples.json");
                var pins = JsonConvert.DeserializeObject<List<PinMetadata>>(pinConfig);

                MongoConnection connection = new MongoConnection(pins, token);

                Log.Information("Call emotion logic");
                Console.WriteLine("Call emotion logic");

                FaceDetactionProcess faceDetactionProcess = new FaceDetactionProcess();
                List<string> emotionDetaction = faceDetactionProcess.FaceDetactionLogic(pins, token);



                // Create an instance of JobRestClient that will be used for sending a proper token message
                //  after data processing will finish.
                var restClient = new JobRestClient(token.MsgUid);

                TokenData tokenData = JsonConvert.DeserializeObject<TokenData>(token.Values);

                // example of outputHandle
                var outputHandle = new Dictionary<string, string>
                {
                    { "FileName", tokenData.FileName },
                    { "Database", tokenData.Database },
                    { "Collection", tokenData.Collection },
                    { "ObjectId", tokenData.ObjectId }
                };

                JobStatus.Status = ComputationStatus.Idle;
                JobStatus.JobProgress = 100;
                PinMetadata pinOutput = null;
                foreach (PinMetadata pin in pins)
                {
                    if (pin.PinName == "Output")
                        pinOutput = pin;
                }
                if (pinOutput != null)
                {

                    Log.Information("Sending output token");
                    Console.WriteLine("Sending output token");
                    Task t = restClient.SendOutputToken(outputHandle, true, "Output");
                    t.Wait();
                    List<string> MsgUids = new List<string>();
                    MsgUids.Add(token.MsgUid);
                    Log.Information("Sending ack token");
                    Console.WriteLine("Sending ack token");
                    restClient.SendAckToken(MsgUids);
                }
                else
                {
                    Log.Debug("Output pin not found");
                    Console.WriteLine("Output pin not found");
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Error ComputationModule: " + ex.Message);
                Console.WriteLine("Error Computation Module:" + ex.Message);
            }
        }

        public static void DataProcessing2(XInputTokenMessage token)
        {
            //var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(token.Values);

            DataProcessing1(token);
        }
    }
}
