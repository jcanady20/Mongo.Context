using System;
using System.Text;

namespace Mongo.Context.Example.NameGenerator
{
    public static class PhoneGenerator
    {
        private static Random _random = new Random();
        public static string GenerateRandomPhone()
        {
            var sb = new StringBuilder(12);
            int number = _random.Next(1, 999);
            sb.AppendFormat("{0:D3}", number.ToString());
            sb.Append("-");
            number = _random.Next(1, 999);
            sb.AppendFormat("{0:D3}", number.ToString());
            sb.Append("-");
            number = _random.Next(1000, 9999);
            sb.AppendFormat("{0:D4}", number.ToString());
            return sb.ToString();
        }
    }
}
