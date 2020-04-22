using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Server
{
    public class COVIDDataPoint
    {
        public String Date = "";
        public String Country = "";
        public String Sex = "";
        public String Age = "";
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

                    Regex matchCommas = new Regex(","); // Matches all commas in data that aren't inside quotes
                    String[] values = matchCommas.Split(line);

                    COVIDDataPoint point = new COVIDDataPoint();

                    for (int i = 0; i < values.Length; ++i)
                    {
                        values[i] = values[i].TrimStart('"'); // Remove quotes from data elements if present
                        values[i] = values[i].TrimEnd('"');
                    }

                    int[] dataIndices = {1, 2, 5, 12}; // Indices we care about 1-Age, 2-Sex, 5-Country, 12-Date
                    foreach (int index in dataIndices)
                    {
                        String val = values[index];
                        if (val != "") // Ignore empty data
                        {
                            switch(index)
                            {
                                case 1:
                                    point.Age = val;
                                    break;

                                case 2:
                                    point.Sex = val;
                                    break;

                                case 5:
                                    point.Country = val;
                                    break;

                                case 12:
                                    point.Date = val;
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
