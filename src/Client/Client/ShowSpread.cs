using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Windows;

namespace Client
{
    public partial class ShowSpread : Form
    {
        public Form2 Parent;                                    // Reference to Form1
        List<COVIDDataPoint> ResultRef;                         // Reference to Form1's Result
        private Color RectColor = Color.FromArgb(255, 0, 0, 0); // Initially black
        private Graphics SprdGraphics;
        private SolidBrush SprdBrush;

        public ShowSpread(Form2 ParentForm, string country)
        {
            Parent = ParentForm;
            List<COVIDDataPoint> results = new List<COVIDDataPoint>();
            foreach(COVIDDataPoint point in Parent.Result)
            {
                if (point.Country == country)
                    results.Add(point);
            }
            ResultRef = results;

            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (ResultRef.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("No Data for specified country");
                this.Close();
            }
            base.OnPaint(e);

            // Draw centered rectangle using color member
            SprdGraphics = e.Graphics;
            SprdBrush = new SolidBrush(RectColor);
            int RectWidth = 200;
            int RectHeight = 200;
            SprdGraphics.FillRectangle(SprdBrush, new Rectangle(this.ClientSize.Width / 2 - RectWidth / 2, this.ClientSize.Height / 2 - RectHeight / 2, RectWidth, RectHeight));
        }

        private void UpdateColor(int a, int r, int g, int b)
        {
            RectColor = Color.FromArgb(a, r, g, b);
            this.Refresh();
        }

        public void Visualize()
        {

            // Redden rectangle as virus spreads, becoming completely red when reach most recent data
            int Cases = ResultRef.Count;
            int TimeStep = 200;                 // Time per day (ms)
            double RednessStep = 255.0 / Cases; // How much to redden for each case
            double TotalRedness = 0;            // Total redness from accumulated steps, which must then be rounded to an int

            // First case
            TotalRedness += RednessStep;
            UpdateColor(255, (int)TotalRedness, 0, 0);
            // Loop for the rest of cases
            for (int i = 0; i + 1 < Cases; ++i)
            {
                TotalRedness += RednessStep;
                int DayGap = (DateTime.ParseExact(ResultRef[i + 1].Date, "dd.MM.yyyy", null) - DateTime.ParseExact(ResultRef[i].Date, "dd.MM.yyyy", null)).Days;
                Thread.Sleep(DayGap * TimeStep);
                UpdateColor(255, (int)TotalRedness, 0, 0);
            }

            this.label1.Visible = true; // Let user know it's done
        }

    }
}
