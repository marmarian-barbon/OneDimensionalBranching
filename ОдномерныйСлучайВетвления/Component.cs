namespace ОдномерныйСлучайВетвления
{
    using System;

    /// <inheritdoc />
    /// <summary>
    /// Представляет собой один из членов степенного разложения.
    /// </summary>
    public class Component : IComparable<Component>
    {
        /// <summary>
        /// Копирование экземпляра существующего члена разложения.
        /// </summary>
        /// <param name="old">Существующий член разложения</param>
        public Component(Component old)
        {
            this.Deg = old.Deg;
            this.K = old.K;
        }

        /// <summary>
        /// Создает новый член разложения.
        /// </summary>
        /// <param name="deg">Степень члена.</param>
        /// <param name="k">Коэффициент перед членом</param>
        public Component(double deg, double k)
        {
            this.K = k;
            this.Deg = deg;
        }

        /// <summary>
        /// Степень этого члена в разложении.
        /// </summary>
        public double Deg { get; private set; }

        /// <summary>
        /// Коэффициент этого члена в разложении.
        /// </summary>
        public double K { get; set; }

        int IComparable<Component>.CompareTo(Component other)
        {
            return this.Deg.CompareTo(other.Deg);
        }

        /// <summary>
        /// Возвращает новый член разложения, равный этому, возведенному в степень.
        /// </summary>
        /// <param name="e">Степень возведения члена разложения.</param>
        /// <returns></returns>
        public Component Pow(double e)
        {
            return new Component(this.Deg * e, Math.Pow(this.K, e));
        }

        /// <summary>
        /// Возвращает новый член разложения, равный умножению этого члена разложения на другой.
        /// </summary>
        /// <param name="other">Другой член разложения, на который необходимо умножить.</param>
        /// <returns></returns>
        public Component Multiply(Component other)
        {
            return new Component(this.Deg + other.Deg, this.K * other.K);
        }

        public double Value(double x)
        {
            return this.K * Math.Pow(x, this.Deg);
        }
    }
}