using System;
using System.Xml;

namespace WeatherLib
{
    public static class WeatherConversion
    {
        public static double GetAverageForOneDay(double min, double max, double value)
        {
            return (min + max + value) / 3.0;
        }

        public static double GetAverageForOneDay(double min, double max)
        {
            return (min + max) / 2.0;
        }

        public static double[] GetAverageForSequence(double[] min, double[] max)
        {
            double[] numArray = new double[max.Length];
            for (int index = 0; index < numArray.Length; ++index)
                numArray[index] = (min[index] + max[index]) / 2.0;
            return numArray;
        }

        public static double ConvertToCelsius(double temperatureInKelvin)
        {
            return temperatureInKelvin - 273.15;
        }

        public static double GetCurrentTemperatureKelFromDoc(XmlDocument doc, string attribute)
        {
            var xmlNode = doc.DocumentElement.SelectSingleNode("temperature").Attributes[attribute].Value;
            return Math.Round(double.Parse(xmlNode.Replace('.', ',')), 2);
        }

        public static double GetCurrentTemperatureCelFromDoc(XmlDocument doc, string attribute)
        {
            var xmlNode = doc.DocumentElement.SelectSingleNode("temperature").Attributes[attribute].Value;
            return Math.Round(double.Parse(xmlNode.Replace('.', ',')) - 273.15, 2);
        }

        public static double[] GetTemperatureSequenceKelFromDoc(XmlDocument doc, string attribute)
        {
            XmlNodeList xmlNodeList = doc.DocumentElement.SelectSingleNode("forecast").SelectNodes("time");
            double[] numArray = new double[xmlNodeList.Count];
            for (int index = 0; index < xmlNodeList.Count; ++index)
                numArray[index] = Convert.ToDouble(xmlNodeList.Item(index).SelectSingleNode("temperature").Attributes[attribute].Value.Replace('.', ','));
            return numArray;
        }

        public static double[] GetTemperatureSequenceCelFromDoc(XmlDocument doc, string attribute)
        {
            XmlNodeList xmlNodeList = doc.DocumentElement.SelectSingleNode("forecast").SelectNodes("time");
            double[] numArray = new double[xmlNodeList.Count];
            for (int index = 0; index < xmlNodeList.Count; ++index)
                numArray[index] = Convert.ToDouble(xmlNodeList.Item(index).SelectSingleNode("temperature").Attributes[attribute].Value.Replace('.', ',')) - 273.15;
            return numArray;
        }

        public static double[] GetMaxForDays(double[] temperatures, int frequencyMeasurement)
        {
            double[] numArray = new double[temperatures.Length / frequencyMeasurement];
            double num = -1.0;
            for (int index1 = 0; index1 < numArray.Length; ++index1)
            {
                for (int index2 = 0; index2 < frequencyMeasurement; ++index2)
                {
                    if (temperatures[index2 + index1 * 8] > num)
                        num = temperatures[index2 + index1 * frequencyMeasurement];
                }
                numArray[index1] = num;
                num = -1.0;
            }
            return numArray;
        }

        public static double[] GetMinForDays(double[] temperatures, int frequencyMeasurement)
        {
            double[] numArray = new double[temperatures.Length / frequencyMeasurement];
            double num = double.MaxValue;
            for (int index1 = 0; index1 < numArray.Length; ++index1)
            {
                for (int index2 = 0; index2 < frequencyMeasurement; ++index2)
                {
                    if (temperatures[index2 + index1 * frequencyMeasurement] < num)
                        num = temperatures[index2 + index1 * frequencyMeasurement];
                }
                numArray[index1] = num;
                num = double.MaxValue;
            }
            return numArray;
        }

        public static double[] GetAverageForDays(double[] temperatures, int frequencyMeasurement)
        {
            double[] numArray = new double[temperatures.Length / frequencyMeasurement];
            double num = 0.0;
            for (int index1 = 0; index1 < numArray.Length; ++index1)
            {
                for (int index2 = 0; index2 < frequencyMeasurement; ++index2)
                    num += temperatures[index2 + index1 * frequencyMeasurement];
                numArray[index1] = num / (double)frequencyMeasurement;
                num = 0.0;
            }
            return numArray;
        }
    }
}
