using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Summarizer.Model
{
    public class ResultsJson
    {
        public string fileName { get; set; }
        public int numberOfFacesDetected { get; set; }
        public Dictionary<string, string> result { get; set; }

        public ResultsJson() 
        {
            result = new Dictionary<string, string>();
        }

        public void AddResult(string number, string emotion)
        {
            result.Add(number, emotion);
        }
    }
}
