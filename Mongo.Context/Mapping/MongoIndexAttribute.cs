using System;

namespace Mongo.Context.Mapping
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class MongoIndexAttribute : Attribute
    {
        private int _timeToLive = -1;

        public bool Descending { get; set; }
        public string[] Keys { get; set; }
        public bool Unique { get; set; }
        /// <summary>
        /// Time to live in seconds
        /// </summary>
        public int TimeToLive
        {
            get
            {
                return _timeToLive;
            }
            set
            {
                _timeToLive = value;
            }
        }
    }
}
