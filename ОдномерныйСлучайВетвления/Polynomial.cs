namespace ОдномерныйСлучайВетвления
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Представляет собой многочлен
    /// </summary>
    public class Polynomial
    {
        /// <summary>
        /// Нулевой многочлен.
        /// </summary>
        public static readonly Polynomial Nullear = new Polynomial() { Deg = -1, A = new double[] { 0 } };

        /// <summary>
        /// Коэффициенты при соответствующих степенях
        /// </summary>
        public double[] A;

        /// <summary>
        /// Степень полинома
        /// </summary>
        public int Deg;

        /// <summary>
        /// Найти все корни полинома.
        /// </summary>
        /// <returns></returns>
        public List<double> Roots()
        {
            var result = new List<double>();
            var normal = this.Normalize();
            while (normal.Deg > 0)
            {
                var newRoot = normal.RootWithBisection();
                result.Add(newRoot);
                normal = normal.EraseRoot(newRoot);
            }

            return result;
        }

        /// <summary>
        /// Создает новый полином, являющийся остатком от деления этого полинома на другой.
        /// </summary>
        /// <param name="other">Другой полином, на который необходимо разделить.</param>
        /// <returns></returns>
        public Polynomial Rest(Polynomial other)
        {
            var div1 = this;
            var div2 = other;
            while (div1.Deg >= div2.Deg)
            {
                var shift = div1.Deg - div2.Deg;
                var koef = div1[div1.Deg] / div2[div2.Deg];
                var div3 = new Polynomial(new double[div1.Deg]);

                for (var i = div1.Deg - 1; i >= shift; i--)
                {
                    div3[i] = div1[i] - koef * div2[i - shift];
                }

                for (var i = shift - 1; i >= 0; i--)
                {
                    div3[i] = div1[i];
                }

                div1 = div3;
            }

            if (Approximation.EqualDoubles(div1[div1.Deg], 0, this.Deg) && div1.Deg > 0)
            {
                var shift = 1;
                while (Approximation.EqualDoubles(div1[div1.Deg - shift], 0, this.Deg))
                {
                    shift++;
                    if (shift == div1.Deg)
                    {
                        return Nullear;
                    }
                }

                var newResult = new Polynomial(new double[div1.Deg + 1 - shift]);
                for (var i = div1.Deg - shift; i >= 0; i--)
                {
                    newResult[i] = div1[i];
                }

                div1 = newResult;
            }

            for (var i = 0; i <= div1.Deg; i++)
            {
                div1[i] *= -1;
            }

            return div1;
        }

        /// <summary>
        /// Количество знакоперемен в ряде Штурма
        /// </summary>
        /// <param name="series">Ряд Штурма</param>
        /// <param name="x">Точка, в которой вычисляются знакоперемены.</param>
        /// <returns></returns>
        public static int NumberOfResigns(List<Polynomial> series, double x)
        {
            var i0 = 0;
            while (Approximation.EqualDoubles(series[i0].Value(x), 0, series[i0].Deg) && i0 < series.Count)
            {
                i0++;
            }

            var lastSign = Math.Sign(series[i0].Value(x));
            var result = 0;
            for (var i = i0 + 1; i < series.Count; i++)
            {
                var value = series[i].Value(x);
                if (Approximation.EqualDoubles(value, 0, series[i].Deg))
                {
                    continue;
                }

                var nextSign = Math.Sign(value);
                if (lastSign != nextSign)
                {
                    result++;
                    lastSign = nextSign;
                }
            }

            if (lastSign < 0 && series.Last() == Nullear)
            {
                result++;
            }

            return result;
        }

        private static IEnumerable<double> Shturm(List<Polynomial> series, double a, double b)
        {
            var result = new List<double>();
            if (Approximation.EqualDoubles(a, b, series[0].Deg))
            {
                return result;
            }

            var aN = NumberOfResigns(series, a);
            var bN = NumberOfResigns(series, b);
            var n = aN - bN;
            if (n > 1)
            {
                var c = (a + b) / 2;
                if (Approximation.EqualDoubles(series[0].Value(c), 0, series[0].Deg))
                {
                    result.Add(c);
                }
                else
                {
                    var listA = Shturm(series, a, c);
                    var listB = Shturm(series, c, b);

                    result.AddRange(listA);
                    result.AddRange(listB);
                }

                for (var i = 0; i < result.Count - 1; i++)
                {
                    for (var j = i + 1; j < result.Count; j++)
                    {
                        if (Approximation.EqualDoubles(result[i], result[j], (int)Math.Pow(series[0].Deg, 4)))
                        {
                            result.RemoveAt(j);
                        }
                    }
                }

                return result;
            }

            if (n != 1)
            {
                return result;
            }

            var root = series[0].RootWithBisection(a, b);
            if (!Approximation.EqualDoubles(a, root, series[0].Deg) &&
                !Approximation.EqualDoubles(root, b, series[0].Deg))
            {
                result.Add(root);
            }

            return result;
        }

        /// <summary>
        /// Находит все различные положительные корни.
        /// </summary>
        /// <returns></returns>
        public List<double> RootsWithSturm()
        {
            var result = new List<double>();

            if (Math.Abs(this[0]) < double.Epsilon * this.Deg)
            {
                result.Add(0);
            }

            var normal = this.Normalize();

            var maxRootValue = 0d;
            for (var i = normal.Deg - 1; i >= 0; i--)
            {
                maxRootValue += Math.Abs(normal[i]);
            }

            var a = -maxRootValue;
            if (Approximation.EqualDoubles(normal.Value(a), 0, normal.Deg))
            {
                result.Add(a);
            }

            var b = maxRootValue;
            if (Approximation.EqualDoubles(normal.Value(b), 0, normal.Deg))
            {
                result.Add(b);
            }

            var sturm = new List<Polynomial>();
            var lastSturm = normal;
            sturm.Add(lastSturm);
            var nextSturm = normal.Derivate();
            sturm.Add(nextSturm);
            while (nextSturm.Deg > 0)
            {
                var newSturm = lastSturm.Rest(nextSturm);
                lastSturm = nextSturm;
                nextSturm = newSturm;
                sturm.Add(nextSturm);
            }

            result.AddRange(Shturm(sturm, a, b));
            return result;
        }

        private double RootWithBisection()
        {
            var normal = this;
            if (!Approximation.EqualDoubles(normal[normal.Deg], 1, normal.Deg))
            {
                normal = normal.Normalize();
            }

            var maxRootValue = 0d;
            for (var i = normal.Deg - 1; i >= 0; i--)
            {
                maxRootValue += Math.Abs(normal[i]);
            }

            var a = -maxRootValue;
            if (Approximation.EqualDoubles(normal.Value(a), 0, normal.Deg))
            {
                return a;
            }

            var b = maxRootValue;
            if (Approximation.EqualDoubles(normal.Value(b), 0, normal.Deg))
            {
                return b;
            }

            while (!Approximation.EqualDoubles(a, b, normal.Deg))
            {
                var c = (b + a) / 2;
                var cValue = normal.Value(c);
                if (Approximation.EqualDoubles(cValue, 0, normal.Deg))
                {
                    return c;
                }

                if (Math.Sign(normal.Value(a)) == Math.Sign(cValue))
                {
                    a = c;
                }
                else
                {
                    b = c;
                }
            }

            return (a + b) / 2;
        }

        private double RootWithBisection(double a0, double b0)
        {
            var normal = this.Normalize();
            var a = a0;
            var b = b0;
            while (!Approximation.EqualDoubles(a, b, normal.Deg))
            {
                var c = (b / 2) + (a / 2);
                var cValue = normal.Value(c);
                if (Approximation.EqualDoubles(cValue, 0, normal.Deg))
                {
                    return c;
                }

                if (Math.Sign(normal.Value(a)) == Math.Sign(cValue))
                {
                    a = c;
                }
                else
                {
                    b = c;
                }
            }

            return (a + b) / 2;
        }

        /// <summary>
        /// Отделить корень полинома.
        /// </summary>
        /// <param name="root">Один из найденных корней</param>
        /// <returns></returns>
        public Polynomial EraseRoot(double root)
        {
            var normal = this.Normalize();
            var result = new Polynomial(new double[this.Deg]);
            result[result.Deg] = 1;
            for (var i = result.Deg - 1; i >= 0; i--)
            {
                result[i] = normal[i + 1] + root * result[i + 1];
            }

            if (result.Deg == 0 && Math.Abs(result[0]) < double.Epsilon)
            {
                return Nullear;
            }

            return result;
        }

        /// <summary>
        /// Найти производную полинома
        /// </summary>
        /// <returns></returns>
        public Polynomial Derivate()
        {
            var result = new Polynomial(new double[this.Deg]);
            for (var i = 0; i <= result.Deg; i++)
            {
                result[i] = (i + 1) * this[i + 1];
            }

            if (result.Deg == 0 && Math.Abs(result[0]) < double.Epsilon)
            {
                return Nullear;
            }

            return result;
        }

        /// <summary>
        /// Значение полинома в точке.
        /// </summary>
        /// <param name="x">Точка, в которой ищется значение.</param>
        /// <returns></returns>
        public double Value(double x)
        {
            var result = 0d;
            for (var i = 0; i <= this.Deg; i++)
            {
                result += this[i] * Math.Pow(x, i);
            }

            return result;
        }

        /// <summary>
        /// Получает или возвращает коэффициент при соответствующей степени члена полинома.
        /// </summary>
        /// <param name="index">Степень члена полинома.</param>
        /// <returns></returns>
        public double this[int index]
        {
            get => this.A[index];

            set => this.A[index] = value;
        }

        /// <summary>
        /// Создает новый полином, степени которого будут соответствовать индексу элемента в массиве, на основе которого он создается, степень определяется автоматически.
        /// </summary>
        /// <param name="a">Массив для создания полинома.</param>
        public Polynomial(double[] a)
        {
            this.A = a;
            Deg = a.Length - 1;
        }

        private Polynomial()
        {
            // TODO: Complete member initialization
        }

        /// <summary>
        /// Нормализует многочлен так, что коэффициент при старшей степени будет равен еденице.
        /// </summary>
        public Polynomial Normalize()
        {
            var resultDeg = Deg;
            for (var i = Deg; i >= 0; i--)
            {
                if (Math.Abs(this[i]) < double.Epsilon * Deg)
                {
                    resultDeg--;
                }
                else
                {
                    break;
                }
            }

            if (resultDeg < 0)
            {
                return Nullear;
            }

            var shift = this.Deg - resultDeg;
            var a0 = this.A[this.Deg - shift];
            var result = new Polynomial(new double[resultDeg + 1]) { [resultDeg] = 1 };
            for (var i = result.Deg - 1; i >= 0; i--)
            {
                result[i] = this[i] / a0;
            }

            return result;
        }
    }
}
