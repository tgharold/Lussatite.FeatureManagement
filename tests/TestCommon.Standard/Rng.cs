using System;

namespace TestCommon.Standard
{
    public static class Rng
    {
        private static readonly Random Random = new Random();

        public static bool? GetNullableBoolean()
        {
            var result = Random.Next(-1, 2);
            switch (result)
            {
                case 0: return false;
                case 1: return true;
                default: return null;
            }
        }

        public static bool GetBoolean()
        {
            var result = Random.Next(0, 2);
            return result == 1;
        }

        public static int GetInteger(int minValue, int maxValue) => Random.Next(minValue, maxValue);
    }
}
