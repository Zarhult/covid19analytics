using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
//Allowing communication between server and client
using System.Net;
using System.Net.Sockets;
using System.IO;


namespace Server
{

    public partial class Form1 : Form
    {
        private TcpClient client;
        public StreamReader STR;
        public StreamWriter STW;
        public string recieve;
        public String text_to_send;
        public List<COVIDDataPoint> Rows;
        public List<COVIDDataPoint> Results;
        public Dictionary<string, string> maps = new Dictionary<string, string>();
        public Form1(List<COVIDDataPoint> data)
        {
            Rows = data;
            InitializeComponent();

        }

        public string SearchAlgo(String recieve)
        {
            Regex matchCommas = new Regex(",");
            String[] values = matchCommas.Split(recieve);
            int ret = 0;
            string rets = "";
            string r = "";
            
            foreach (COVIDDataPoint point in Rows)
                {
                    if (values[0].Length > 0 && values[1].Length > 0 && Rows.Count() > 100)
                    {
                        if (DateSearch(values[0].ToString(), point.Date.ToString()) && AgeSearch(values[1].ToString(), point.Age.ToString())
                            && GenderSearch(values[2].ToString(), point.Sex.ToString()) && CountrySearch(values[3].ToString(), point.Country.ToString()))
                        {
                            rets += point.ID.ToString() + ",";
                            rets += (point.Date != "") ? point.Date + "," : " ,";
                            rets += (point.Country != "") ? point.Country + "," : " ,";
                            rets += (point.Sex != "") ? point.Sex + "," : " ,";
                            rets += (point.Age != "") ? point.Age + "," : " ,";
                            ret++;
                        }
                    }
                }
                return ret.ToString() + "," + rets;
            //}
        }
        public bool DateSearch(String Date, String COVIDDate)
        {
            if (Date.Length > 7 && COVIDDate.Length > 9)
            {
                if (Date == "All Dates" || COVIDDate == "") return true;
                int Year = Int32.Parse(Date[6].ToString() + Date[7].ToString());
                int Month = Int32.Parse(Date[0].ToString() + Date[1].ToString());
                int Day = Int32.Parse(Date[3].ToString() + Date[4].ToString());
                int COVIDYear = Int32.Parse(COVIDDate[8].ToString() + COVIDDate[9].ToString());
                int COVIDMonth = Int32.Parse(COVIDDate[3].ToString() + COVIDDate[4].ToString());
                int COVIDDay = Int32.Parse(COVIDDate[0].ToString() + COVIDDate[1].ToString());
                if (Date.Length == 13)
                {
                    if (COVIDYear > Year) return false;
                    else if (COVIDYear < Year) return true;
                    else if (COVIDMonth > Month) return false;
                    else if (COVIDMonth < Month) return true;
                    else if (COVIDDay > Day) return false;
                    else return true;
                }
                else if (Date.Length == 11)
                {
                    if (COVIDYear < Year) return false;
                    else if (COVIDYear > Year) return true;
                    else if (COVIDMonth < Month) return false;
                    else if (COVIDMonth > Month) return true;
                    else if (COVIDDay < Day) return false;
                    else return true;
                }
                else
                {
                    int Year2 = Int32.Parse(Date[17].ToString() + Date[18].ToString());
                    int Month2 = Int32.Parse(Date[11].ToString() + Date[12].ToString());
                    int Day2 = Int32.Parse(Date[14].ToString() + Date[15].ToString());
                    if (COVIDYear < Year || COVIDYear > Year2) return false;
                    else if ((COVIDYear > Year || COVIDYear < Year2)) return true;
                    else if (COVIDMonth < Month || COVIDMonth > Month2) return false;
                    else if (COVIDMonth > Month || COVIDMonth < Month2) return true;
                    else if (COVIDDay < Day || COVIDDay > Day2) return false;
                    else return true;
                }
            }
            else return false;
            
        }

