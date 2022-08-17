using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Face_Detection.Model
{
    public class MongoData
    {
        public ObjectId _id { get; set; }

        public string fileName { get; set; }

        public byte[] fileContent { get; set; }
    }
}
