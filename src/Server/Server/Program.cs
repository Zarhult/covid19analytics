using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            List<COVIDDataPoint> data = Parser.ParseCSV("<FILENAME>");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            // For testing the parser
            /*
            foreach(COVIDDataPoint point in data)
            {
                System.Diagnostics.Debug.WriteLine(point.city);
                System.Diagnostics.Debug.WriteLine(point.country);
                System.Diagnostics.Debug.WriteLine(point.lastUpdate);
                System.Diagnostics.Debug.WriteLine(point.confirmed);
                System.Diagnostics.Debug.WriteLine(point.dead);
                System.Diagnostics.Debug.WriteLine(point.recovered);
            }
            */
        }
    }
}
