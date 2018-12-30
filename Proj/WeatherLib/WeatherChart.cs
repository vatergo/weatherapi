using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WeatherLib
{
    public class WeatherChart
    {
        public Form Parent;
        public Chart TemperatureChart;
        private Series maxSeries;
        private Series averageSeries;
        private Series minSeries;
        
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }

        public WeatherChart(int x1, int y1, int x2, int y2, Form parent)
        {
            this.X1 = x1;
            this.X2 = x2;
            this.Y1 = y1;
            this.Y2 = y2;
            this.Parent = parent;
            this.TemperatureChart = new Chart();     
            this.TemperatureChart.Parent = (Control)this.Parent;
            this.TemperatureChart.Height = Math.Abs(y2 - y1);
            this.TemperatureChart.Width = Math.Abs(x2 - x1);
            this.TemperatureChart.Left = Math.Min(x1, x2);
            this.TemperatureChart.Top = Math.Min(y1, y2);
            this.TemperatureChart.ChartAreas.Add(new ChartArea());
            this.TemperatureChart.ChartAreas[0].AxisX.Title = "Число месяца";
            this.TemperatureChart.ChartAreas[0].AxisY.Title = "°C";
        }

        public void AddFullChart(double[] points, int dayNumber, int frequencyMeasurement, string city)
        {
            maxSeries = new Series();
            maxSeries.ChartType = SeriesChartType.Line;
            minSeries = new Series();
            minSeries.ChartType = SeriesChartType.Line;
            averageSeries = new Series();
            averageSeries.ChartType = SeriesChartType.Line;

            var maxForAxis = -70.0;
            var minForAxis = 70.0;

            var today = DateTime.Now.Day;
            for (var index = 0; index < dayNumber; ++index)
            {
                var max = -50.0;
                var min = 50.0;
                for(var j = index * frequencyMeasurement; j < frequencyMeasurement * (index + 1); j++)
                {
                    if (points[j] > max) max = points[j];
                    if (points[j] < min) min = points[j];
                    if (maxForAxis < max) maxForAxis = max;
                    if (minForAxis > min) minForAxis = min;
                }
                maxSeries.Points.AddXY(today.ToString(), max);
                minSeries.Points.AddXY(today.ToString(), min);
                averageSeries.Points.AddXY(today.ToString(), (max+min)/2);
                today++;
                if (today >
                    DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
                    today = 1;
            }

            TemperatureChart.Series.Clear();
            TemperatureChart.Titles.Clear();
            TemperatureChart.Legends.Clear();
            this.TemperatureChart.Series.Add(maxSeries);
            this.TemperatureChart.Series.Add(averageSeries);
            this.TemperatureChart.Series.Add(minSeries);
            this.TemperatureChart.Titles.Add(city + ": таблица погоды на пять дней");
            this.TemperatureChart.Legends.Add(new Legend());

            this.TemperatureChart.Series[0].BorderWidth = 5;
            this.TemperatureChart.Series[0].Color = Color.Red;
            this.TemperatureChart.Series[0].LegendText = "Макс. T°C";
            this.TemperatureChart.Series[1].BorderWidth = 5;
            this.TemperatureChart.Series[1].Color = Color.Black;
            this.TemperatureChart.Series[1].LegendText = "Ср. T°C";
            this.TemperatureChart.Series[2].BorderWidth = 5;
            this.TemperatureChart.Series[2].Color = Color.Blue;
            this.TemperatureChart.Series[2].LegendText = "Мин. T°C";

            TemperatureChart.ChartAreas[0].AxisY.Maximum = Math.Round(maxForAxis) + 5;
            TemperatureChart.ChartAreas[0].AxisY.Minimum = Math.Round(minForAxis) - 5;
        }

        public void AddTemperaturePointsToChart(double[] points, int dayNumber, int frequencyMeasurement, string city)
        {
            Series series = new Series();
            series.ChartType = SeriesChartType.Line;
            double day = DateTime.Now.Day;

            for (int index = 0; index < dayNumber * frequencyMeasurement; ++index)
            {
                series.Points.AddXY(day, points[index]);
                day = day + 1.0 / frequencyMeasurement;
            }

            if (this.TemperatureChart.Series.Count == 0)
            {
                this.TemperatureChart.Series.Add(series);
                this.TemperatureChart.Titles.Add(city + ": таблица погоды на пять дней");
            }
            else
            {
                this.TemperatureChart.Series[0] = series;
                this.TemperatureChart.Titles[0].Text = city + ": таблица погоды на пять дней";
            }
            this.TemperatureChart.Series[0].BorderWidth = 5;
            this.TemperatureChart.Series[0].Color = Color.Red;
        }
    }
}