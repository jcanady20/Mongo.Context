using System;
using System.Linq;

namespace Mongo.Context.Example.NameGenerator
{
    public class NameGenerator
    {
        private static readonly Random _random = new Random();
        private static readonly object _lockObj = new object();
        private static NameGenerator _instance;
        private readonly Names _names;
        private NameGenerator()
        {
            _names = new Names();
        }

        public static NameGenerator Instance
        {
            get
            {
                lock (_lockObj)
                {
                    if (_instance == null)
                    {
                        _instance = new NameGenerator();
                    }
                }
                return _instance;
            }
        }

        public string GenerateFullName(Gender gender)
        {
            return string.Format("{0} {1}", GenerateFirstName(gender), GenerateLastName());
        }

        public string GenerateLastName()
        {
            var cnt = _names.LastNames.Count();
            return _names.LastNames[_random.Next(0, cnt)];
        }

        private string GenerateMaleName()
        {
            var cnt = _names.MaleNames.Count();
            return _names.MaleNames[_random.Next(0, cnt)];
        }

        private string GenerateFemaleName()
        {
            var cnt = _names.FemaleNames.Count();
            return _names.FemaleNames[_random.Next(0, cnt)];
        }

        public string GenerateFirstName(Gender gender)
        {
            if (gender == Gender.Male)
                return GenerateMaleName();
            return GenerateFemaleName();
        }
    }
}
