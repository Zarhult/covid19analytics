using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Globalization;

// Ignoring observation date and serial number

namespace Server
{
    public class COVIDDataPoint
    {
        public String city = ""; // For some data points this is left blank and will remain an empty string
        public String country = "";
        public String lastUpdate = "";
        public double confirmed = 0;
        public double dead = 0;
        public double recovered = 0;
    }

    public class Parser
    {
        public static List<COVIDDataPoint> ParseCSV(String fileName)
        {
            List<COVIDDataPoint> data = new List<COVIDDataPoint>();

            using (StreamReader sr = new StreamReader(fileName))
            {
                // Skip first line to get to the data
                sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();
                    String[] values;

                    values = line.Split(',');

                    COVIDDataPoint point = new COVIDDataPoint();

                    int quoteOffset = 0; // Adjust indices accordingly for city data like "Los Angeles, CA"
                    if (line.Contains("\""))
                    {
                        quoteOffset = 1;
                        point.city = values[2] + ", " + values[3];
                    }
                    else
                    {
                        point.city = values[2];
                    }
                    point.country = values[3 + quoteOffset];
                    point.lastUpdate = values[4 + quoteOffset];
                    point.confirmed = double.Parse(values[5 + quoteOffset], CultureInfo.InvariantCulture);
                    point.dead = double.Parse(values[6 + quoteOffset], CultureInfo.InvariantCulture);
                    point.recovered = double.Parse(values[7] + quoteOffset, CultureInfo.InvariantCulture);

                    data.Add(point);
                }
            }

            return data;
        }
    }
}