        public bool AgeSearch(String Age, String COVIDAge)
        {
            if (Age == "All Ages") return true;
            Regex matchSpace = new Regex(" ");
            String[] Ages = matchSpace.Split(Age);
            int Age1;
            int Covid;
            if (Int32.TryParse(Ages[0].ToString(), out Age1)) Age1 = Int32.Parse(Ages[0].ToString());
            else return false;
            if (Int32.TryParse(COVIDAge, out Covid)) Covid = Int32.Parse(Ages[0].ToString());
            else return true;
            if (Ages[1] == "to")
            {
                if (Age1 <= Covid) return true;
                else return false;
            }
            else if (Ages[1] == "from")
            {
                if (Age1 >= Covid) return true;
                else return false;
            }
            else
            {
                int Age2;
                if (Int32.TryParse(Ages[2].ToString(), out Age2)) Age2 = Int32.Parse(Ages[2].ToString());
                if (Age1 <= Covid && Age2 >= Covid) return true;
                else return false;
            }
        }

        public bool GenderSearch(String Gender, String COVIDGender)
        {
            if (Gender == "All Genders") return true;
            else if(Gender == "Male Only")
            {
                if (COVIDGender == "male") return true;
                else return false;
            }
            else
            {
                if (COVIDGender == "female") return true;
                else return false;
            }
        }

        public bool CountrySearch(String Country, String COVIDCountry)
        {
            if (Country == "All Countries") return true;
            Regex matchSpace = new Regex(" ");
            String[] Countries = matchSpace.Split(Country);
            foreach (string country in Countries)
            {
                if (country == COVIDCountry) return true;
            }
            return false;
        }

        private void button1_Click(object sender, EventArgs e) //Starting Server
        {
            TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(textBox1.Text)); //Set Port For Example 123
            listener.Start();
            client = listener.AcceptTcpClient();
            STR = new StreamReader(client.GetStream());
            STW = new StreamWriter(client.GetStream());
            STW.AutoFlush = true;

