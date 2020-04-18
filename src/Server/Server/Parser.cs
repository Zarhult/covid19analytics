using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// Ignoring observation date and serial number

namespace Server
{
    public class COVIDDataPoint
    {
        // Todo: convert date string to DateTime
        public int age = 0;
        public String gender = "";
        public String city = "";
        public String province = "";
        public String country = "";
        public String date_confirmation = "";
        public String outcome = "";
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

                    if (values.Length >= 45) // Ignore incomplete data points
                    {
                        COVIDDataPoint point = new COVIDDataPoint();

                        for (int i = 0; i < values.Length; ++i)
                        {
                            values[i] = values[i].TrimStart('"'); // Remove quotes from data elements if present
                            values[i] = values[i].TrimEnd('"');
                        }

                        int[] dataIndices = { 1, 2, 3, 4, 5, 12, 23 }; // Indices we care about
                        foreach (int index in dataIndices)
                        {
                            String val = values[index];
                            if (val != "") // Ignore empty data
                            {
                                switch(index)
                                {
                                    case 1:
                                        bool success = Int32.TryParse(val, out int result);
                                        if (success)
                                        {
                                            point.age = result;
                                        }
                                        break;

                                    case 2:
                                        point.gender = val;
                                        break;

                                    case 3:
                                        point.city = val;
                                        break;

                                    case 4:
                                        point.province = val;
                                        break;

                                    case 5:
                                        point.country = val;
                                        break;

                                    case 12:
                                        point.date_confirmation = val;
                                        break;

                                    case 23:
                                        point.outcome = val;
                                        break;
                                }
                            }
                        }

                        //System.Diagnostics.Debug.WriteLine(point.age + point.gender + point.city + point.province + point.country + point.date_confirmation + point.outcome);

                        data.Add(point);
                    }
                }
            }

            return data;
        }
    }
}
