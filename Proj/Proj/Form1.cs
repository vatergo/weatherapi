using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;
using YandexAPI.Maps;
using WeatherLib;

namespace Proj
{
    public partial class StartWindow : Form
    {
        int Dist = 3;
        string Point;
        GeoCode geoCode = new GeoCode();
        WeatherChart myChart;

        public StartWindow()
        {
            InitializeComponent();
            this.MouseWheel += new MouseEventHandler(StartWindow_MouseWheel);
            drawMap("Россия");
            myChart = new WeatherChart(12, 360, 472, 548, this);
            TemperatureChartDraw("Russia");
        }

        void StartWindow_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta == 120 && Dist < 18)
            {
                Dist++;
                drawMap();
            }               
            else if (e.Delta == -120 && Dist > 1)
            {
                Dist--;
                drawMap();
            }
        }

        private void drawMap(string name)
        {
            var SearchObject = geoCode.SearchObject(name);
            Point = geoCode.GetPoint(SearchObject);
            string ImageUrl = geoCode.GetUrlMapImage(SearchObject, Dist, 460, 305);
            pictureBoxMap.Image = geoCode.DownloadMapImage(ImageUrl);
        }

        private void drawMap()
        {
            string ImageUrl = geoCode.GetUrlMapImage(Dist, Point, 460, 305);
            pictureBoxMap.Image = geoCode.DownloadMapImage(ImageUrl);
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (textBoxCity.ForeColor != Color.Gray || textBoxCity.Text == "")
            {
                Dist = 8;
                drawMap(textBoxCity.Text.Trim());
                TemperatureChartDraw(textBoxCity.Text.Trim());
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBoxCity.Text = "Введите город";
            textBoxCity.ForeColor = Color.Gray;
            Dist = 3;
            drawMap("Россия");
            TemperatureChartDraw("Russia");
        }

        private void textBoxCity_TextChanged(object sender, EventArgs e)
        {
            if (textBoxCity.ForeColor == Color.Gray)
            {
                textBoxCity.ForeColor = Color.Black;
                textBoxCity.Text = "";
            }
        }

        private void textBoxCity_Click(object sender, EventArgs e)
        {
            if (textBoxCity.ForeColor == Color.Gray)
            {
                textBoxCity.ForeColor = Color.Black;
                textBoxCity.Text = "";
            }
        }

        private async void CurrentTemp(string city)
        {
            string weburl = "http://api.openweathermap.org/data/2.5/weather?q=" + city + "&mode=xml&appid=ac8f09c254df15f52f73415ebeee573a";
            var xml = await new WebClient().DownloadStringTaskAsync(new Uri(weburl));
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            CurrentTemperature.Text = WeatherConversion.GetCurrentTemperatureCelFromDoc(doc, "value").ToString() + "°C";
        }

        private async void TemperatureChartDraw(string city)
        {
            try
            {
                string weburl = "http://api.openweathermap.org/data/2.5/forecast?q=" + city + "&mode=xml&appid=ac8f09c254df15f52f73415ebeee573a";
                var xml = await new WebClient().DownloadStringTaskAsync(new Uri(weburl));
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                var tempSequence = WeatherConversion.GetTemperatureSequenceCelFromDoc(doc, "value");
                myChart.AddFullChart(tempSequence, 5, tempSequence.Length / 5, city);
                CurrentTemp(city);
            }
            catch
            {

            }
        }

        private void moveMap(double difX, double difY)
        {
            Point = ((double.Parse(Point.Split(',')[0].Replace('.', ',')) - difX).ToString().Replace(',', '.') + ',' + (double.Parse(Point.Split(',')[1].Replace('.', ',')) - difY).ToString().Replace(',', '.'));
            string ImageUrl = geoCode.GetUrlMapImage(Dist, Point, 460, 305);
            pictureBoxMap.Image = geoCode.DownloadMapImage(ImageUrl);
        }

        private void pictureBoxMap_MouseClick(object sender, MouseEventArgs e)
        {
            var posMap = pictureBoxMap.Location;
            posMap.X += 230;
            posMap.Y += 152;
            double difX = posMap.X - e.X;
            double difY = posMap.Y - e.Y;
            moveMap(difX / (Dist * 100), -difY / (Dist * 100));
        }

        private void textBoxCity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                buttonSearch_Click(sender, e);
        }
    }
}
