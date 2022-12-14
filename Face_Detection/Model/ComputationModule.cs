using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DlibDotNet;
using Face_Detection.Model.BalticDataModel;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Serilog;

namespace Face_Detection.Model
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

                Log.Information("Getting immage bytes");
                Console.WriteLine("Getting immage bytes");

                byte[] imageByte = connection.GetImageByte();
                bool faceDetected;

                Log.Information("Face detection starting");
                Console.WriteLine("Face detection starting");
                using (var fd = Dlib.GetFrontalFaceDetector())
                {
                    Log.Information("Loading immage");
                    Console.WriteLine("Loading immage");
                    var img = Dlib.LoadPng<RgbPixel>(imageByte);
                    Log.Information("Image size: " + img.Size);
                    Console.WriteLine("Image size: " + img.Size);

                    var faces = fd.Operator(img);
                    Log.Information("Faces count: " + faces.Length);
                    Console.WriteLine("Faces count: " + faces.Length);
                    if (faces.Length > 0)
                        faceDetected = true;
                    else
                        faceDetected = false;
                }
                if (faceDetected == true)
                {
                    Log.Information("Face detected");
                    Console.WriteLine("Face detected");
                }
                else
                {
                    Log.Information("No face detected");
                    Console.WriteLine("No face detected");
                    var update = new BsonDocument
                {
                    {"result", "No_face_detected"}
                };
                    connection.Update(update);
                }

                Log.Information("Face detection ended");
                Console.WriteLine("Face detection ended");

                // Create an instance of JobRestClient that will be used for sending a proper token message
                //  after data processing will finish.
                var restClient = new JobRestClient(token.MsgUid);
                Dictionary<string, string> outputHandle;

                TokenData tokenData = JsonConvert.DeserializeObject<TokenData>(token.Values);

                // example of outputHandle
                outputHandle = new Dictionary<string, string>
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
                    Task t = restClient.SendOutputToken(outputHandle, true, pins[1].PinName);
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
