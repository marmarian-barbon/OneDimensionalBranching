namespace ОдномерныйСлучайВетвления
{
    using System;
    using System.Windows.Forms;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms.DataVisualization.Charting;

    /// <summary>
    /// Основная форма.
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Основная форма.
        /// </summary>
        public Form1()
        {
            this.InitializeComponent();

            var chartArea = this.diaramChart.ChartAreas[0];
            chartArea.AxisX.Title = "s";
            chartArea.AxisX.TitleFont = new Font("Consolas", 16f);
            chartArea.AxisY.Title = "p";
            chartArea.AxisY.TitleFont = new Font("Consolas", 16f);
            chartArea.AxisY.TextOrientation = TextOrientation.Horizontal;

            this.chart1.Legends[0].Font = new Font("Consolas", 15f);
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            var eps = 0.05;
            this.chart1.Series.Clear();
            var f = new Function(new[]
                                     {
                                         new List<Component> { new Component(0, -1), new Component(2, 1) },
                                         new List<Component>(),
                                         new List<Component> { new Component(0, 1) }
                                     });

            // Пример 2.2 из книги (43 стр)
            /*{
                new List<Component> { new Component(1, -1) }, // 0
                new List<Component> { new Component(0, 1) }, // 1
                new List<Component> { new Component(0, -1) }, // 2
                new List<Component> { new Component(3, -1) }, // 3
                new List<Component>(), // 4
                new List<Component>(), // 5
                new List<Component>(), // 6
                new List<Component> { new Component(2, 2) }, // 7
                new List<Component>(), // 8
                new List<Component>(), // 9
                new List<Component>(), // 10
                new List<Component>(), // 11
                new List<Component> { new Component(4, -1) }, // 12
            });*/

            // лемниската
            /*{
                new List<Component>{new Component(2, -2), new Component(4, 1)},
                new List<Component>(),
                new List<Component>{new Component(0, 2), new Component(2, 2)},
                new List<Component>(),
                new List<Component>{new Component(0, 1)}
            });*/

            // окружность
            /*{
                new List<Component> { new Component(0, -1), new Component(2, 1) },
                new List<Component>(),
                new List<Component> { new Component(0, 1) }
            });*/

            // парабола
            /*{
                new List<Component> { new Component(0, 1), new Component(2, -1) },
                new List<Component> { new Component(0, 1) }
            });*/

            if (!int.TryParse(this.textBox1.Text, out var iterations))
            {
                return;
            }

            var min = -2;
            var max = 2;

            this.chart1.ChartAreas[0].AxisX.Minimum = min;
            this.chart1.ChartAreas[0].AxisX.Maximum = max;

            this.chart1.ChartAreas[0].AxisY.Minimum = -2;
            this.chart1.ChartAreas[0].AxisY.Maximum = 2;

            this.chart1.Series.Add(string.Empty);
            var mainSeries = this.chart1.Series[0];
            mainSeries.MarkerStyle = MarkerStyle.Circle;
            mainSeries.MarkerSize = 1;
            mainSeries.Color = Color.Gray;
            mainSeries.Name = string.Empty;
            mainSeries.ChartType = SeriesChartType.Point;
            for (var angle = (double)0; angle <= Math.PI * 2; angle += 0.0005)
            {
                mainSeries.Points.AddXY(Math.Cos(angle), Math.Sin(angle));

                //mainSeries.Points.AddXY(x, x * x);

                //var sqr = Math.Sqrt(1 + (4 * x * x)) - (x * x) - 1;
                //if (sqr >= 0)
                //{
                //    mainSeries.Points.AddXY(x, Math.Sqrt(sqr));
                //    mainSeries.Points.AddXY(x, -Math.Sqrt(sqr));
                //}
            }

            f.Determinate(iterations);
            var mathematicSeriesList = f.GetSeries();
            foreach (var mathematicSeries in mathematicSeriesList)
            {
                var seriesName = $"{this.chart1.Series.Count}) ξ(λ)= {mathematicSeries.Name('λ', "0.###")}";
                this.chart1.Series.Add(seriesName);
                var graphicSeries = this.chart1.Series[this.chart1.Series.Count - 1];
                graphicSeries.ChartType = SeriesChartType.Spline;
                graphicSeries.LabelBorderWidth = 100;
                for (var x = this.chart1.ChartAreas[0].AxisX.Minimum; x < this.chart1.ChartAreas[0].AxisX.Maximum; x += eps)
                {
                    graphicSeries.Points.AddXY(x, mathematicSeries.Value(x));
                }
            }

            this.chart1.ChartAreas[0].RecalculateAxesScale();

            var diagram = f.NextF[1].Diagram;
            // var diagram = f.Diagram;
            if (diagram == null)
            {
                return;
            }

            this.diaramChart.Series.Clear();

            var mainPointsSeries = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Green,
                MarkerStyle = MarkerStyle.Cross,
                MarkerSize = 20,
                BorderWidth = 4
            };

            foreach (var mainPoint in diagram.MainPoints)
            {
                mainPointsSeries.Points.Add(mainPoint);
            }

            this.diaramChart.Series.Add(mainPointsSeries);

            var internalPointsSeries = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                ChartType = SeriesChartType.Point
            };

            foreach (var internalPoint in diagram.InternalPoints)
            {
                internalPointsSeries.Points.Add(internalPoint);
            }

            internalPointsSeries.Color = Color.Blue;
            internalPointsSeries.MarkerStyle = MarkerStyle.Cross;
            internalPointsSeries.MarkerSize = 20;
            this.diaramChart.Series.Add(internalPointsSeries);

            var unusedPointsSeries = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                ChartType = SeriesChartType.Point
            };

            foreach (var unusedPoint in diagram.UnusedPoints)
            {
                unusedPointsSeries.Points.Add(unusedPoint);
            }

            unusedPointsSeries.Color = Color.Red;
            unusedPointsSeries.MarkerStyle = MarkerStyle.Cross;
            unusedPointsSeries.MarkerSize = 20;
            this.diaramChart.Series.Add(unusedPointsSeries);
        }
    }
}
