using System;
using System.IO;
using System.Collections.Generic;
using PointFindingModule.Model.BalticDataModel;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using DlibDotNet;
using System.Runtime.Serialization.Formatters.Binary;
using Serilog;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


namespace PointFindingModule.Model
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

                var pinConfig = File.ReadAllText(Environment.GetEnvironmentVariable("SYS_PIN_CONFIG_FILE_PATH"));
                Log.Information("I got this pinConfig:" + pinConfig);
                Console.WriteLine(pinConfig);

                //var pinConfig = File.ReadAllText("pin_Examples.json");
                var pins = JsonConvert.DeserializeObject<List<PinMetadata>>(pinConfig);



                MongoConnection connection = new MongoConnection(pins, token);

              

                byte[] imageByte = connection.GetImageByte();
                BsonDocument points;
                List<BsonDocument> pointToFile = new List<BsonDocument>();
                List<List<BsonDocument>> pointsToMongo = new List<List<BsonDocument>>();
                Console.WriteLine("point fainding start");
                Log.Information("point finding start");
                int NumberOfPoints = 0;
                int NumberOfFace = 0;
                var img = Dlib.LoadPng<RgbPixel>(imageByte);
                //Dlib.SaveJpeg(img, "output.jpg");

                using (var fd = Dlib.GetFrontalFaceDetector())
                using (var sp = ShapePredictor.Deserialize("shape_predictor_68_face_landmarks.dat"))
                {
                    // load input image
                    //var img = Dlib.LoadPng<RgbPixel>(imageByte);
                    Console.WriteLine("load input image");
                    Log.Information("load input image");

                    // find all faces in the image
                    var faces = fd.Operator(img);
                    foreach (var face in faces)
                    {
                        pointToFile.Clear();
                        NumberOfFace++;
                        // find the landmark points for this face
                        var shape = sp.Detect(img, face);
                        

                        // draw the landmark points on the image
                        for (var i = 0; i < shape.Parts; i++)
                        {
                            var point = shape.GetPart((uint)i);
                            points = point.ToBsonDocument();
                            var rect = new Rectangle(point);
                            //Dlib.DrawRectangle(img, rect, color: new RgbPixel(255, 255, 0), thickness: 4);
                            
                            pointToFile.Add(points);
                            NumberOfPoints++;

                        }
                        
                        
                        //pointsToMongo.Add(pointToFile);
                        Dlib.DrawRectangle(img, face, color: new RgbPixel(0, 255, 255), thickness: 4);
                        
                        connection.UpdatePoints(pointToFile, "points");
                    }
                    
                   
                     
                }
                Dlib.SavePng(img, "output.png");
                //connection.UpdatePoints(pointsToMongo, "Points");
                byte[] imageByte2 = File.ReadAllBytes("output.png");
                

                connection.UpdateImage(imageByte2, "fileContent");

                byte[] imageTest = connection.GetImageByte();
                var imgTest = Dlib.LoadPng<RgbPixel>(imageByte);
                Dlib.SavePng(imgTest, "output2.png");

                Console.WriteLine("Point Finding completed");
                Log.Information("Point Finding completed");
                Console.WriteLine("----------------------");
                Log.Information("------------------------");
                Console.WriteLine("Number of finding points: " + NumberOfPoints);
                Log.Information("Number of finding points: " + NumberOfPoints);


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
                Log.Information("outputHandle: " + outputHandle);
                Console.WriteLine("outputHandle: " + outputHandle);
                JobStatus.Status = ComputationStatus.Idle;
                JobStatus.JobProgress = 100;
                Log.Information("Sending output token");
                Console.WriteLine("Sending output token");
                Task t = restClient.SendOutputToken(outputHandle, true, pins[1].PinName);
                t.Wait();
                List<string> MsgUids = new List<string>();
                MsgUids.Add(token.MsgUid);
                Log.Information("Sending ack token");
                Console.WriteLine("Sending ack token");
                restClient.SendAckToken(MsgUids, "", false);
            }
            catch (Exception e)
            {
                var restClient = new JobRestClient(token.MsgUid);
                List<string> MsgUids = new List<string>();
                MsgUids.Add(token.MsgUid);
                Log.Debug("Error ComputationModule: " + e.Message);
                Console.WriteLine("Error ComputationModule: " + e.Message);
                restClient.SendAckToken(MsgUids, "the operation failed, the problem is:" + e, true);
                
            }
        }
        
        



        public static void DataProcessing2(XInputTokenMessage token)
        {
            ///var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(token.Values);

            DataProcessing1(token);
        }
    }
}
