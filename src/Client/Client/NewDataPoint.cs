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
    public partial class NewDataPoint : Form
    {
        public Form2 Parent;
        public NewDataPoint(Form2 ParentClass)
        {
            Parent = ParentClass;
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
        public bool ageCheck(String age)
        {
            if (age == "") return true;
            if (age.Length > 3) return false;
            if (age.Length == 3 && (age[0] != '1' || (age[1] > '2' || age[1] < '0') || (age[2] > '9' || age[2] < '0'))) return false;
            if (age.Length == 2 && ((age[0] > '9' || age[0] < '1') || (age[1] > '9' || age[1] < '0'))) return false;
            if (age.Length == 1 && (age[0] > '9' || age[0] < '1')) return false;
            return true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string Date = textBox2.Text;
            string Age = textBox1.Text;
            string Sex = comboBox2.Text;
            string Country = comboBox1.Text;
            if(dateCheck(Date))
            {
                if(ageCheck(Age))
                {
                    string Ret = "New Data Point: " + Date + "," + Country + "," + Sex + "," + Age;
                    Parent.CommunicateParent(Ret);
                    this.Close();
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
    }
}
