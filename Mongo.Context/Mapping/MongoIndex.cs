using System;
using System.Collections.Generic;

namespace Mongo.Context.Mapping
{
    public class MongoIndex
    {
        public MongoIndex()
        {
            Keys = new List<string>();
        }

        public ICollection<string> Keys { get; set; }

        public bool Desending { get; set; }
        public bool Unique { get; set; }
        public int TimeToLive { get; set; }
    }
}
