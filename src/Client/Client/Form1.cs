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

namespace Client
{

    public partial class Form1 : Form
    {
        private TcpClient client;
        public StreamReader STR;
        public StreamWriter STW;
        public string recieve;
        public String text_to_send;
        public String date;
        public String age;
        public String gender;
        public String countries;
        public String SendMessage;
        public Form2 DataTable;
        public ShowSpread SpreadVisualize;
        public Import ImportWindow;
        public List<COVIDDataPoint> Result;
        public Form1()
        {
            InitializeComponent();
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
            if (date[6] > '2' || date[6] < '0') return false;
            if (date[6] == '0' && (date[7] > '9' || date[7] < '0')) return false;
            if (date[6] == '1' && (date[7] > '9' || date[7] < '0')) return false;
            if (date[6] == '2' && date[7] != '0') return false;
            return true;
        }

        public bool dateOrient(String date1, String date2) //mm/dd/yy
        {
            if (date1 == "") return true;
            if (date2 == "") return true;
            if (date1[6] > date2[6]) return false;
            if (date1[6] == date2[6] && date1[7] > date2[7]) return false;
            if (date1[0] > date2[0]) return false;
            if (date1[0] == date2[0] && date1[1] > date2[1]) return false;
            if (date1[3] > date2[3]) return false;
            if (date1[3] == date2[3] && date1[4] > date2[4]) return false;
            return true;
        }

        public bool ageCheck(String age)
        {
            if (age == "") return true;
            if (age.Length > 3) return false;
            if (age.Length == 3 && (age[0] != '1' || (age[1] > '2' || age[1] < '0') || (age[2] > '9' || age[2] < '0'))) return false;
            if (age.Length == 2 && ((age[0] > '9' || age[0] < '1') || (age[1] > '9' || age[1] < '0'))) return false;
            if (age.Length == 1 && (age[0] > '9' || age[0] < '1')) return false;
            return true;
        }

