using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class pageSetting : Form
    {
        public pageSetting()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string base_url = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = @""+base_url+"/setting.txt";
            MessageBox.Show("Base URL", base_url);

            try
            {
                // Check if file already exists. If yes, delete it.
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Create a new file
                using (FileStream fs = File.Create(fileName))
                {
                    // Add some text to file
                    Byte[] title = new UTF8Encoding(true).GetBytes("New Text File");
                    fs.Write(title, 0, title.Length);
                    byte[] author = new UTF8Encoding(true).GetBytes("Mahesh Chand");
                    fs.Write(author, 0, author.Length);
                }

                // Open the stream and read it back.
                using (StreamReader sr = File.OpenText(fileName))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }
                }

            }
            catch (Exception Ex)
            {
                MessageBox.Show("An error occurred while printing", Ex.ToString());
                //Console.WriteLine(Ex.ToString());
            }
        }
    }
}
