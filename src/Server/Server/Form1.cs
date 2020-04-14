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


namespace Server
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
        // Initialization Read File HERE
            InitializeComponent();

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
