using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
//For debugging
using System.Diagnostics;

// Ignoring inconsistent "last updated" data to keep things simpler

namespace Server
{
    public class COVIDDataPoint
    {
        public int num = 0;
        public String obsDate = "";
        public String city = "";
        public String country = "";
        public double confirmed = 0;
        public double dead = 0;
        public double recovered = 0;
    }

    public class Parser
    {
        public static List<COVIDDataPoint> ParseCSV(String fileName)
        {
            using (StreamReader sr = new StreamReader(fileName))
            {
                List<COVIDDataPoint> data = new List<COVIDDataPoint>();

                // Skip to the data
                sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();
                    String[] values;

                    if (!line.Contains("\""))
                    {
                        values = line.Split(',');
                    }
                    else
                    {
                        // Handle quoted data here
                    }

                    COVIDDataPoint point = new COVIDDataPoint();

                    // Skipping index 4 since ignoring "last updated" for now
                    point.num = Int32.Parse(values[0]);
                    point.obsDate = values[1];
                    point.city = values[2];
                    point.country = values[3];
                    //point.confirmed = double.Parse(values[5], CultureInfo.InvariantCulture);
                    //point.dead = double.Parse(values[6]);
                    //point.recovered = double.Parse(values[7]);

                    //Debug.WriteLine(values[5]);

                    data.Add(point);
                }

                return data;
            }
        }
    }
}
