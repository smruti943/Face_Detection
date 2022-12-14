using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emotion.Model
{
    public class MongoData
    {
        public ObjectId _id { get; set; }
        public string fileName { get; set; }
         public string result { get; set; }
        public byte[] fileContent { get; set; }
        public List<List<dynamic>> points { get; set; }
    }
}
