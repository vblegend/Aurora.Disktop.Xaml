import { namespace } from "d3-selection";




export namespace Tween {

    /**
     * 缓动函数列表类
     */
    export class Easing {

        public static Linear = {
            None(k: number): number {
                return k;

            }
        }

        public static Quadratic = {

            In(k) {
                return k * k;
            },

            Out(k) {
                return k * (2 - k);
            },

            InOut(k) {
                if ((k *= 2) < 1) {
                    return 0.5 * k * k;
                }
                return -0.5 * (--k * (k - 2) - 1);
            }
        }

        public static Cubic = {

            In(k) {
                return k * k * k;
            },

            Out(k) {
                return --k * k * k + 1;
            },

            InOut(k) {
                if ((k *= 2) < 1) {
                    return 0.5 * k * k * k;
                }
                return 0.5 * ((k -= 2) * k * k + 2);
            }

        }

        public static Quartic = {

            In(k) {

                return k * k * k * k;
            },

            Out(k) {
                return 1 - (--k * k * k * k);
            },

            InOut(k) {
                if ((k *= 2) < 1) {
                    return 0.5 * k * k * k * k;
                }
                return -0.5 * ((k -= 2) * k * k * k - 2);
            }
        }

        public static Quintic = {
            In(k) {
                return k * k * k * k * k;
            },

            Out(k) {

                return --k * k * k * k * k + 1;
            },

            InOut(k) {
                if ((k *= 2) < 1) {
                    return 0.5 * k * k * k * k * k;
                }
                return 0.5 * ((k -= 2) * k * k * k * k + 2);

            }

        }

        public static Sinusoidal = {

            In: function (k) {

                return 1 - Math.cos(k * Math.PI / 2);

            },

            Out: function (k) {

                return Math.sin(k * Math.PI / 2);

            },

            InOut: function (k) {

                return 0.5 * (1 - Math.cos(Math.PI * k));

            }

        }

        public static Exponential = {

            In: function (k) {

                return k === 0 ? 0 : Math.pow(1024, k - 1);

            },

            Out: function (k) {

                return k === 1 ? 1 : 1 - Math.pow(2, -10 * k);

            },

            InOut: function (k) {

                if (k === 0) {
                    return 0;
                }

                if (k === 1) {
                    return 1;
                }

                if ((k *= 2) < 1) {
                    return 0.5 * Math.pow(1024, k - 1);
                }

                return 0.5 * (-Math.pow(2, -10 * (k - 1)) + 2);

            }

        }

        public static Circular = {

            In: function (k) {

                return 1 - Math.sqrt(1 - k * k);

            },

            Out: function (k) {

                return Math.sqrt(1 - (--k * k));

            },

            InOut: function (k) {

                if ((k *= 2) < 1) {
                    return -0.5 * (Math.sqrt(1 - k * k) - 1);
                }

                return 0.5 * (Math.sqrt(1 - (k -= 2) * k) + 1);

            }

        }

        public static Elastic = {

            In: function (k) {

                if (k === 0) {
                    return 0;
                }

                if (k === 1) {
                    return 1;
                }

                return -Math.pow(2, 10 * (k - 1)) * Math.sin((k - 1.1) * 5 * Math.PI);

            },

            Out: function (k) {

                if (k === 0) {
                    return 0;
                }

                if (k === 1) {
                    return 1;
                }

                return Math.pow(2, -10 * k) * Math.sin((k - 0.1) * 5 * Math.PI) + 1;

            },

            InOut: function (k) {

                if (k === 0) {
                    return 0;
                }

                if (k === 1) {
                    return 1;
                }

                k *= 2;

                if (k < 1) {
                    return -0.5 * Math.pow(2, 10 * (k - 1)) * Math.sin((k - 1.1) * 5 * Math.PI);
                }

                return 0.5 * Math.pow(2, -10 * (k - 1)) * Math.sin((k - 1.1) * 5 * Math.PI) + 1;

            }

        }

