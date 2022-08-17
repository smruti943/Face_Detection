using Data_Summarizer.Model.BalticDataModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Summarizer.Model
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

            //string user = pins[0].AccessCredential["User"];
            //string password = pins[0].AccessCredential["Password"];
            //string cluster = pins[0].AccessCredential["Cluster"];
            //string databaseName = pins[0].AccessCredential["Database"];

            //string connectionString = "mongodb+srv://" + user + ":" + password + "@" + cluster + ".eqggg.mongodb.net/" + databaseName + "?retryWrites=true&w=majority";

            string user = pins[0].AccessCredential["User"];
            string password = pins[0].AccessCredential["Password"];
            string port = pins[0].AccessCredential["Port"];
            string host = pins[0].AccessCredential["Host"];

            string connectionString = "mongodb://" + user + ":" + password + "@" + host + ":" + port;
            Log.Information("Connection string: " + connectionString);
            Console.WriteLine("Connection string: " + connectionString);

            try
            {
                Log.Information("Mongo connecting");
                Console.WriteLine("Mongo connecting");
                client = new MongoClient(connectionString);

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
            catch(Exception e)
            {
                Log.Information("Error MongoConnection: " + e.Message);
                Console.WriteLine("Error MongoConnection: " + e.Message);
            }

            Log.Information("Mongo data read");
            Console.WriteLine("Mongo data read");
        }

        public string GetFileName()
        {
            return row.fileName;
        }

        public string GetID()
        {
            return row._id.ToString();
        }

        public byte[] GetImageByte()
        {
            return row.fileContent;
        }

        public List<string> GetResults()
        {
            return row.result;
        }

        public void Insert(BsonDocument insert)
        {
            data.InsertOne(insert);

            string idStr = insert["_id"].ToString();
        }

        public string Insert(BsonDocument insert, bool returnID)
        {
            data.InsertOne(insert);
            return insert["_id"].ToString();
        }

        public void Update(BsonDocument updateBson)
        {
            try
            {
                var update = Builders<BsonDocument>.Update.Push("result", updateBson);
                var filter = Builders<BsonDocument>.Filter.Eq("_id", new MongoDB.Bson.ObjectId(tokenData.ObjectId));
                data.UpdateOne(filter, update);
            }
            catch (Exception e)
            {
                Log.Debug("Error Update: " + e.Message);
                Console.WriteLine("Error Update: " + e.Message);
            }
        }

        public void UpdatePoints(List<BsonDocument> points)
        {
            var update = Builders<BsonDocument>.Update.Push("Points", points.ToArray());
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new MongoDB.Bson.ObjectId(tokenData.ObjectId));
            data.UpdateOne(filter, update);
        }
        ////insert to database
        //var insert = new BsonDocument
        //{
        //    {"text", "text2"}
        //};
    }
}
