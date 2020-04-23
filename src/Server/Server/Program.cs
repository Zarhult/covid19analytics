using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Server
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            List<COVIDDataPoint> Data = Parser.ParseCSV("..\\..\\COVID19_open_line_list_Test.csv"); // Go up a couple directories to the data file
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(Data));

            // For testing the parser
            /*
            foreach(COVIDDataPoint point in data)
            {
                System.Diagnostics.Debug.WriteLine(point.obsDate + " " + point.province + " " + point.country + " " + point.confirmed + " " + point.dead + " " + point.recovered);
            }
            */
        }
    }
}