        public static Back = {

            In: function (k) {

                const s = 1.70158;

                return k * k * ((s + 1) * k - s);

            },

            Out: function (k) {

                const s = 1.70158;

                return --k * k * ((s + 1) * k + s) + 1;

            },

            InOut: function (k) {

                const s = 1.70158 * 1.525;

                if ((k *= 2) < 1) {
                    return 0.5 * (k * k * ((s + 1) * k - s));
                }

                return 0.5 * ((k -= 2) * k * ((s + 1) * k + s) + 2);

            }

        }

        public static Bounce = {

            In(k) {

                return 1 - Tween.Easing.Bounce.Out(1 - k);

            },

            Out(k) {

                if (k < (1 / 2.75)) {
                    return 7.5625 * k * k;
                } else if (k < (2 / 2.75)) {
                    return 7.5625 * (k -= (1.5 / 2.75)) * k + 0.75;
                } else if (k < (2.5 / 2.75)) {
                    return 7.5625 * (k -= (2.25 / 2.75)) * k + 0.9375;
                } else {
                    return 7.5625 * (k -= (2.625 / 2.75)) * k + 0.984375;
                }

            },

            InOut(k) {

                if (k < 0.5) {
                    return Tween.Easing.Bounce.In(k * 2) * 0.5;
                }

                return Tween.Easing.Bounce.Out(k * 2 - 1) * 0.5 + 0.5;

            }

        }


    };

    /**
     * 插值函数列表类 
     */
    export class Interpolation {

        public static Linear(v: number[], k: number): number {

            const m = v.length - 1;
            const f = m * k;
            const i = Math.floor(f);
            const fn = this.UtilsLinear;

            if (k < 0) {
                return fn(v[0], v[1], f);
            }

            if (k > 1) {
                return fn(v[m], v[m - 1], m - f);
            }
            return fn(v[i], v[i + 1 > m ? m : i + 1], f - i);

        }

        public static Bezier(v, k) {

            let b = 0;
            const n = v.length - 1;
            const pw = Math.pow;
            const bn = this.UtilsBernstein;

            for (let i = 0; i <= n; i++) {
                b += pw(1 - k, n - i) * pw(k, i) * v[i] * bn(n, i);
            }

            return b;

        }

        public static CatmullRom(v, k) {

            const m = v.length - 1;
            let f = m * k;
            let i = Math.floor(f);
            const fn = this.UtilsCatmullRom;

            if (v[0] === v[m]) {

                if (k < 0) {
                    i = Math.floor(f = m * (1 + k));
                }

                return fn(v[(i - 1 + m) % m], v[i], v[(i + 1) % m], v[(i + 2) % m], f - i);

            } else {

                if (k < 0) {
                    return v[0] - (fn(v[0], v[0], v[1], v[1], -f) - v[0]);
                }

                if (k > 1) {
                    return v[m] - (fn(v[m], v[m], v[m - 1], v[m - 1], f - m) - v[m]);
                }

                return fn(v[i ? i - 1 : 0], v[i], v[m < i + 1 ? m : i + 1], v[m < i + 2 ? m : i + 2], f - i);

            }

        }

        private static UtilsLinear(p0: number, p1: number, t: number): number {
            return (p1 - p0) * t + p0;
        }

        private static UtilsBernstein(n: number, i: number): number {

            const fc = this.UtilsFactorial;

            return fc(n) / fc(i) / fc(n - i);

        }

        static a: number[] = [1];
        private static UtilsFactorial(n: number): number {
            let s = 1;
            if (this.a[n]) {
                return this.a[n];
            }
            for (let i = n; i > 1; i--) {
                s *= i;
            }
            this.a[n] = s;
            return s;
        }

        private static UtilsCatmullRom(p0: number, p1: number, p2: number, p3: number, t: number): number {

            const v0 = (p2 - p0) * 0.5;
            const v1 = (p3 - p1) * 0.5;
            const t2 = t * t;
            const t3 = t * t2;

            return (2 * p1 - 2 * p2 + v0 + v1) * t3 + (-3 * p1 + 3 * p2 - 2 * v0 - v1) * t2 + v0 * t + p1;

        }

    }



}