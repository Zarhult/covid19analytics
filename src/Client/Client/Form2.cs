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
using System.Threading;

namespace Client
{
    public partial class Form2 : Form
    {
        private TableLayoutPanel panel;
        private TabPage DataPage;
        public List<COVIDDataPoint> Result;
        public int page;
        public Form1 Parent;
        public NewDataPoint NewPoint;
        public UpdateDataPoint UpdatePoint;
        public ShowSpread SpreadVisualize;
        public List<String> X;
        public List<int> Y;
        public String GraphType;


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
            panel.Controls.Add(new Label() { Text = "ID"        }, 0, 0);
            panel.Controls.Add(new Label() { Text = "Date"      }, 1, 0);
            panel.Controls.Add(new Label() { Text = "Country"   }, 2, 0);
            panel.Controls.Add(new Label() { Text = "Sex"       }, 3, 0);
            panel.Controls.Add(new Label() { Text = "Age"       }, 4, 0);

            // Fill with data
            if(data.Count < 100)
                foreach (COVIDDataPoint point in data)
                {
                    addRow(point.ID, point.Date, point.Country, point.Sex, point.Age);
                }
            else
                for(int i = 0; i < 100; i++)
                {
                    addRow(data[i].ID, data[i].Date, data[i].Country, data[i].Sex, data[i].Age);
                }
            page = 1;
        }

        public int getRowId(int PointID)
        {
            int rowId = -1;

            try
            {
                for (int i = 0; i < panel.Controls.Count; i += 5) // i += 5 because 5 controls per row
                {
                    var control = panel.Controls[i];

                    if (control is Label && ((Label)control).Text == PointID.ToString())
                    {
                        rowId = i / 5;
                    }
                }

                if (rowId == -1) // Failure
                {
                    throw new System.Exception("getRowId failed.");
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message.ToString());
            }

            return rowId;
        }

        public void clearRow(int PointID)
        {
            int rowId = getRowId(PointID);

            panel.RowStyles[rowId].Height = 0;

            for (int i = 0; i < 5; ++i)
            {
                panel.Controls[rowId * 5 + i].Hide(); // Hide all 5 controls
            }
        }

