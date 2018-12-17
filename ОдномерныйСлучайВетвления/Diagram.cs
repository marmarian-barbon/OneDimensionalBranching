namespace ОдномерныйСлучайВетвления
{
    using System.Collections.Generic;
    using System.Windows.Forms.DataVisualization.Charting;

    /// <summary>
    /// Представляет собой компоненты диаграммы Ньютона.
    /// </summary>
    public class Diagram
    {
        /// <summary>
        /// Основные точки в диаграмме.
        /// </summary>
        public ICollection<DataPoint> MainPoints { get; private set; } = new List<DataPoint>();

        /// <summary>
        /// Неиспользуемые точки.
        /// </summary>
        public ICollection<DataPoint> UnusedPoints { get; private set; } = new List<DataPoint>();

        /// <summary>
        /// Точки на линиях.
        /// </summary>
        public ICollection<DataPoint> InternalPoints { get; private set; } = new List<DataPoint>();
    }
}
