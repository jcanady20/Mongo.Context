using System;
using System.Collections.Generic;

namespace Mongo.Context.Mapping;
public class MongoIndex
{
    public MongoIndex()
    {
        Keys = new List<string>();
        TimeToLive = -1;
    }
    public ICollection<string> Keys { get; set; }
    public bool Descending { get; set; }
    public bool Unique { get; set; }
    public int TimeToLive { get; set; }
}
