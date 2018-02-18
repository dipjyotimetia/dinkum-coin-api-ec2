using System;
namespace DinkumCoin.Services.Util
{

    public static class NumberHelpers
    {
        private static Random random = new Random();

        public static int GenerateRandomNumber(int min, int max)
        {
            return random.Next(min, max);
        }
    }
}
