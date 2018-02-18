using System;
using DinkumCoin.Core.Contracts;

namespace DinkumCoin.Services
{
    public class MathService : IMathService
    {
        public bool IsPrime(int candidate)
        {
            if ((candidate & 1) == 0)
            {
                if (candidate == 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            for (int i = 3; (i * i) <= candidate; i += 2)
            {
                if ((candidate % i) == 0)
                {
                    return false;
                }
            }
            return candidate != 1;
        } 
    }
}
