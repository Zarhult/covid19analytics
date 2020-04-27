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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //for each input
                //for each column
                    //get item in coumn
                    //check for correctness (valid date, age, gender, etc)
                    //if correct
                        //add to dataset
                    //end if
                    //else
                        MessageBox.Show("Invalid Feature");//error message
                        //exit for loop
                    //end else
                //end for
            //end for

        }

        public bool dateCheck(String date)
        {
            if (date == "") return true;
            if (date.Length != 8) return false; //Make sure it is correct format
            if (date[0] > '1' || date[0] < '0') return false; //Tens place check
            if (date[0] == '0' && (date[1] > '9' || date[1] < '0')) return false; //Single digit dates
            if (date[0] == '1' && (date[1] > '2' || date[1] < '0')) return false; //Double digit dates
            if (date[2] != '.') return false; //Format
            if (date[3] > '3' || date[3] < '0') return false; //Tens place
            if (date[3] == '0' && (date[4] > '9' || date[4] < '0')) return false;
            if (date[3] == '1' && (date[4] > '9' || date[4] < '0')) return false;
            if (date[3] == '3' && (date[4] > '1' || date[4] < '0')) return false;
            if (date[5] != '.') return false;
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

        public bool genderCheck(String gender)
        {
            gender = gender.ToLower(); //upper and lower case don't matter 
            if (gender == "") return true; //empty string fine
            if (gender == "male") return true;
            if (gender == "female") return true;
            if (gender == "n/a") return true; //also okay
            return false; //ant other input is incorrect
        }

        public bool wuhancheck(String wuhan) 
        {
            if (wuhan == "" || wuhan == "1" || wuhan == "0") return true;
            return false;
        }

        public bool numericCheck(String str) //checks if input string is a number
        {
            int i = 0;
            bool result = int.TryParse(s, out i); 
            return result;
        }

        public bool longitudecheck(String longitude) 
        {
            bool dec = false;
            string num = "";
            for (int i = 0; i < longitude.Length; i++) 
            {
                if (i > 0 && i < 3 && !dec && longitude[i] == '.') //check for decimal
                {
                    dec = true;
                }
                else if (longitude[i] == '.') //make sure no second decimal
                {
                    return false;
                }
                else
                {
                    num = num + longitude[i]; 
                }
            }
            if (!dec) return false; // no decimal found in expected spot
            if (numericCheck(num)) //check if all numbers
            {
                //can't be greater than 180 or less than 0
                if (longitude[0] > '1') return false; 
                if (longitude[0] == '1' && longitude[1] > '8') return false; 
                if (longitude[0] == '1' && longitude[1] == '8' && longitude[2] > '0') return false;
                if (longitude[0] < '0') return false;
                return true;
            }
        }

        public bool latitudecheck(String latititude)
        {
            bool dec = false;
            string num = "";
            for(int i = 0; i < latititude.Length; i++) 
            {
                if (i > 0 && i < 2 && !dec && latititude[i] == '.') //check for decimal
                {
                    dec = true;
                }
                else if (latititude[i] == '.')
                {
                    return false;
                }
                else
                {
                    num = num + latititude[i];
                }
            }
            if (!dec) return false; // no decimal found in expected spot
            if (numericCheck(num)) //check for all numbers
            {
                if (latititude[0] == '9' && latititude[1] > '0') return false; //can't be > 90
                if (latititude[0] < '0') return false; //cant be < 0
                return true;
            }
            return false;
        }

    }
}