            backgroundWorker1.RunWorkerAsync(); //start recieving Data
            backgroundWorker2.WorkerSupportsCancellation = true; //Ability to cancel this thread
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) //Updates the Log with Request
        {
            while (client.Connected)
            {
                try
                {
                    recieve = STR.ReadLine();

                    this.textBox2.Invoke(new MethodInvoker(delegate () { textBox2.AppendText("Client Requests: " + recieve + "\n"); })); //recieve is the request <--
                    //text_to_send = "There are ";
                    string search = "";
                    if (recieve.Substring(0,3) == "New")
                    {
                        AddNewPoint(recieve);
                        text_to_send = "Added";
                    }
                    //text_to_send += " data points with this search";
                    else if(recieve.Substring(0,6) == "Update")
                    {
                        UpdateDataPoint(recieve);
                        text_to_send = "Updated";
                    }
                    else if(recieve.Substring(0,6) == "Delete")
                    {
                        DeleteDataPoint(recieve);
                        text_to_send = "Deleted";
                    }
                    else if(recieve.Substring(0,6) == "Import")
                    {
                        ImportData(recieve);
                        text_to_send = "Imported";
                    }
                    else
                    {
                        search = SearchAlgo(recieve);
                        text_to_send = search;
                    }
                    backgroundWorker2.RunWorkerAsync();
                    recieve = "";
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e) //Updates the Log with Request
        {
            if (client.Connected)
            {
                STW.WriteLine(text_to_send);

                this.textBox2.Invoke(new MethodInvoker(delegate () { textBox2.AppendText("Server Responds: " + text_to_send + "\n"); }));

            }
            else
            {
                MessageBox.Show("Send failed");
            }
            backgroundWorker2.CancelAsync();
        }

        private void button2_Click(object sender, EventArgs e) //Send Information
        {
            if (textBox3.Text != "")
            {
                text_to_send = textBox3.Text;
                backgroundWorker2.RunWorkerAsync();
            }
            textBox3.Text = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            StringBuilder csvBackup = new StringBuilder();
            csvBackup.AppendLine("ID,Date,Country,Sex,Age");
            string Line;
            foreach (COVIDDataPoint Point in Rows)
            {
                Line = Point.ID.ToString() + "," + Point.Date + "," + Point.Country + "," + Point.Sex + "," + Point.Age;
                csvBackup.AppendLine(Line);
            }
            string Path = "..\\..\\backup\\BackedupData.csv";
            if (File.Exists(Path))
            {
                File.Delete(Path);
            }
            File.AppendAllText(Path, csvBackup.ToString());
        }
        //Add New Data Point
        public void AddNewPoint(string Point)
        {
            string DataPoint = Point.Substring(16);
            Regex matchCommas = new Regex(",");
            String[] values = matchCommas.Split(DataPoint);
            COVIDDataPoint NewPoint = new COVIDDataPoint();
            NewPoint.Date = values[0];
            NewPoint.Country = values[1];
            NewPoint.Sex = values[2];
            NewPoint.Age = values[3];
            NewPoint.ID = Rows[Rows.Count - 1].ID + 1;
            Rows.Add(NewPoint);
            string NewDate = NewPoint.Date[3].ToString() + NewPoint.Date[4].ToString() + '.' + NewPoint.Date[0].ToString() + NewPoint.Date[1].ToString() + ".20" + NewPoint.Date[6].ToString() + NewPoint.Date[7].ToString();
            string Path = "..\\..\\COVID19_open_line_list_Test.csv";
            StringBuilder Line = new StringBuilder();
            string newLine = NewPoint.ID.ToString() + "," + NewPoint.Age + "," + NewPoint.Sex + "," + "," + "," + NewPoint.Country + "," + "," + "," + "," + "," + "," + "," + NewDate;
            Line.Append(newLine);
            File.AppendAllText(Path, Line.ToString());
        }
        public void UpdateDataPoint(string Point)
        {
            string DataPoint = Point.Substring(19);
            Regex matchCommas = new Regex(",");
            String[] values = matchCommas.Split(DataPoint);
            int i = 0;
            while (Rows[i].ID.ToString() != values[0])
                i++;
            Rows[i].Date = values[1];
            Rows[i].Country = values[2];
            Rows[i].Sex = values[3];
            Rows[i].Age = values[4];
            string NewDate = Rows[i].Date[3].ToString() + Rows[i].Date[4].ToString() + '.' + Rows[i].Date[0].ToString() + Rows[i].Date[1].ToString() + ".20" + Rows[i].Date[6].ToString() + Rows[i].Date[7].ToString();
            string Path = "..\\..\\COVID19_open_line_list_Test.csv";
            List<String> lines = new List<String>();
            if (File.Exists(Path))
            {
                using (StreamReader reader = new StreamReader(Path))
                {
                    String line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains(","))
                        {
                            String[] split = line.Split(',');

                            if (split[0].Contains(values[0]))
                            {
                                split[1] = values[4];
                                split[2] = values[3];
                                split[5] = values[2];
                                split[12] = NewDate;
                                line = String.Join(",", split);
                            }
                        }

                        lines.Add(line);
                    }
                }

                using (StreamWriter writer = new StreamWriter(Path, false))
                {
                    foreach (String line in lines)
                        writer.WriteLine(line);
                }
            }
        }
        public void DeleteDataPoint(string Point)
        {
            string DataPoint = Point.Substring(21);
            int i = 0;
            while (Rows[i].ID.ToString() != DataPoint)
                i++;
            Rows.RemoveAt(i);
            string Path = "..\\..\\COVID19_open_line_list_Test.csv";
            List<String> lines = new List<String>();
            if (File.Exists(Path))
            {
                using (StreamReader reader = new StreamReader(Path))
                {
                    String line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!(line.Contains(DataPoint)))
                        {
                            lines.Add(line);
                        }
                    }
                }

                using (StreamWriter writer = new StreamWriter(Path, false))
                {
                    foreach (String line in lines)
                        writer.WriteLine(line);
                }
            }

        }
        public void ImportData(string Data)
        {
            Regex matchLines = new Regex(";");
            String[] values = matchLines.Split(Data);
            string sent = "";
            for(int i = 1; i < values.Length - 1; i++)
            {
                sent = "New Data Point: " + values[i];
                UpdateDataPoint(sent);
            }
        }
    }
}