using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form2 : Form
    {
        private TableLayoutPanel panel;

        public Form2(List<COVIDDataPoint> data)
        {
            InitializeComponent();

            // Initialize data view table
            panel = new TableLayoutPanel();
            panel.Location = new System.Drawing.Point(88, 100);
            panel.Name = "TableLayoutPanel1";
            panel.Size = new System.Drawing.Size(624, 279);
            panel.TabIndex = 0;
            panel.ColumnCount = 4;
            panel.RowCount = 1;
            panel.AutoScroll = true;

            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));

            Controls.Add(panel);
            panel.Controls.Add(new Label() { Text = "Date" }, 0, 0);
            panel.Controls.Add(new Label() { Text = "Country" }, 1, 0);
            panel.Controls.Add(new Label() { Text = "Sex" }, 2, 0);
            panel.Controls.Add(new Label() { Text = "Age" }, 3, 0);

            // Fill with data
            int row = 1;

            foreach (COVIDDataPoint point in data)
            {
                panel.RowCount += 1;
                panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));

                for (int i = 0; i <= 3; ++i)
                {
                    switch (i)
                    {
                        case 0:
                            panel.Controls.Add(new Label() { Text = point.Date }, i, row);
                            break;

                        case 1:
                            panel.Controls.Add(new Label() { Text = point.Country }, i, row);
                            break;

                        case 2:
                            panel.Controls.Add(new Label() { Text = point.Sex }, i, row);
                            break;

                        case 3:
                            panel.Controls.Add(new Label() { Text = point.Age }, i, row);
                            break;
                    }
                }

                ++row;
            }
        }
    }
}
