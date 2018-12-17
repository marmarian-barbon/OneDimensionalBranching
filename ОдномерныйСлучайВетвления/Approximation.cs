namespace ОдномерныйСлучайВетвления
{
    using System;

    /// <summary>
    /// Набор методов для сравнения чисел.
    /// </summary>
    public static class Approximation
    {
        /// <summary>
        /// Представляет собой еденицу в мантиссе.
        /// </summary>
        public static double Epsilon = double.MinValue;

        private static double DeterminateEpsilon()
        {
            var result = 1d;
            while (1 != 1 + result)
            {
                result /= 2;
            }

            return result * 2;
        }

        /// <summary>
        /// Сравнивает два <see cref="double"/> числа и показывает, могут ли они быть равны.
        /// </summary>
        /// <param name="a">Первое число для сравнения.</param>
        /// <param name="b">Второе число для сравнения.</param>
        /// <param name="complexity">Сложность произведенного алгоритма, который влечет за собой погрешность.</param>
        /// <returns></returns>
        public static bool EqualDoubles(double a, double b, int complexity)
        {
            if (Epsilon < 0)
            {
                Epsilon = DeterminateEpsilon();
            }

            var eps = Math.Max(Math.Abs(a), Math.Abs(b)) * Epsilon;
            return Math.Abs(b - a) <= eps * complexity;
        }
    }
}
