using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public Form1()
        {
            InitializeComponent();
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
                    this.textBox2.Invoke(new MethodInvoker(delegate () { textBox2.AppendText("Server Responds: " + recieve + "\n"); }));
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
            if (textBox3.Text != "")
            {
                text_to_send = textBox3.Text; //--> INFORMATION SENT <--
                backgroundWorker2.RunWorkerAsync();
            }
            textBox3.Text = "";
        }
        //the following is semi psuedo code for feature 1
        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e) //gets data from inputed date range
        {
            vector<COVIDDataPoint> rows; //for searched rows

            if (client.Connected)
            {
                if(textBox4.Text == "" && textBox5.Text == "")//if both inputs are empty
                    //append all
                    foreach (COVIDDataPoint point in data)
                    {
                        rows.append(point);
                    }
                else{//else
                    if (textBox4.Text == "")//if left is empty
                        //append all on or before right date
                        foreach (COVIDDataPoint point in data)
                        {
                            if (point.date_confirmation <= textBox5.Text)
                            { //get data on or before right date
                                rows.append(point);
                            }
                        }

                    else if (textBox5.Text == "")//else if right empty
                            //appned all on or after left date
                            foreach (COVIDDataPoint point in data)
                            {
                                if (point.date_confirmation >= textBox4.Text)
                                { //get data on or before right date
                                   rows.append(point);
                                }
                            }
                    //else
                    if (invalidDate(textBox4.Text) || invalidDate(textBox5.Text))//if either invalid inputs 
                        MessageBox.Show("Invalid date input");//display error
                    else if (textBox4.Text > textBox5.Text)//else if right date is before left date 
                        MessageBox.Show("end date must be after start date");//display error
                    else
                    {//else
                        foreach (COVIDDataPoint point in data)
                        {
                            if (point.date_confirmation >= textBox4.Text && point.date_confirmation <= textBox5.Text)
                            {//get data after left and before right
                                rows.append(point);
                            }
                        }
                    }
                    if (rows == null){ //if rows is empty 
                        MessageBox.Show("No data for that date range");
                    }
                    else //rows not empty
                    {
                        MessageBox.Show(rows); //display all data points
                    }
                }
                
                //function to check date (did it this way to make it cleaner somewhat)
                bool invalidDate(Text t) //checks for invalid date 
                {
                    var inputDate = t.Split('.'); //split at each partion (in this case '.' in DD.MM.YYYY)

                    if (inputDate[1] < 1 || inputDate[1] > 12){ //invalid month
                        return false;
                    }
                    else if () {//check for invalid day for month 
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                //end of semi psuedo code
            }
        }