        public bool ageOrient(String age1, String age2)
        {
            if (age1 == "") return true;
            if (age2 == "") return true;
            if (age2.Length < age1.Length) return false;
            if (age2.Length > age1.Length) return true;
            if (age2.Length == 3 && age2[1] < age1[1]) return false;
            if (age2.Length == 3 && age2[2] < age1[2]) return false;
            if (age2.Length == 2 && age2[1] < age1[1]) return false;
            if (age2.Length == 2 && age2[2] < age1[2]) return false;
            if (age2[0] < age1[0]) return false;
            return true;
        }
        private void button1_Click(object sender, EventArgs e) //Connection
        {
            string IP = "";
            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName()); //Get IP
            foreach (IPAddress address in localIP)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                   IP = address.ToString();
                }

            }
            client = new TcpClient();
            IPEndPoint IP_End = new IPEndPoint(IPAddress.Parse(IP), int.Parse(textBox1.Text)); //Use the same port as the server

            try
            {
                client.Connect(IP_End);
                if (client.Connected)
                {
                    textBox2.AppendText("Connected to Server" + '\n');
                    STW = new StreamWriter(client.GetStream());
                    STR = new StreamReader(client.GetStream());
                    STW.AutoFlush = true;
                    backgroundWorker1.RunWorkerAsync(); //start recieving Data
                    backgroundWorker2.WorkerSupportsCancellation = true; //Ability to cancel this thread
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message.ToString());
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) //Updates the Log with Response
        {
            while (client.Connected)
            {
                try
                {
                    recieve = STR.ReadLine();
                    Regex matchComma = new Regex(",");
                    String[] Results = matchComma.Split(recieve);
                    int numOfResults = Int32.Parse(Results[0]);
                    if(numOfResults == 0)
                    {
                        recieve = "There are no Data points with the given Search";
                        this.textBox2.Invoke(new MethodInvoker(delegate () { textBox2.AppendText("Server Responds: " + recieve + "\n"); }));
                    }
                    else
                    {
                        int numOfPoints = 5 * numOfResults;
                        List<COVIDDataPoint> SearchResults = new List<COVIDDataPoint>();
                        for(int i = 1; i < numOfPoints; i += 5)
                        {
                            COVIDDataPoint point = new COVIDDataPoint();
                            point.ID = Int32.Parse(Results[i]);
                            point.Date = Results[i+1];
                            point.Country = Results[i + 2];
                            point.Sex = Results[i + 3];
                            point.Age = Results[i + 4];
                            SearchResults.Add(point);
                        }
                        Result = SearchResults;
                        recieve = "There are " + numOfResults.ToString() + " Results for the specific Search";
                        this.textBox2.Invoke(new MethodInvoker(delegate () { textBox2.AppendText("Server Responds: " + recieve + "\n"); }));
                        // PROBLEM: "country" is missing from most of the data in Result
                        //DataTable.Close();
                        DataTable = new Form2(this, Result);
                        Application.Run(DataTable);
                    }
                    recieve = "";

                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e) //Updates the Log with request
        {
            if (client.Connected)
            {
                STW.WriteLine(text_to_send);
                this.textBox2.Invoke(new MethodInvoker(delegate () { textBox2.AppendText("Client Requests: " + text_to_send + "\n"); }));
            }
            else
            {
                MessageBox.Show("Send failed");
            }
            backgroundWorker2.CancelAsync();
        }

        private void button2_Click(object sender, EventArgs e) //Send Information LOOK HERE
        {
            text_to_send = "";
            //Dates
            if (dateCheck(textBox3.Text) && dateCheck(textBox4.Text) && dateOrient(textBox3.Text, textBox4.Text))
            {
                if ((textBox3.Text == "") && (textBox4.Text == ""))
                {
                    date = "All Dates";
                }
                else if ((textBox3.Text == ""))
                {
                    date = textBox4.Text + " from"; //13
                }
                else if ((textBox4.Text == ""))
                {
                    date = textBox3.Text + " to"; //11
                }
                else
                {
                    date = textBox3.Text + " - " + textBox4.Text;
                }

                //Age
                if (ageCheck(textBox5.Text) && ageCheck(textBox6.Text) && ageOrient(textBox5.Text, textBox6.Text))
                {
                    if ((textBox5.Text == "") && (textBox6.Text == ""))
                    {
                        age = "All Ages";
                    }
                    else if ((textBox5.Text == ""))
                    {
                        age = textBox6.Text + " from"; 
                    }
                    else if ((textBox6.Text == ""))
                    {
                        age = textBox5.Text +  " to";
                    }
                    else
                    {
                        age = textBox5.Text + " - " + textBox6.Text; 
                    }
                    textBox3.Text = "";
                    textBox4.Text = "";
                    textBox5.Text = "";
                    textBox6.Text = "";


                    //Gender
                    if (checkBox1.Checked == false && checkBox2.Checked == false) gender = "All Genders";
                    if (checkBox1.Checked == true && checkBox2.Checked == false) gender = "Male Only";
                    if (checkBox1.Checked == false && checkBox2.Checked == true) gender = "Female Only";
                    if (checkBox1.Checked == true && checkBox2.Checked == true) gender = "All Genders";
                    checkBox1.Checked = false;
                    checkBox2.Checked = false;

                    //Countries
                    countries = "";
                    for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                        countries += checkedListBox1.CheckedItems[i].ToString() + ' ';
                    if (countries == "") countries = "All Countries";
                    text_to_send = date + "," + age + "," + gender + "," + countries;
                    SendMessage = date + ", " + age + ", " + gender + ", " + countries;
                    backgroundWorker2.RunWorkerAsync();
                }
                else
                {
                    MessageBox.Show("Invalid Age");
                }
            }
            else
            {
                MessageBox.Show("Invalid Date");
            }


        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++) checkedListBox1.SetItemChecked(i, true);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++) checkedListBox1.SetItemChecked(i, false);
        }
        public void SendMsg(string msg)
        {
            text_to_send = msg;
            backgroundWorker2.RunWorkerAsync();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ImportWindow = new Import(this);
            ImportWindow.Show();
        }

    }
    public class COVIDDataPoint
    {
        public int ID = 0;
        public String Date = "";
        public String Country = "";
        public String Sex = "";
        public String Age = "";
    }
}
