using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Server
{
    public class COVIDDataPoint
    {
        public DateTime obsDate;
        public String province = "";
        public String country = "";
        public double confirmed = 0.0;
        public double dead = 0.0;
        public double recovered = 0.0;
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

                    Regex matchCommas = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))"); // Matches all commas in data that aren't inside quotes
                    String[] values = matchCommas.Split(line);

                    COVIDDataPoint point = new COVIDDataPoint();

                    for (int i = 0; i < values.Length; ++i)
                    {
                        values[i] = values[i].TrimStart('"'); // Remove quotes from data elements if present
                        values[i] = values[i].TrimEnd('"');
                    }

                    int[] dataIndices = { 1, 2, 3, 5, 6, 7 }; // Indices we care about
                    foreach (int index in dataIndices)
                    {
                        String val = values[index];
                        if (val != "") // Ignore empty data
                        {
                            switch(index)
                            {
                                case 1:
                                    point.obsDate = DateTime.Parse(val);
                                    break;

                                case 2:
                                    point.province = val;
                                    break;

                                case 3:
                                    point.country = val;
                                    break;

                                case 5:
                                    if (double.TryParse(val, out double result1))
                                    {
                                        point.confirmed = result1;
                                    }
                                    break;

                                case 6:
                                    if (double.TryParse(val, out double result2))
                                    {
                                        point.dead = result2;
                                    }
                                    break;

                                case 7:
                                    if (double.TryParse(val, out double result3))
                                    {
                                        point.recovered = result3;
                                    }
                                    break;
                            }
                        }
                    }
                    data.Add(point);
                }
            }

            return data;
        }
    }
}
