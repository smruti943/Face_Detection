using Emotion.Model;
using Emotion.Model.BalticDataModel;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Emotion
{
    public class FaceDetactionProcess
    {

        Boolean facedetected = false;
        EmotionDetectionAsset eda;
        Int32 counter = 0;
        string database = System.IO.Directory.GetCurrentDirectory() + "\\LandMarkFiles\\shape_predictor_68_face_landmarks.dat";
        string furia = System.IO.Directory.GetCurrentDirectory() + "/LandMarkFiles/FURIA Fuzzy Logic Rules.txt";
        string dlibWrapper = System.IO.Directory.GetCurrentDirectory() + "\\bin\\Debug\\netcoreapp3.1\\publish"; //"\\bin\\x64\\Debug\\netcoreapp3.1";
        public FaceDetactionProcess()
        {
            eda = new EmotionDetectionAsset();
            eda.Initialize(dlibWrapper, database);
            eda.ParseRules(File.ReadAllLines(furia));
            (eda.Settings as EmotionDetectionAssetSettings).SuppressSpikes = true;
        }
        public List<string> FaceDetactionLogic(List<PinMetadata> pins, XInputTokenMessage token)
        {
            List<string> emotionDetaction = new List<string>(); ;
            try
            {
                Console.WriteLine("Call FaceDetactionLogic");
                MongoConnection connection = new MongoConnection(pins, token);
                var filter = Builders<BsonDocument>.Filter.Eq("id", connection.GetID());

                List<List<dynamic>> imagePoints = connection.GetPoints(filter);
                string emotionResult = connection.GetResult(filter);

                //byte[] imageByte = connection.GetImageByte();
                //using (var ms = new MemoryStream(imageByte))
                //{
                //    Image img = Image.FromStream(ms);

                //    emotionDetaction = ProcessImageIntoEmotions(img, true);
                //    //ProcessImageIntoEmotions(img, true);
                //    //img.Save("E:\\Anei\\Smruti\\module_example\\C# test\\img\\output.jpg", ImageFormat.Jpeg);
                //}
                Console.WriteLine("Pass Image Point");
                Log.Information("Pass Image Point");
                emotionDetaction = ShowEmotion(imagePoints);
                if (!string.IsNullOrEmpty(emotionResult))
                {
                    connection.UpdateEmotion(emotionDetaction);
                }
                else
                {
                    connection.InsertEmotion(emotionDetaction);
                }
                Console.WriteLine("Get result");
            }
            catch (Exception ex)
            {
                Log.Information("Emotion detaction logic error: " + ex.Message);
                Console.WriteLine("Emotion detaction logic error: " + ex.Message);
            }
            return emotionDetaction;
        }
        /// <summary>
        /// Process the image into emotions.
        /// </summary>
        ///
        /// <param name="img">      The image. </param>
        /// <param name="redetect"> True to redetect. </param>
        public string ProcessImageIntoEmotions(Image img, Boolean redetect)
        {
            //! Skipping does not seem to work properly
            //
            string s = "";
            if (redetect)
            {
                facedetected = eda.ProcessImage(img);

                if (facedetected)
                {
                    counter++;

                    //! Process detect faces.
                    //
                    if (eda.ProcessFaces())
                    {
                        s = ShowEmotion();
                    }
                }
            }
            return s;
        }
        public string ShowEmotion()
        {
            string s = "";
            if (eda.ProcessLandmarks())
            {
                foreach (String emotion in eda.Emotions)
                {
                    for (Int32 i = 0; i < eda.Faces.Count; i++)
                    {
                        String emo = String.Format("{0:0.00}", eda[i, emotion]);
                        if (double.Parse(emo) > 0)
                        {
                            s = emotion;
                        }
                    }
                }
            }
            return s;
        }
        public List<string> ShowEmotion(List<List<dynamic>> imagePoints)
        {
            try
            {
                string s = "";
                List<string> emotionResult = new List<string>();
                foreach (var item in imagePoints)
                {
                    if (eda.ProcessLandmarks(item))
                    {
                        foreach (String emotion in eda.Emotions)
                        {
                            for (Int32 i = 0; i < item.Count; i++)
                            {
                                String emo = String.Format("{0:0.00}", eda[i, emotion]);
                                if (double.Parse(emo) > 0)
                                {
                                    s = emotion;
                                    break;
                                }
                            }
                            if (!string.IsNullOrEmpty(s)) { break; }
                        }
                    }
                    emotionResult.Add(s);
                }
                return emotionResult;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
