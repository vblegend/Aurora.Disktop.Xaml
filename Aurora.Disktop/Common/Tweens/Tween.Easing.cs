using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Disktop.Common.Tweens
{

    public delegate Double EasingFunction(Double z);
    public class Easing
    {
        public static class Linear
        {
            public static Double None(Double k)
            {
                return k;
            }
        }

        public static class Quadratic
        {

            public static Double In(Double k)
            {
                return k * k;
            }

            public static Double Out(Double k)
            {
                return k * (2 - k);
            }

            public static Double InOut(Double k)
            {
                if ((k *= 2) < 1)
                {
                    return 0.5 * k * k;
                }
                return -0.5 * (--k * (k - 2) - 1);
            }
        }

        public static class Cubic
        {

            public static Double In(Double k)
            {
                return k * k * k;
            }

            public static Double Out(Double k)
            {
                return --k * k * k + 1;
            }

            public static Double InOut(Double k)
            {
                if ((k *= 2) < 1)
                {
                    return 0.5 * k * k * k;
                }
                return 0.5 * ((k -= 2) * k * k + 2);
            }

        }


        public static class Quartic
        {
            public static Double In(Double k)
            {
                return k * k * k * k;
            }

            public static Double Out(Double k)
            {
                return 1 - (--k * k * k * k);
            }

            public static Double InOut(Double k)
            {
                if ((k *= 2) < 1)
                {
                    return 0.5 * k * k * k * k;
                }
                return -0.5 * ((k -= 2) * k * k * k - 2);
            }
        }

        public static class Quintic
        {
            public static Double In(Double k)
            {
                return k * k * k * k * k;
            }

            public static Double Out(Double k)
            {
                return --k * k * k * k * k + 1;
            }

            public static Double InOut(Double k)
            {
                if ((k *= 2) < 1)
                {
                    return 0.5 * k * k * k * k * k;
                }
                return 0.5 * ((k -= 2) * k * k * k * k + 2);
            }
        }

        public static class Sinusoidal
        {
            public static Double In(Double k)
            {
                return 1 - Math.Cos(k * Math.PI / 2);
            }

            public static Double Out(Double k)
            {
                return Math.Sin(k * Math.PI / 2);
            }

            public static Double InOut(Double k)
            {
                return 0.5 * (1 - Math.Cos(Math.PI * k));
            }
        }

        public static class Exponential
        {
            public static Double In(Double k)
            {
                return k == 0 ? 0 : Math.Pow(1024, k - 1);
            }

            public static Double Out(Double k)
            {
                return k == 1 ? 1 : 1 - Math.Pow(2, -10 * k);
            }

            public static Double InOut(Double k)
            {
                if (k == 0)
                {
                    return 0;
                }

                if (k == 1)
                {
                    return 1;
                }

                if ((k *= 2) < 1)
                {
                    return 0.5 * Math.Pow(1024, k - 1);
                }

                return 0.5 * (-Math.Pow(2, -10 * (k - 1)) + 2);
            }
        }

        public static class Circular
        {
            public static Double In(Double k)
            {
                return 1 - Math.Sqrt(1 - k * k);
            }

            public static Double Out(Double k)
            {
                return Math.Sqrt(1 - (--k * k));
            }

            public static Double InOut(Double k)
            {
                if ((k *= 2) < 1)
                {
                    return -0.5 * (Math.Sqrt(1 - k * k) - 1);
                }
                return 0.5 * (Math.Sqrt(1 - (k -= 2) * k) + 1);
            }
        }

        public static class Elastic
        {
            public static Double In(Double k)
            {
                if (k == 0)
                {
                    return 0;
                }

                if (k == 1)
                {
                    return 1;
                }

                return -Math.Pow(2, 10 * (k - 1)) * Math.Sin((k - 1.1) * 5 * Math.PI);
            }

            public static Double Out(Double k)
            {
                if (k == 0)
                {
                    return 0;
                }

                if (k == 1)
                {
                    return 1;
                }

                return Math.Pow(2, -10 * k) * Math.Sin((k - 0.1) * 5 * Math.PI) + 1;
            }

            public static Double InOut(Double k)
            {
                if (k == 0)
                {
                    return 0;
                }

                if (k == 1)
                {
                    return 1;
                }

                k *= 2;

                if (k < 1)
                {
                    return -0.5 * Math.Pow(2, 10 * (k - 1)) * Math.Sin((k - 1.1) * 5 * Math.PI);
                }

                return 0.5 * Math.Pow(2, -10 * (k - 1)) * Math.Sin((k - 1.1) * 5 * Math.PI) + 1;
            }
        }
    }

}
