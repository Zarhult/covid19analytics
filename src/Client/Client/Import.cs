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
    public partial class Import : Form
    {
        public Form1 Parent;
        public Import(Form1 ParentWindow)
        {
            Parent = ParentWindow;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Path = textBox1.Text;
            if(File.Exists(Path))
            {
                string ImportData = "Import: ;";
                using (StreamReader reader = new StreamReader(Path))
                {
                    String line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        ImportData += line + ";";
                    }
                }
                Parent.SendMsg(ImportData);
                this.Close();
            }
            else
            {
                MessageBox.Show("File doesn't Exist");
            }
        }
    }
}