        public void addRow(int PointID, String Date, String Country, String Sex, String Age)
        {
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));

            int rowNum = panel.RowCount;
            panel.Controls.Add(new Label() { Text = PointID.ToString()  }, 0, rowNum);
            panel.Controls.Add(new Label() { Text = Date                }, 1, rowNum);
            panel.Controls.Add(new Label() { Text = Country             }, 2, rowNum);
            panel.Controls.Add(new Label() { Text = Sex                 }, 3, rowNum);
            panel.Controls.Add(new Label() { Text = Age                 }, 4, rowNum);

            panel.RowCount += 1;
        }

        public void updateRow(int PointID, String Date, String Country, String Sex, String Age)
        {
            int rowId = getRowId(PointID);

            panel.Controls[rowId * 5 + 0].Text = PointID.ToString();
            panel.Controls[rowId * 5 + 1].Text = Date;
            panel.Controls[rowId * 5 + 2].Text = Country;
            panel.Controls[rowId * 5 + 3].Text = Sex;
            panel.Controls[rowId * 5 + 4].Text = Age;
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
                var confirmResult = MessageBox.Show("Are you sure to delete?",
                                     "Confirm Delete!!",
                                     MessageBoxButtons.YesNo);
                if(confirmResult == DialogResult.Yes)
                {
                    string deleteMsg = "Delete Data with ID: " + Data_ID;
                    CommunicateParent(deleteMsg);
                    clearRow(Int32.Parse(Data_ID));
                }
                
            }
            else
            {
                MessageBox.Show("No Data Point Searched with that ID");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Result.Sort((x, y) => DateTime.Compare(DateTime.ParseExact(x.Date, "dd.MM.yyyy", null), DateTime.ParseExact(y.Date, "dd.MM.yyyy", null)));

            // Make a form popup to visualize spread using the sorted array
            SpreadVisualize = new ShowSpread(this, comboBox4.Text);
            SpreadVisualize.Show();
            SpreadVisualize.Visualize();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            double projection = Result.Count; //initialized with known number of cases 
            double numDays = 0;
            //check date
            if (dateCheck(textBox2.Text) && textBox2.Text != "" && futurecheck(textBox2.Text))
            {
                int begin = beginDate(Result); //earliest date
                int end = endDate(Result); //latest date
                double days = begin - end; // total number of days in Result data
                double rate = (Result.Count / days); //avg num of cases per day
                numDays = getFutureDays(textBox2.Text); //get number of days 
                projection = Math.Round(projection + (rate * numDays)); //get projection

                //display to the user
                textBox4.Text = "There are " + projection.ToString() + " projected cases on " + textBox2.Text +
                    " for the searched data. Note: the projection is just an estimate based on the data searched for and as such may not be accurate.";
            }
            else
            {
                MessageBox.Show("Invalid or no Date"); //error message
            }
        }
        public int endDate(List<COVIDDataPoint> data) //gets latest date in data; returns -1 if there is an error
        {
            int day = 1000;
            foreach (COVIDDataPoint point in data)
            {
                if (day > getDays(point.Date.ToString()))
                {
                    day = getDays(point.Date.ToString());
                }
            }
            if (day == 1000) //if for some reason could not get latest date 
            {
                return -1; //return error
            }

            return day;
        }

        public int beginDate(List<COVIDDataPoint> data) //gets earliest date in data; returns -1 if there is an error
        {
            int day = -1;
            foreach (COVIDDataPoint point in data)
            {
                if (day < getDays(point.Date.ToString()))
                {
                    day = getDays(point.Date.ToString());
                }
            }
            return day;
        }

        public bool dateCheck(String date)
        {
            if (date == "") return true;
            if (date.Length != 8) return false; //Make sure it is correct format
            if (date[0] > '1' || date[0] < '0') return false; //Tens place check
            if (date[0] == '0' && (date[1] > '9' || date[1] < '0')) return false; //Single digit dates
            if (date[0] == '1' && (date[1] > '2' || date[1] < '0')) return false; //Double digit dates
            if (date[2] != '/') return false; //Format
            if (date[3] > '3' || date[3] < '0') return false; //Tens place
            if (date[3] == '0' && (date[4] > '9' || date[4] < '0')) return false;
            if (date[3] == '1' && (date[4] > '9' || date[4] < '0')) return false;
            if (date[3] == '3' && (date[4] > '1' || date[4] < '0')) return false;
            if (date[5] != '/') return false;
            if (date[6] > '9' || date[6] < '0') return false;
            if ((date[7] > '9' || date[7] < '0')) return false;
            return true;
        }

        public bool futurecheck(String date) //check if date is after latest date in data set (Feb. 29, 2020)
        {
            if (date[6] < '2') return false;
            if (date[6] == '2' && date[7] == '0' && date[0] == '0' && date[1] < '3') return false;
            return true;
        }

        public int getDays(String date) //date in dataset where Jan 12, 2020 = 0; Jan 13, 2020 = 1; etc.
        {
            int num = 0;
            num = ((date[4] - '1') * 31) + ((date[0] - '1') * 10) + (date[1] - '2');
            return num;
        }

        public int getFutureDays(String date) //number of days from Feb 29, 2020 to given date
        {
            int num = 0;
            num = ((date[6] - '2') * 3650) + ((date[7] - '0') * 365) + ((date[3] - '0') * 10) + (date[4] - '0'); //add amount of days in year and day given

            if (date[0] == '0' && date[1] < '3') //calculate ammount of days in month given
            {
                num = num - (('3' - date[1]) * 30);
            }

            else if (date[0] == '1')
            {
                if (date[1] == '1')
                {
                    num = num + (9 * 30);
                }

                else if (date[1] == '2')
                {
                    num = num + (10 * 30);
                }

            }

            else
            {
                num = num + ((date[1] - '3') * 30);
            }

            return num;

        }

        private void genGraph()
        {
            bool meanDiff = false;
            string countries = Parent.countries.Replace(" ", "");
            chart1.Series["Series1"].Points.Clear();
            GraphType = comboBox3.Text;
            int n;
            // X axis dataset
            List<String> Xaxis = new List<String>();
            List<int> Yaxis = new List<int>();
            if (comboBox1.Text == "Date")
            {
                foreach (COVIDDataPoint point in Result)
                {
                    if (point.Date != "" && point.Date != " ")
                        Xaxis.Add(point.Date);
                }
            }
            else if (comboBox1.Text == "Country")
            {
                foreach (COVIDDataPoint point in Result)
                {
                    if(countries.Contains(point.Country))
                        
                        Xaxis.Add(point.Country);
                }
            }
            else if (comboBox1.Text == "Sex")
            {
                foreach (COVIDDataPoint point in Result)
                {
                    if (point.Sex.ToLower() == "male" || point.Sex.ToLower() == "female")
                            Xaxis.Add(point.Sex.ToLower());
                }
            }
            else if (comboBox1.Text == "Age")
            {
                meanDiff = true;
                foreach (COVIDDataPoint point in Result)
                {
                    if (point.Age != "" && point.Age != " " && Parent.ageCheck(point.Age))
                        Xaxis.Add(point.Age);
                }
            }
            Yaxis = Yaxis_calculations(Xaxis);
            List<String> Unique = new List<String>();
            Unique = Xaxis.Distinct().ToList();
            for (int i = 0; i < Unique.Count; i++)
            {
                chart1.Series["Series1"].Points.AddXY(Unique[i], Yaxis[i]);
            }
            if (GraphType == "Line")
                chart1.Series["Series1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            else if (GraphType == "Bar")
                chart1.Series["Series1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
            else if (GraphType == "ScatterPlot")
                chart1.Series["Series1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            else if (GraphType == "Pie")
                chart1.Series["Series1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            int max = 0;
            int pos = -1;
            int total = 0;
            for (int i = 0; i < Unique.Count; i++)
            {
                total += Yaxis[i];
                if (max < Yaxis[i])
                {
                    max = Yaxis[i];
                    pos = i;
                }
            }
            double mean = 0;
            textBox5.Text = Unique[pos];
            textBox6.Text = Xaxis[total / 2];
            if (meanDiff)
            {
                foreach (string age in Xaxis)
                {
                    mean += int.Parse(age);
                }
                mean = mean / Xaxis.Count;
                textBox7.Text = mean.ToString();
            }
            else
                textBox7.Text = Xaxis[total / 2];
        }

        private void button7_Click(object sender, EventArgs e)
        {
            genGraph();
        }

        List<int> Yaxis_calculations(List<String> Xaxis)
        {
            List<int> ret = new List<int>();
            int count = 0;
            List<String> Unique = new List<String>();
            Unique = Xaxis.Distinct().ToList();
            foreach(String element in Unique)
            {
                count = 0;
                foreach(String check in Xaxis)
                {
                    if (check == element)
                        count++;
                }
                ret.Add(count);
            }
            return ret;
        }


        private void button9_Click(object sender, EventArgs e)
        {
            if(page > 1)
            {
                page--;
                int j = 0;
                for(int i = 100*(page-1); i < 100*page; i++)
                {
                    panel.Controls[j * 5 + 0].Text = Result[i].ID.ToString();
                    panel.Controls[j * 5 + 1].Text = Result[i].Date;
                    panel.Controls[j * 5 + 2].Text = Result[i].Country;
                    panel.Controls[j * 5 + 3].Text = Result[i].Sex;
                    panel.Controls[j * 5 + 4].Text = Result[i].Age;
                    j++;
                }

            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if((page+1)*100 < Result.Count)
            {
                int j = 0;
                for (int i = 100 * page; i < 100 * (page + 1); i++)
                {
                    panel.Controls[j * 5 + 0].Text = Result[i].ID.ToString();
                    panel.Controls[j * 5 + 1].Text = Result[i].Date;
                    panel.Controls[j * 5 + 2].Text = Result[i].Country;
                    panel.Controls[j * 5 + 3].Text = Result[i].Sex;
                    panel.Controls[j * 5 + 4].Text = Result[i].Age;
                    j++;
                }
                page++;
            }
            else if(Result.Count - (page)*100 > 0)
            {
                int j = 0;
                for (int i = 100 * page; i < Result.Count; i++)
                {
                    panel.Controls[j * 5 + 0].Text = Result[i].ID.ToString();
                    panel.Controls[j * 5 + 1].Text = Result[i].Date;
                    panel.Controls[j * 5 + 2].Text = Result[i].Country;
                    panel.Controls[j * 5 + 3].Text = Result[i].Sex;
                    panel.Controls[j * 5 + 4].Text = Result[i].Age;
                    j++;
                }
                page++;
            }

        }

        private void button11_Click(object sender, EventArgs e)
        {
            int maleCount = 0;
            int femaleCount = 0;
            foreach (COVIDDataPoint point in Result)
            {
                if (point.Sex == "male")
                    maleCount++;
                else if (point.Sex == "female")
                    femaleCount++;
            }
            string answer = (maleCount > femaleCount) ? "Men: " + maleCount.ToString() : "Women: " + femaleCount.ToString();
            label10.Text = answer;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            String type = comboBox2.Text;
            decimal top = numericUpDown1.Value;
            string Answer = "";
            string countries = Parent.countries.Replace(" ", "");
            /*
            if (type == "Month")
            {
                List<DateDataPoint> DateList = new List<DateDataPoint>();
                DateDataPoint max = new DateDataPoint();
                DateList = Parent.D;
                max.Count = 0;

                if(top < DateList.Count)
                {
                    for (int i = 0; i < top; i++)
                    {
                        foreach (DateDataPoint point in DateList)
                        {
                            if (max.Count < point.Count)
                                max = point;
                        }
                        Answer += (i + 1).ToString() + ". " + max.Date[3].ToString() + max.Date[4].ToString() + '\n';
                        DateList.Remove(max);
                    }
                }
                else
                {
                    for (int i = 0; i < DateList.Count; i++)
                    {
                        foreach (DateDataPoint point in DateList)
                        {
                            if (max.Count < point.Count)
                                max = point;
                        }
                        Answer += (i + 1).ToString() + ". " + max.Date[3].ToString() + max.Date[4].ToString() + '\n';
                        DateList.Remove(max);
                    }
                    for (int i = DateList.Count; i < top; i++)
                    {
                        Answer += ((i + 1).ToString() + ". " + "\n ");
                    }
                }
            }
            else if (type == "Country")
            {
                List<CountryDataPoint> CountryList = new List<CountryDataPoint>();
                CountryDataPoint max = new CountryDataPoint();
                CountryList = Parent.C;
                max.Count = 0;
                if (top < CountryList.Count)
                {
                    for (int i = 0; i < top; i++)
                    {
                        foreach (CountryDataPoint point in CountryList)
                        {
                            if (max.Count < point.Count)
                                max = point;
                        }
                        Answer += (i + 1).ToString() + ". " + max.Country + '\n';
                        CountryList.Remove(max);
                    }
                }
                else
                {
                    for (int i = 0; i < CountryList.Count; i++)
                    {
                        foreach (CountryDataPoint point in CountryList)
                        {
                            if (max.Count < point.Count)
                                max = point;
                        }
                        Answer += (i + 1).ToString() + ". " + max.Country + '\n';
                        CountryList.Remove(max);
                    }
                    for (int i = CountryList.Count; i < top; i++)
                    {
                        Answer += ((i + 1).ToString() + ". " + "\n ");
                    }
                }
            }
            else if (type == "Age Group")
            {
                List<AgeDataPoint> AgeList = new List<AgeDataPoint>();
                AgeDataPoint max = new AgeDataPoint();
                AgeList = Parent.A;
                max.Count = 0;
                if (top < AgeList.Count)
                {
                    for (int i = 0; i < top; i++)
                    {
                        foreach (AgeDataPoint point in AgeList)
                        {
                            if (max.Count < point.Count)
                                max = point;
                        }
                        Answer += (i + 1).ToString() + ". " + max.Age + '\n';
                        AgeList.Remove(max);
                    }
                }
                else
                {
                    for (int i = 0; i < AgeList.Count; i++)
                    {
                        foreach (AgeDataPoint point in AgeList)
                        {
                            if (max.Count < point.Count)
                                max = point;
                        }
                        Answer += (i + 1).ToString() + ". " + max.Age + '\n';
                        AgeList.Remove(max);
                    }
                    for (int i = AgeList.Count; i < top; i++)
                    {
                        Answer += ((i + 1).ToString() + ". " + "\n ");
                    }
                }
            }*/
            List<String> typeIN = new List<String>();
            foreach (COVIDDataPoint point in Result)
            {
                if (type == "Month" && dateCheck(point.Date))
                {
                    typeIN.Add(point.Date[3].ToString() + point.Date[4].ToString());
                }
                else if (type == "Country" && countries.Contains(point.Country))
                {
                    typeIN.Add(point.Country);
                }
                else if (type == "Age Group" && Parent.ageCheck(point.Age))
                {
                    typeIN.Add(point.Age);
                }
            }
            List<string> Unique = typeIN.Distinct().ToList();

            List<int> UniqueVals = new List<int>();
            int count;
            for (int i = 0; i < Unique.Count; i++)
            {
                count = 0;
                for (int j = 0; j < typeIN.Count; j++)
                {
                    if (Unique[i] == typeIN[j])
                        count++;
                }
                UniqueVals.Add(count);
            }
            int maxa = 0;
            int pos = 0;
            if (top < UniqueVals.Count)
            {
                for (int i = 0; i < top; i++)
                {
                    maxa = 0;
                    pos = 0;
                    for (int j = 0; j < UniqueVals.Count; j++)
                    {
                        if (maxa < UniqueVals[j])
                        {
                            pos = j;
                            maxa = UniqueVals[j];
                        }
                    }
                    Answer += ((i + 1).ToString() + ". " + Unique[pos] + "\n ");
                    UniqueVals.RemoveAt(pos);
                    Unique.RemoveAt(pos);
                }
            }
            else
            {
                for (int i = 0; i < UniqueVals.Count; i++)
                {
                    maxa = 0;
                    pos = 0;
                    for (int j = 0; j < UniqueVals.Count; j++)
                    {
                        if (maxa < UniqueVals[j])
                        {
                            pos = j;
                            maxa = UniqueVals[j];
                        }
                    }
                    Answer += ((i + 1).ToString() + ". " + Unique[pos] + "\n ");
                    UniqueVals.RemoveAt(pos);
                    Unique.RemoveAt(pos);
                }
                for (int i = UniqueVals.Count; i < top; i++)
                {
                    Answer += ((i + 1).ToString() + ". " + "\n ");
                }
            }

            System.Threading.Thread.Sleep(Result.Count / 10);
            textBox8.Text = Answer;
        }
    }
}
