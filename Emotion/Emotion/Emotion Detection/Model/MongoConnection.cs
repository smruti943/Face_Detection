using Emotion.Model.BalticDataModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emotion.Model
{
    public class MongoConnection
    {
        List<PinMetadata> pins;
        XInputTokenMessage token;
        MongoClient client;
        IMongoDatabase database;
        IMongoCollection<BsonDocument> data;
        MongoData row;
        TokenData tokenData;

        public MongoConnection(List<PinMetadata> pins, XInputTokenMessage token)
        {
            this.pins = pins;
            this.token = token;
            Console.WriteLine("Start Mongo Connection");
            string user = pins[0].AccessCredential["User"];
            string password = pins[0].AccessCredential["Password"];
            // string claster = pins[0].AccessCredential["Cluster"];
            //string databaseName = pins[0].AccessCredential["Database"];
            string port = pins[0].AccessCredential["Port"];
            string host = pins[0].AccessCredential["Host"];

            //string connectionString = "mongodb+srv://" + user + ":" + password + "@" + claster + ".eqggg.mongodb.net/" + databaseName + "?retryWrites=true&w=majority";
            string connectionString = "mongodb://" + user + ":" + password + "@" + host + ":" + port;
            Log.Information("Connection string: " + connectionString);
            Console.WriteLine("Connection string: " + connectionString);
            try
            {
                Log.Information("Mongo connecting");
                Console.WriteLine("Mongo connecting");
                client = new MongoClient(connectionString);
                Console.WriteLine("Mongo Connected");
                tokenData = JsonConvert.DeserializeObject<TokenData>(token.Values);
                //pobieranie danych
                database = client.GetDatabase(tokenData.Database);
                data = database.GetCollection<BsonDocument>(tokenData.Collection);
                Log.Information("Mongo connected");
                Console.WriteLine("Mongo connected");

                Log.Information("Reading documents with id: " + tokenData.ObjectId);
                Console.WriteLine("Reading documents with id: " + tokenData.ObjectId);

                var documents = data.Find($"{{ _id: ObjectId('{tokenData.ObjectId}') }}").FirstOrDefault();
                Log.Information("Deserializing document");
                Console.WriteLine("Deserializing document");
                row = BsonSerializer.Deserialize<MongoData>(documents.ToBsonDocument());
            }
            catch (Exception e)
            {
                Log.Information("Error MongoConnection: " + e.Message);
                Console.WriteLine("Error MongoConnection: " + e.Message);
            }

            Log.Information("Mongo data read");
            Console.WriteLine("Mongo data read");
        }

        public string GetText()
        {
            return row.fileName;
        }

        public void InsertEmotion(List<string> emotionValue)
        {
            try
            {
                var update = Builders<BsonDocument>.Update.Set("result", emotionValue.ToArray());
                var filter = Builders<BsonDocument>.Filter.Eq("_id", new MongoDB.Bson.ObjectId(tokenData.ObjectId));
                data.UpdateOne(filter, update);
                Log.Debug("Update success");
                Console.WriteLine("Update success");
            }
            catch (Exception e)
            {
                Log.Debug("Error Update: " + e.Message);
                Console.WriteLine("Error Update: " + e.Message);

            }
            //try
            //{
            //    var update = Builders<BsonDocument>.Update.Set("result", emotionValue.ToString());
            //    var filter = Builders<BsonDocument>.Filter.Eq("_id", new MongoDB.Bson.ObjectId(tokenData.ObjectId));
            //    data.UpdateOne(filter, update);
            //    Log.Debug("Add Emotion to Mongo");
            //    Console.WriteLine("Add Emotion to Mongo");
            //}
            //catch (Exception e)
            //{
            //    Log.Debug("InsertEmotion Insert: " + e.Message);
            //    Console.WriteLine("InsertEmotion Insert: " + e.Message);
            //}
        }
        public void UpdateEmotion(List<string> emotionValue)
        {
            try
            {
                var update = Builders<BsonDocument>.Update.Push("result", emotionValue.ToArray());
                var filter = Builders<BsonDocument>.Filter.Eq("_id", new MongoDB.Bson.ObjectId(tokenData.ObjectId));
                data.UpdateOne(filter, update);
                Log.Debug("Update success");
                Console.WriteLine("Update success");
            }
            catch (Exception e)
            {
                Log.Debug("Error Update: " + e.Message);
                Console.WriteLine("Error Update: " + e.Message);

            }
            //try
            //{
            //    var update = Builders<BsonDocument>.Update.Push("result", emotionValue.ToString());
            //    var filter = Builders<BsonDocument>.Filter.Eq("_id", new MongoDB.Bson.ObjectId(tokenData.ObjectId));
            //    data.FindOneAndUpdateAsync(filter, update);
            //    Console.WriteLine("Add Emotion to Mongo");
            //    Log.Debug("Add Emotion to Mongo");
            //}
            //catch (Exception e)
            //{
            //    Log.Debug("InsertEmotion Insert: " + e.Message);
            //    Console.WriteLine("InsertEmotion Insert: " + e.Message);
            //}
        }
        public byte[] GetImageByte()
        {
            byte[] ds = new byte[10];
            return ds;// row.image;
        }
        public void Update(FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update)
        {
            data.FindOneAndUpdateAsync(filter, update);
        }
        public List<List<dynamic>> GetPoints(FilterDefinition<BsonDocument> filter)
        {
            //data.Find(filter).ToBsonDocument();
            return row.points;

        }
        public string GetResult(FilterDefinition<BsonDocument> filter)
        {
            //data.Find(filter).ToBsonDocument();
            return row.result;

        }
        public string GetID()
        {
            return row._id.ToString();
        }
    }
}
