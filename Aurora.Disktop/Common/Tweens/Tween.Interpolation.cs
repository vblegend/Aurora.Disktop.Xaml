using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Disktop.Common.Tweens
{
    public delegate Double InterpolationFunction(Double[] v, Double k);

    public class Interpolation
    {
        public static Double Linear(Double[] v, Double k)
        {
            int m = v.Length - 1;
            Double f = m * k;
            int i = (int)f;

            if (k < 0)
            {
                return UtilsLinear(v[0], v[1], f);
            }

            if (k > 1)
            {
                return UtilsLinear(v[m], v[m - 1], m - f);
            }

            return UtilsLinear(v[i], (i + 1 > m) ? v[m] : v[i + 1], f - i);
        }

        public static Double Bezier(Double[] v, Double k)
        {
            Double b = 0;
            int n = v.Length - 1;

            for (int i = 0; i <= n; i++)
            {
                b += UtilsBernstein(n, i) * Math.Pow(1 - k, n - i) * Math.Pow(k, i) * v[i];
            }

            return b;
        }

        public static Double CatmullRom(Double[] v, Double k)
        {
            int m = v.Length - 1;
            Double f = m * k;
            int i = (int)f;

            if (v[0] == v[m])
            {
                if (k < 0)
                {
                    i = (int)(f = m * (1 + k));
                }

                return UtilsCatmullRom(v[(i - 1 + m) % m], v[i], v[(i + 1) % m], v[(i + 2) % m], f - i);
            }
            else
            {
                if (k < 0)
                {
                    return v[0] - (UtilsCatmullRom(v[0], v[0], v[1], v[1], -f) - v[0]);
                }

                if (k > 1)
                {
                    return v[m] - (UtilsCatmullRom(v[m], v[m], v[m - 1], v[m - 1], f - m) - v[m]);
                }

                return UtilsCatmullRom(
                    v[i > 0 ? i - 1 : 0],
                    v[i],
                    v[m < i + 1 ? m : i + 1],
                    v[m < i + 2 ? m : i + 2],
                    f - i
                );
            }
        }

        private static Double UtilsLinear(Double p0, Double p1, Double t)
        {
            return (p1 - p0) * t + p0;
        }

        private static Double UtilsBernstein(int n, int i)
        {
            return UtilsFactorial(n) / (UtilsFactorial(i) * UtilsFactorial(n - i));
        }

        private static Double[] a = new Double[] { 1.0 };

        private static Double UtilsFactorial(int n)
        {
            Double s = 1.0;
            if (a.Length > n)
            {
                return a[n];
            }
            for (int i = n; i > 1; i--)
            {
                s *= i;
            }
            Array.Resize(ref a, n + 1);
            a[n] = s;
            return s;
        }

        private static Double UtilsCatmullRom(Double p0, Double p1, Double p2, Double p3, Double t)
        {
            Double v0 = (p2 - p0) * 0.5;
            Double v1 = (p3 - p1) * 0.5;
            Double t2 = t * t;
            Double t3 = t * t2;

            return (2 * p1 - 2 * p2 + v0 + v1) * t3 + (-3 * p1 + 3 * p2 - 2 * v0 - v1) * t2 + v0 * t + p1;
        }
    }

}
