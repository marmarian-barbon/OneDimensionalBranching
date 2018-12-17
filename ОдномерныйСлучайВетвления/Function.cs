namespace ОдномерныйСлучайВетвления
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms.DataVisualization.Charting;

    /// <summary>
    /// Представляет собой псевдомногочлен с коэффициентами <see cref="F"/> при соответствующих степенях.
    /// </summary>
    public class Function
    {
        /// <summary>
        /// Создает новый экземпляр псевдомногочлена по заданным коэффициентам.
        /// </summary>
        /// <param name="f">Коэффициенты для псевдомногочлена.</param>
        public Function(List<Component>[] f)
        {
            this.F = f;
            this.N = f.Length - 1;
            foreach (var fs in this.F)
            {
                fs.Sort();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Создает новый экземпляр псевдомногочлена по заданным коэффициентам с уже имеющимся разложением по параметру.
        /// </summary>
        /// <param name="f">Коэффициенты для псевдомногочлена.</param>
        /// <param name="componentInSeries">Коэффициент и степень в разложении по параметру.</param>
        /// <param name="binomial">Биномиальные коэффициенты.</param>
        public Function(List<Component>[] f, Component componentInSeries, int[][] binomial) : this(f)
        {
            this.ComponentInSeries = componentInSeries;
            this.Binomial = binomial;
        }

        /// <summary>
        /// Представляет собой положительные биномиальные коэффициенты. Первый индекс соответсвует степени суммы, второй - степени первого слагаемого.
        /// </summary>
        public int[][] Binomial { get; private set; }

        /// <summary>
        /// Представляет собой наборы из разложений по другому параметру. Индекс соответствует степени переменной в уравнении.
        /// </summary>
        public List<Component>[] F { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public SeriesType ComponentType { get; private set; } = SeriesType.Common;

        /// <summary>
        /// Степень псевдомногочлена.
        /// </summary>
        public int N { get; private set; }

        /// <summary>
        /// Представляет собой степень и коэффициент параметра в разложении.
        /// </summary>
        public Component ComponentInSeries { get; private set; } = new Component(double.MinValue, 0);

        /// <summary>
        /// Следующие псевдомногочлены при данном разложении по параметру.
        /// </summary>
        public List<Function> NextF { get; private set; } = new List<Function>();

        public Diagram Diagram { get; private set; }

        /// <summary>
        /// Определяет следующие разложения для данного псевдомногочлена по параметру.
        /// </summary>
        /// <returns></returns>
        public void DeterminateNext()
        {
            if (this.Binomial == null)
            {
                this.Binomial = new int[this.N + 1][];
                this.Binomial[0] = new[] { 1 };
                for (var i = 1; i < this.Binomial.Length; i++)
                {
                    this.Binomial[i] = new int[i + 1];
                    this.Binomial[i][0] = 1;
                    this.Binomial[i][this.Binomial[i].Length - 1] = 1;
                }

                for (var i = 2; i < this.Binomial.Length; i++)
                {
                    var binomialThis = this.Binomial[i];
                    var binomialPrev = this.Binomial[i - 1];
                    for (var j = 1; j < binomialPrev.Length; j++)
                    {
                        binomialThis[j] = binomialPrev[j - 1] + binomialPrev[j];
                    }
                }
            }

            var x = new List<int>();
            var y = new List<double>();
            var points = new List<DataPoint>();

            for (var s = 0; s <= this.N; s++)
            {
                if (this.F[s].Count <= 0)
                {
                    continue;
                }

                x.Add(s);
                y.Add(this.F[s][0].Deg);
                points.Add(new DataPoint(s, this.F[s][0].Deg));
            }

            this.Diagram = new Diagram();

            var begin = 0;
            while (begin < points.Count - 1)
            {
                var end = begin + 1;
                var tan = (y[end] - y[begin]) / (x[end] - x[begin]);
                for (var current = begin + 2; current < points.Count; current++)
                {
                    var newTan = (y[current] - y[begin]) / (x[current] - x[begin]);
                    if (newTan > tan && !Approximation.EqualDoubles(newTan, tan, this.N * this.N))
                    {
                        continue;
                    }

                    end = current;
                    tan = newTan;
                }

                if (-tan <= this.ComponentInSeries.Deg)
                {
                    begin = end;
                    continue;
                }

                var polynomialDeg = x[end] - x[begin];
                var polynomial = new Polynomial(new double[polynomialDeg + 1])
                {
                    [0] = this.F[x[begin]][0].K,
                    [polynomialDeg] = this.F[x[end]][0].K
                };

                if (!this.Diagram.MainPoints.Contains(points[begin]))
                {
                    this.Diagram.MainPoints.Add(points[begin]);
                }

                this.Diagram.MainPoints.Add(points[end]);

                for (var current = begin + 1; current < end; current++)
                {
                    var newTan = (y[current] - y[begin]) / (x[current] - x[begin]);
                    if (Approximation.EqualDoubles(newTan, tan, this.N))
                    {
                        polynomial[x[current] - x[begin]] = this.F[x[current]][0].K;
                        this.Diagram.InternalPoints.Add(points[current]);
                    }
                }

                var roots = polynomial.RootsWithSturm();
                foreach (var root in roots)
                {
                    var newComponentInSeries = new Component(-tan, root);
                    var newF = new List<Component>[this.N + 1];
                    for (var s = 0; s <= this.N; s++)
                    {
                        newF[s] = new List<Component>();
                        foreach (var component in this.F[s])
                        {
                            newF[s].Add(new Component(component));
                        }

                        for (var i = 0; i < s; i++)
                        {
                            var temp = newComponentInSeries.Pow(s - i);
                            temp.K *= this.Binomial[s][i];
                            foreach (var component in this.F[s])
                            {
                                newF[i].Add(component.Multiply(temp));
                            }
                        }
                    }

                    foreach (var newFs in newF)
                    {
                        for (var i = 0; i < newFs.Count; i++)
                        {
                            for (var j = i + 1; j < newFs.Count; j++)
                            {
                                if (!Approximation.EqualDoubles(newFs[i].Deg, newFs[j].Deg, this.N * this.N))
                                {
                                    continue;
                                }

                                newFs[i].K += newFs[j].K;
                                newFs.RemoveAt(j);
                                j--;
                            }

                            if (!Approximation.EqualDoubles(newFs[i].K, 0, this.N * this.N))
                            {
                                continue;
                            }

                            newFs.RemoveAt(i);
                            i--;
                        }

                        newFs.Sort();
                    }

                    this.NextF.Add(new Function(newF, newComponentInSeries, this.Binomial));
                }

                begin = end;
            }

            foreach (var point in points)
            {
                if (!this.Diagram.MainPoints.Contains(point) && !this.Diagram.InternalPoints.Contains(point))
                {
                    this.Diagram.UnusedPoints.Add(point);
                }
            }
        }

        /// <summary>
        /// Определяет следующие разложения этого псевдомногочлена по параметру.
        /// </summary>
        /// <param name="iterations">Глубина разложений</param>
        public void Determinate(int iterations)
        {
            this.NextF.Clear();
            if (iterations <= 0)
            {
                this.ComponentType = SeriesType.O;
                return;
            }

            this.DeterminateNext();

            if (this.NextF.Count == 0)
            {
                this.ComponentType = SeriesType.End;
                return;
            }

            foreach (var nextF in this.NextF)
            {
                nextF.Determinate(iterations - 1);
            }
        }

        /// <summary>
        /// Значение псевдомногочлена в точке <seealso cref="xi"/> с параметром <seealso cref="lambda"/>.
        /// </summary>
        /// <param name="xi"></param>
        /// <param name="lambda"></param>
        /// <returns></returns>
        public double Value(double xi, double lambda)
        {
            var result = 0d;
            for (var s = 0; s < this.F.Length; s++)
            {
                var powXiS = Math.Pow(xi, s);
                foreach (var component in this.F[s])
                {
                    result += powXiS * component.Value(lambda);
                }
            }

            return result;
        }

        /// <summary>
        /// Составляет все ряды Тейлора, если псевдомногочлен разложен.
        /// </summary>
        /// <returns></returns>
        public List<Series> GetSeries()
        {
            switch (this.ComponentType)
            {
                case SeriesType.End:
                    {
                        var result = new List<Series> { new Series { EndType = SeriesType.End } };
                        return result;
                    }

                case SeriesType.O:
                    {
                        var result = new List<Series> { new Series { EndType = SeriesType.O } };
                        return result;
                    }

                case SeriesType.Common:
                    {
                        var result = new List<Series>();
                        foreach (var nextF in this.NextF)
                        {
                            var nextResult = nextF.GetSeries();
                            foreach (var series in nextResult)
                            {
                                series.Components.Insert(0, nextF.ComponentInSeries);
                            }

                            result.AddRange(nextResult);
                        }

                        return result;
                    }

                default:
                    return null;
            }
        }
    }
}
