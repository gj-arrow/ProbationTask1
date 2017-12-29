using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkApi.Utils
{
    public class RandomUtils
    {
        private static readonly Random random = new Random();
        private const string Chars = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string RandomString(int length)
        {
            return new string(Enumerable.Repeat(Chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
