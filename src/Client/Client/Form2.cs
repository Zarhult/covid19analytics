using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Client
{
    public partial class Form2 : Form
    {
        private TableLayoutPanel panel;
        private TabPage DataPage;
        public List<COVIDDataPoint> Result;
        public Form1 Parent;
        public NewDataPoint NewPoint;
        public UpdateDataPoint UpdatePoint;

        public Form2(Form1 ParentForm, List<COVIDDataPoint> data)
        {
            InitializeComponent();
            Result = data;
            Parent = ParentForm;
            tabControl1.SelectTab("tabPage1");
            DataPage = tabControl1.SelectedTab;
            // Initialize data view table
            panel = new TableLayoutPanel();
            panel.Location = new System.Drawing.Point(88, 100);
            panel.Name = "TableLayoutPanel1";
            panel.Size = new System.Drawing.Size(624, 279);
            panel.TabIndex = 0;
            panel.ColumnCount = 5;
            panel.RowCount = 1;
            panel.AutoScroll = true;

            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));

            DataPage.Controls.Add(panel);
            panel.Controls.Add(new Label() { Text = "ID" }, 0, 0);
            panel.Controls.Add(new Label() { Text = "Date" }, 1, 0);
            panel.Controls.Add(new Label() { Text = "Country" }, 2, 0);
            panel.Controls.Add(new Label() { Text = "Sex" }, 3, 0);
            panel.Controls.Add(new Label() { Text = "Age" }, 4, 0);

            // Fill with data
            int row = 1;

            foreach (COVIDDataPoint point in data)
            {
                panel.RowCount += 1;
                panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));

                for (int i = 0; i <= 4; ++i)
                {
                    switch (i)
                    {
                        case 0:
                            panel.Controls.Add(new Label() { Text = point.ID.ToString() }, i, row);
                            break;

                        case 1:
                            panel.Controls.Add(new Label() { Text = point.Date }, i, row);
                            break;

                        case 2:
                            panel.Controls.Add(new Label() { Text = point.Country }, i, row);
                            break;

                        case 3:
                            panel.Controls.Add(new Label() { Text = point.Sex }, i, row);
                            break;

                        case 4:
                            panel.Controls.Add(new Label() { Text = point.Age }, i, row);
                            break;
                    }
                }

                ++row;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            NewPoint = new NewDataPoint(this);
            NewPoint.Show();
        }

        public void CommunicateParent(string Msg)
        {
            Parent.SendMsg(Msg);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            StringBuilder csvExport = new StringBuilder();
            csvExport.AppendLine("ID,Date,Country,Sex,Age");
            string Line;
            foreach (COVIDDataPoint Point in Result)
            {
                Line = Point.ID.ToString() + "," + Point.Date + "," + Point.Country + "," + Point.Sex + "," + Point.Age;
                csvExport.AppendLine(Line);
            }
            string Path = "..\\..\\Export\\ExportedData.csv";
            if(File.Exists(Path))
            {
                File.Delete(Path);
            }
            File.AppendAllText(Path, csvExport.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool Found = false;
            string Data_ID = textBox1.Text;
            foreach (COVIDDataPoint point in Result)
            {
                if (point.ID.ToString() == Data_ID)
                    Found = true;
            }
            if(Found)
            {
                UpdatePoint = new UpdateDataPoint(this, Data_ID);
                UpdatePoint.Show();
            }
            else
            {
                MessageBox.Show("No Data Point Searched with that ID");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            bool Found = false;
            string Data_ID = textBox3.Text;
            foreach (COVIDDataPoint point in Result)
            {
                if (point.ID.ToString() == Data_ID)
                    Found = true;
            }
            if (Found)
            {
                string deleteMsg = "Delete Data with ID: " + Data_ID;
                CommunicateParent(deleteMsg);
            }
            else
            {
                MessageBox.Show("No Data Point Searched with that ID");
            }
        }
    }
}
