using PointFindingModule.Model.BalticDataModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace PointFindingModule.Model
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

            string user = pins[0].AccessCredential["User"];
            string password = pins[0].AccessCredential["Password"];
            string host = pins[0].AccessCredential["Host"];
            string port = pins[0].AccessCredential["Port"];



            string connectionString = "mongodb://" + user + ":" + password + "@" + host + ":" + port;
            Log.Information("I try to connect to MongoDB with this connectionString:" + connectionString);
            Console.WriteLine("connectionString: " + connectionString);

            //string user = pins[0].AccessCredential["User"];
            //string password = pins[0].AccessCredential["Password"];
            //string cluster = pins[0].AccessCredential["Cluster"];
            //string databaseName = pins[0].AccessCredential["Database"];

            //string connectionString = "mongodb+srv://" + user + ":" + password + "@" + cluster + ".eqggg.mongodb.net/" + databaseName + "?retryWrites=true&w=majority";

            client = new MongoClient(connectionString);

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

        //public void Insert(BsonDocument insert)
        //{
        //    data.InsertOne(insert);
        //    string idStr = insert["_id"].ToString();
        //}

        internal byte[] GetImageByte()
        {
            return row.fileContent;
        }

        public string GetID()
        {
            return row._id.ToString();
        }
        
       

        public void UpdatePoints(List<BsonDocument> list, string FieldName)
        {
            
            try
            {
                var update = Builders<BsonDocument>.Update.Push(FieldName, list.ToArray());
                var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(tokenData.ObjectId));
                data.UpdateOne(filter, update);
                Log.Debug("Update success");
                Console.WriteLine("Update success");
            }
            catch (Exception e)
            {
                Log.Debug("Error Update: " + e.Message);
                Console.WriteLine("Error Update: " + e.Message);
              
            }
        }

        public void UpdateImage(byte[] image, string fieldName)
        {
           

            var update = Builders<BsonDocument>.Update.Set(fieldName, image);
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(tokenData.ObjectId));
            
            data.UpdateOne(filter, update);
        }


    }
}
