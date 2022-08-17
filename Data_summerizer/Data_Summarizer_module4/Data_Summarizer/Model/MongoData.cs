using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Summarizer.Model
{
    public class MongoData
    {
        public ObjectId _id { get; set; }

        public string fileName { get; set; }

        public byte[] fileContent { get; set; }
        
        public List<List<dynamic>> points { get; set; }

        public List<String> result { get; set; }
    }
}
