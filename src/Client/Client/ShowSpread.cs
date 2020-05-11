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
        public Form1 Parent;
        List<COVIDDataPoint> ResultRef; // reference to Form1's Result
        private Color RectColor = Color.FromArgb(255, 0, 0, 0); // Initially black
        private Graphics SprdGraphics;
        private SolidBrush SprdBrush;

        public ShowSpread(Form1 ParentForm)
        {
            Parent = ParentForm;
            ResultRef = Parent.Result;
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
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
            // test
            UpdateColor(255, 0, 0, 0);
            Thread.Sleep(1000);
            UpdateColor(255, 255, 255, 255);
        }
    }
}
