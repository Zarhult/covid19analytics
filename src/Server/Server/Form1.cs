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
            foreach (COVIDDataPoint point in Rows)
            {
                if (values[0].Length > 0 && values[1].Length > 0 && Rows.Count() > 100)
                {
                    if (DateSearch(values[0].ToString(), point.Date.ToString()) && AgeSearch(values[1].ToString(), point.Age.ToString())
                        && GenderSearch(values[2].ToString(), point.Sex.ToString()) && CountrySearch(values[3].ToString(), point.Country.ToString()))
                    {
                        rets += (point.Date != "") ? point.Date + "," : " ,";
                        rets += (point.Country != "") ? point.Country + "," : " ,";
                        rets += (point.Sex != "") ? point.Sex + "," : " ,";
                        rets += (point.Age != "") ? point.Age + "," : " ,";
                        ret++;
                    }
                }
            }
            return ret.ToString() + "," + rets;
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
                    search = SearchAlgo(recieve);
                    //text_to_send += " data points with this search";
                    text_to_send = search;
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
    }
}