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
    }
}
