using System;

namespace ApiTestingSolution.Helpers
{
    public static class RandomHelper
    {
        private const string CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private static Random Random { get; } = new Random();

        public static int GetRandomInt(int maxValue, int minValue = 0)
        {
            if (minValue > maxValue)
            {
                var temp = minValue;
                minValue = maxValue;
                maxValue = temp;
            }

            var value = Random.Next(minValue, maxValue);

            return value == maxValue ? value - 1 : value;
        }

        public static string GetRandomString(int length = 5)
        {
            char[] buffer = new char[length];

            for (int i = 0; i < length; i++)
            {
                buffer[i] = CHARS[Random.Next(CHARS.Length)];
            }

            return new string(buffer);
        }
    }
}
