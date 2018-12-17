namespace ОдномерныйСлучайВетвления
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Означает тип разложения.
    /// </summary>
    public enum SeriesType : byte
    {
        /// <summary>
        /// Обычное разложение.
        /// </summary>
        Common = 0,

        /// <summary>
        /// Конец разложения.
        /// </summary>
        End = 1,

        /// <summary>
        /// Незаконченное разложение.
        /// </summary>
        O = 2,

        /// <summary>
        /// Комплексное разложение.
        /// </summary>
        Complex = 3
    }

    /// <summary>
    /// Представляет собой ряд.
    /// </summary>
    public class Series
    {
        /// <summary>
        /// Представляет собой члены ряда.
        /// </summary>
        public List<Component> Components { get; set; } = new List<Component>();

        /// <summary>
        /// Определяет, какого типа являет конец этого разложения.
        /// </summary>
        public SeriesType EndType { get; set; } = SeriesType.Common;

        /// <summary>
        /// Вычисляет значение ряда в точке.
        /// </summary>
        /// <param name="x">Точка для ряда.</param>
        /// <returns></returns>
        public double Value(double x)
        {
            return this.Components.Select((t, i) => this.Components[i].K * Math.Pow(x, this.Components[i].Deg)).Sum();
        }

        /// <summary>
        /// Представляет ряд как формулу в виде строки.
        /// </summary>
        /// <param name="s">Символ переменной.</param>
        /// <param name="format">Числовой формат</param>
        /// <returns></returns>
        public string Name(char s, string format)
        {
            var result = string.Empty;
            foreach (var component in this.Components)
            {
                if (Math.Sign(component.K) >= 0)
                {
                    result += '+';
                }

                result += component.K.ToString(format) + '*' + s + '^' + component.Deg.ToString(format) + ' ';
            }

            switch (this.EndType)
            {
                case SeriesType.Complex:
                    {
                        result += "+Complex(" + s + ')';
                        break;
                    }

                case SeriesType.O:
                    {
                        result += "+o(" + s + ')';
                        break;
                    }
            }

            return result;
        }
    }
}
