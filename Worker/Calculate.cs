using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Worker
{

    public class Calculate
    {
        public long calculate (long number)
        {
            long p = factTree(number);
            return Math.Abs(p);
        }

        private static long factTree(long k)
        {
            if (k < 0) return 0;
            if (k == 0) return 1;
            if (k == 1 || k == 2) return k;
            return prodTree(2, k);
        }

        private static long prodTree(long a, long b)
        {
            long k;
            if (a > b) return 1;
            if (a == b) return a;
            if (b - a == 1) return (long)a * b;
            //long m = (a + b) / 2;
            //k = prodTree(a, m) * prodTree(m + 1, b);
            k = firstProdTree(a,b) * secondProdTree(a,b);
            return k;
        }

        private static long firstProdTree(long a, long b)
        {
            if (a > b) return 1;
            if (a == b) return a;
            if (b - a == 1) return (long)a * b;
            long m = (a + b) / 2;
            return prodTree(a, m);
        }
        private static long secondProdTree(long a, long b)
        {
            if (a > b) return 1;
            if (a == b) return a;
            if (b - a == 1) return (long)a * b;
            long m = (a + b) / 2;
            return prodTree(m + 1, b);
        }


    }
}
