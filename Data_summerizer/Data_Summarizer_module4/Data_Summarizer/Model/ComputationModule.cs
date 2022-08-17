using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Data_Summarizer.Model.BalticDataModel;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Serilog;

namespace Data_Summarizer.Model
{
    public static class ComputationModule
    {
        public static JobStatus JobStatus = new JobStatus
        {
            JobInstanceUid = Environment.GetEnvironmentVariable("SYS_MODULE_INSTANCE_UID"),
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

                List<string> results = connection.GetResults();
                ResultsJson resultsJson = new ResultsJson();
                resultsJson.fileName = connection.GetFileName();

                int i_results = 0;

                foreach(string result in results)
                {
                    i_results++;
                    resultsJson.result.Add(i_results.ToString(), result);
                }

                resultsJson.numberOfFacesDetected = i_results;

                //string jsonString = JsonConvert.SerializeObject(resultsJson);

                string fileName = connection.GetFileName();
                int index = fileName.IndexOf(".");
                if (index > 0)
                    fileName = fileName.Substring(0, index);

                using (StreamWriter file = File.CreateText(fileName + ".json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, resultsJson);
                }

                byte[] binaryContent = File.ReadAllBytes(fileName + ".json");

                var insert = new BsonDocument
                {
                    {"fileName", fileName + ".json"},
                    {"fileContent", binaryContent }
                };

                string _id = connection.Insert(insert, true);

                // Create an instance of JobRestClient that will be used for sending a proper token message
                //  after data processing will finish.
                var restClient = new JobRestClient(token.MsgUid);
                Dictionary<string, string> outputHandle;
                Dictionary<string, string> outputHandle2;

                TokenData tokenData = JsonConvert.DeserializeObject<TokenData>(token.Values);

                // example of outputHandle
                outputHandle = new Dictionary<string, string>
                {
                    { "FileName", tokenData.FileName },
                    { "Database", tokenData.Database },
                    { "Collection", tokenData.Collection },
                    { "ObjectId", tokenData.ObjectId }
                };

                outputHandle2 = new Dictionary<string, string>
                {
                    { "FileName", connection.GetFileName() + ".json" },
                    { "Database", tokenData.Database },
                    { "Collection", tokenData.Collection },
                    { "ObjectId", _id }
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
                    Task t2 = restClient.SendOutputToken(outputHandle2, true, "Output2");
                    t2.Wait();
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
            catch (Exception e)
            {
                Log.Debug("Error ComputationModule: " + e.Message);
                Console.WriteLine("Error ComputationModule: " + e.Message);
            }
        }

        public static void DataProcessing2(XInputTokenMessage token)
        {
            //var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(token.Values);
            
            DataProcessing1(token);
        }

        
    }
}
