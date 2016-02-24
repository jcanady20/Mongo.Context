using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Mongo.Context.Example.NameGenerator
{
    public class Names
    {
        public Names()
        { }
        public IList<string> FemaleNames { get; set; }
        public IList<string> MaleNames { get; set; }
        public IList<string> LastNames { get; set; }

        public static Names Create()
        {
            var names = new Names();
            var rawJson = String.Empty;
            var asm = typeof(Names).Assembly;
            using (var strm = asm.GetManifestResourceStream("Mongo.Context.Example.NameGenerator.Names.json"))
            {
                var reader = new StreamReader(strm);
                rawJson = reader.ReadToEnd();
            }
            if (String.IsNullOrEmpty(rawJson))
            {
                throw new ArgumentException();
            }
            names = JsonConvert.DeserializeObject<Names>(rawJson);

            return names;
        }
    }
}
